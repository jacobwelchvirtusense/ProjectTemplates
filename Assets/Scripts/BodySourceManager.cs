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
using System.Linq;

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
    /// Returns true if the user is using an azure kinnect sensor.
    /// </summary>
    public static bool IsAzureKinnect
    {
        get
        {
            if (Instance == null || Instance.pluginSettings == null) return false;

            return Instance.pluginSettings.SensorType == SensorType.AZUREKINNECT;
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

    /// <summary>
    /// The skeleton of the first active user.
    /// </summary>
    private Skeleton activeUser1 = null;

    public Skeleton Player1Skeleton
    {
        get
        {
            return activeUser1;
        }
    }

    /// <summary>
    /// The skeleton of the second active user.
    /// </summary>
    private Skeleton activeUser2 = null;

    public Skeleton Player2Skeleton
    {
        get
        {
            return activeUser2;
        }
    }

    public static int NumberOfUsersToTrack
    {
        get
        {
            switch (SettingsManager.Slot1)
            {
                case 0:
                    return 1;
                default:
                    return 2;
            }
        }
    }

    /// <summary>
    /// Holds true if the given ID is an active user.
    /// </summary>
    /// <param name="id">The id of skeleton to be checked.</param>
    /// <returns></returns>
    public static bool IsActiveID(ulong id)
    {
        if (IsntValid(Instance)) return false;

        if (NumberOfUsersToTrack == 1)
        {
            return Instance.activeUser1 != null && id == Instance.activeUser1.trackingId;
        }
        else
        {
            return (Instance.activeUser1 != null && id == Instance.activeUser1.trackingId) || (Instance.activeUser2 != null && id == Instance.activeUser2.trackingId);
        }
    }

    /// <summary>
    /// Holds true if the given index is an active user.
    /// </summary>
    /// <param name="index">The index of the skeleton to be checked.</param>
    /// <returns></returns>
    public static bool IsActiveIndex(short index)
    {
        if (IsntValid(Instance)) return false;

        if (NumberOfUsersToTrack == 1)
        {
            if (Instance.activeUser1 == null) return false;

            return index == Instance.activeUser1.trackingIndex;
        }
        else
        {
            return (Instance.activeUser1 != null && index == Instance.activeUser1.trackingIndex) || (Instance.activeUser2 != null && index == Instance.activeUser2.trackingIndex);
        }
    }

    public static bool ActiveIndexPlayerOne(short index)
    {
        if (IsntValid(Instance)) return false;

        if (NumberOfUsersToTrack == 1)
        {
            if (Instance.activeUser1 == null) return false;

            return index == Instance.activeUser1.trackingIndex;
        }
        else
        {
            return (Instance.activeUser1 != null && index == Instance.activeUser1.trackingIndex);
        }
    }

    public static bool ActiveIndexPlayerTwo(short index)
    {
        if (IsntValid(Instance)) return false;

        if (NumberOfUsersToTrack == 1)
        {
            if (Instance.activeUser2 == null) return false;

            return index == Instance.activeUser2.trackingIndex;
        }
        else
        {
            return (Instance.activeUser2 != null && index == Instance.activeUser2.trackingIndex);
        }
    }

    /// <summary>
    /// Holds true if the given ID is player one.
    /// </summary>
    /// <param name="id">The id of the skeleton to be checked.</param>
    /// <returns></returns>
    public static bool IsPlayerOne(ulong id)
    {
        return id == Instance.activeUser1.trackingId;
    }

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

        SettingsManager.Slot1OnValueChanged.AddListener(UpdateNumberOfUsers);
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

    /// <summary>
    /// Updates the current number of users that need to be tracked.
    /// </summary>
    /// <param name="newNumberOfUsers">The nubmer of users that should be tracked.</param>
    private void UpdateNumberOfUsers(int newNumberOfUsers)
    {
        activeUser1 = null;
        activeUser2 = null;
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
            if (IsntValid(kinnectSensor))
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
        if (IsAzureKinnect)
        {
            StartCoroutine(AzureKinectFrameUpdate());
        }
        else
        {
            kinnectSensor.skeletonFrameReady += OnSkeletonFrameReady;
        }
    }

    public static void RegisterSensorFeedback(SensorFeedback sensorFeedback)
    {
        if (Instance == null || Instance.kinnectSensor == null) return;

        sensorFeedback.InitializeTexture(Instance.kinnectSensor.BodyIndexImageSize);
        Instance.kinnectSensor.bodyIndexFrameReady += sensorFeedback.OnNewBodyIndexFrame;
    }
    #endregion
    #endregion

    #region Frame Reading
    #region Kinect2 Frame Reading
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
    #endregion

    #region Azure Kinect Frame Reading
    /// <summary>
    /// A routine for reading frames and calling tracking updates for the sensor.
    /// </summary>
    /// <returns></returns>
    private IEnumerator AzureKinectFrameUpdate()
    {
        while (true)
        {
            yield return new WaitForEndOfFrame();
            CheckForFrame();
        }
    }

    /// <summary>
    /// Checks for frame updates from the user.
    /// </summary>
    private void CheckForFrame()
    {
        if (kinnectSensor == null) return;

        if (kinnectSensor.ShouldPollForFrames) kinnectSensor.PollForFrames();

        var data = kinnectSensor.GetData();

        if (data != null)
        {
            var listData = data.ToList();

            if (IsNewFrame(skeletonData, listData))
            {
                skeletonData = listData;
                TrackUsers();
            }
        }
    }

    /// <summary>
    /// Checks if the new frame is actually different from the previous frame. This is to ensure new
    /// calls for tracking users is valid.
    /// </summary>
    /// <param name="lastFrameData">The data of the previous valid frame.</param>
    /// <param name="newFrameData">The data of the new frame.</param>
    /// <returns></returns>
    private bool IsNewFrame(List<Skeleton> lastFrameData, List<Skeleton> newFrameData)
    {
        if (lastFrameData == null || lastFrameData.Count != newFrameData.Count) return true;

        for (int i = 0; i < lastFrameData.Count; i++)
        {
            if (lastFrameData[i] != newFrameData[i]) return true;
        }

        return false;
    }
    #endregion
    #endregion

    #region Event Updates
    #region Frame Updates
    /// <summary>
    /// Tracks users and sends out their body data to all listening scripts.
    /// </summary>
    private void TrackUsers()
    {
        if (NumberOfUsersToTrack == 1)
        {
            // TODO Find better way of ensuring users don't steal sensor from each other
            #region Get Kinect Data
            ChangeSensorFound(skeletonData != null);

            if (skeletonData == null) return;

            List<ulong> _trackedIds = new List<ulong>();

            Skeleton centerSkeleton = null;
            float currentLow = Mathf.Infinity;
            bool activeDoesntExists = true;

            foreach (var skeleton in skeletonData)
            {
                if (skeleton == null) continue;

                if (activeUser1 != null && skeleton.trackingId == activeUser1.trackingId)
                {
                    activeDoesntExists = false;
                }

                var lowCheck = Mathf.Abs(skeleton.joints[(int)JointType.SpineBase].position.x);

                if (skeleton.IsTracked() && lowCheck < currentLow)
                {
                    centerSkeleton = skeleton;
                    currentLow = lowCheck;
                }
            }

            if (centerSkeleton != null)
            {
                if (activeUser1 == null || activeDoesntExists)
                {
                    activeUser1 = centerSkeleton;
                }

                _trackedIds.Add(centerSkeleton.trackingId);
            }
            #endregion

            #region Create & Refresh Kinect Bodies
            ChangeUsersFound(_trackedIds.Count);

            foreach (var skeleton in skeletonData)
            {
                if (skeleton == null) continue;

                if (skeleton.IsTracked() && _trackedIds.Contains(skeleton.trackingId))
                {
                    activeUser1 = skeleton;
                    SensorDataUpdateEvent.Invoke(skeleton);
                }
            }
            #endregion
        }
        else
        {
            // TODO Do something with more users
            // TODO Find better way of ensuring users don't steal sensor from each other
            #region Get Kinect Data
            ChangeSensorFound(skeletonData != null);

            if (skeletonData == null) return;

            List<ulong> _trackedIds = new List<ulong>();

            Skeleton leftPlayer = null;
            Skeleton rightPlayer = null;

            float currentLeft = Mathf.Infinity;
            float currentRight = -Mathf.Infinity;

            foreach (var skeleton in skeletonData)
            {
                if (skeleton == null) continue;

                var lowCheck = Mathf.Abs(skeleton.joints[(int)JointType.SpineBase].position.x);

                if (skeleton.IsTracked() && lowCheck > currentRight)
                {
                    leftPlayer = skeleton;
                    currentRight = lowCheck;
                }

                if (skeleton.IsTracked() && lowCheck < currentLeft)
                {
                    rightPlayer = skeleton;
                    currentLeft = lowCheck;
                }
            }

            if (rightPlayer != null)
            {
                if (activeUser1 == null)
                {
                    activeUser1 = rightPlayer;
                }

                _trackedIds.Add(activeUser1.trackingId);
            }

            if (leftPlayer != null && leftPlayer != rightPlayer)
            {
                if (activeUser2 == null)
                {
                    activeUser2 = leftPlayer;
                }

                _trackedIds.Add(activeUser2.trackingId);
            }

            if (activeUser1 == leftPlayer)
            {
                activeUser1 = rightPlayer;
                activeUser2 = leftPlayer;
            }

            if (activeUser1 == activeUser2)
            {
                activeUser2 = null;
            }
            #endregion

            #region Create & Refresh Kinect Bodies
            ChangeUsersFound(_trackedIds.Count);

            foreach (var skeleton in skeletonData)
            {
                if (skeleton == null) continue;

                if (skeleton.IsTracked() && _trackedIds.Contains(skeleton.trackingId))
                {
                    if (IsPlayerOne(skeleton.trackingId))
                    {
                        activeUser1 = skeleton;
                    }
                    else
                    {
                        activeUser2 = skeleton;
                    }

                    SensorDataUpdateEvent.Invoke(skeleton);
                }
            }
            #endregion
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
