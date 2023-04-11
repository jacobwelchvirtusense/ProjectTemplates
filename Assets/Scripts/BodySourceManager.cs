/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/6/2023 8:56:59 AM
 * 
 * Description: Handles the registering and tracking of users bodies.
*********************************/
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using static ValidCheck;
using Assets.AzureKinect;
using Assets.Kinect;
using Assets.SensorAdapters;

public class BodySourceManager : MonoBehaviour 
{
    #region Fields
    /// <summary>
    /// The scene instance of the BodySourceManager.
    /// </summary>
    private static BodySourceManager Instance;

    #region Plugin & Sensors
    /// <summary>
    /// Defines which sensor should be initialized as the kinnectSensor.
    /// </summary>
    private PluginSettings pluginSettings = new PluginSettings();

    /// <summary>
    /// Returns the type of sensor currently being used.
    /// </summary>
    public static SensorType CurrentSensorUsed 
    { 
        get
        {
            if (Instance == null || Instance.pluginSettings == null) return SensorType.KINNECTV2;

            return Instance.pluginSettings.SensorType;
        } 
    }

    /// <summary>
    /// The kinnect sensor that has been initialized.
    /// </summary>
    private ISensorAdapter kinnectSensor;
    #endregion

    #region Data Events & Backing Fields
    /// <summary>
    /// An event that is fired every time sensor data is updated.
    /// </summary>
    [HideInInspector] public UnityEvent<Skeleton> SensorDataUpdateEvent = new UnityEvent<Skeleton>();

    /// <summary>
    /// An event that is fired every time a sensor is found or not found
    /// </summary>
    [HideInInspector] public UnityEvent<bool> SensorFoundEvent = new UnityEvent<bool>();

    /// <summary>
    /// Holds true if sensor data has been found.
    /// </summary>
    private bool hasFoundSensor = true;

    /// <summary>
    /// An event that is fired users are found or unfound.
    /// </summary>
    [HideInInspector] public UnityEvent<int> UsersFoundEvent = new UnityEvent<int>();

    /// <summary>
    /// Holds the number of users found and tracked.
    /// </summary>
    private int numberOfUsersFound = -1;
    #endregion

    [Range(1f, 1f)]
    [Tooltip("Holds the number of users to currently track (only 1 supported atm)")]
    [SerializeField] private int numberOfUsersToTrack = 1;

    /// <summary>
    /// The array of all body data currently found.
    /// </summary>
    private List<Skeleton> skeletonData = null;
    #endregion


    #region Functions
    #region Initialization
    /// <summary>
    /// Initializes the scene instance of the bodysourcemanager.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Initializes the kinnect sensor and its body reader.
    /// </summary>
    private void Start() 
    {
        ChangeSensorFound(false);
        ChangeUsersFound(0);

        StartCoroutine(CheckForSensor());
    }

    #region Initialize Sensor  
    /// <summary>
    /// Checks for an active sensor if the current one does not exist anymore.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckForSensor()
    {
        while (true)
        {
            if(IsntValid(kinnectSensor))
            {
                InitializeSensor();
            }

            yield return new WaitForFixedUpdate();
        }
    }

    /// <summary>
    /// Initializes the sensor with all of its hooks.
    /// </summary>
    private void InitializeSensor()
    {
        SelectSensorToBeUsed();

        kinnectSensor.Initialize();

        InitializeSensorEventListeners();
    }

    /// <summary>
    /// Selects a sensor to be used and constructs it.
    /// </summary>
    private void SelectSensorToBeUsed()
    {
        pluginSettings.LoadPluginSettings();

        switch (pluginSettings.SensorType)
        {
            case SensorType.AZUREKINNECT:
                kinnectSensor = new AzureKinectAdapter();
                print("AzureKinect Adapter Initialized!");
                break;
            case SensorType.KINNECTV2:
            default:
                kinnectSensor = new KinectAdapter();
                print("KinectV2 Adapter Initialized!");
                break;
        }
    }

    /// <summary>
    /// Initializes frame data listeners for the kinnect sensor.
    /// </summary>
    private void InitializeSensorEventListeners()
    {
        kinnectSensor.skeletonFrameReady += OnSkeletonFrameReady;

        // Initializes the sensor feedback listener
        var sensorFeedbackHandler = FindObjectOfType<SensorFeedback>();
        if (sensorFeedbackHandler)
        {
            sensorFeedbackHandler.InitializeTexture(kinnectSensor.BodyIndexImageSize);
            kinnectSensor.bodyIndexFrameReady += sensorFeedbackHandler.OnNewBodyIndexFrame;
        }
    }
    #endregion
    #endregion

    #region Event Updates
    /// <summary>
    /// Handles the event of new skeleton data being recieved.
    /// </summary>
    /// <param name="sender">The object sending the data.</param>
    /// <param name="skeletonFrameData">The skeleton frame data.</param>
    private void OnSkeletonFrameReady(object sender, GenericEventArgs<SkeletonFrame> skeletonFrameData)
    {
        skeletonData = skeletonFrameData.Args.skeletons;

        TrackUsers();
    }

    #region Frame Updates
    /// <summary>
    /// Tracks users and sends out their body data to all listening scripts.
    /// </summary>
    private void TrackUsers()
    {
        if(numberOfUsersToTrack == 1)
        {
            // TODO Find better way of ensuring users don't steal sensor from each other
            #region Get Kinect Data
            ChangeSensorFound(skeletonData != null);

            if (skeletonData == null) return;

            List<ulong> _trackedIds = new List<ulong>();

            ulong centerID = 0;
            float currentLow = Mathf.Infinity;

            foreach (var skeleton in skeletonData)
            {
                if (skeleton == null) continue;

                var lowCheck = Mathf.Abs(skeleton.joints[(int)JointType.SpineBase].position.x);

                if (skeleton.IsTracked() && lowCheck < currentLow)
                {
                    centerID = skeleton.trackingId;
                    currentLow = lowCheck;
                }
            }

            if (centerID != 0) _trackedIds.Add(centerID);
            #endregion

            #region Create & Refresh Kinect Bodies
            ChangeUsersFound(_trackedIds.Count);

            foreach (var skeleton in skeletonData)
            {
                if (skeleton == null) continue;

                if (skeleton.IsTracked() && skeleton.trackingId == centerID)
                {
                    SensorDataUpdateEvent.Invoke(skeleton);
                }
            }
            #endregion
        }
        else
        {
            // TODO Do something with more users
        }
    }
    #endregion

    #region Change Sensor State
    /// <summary>
    /// Updates whether the sensor has been found or not.
    /// </summary>
    /// <param name="newFoundState">The new status of the sensor having been found.</param>
    private void ChangeSensorFound(bool newFoundState)
    {
        if (newFoundState != hasFoundSensor)
        {
            hasFoundSensor = newFoundState;
            SensorFoundEvent.Invoke(hasFoundSensor);
        }
    }

    /// <summary>
    /// Updates the number of users that have been found.
    /// </summary>
    /// <param name="newNumberOfBodiesFound">Updates the amount of detected users by the sensor.</param>
    private void ChangeUsersFound(int newNumberOfBodiesFound)
    {
        if (newNumberOfBodiesFound != numberOfUsersFound)
        {
            numberOfUsersFound = newNumberOfBodiesFound;
            UsersFoundEvent.Invoke(numberOfUsersFound);
        }
    }
    #endregion
    #endregion

    #region Uninitialize Sensors
    /// <summary>
    /// Ensures the kinnect sensor will be unitialized when closing the application.
    /// </summary>
    private void OnApplicationQuit()
    {
        CloseSensor();
    }

    /// <summary>
    /// Closes the kinnect sensor and unitializes it.
    /// </summary>
    public static void CloseSensor()
    {
        Instance.kinnectSensor.UnInitialize();
    }
    #endregion
    #endregion
}
