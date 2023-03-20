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
using Windows.Kinect;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using static ValidCheck;

public class BodySourceManager : MonoBehaviour 
{
    #region Fields
    [Range(1f, 1f)]
    [Tooltip("Holds the number of users to currently track (only 1 supported atm)")]
    [SerializeField] private int numberOfUsersToTrack = 1;

    /// <summary>
    /// An event that is fired every time sensor data is updated.
    /// </summary>
    [HideInInspector] public UnityEvent<Body> SensorDataUpdateEvent = new UnityEvent<Body>();

    /// <summary>
    /// An event that is fired every time a sensor is found or not found
    /// </summary>
    [HideInInspector] public UnityEvent<bool> SensorFoundEvent = new UnityEvent<bool>();

    /// <summary>
    /// An event that is fired users are found or unfound.
    /// </summary>
    [HideInInspector] public UnityEvent<int> UsersFoundEvent = new UnityEvent<int>();

    /// <summary>
    /// Holds true if sensor data has been found.
    /// </summary>
    private bool hasFoundSensor = true;

    /// <summary>
    /// Holds the number of users found and tracked.
    /// </summary>
    private int numberOfUsersFound = -1;

    /// <summary>
    /// The sensor that is currently being used.
    /// </summary>
    private KinectSensor kinnectSensor;

    /// <summary>
    /// The frame reader for bodies.
    /// </summary>
    private BodyFrameReader bodyFrameReader;

    /// <summary>
    /// The array of all body data currently found.
    /// </summary>
    private Body[] bodyData = null;
    #endregion

    #region Functions
    #region Initialization
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
        kinnectSensor = KinectSensor.GetDefault();

        if (IsValid(kinnectSensor))
        {
            if (!kinnectSensor.IsOpen)
            {
                kinnectSensor.Open();
            }

            if (IsntValid(bodyFrameReader))
            {
                bodyFrameReader = kinnectSensor.BodyFrameSource.OpenReader();
                bodyFrameReader.FrameArrived += NewBodyFrameUpdate;
            }
        }
    }
    #endregion

    #region Event Updates
    /// <summary>
    /// Updates whether the sensor has been found or not.
    /// </summary>
    /// <param name="newFoundState">The new status of the sensor having been found.</param>
    private void ChangeSensorFound(bool newFoundState)
    {
        if(newFoundState != hasFoundSensor)
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
        if(newNumberOfBodiesFound != numberOfUsersFound)
        {
            numberOfUsersFound = newNumberOfBodiesFound;
            UsersFoundEvent.Invoke(numberOfUsersFound);
        }
    }

    #region Frame Updates
    /// <summary>
    /// Gets the most recent updated frame and sends out its data.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void NewBodyFrameUpdate(object sender, BodyFrameArrivedEventArgs e)
    {
        UpdateFrame();
        TrackUsers();
    }

    /// <summary>
    /// Gets the most recent frame and disposes of it to clear the pointers location for new frames.
    /// </summary>
    private void UpdateFrame()
    {
        if (bodyFrameReader != null)
        {
            var frame = bodyFrameReader.AcquireLatestFrame();

            if (frame != null)
            {
                print("Current Time: " + Time.time);
                bodyData = new Body[kinnectSensor.BodyFrameSource.BodyCount];
                
                frame.GetAndRefreshBodyData(bodyData);
                frame.Dispose();
            }
        }
    }

    /// <summary>
    /// Tracks users and sends out their body data to all listening scripts.
    /// </summary>
    private void TrackUsers()
    {
        if(numberOfUsersToTrack == 1)
        {
            #region Get Kinect Data
            ChangeSensorFound(bodyData != null);

            if (bodyData == null) return;

            List<ulong> _trackedIds = new List<ulong>();

            ulong centerID = 0;
            float currentLow = Mathf.Infinity;

            foreach (var body in bodyData)
            {
                if (body == null) continue;

                var lowCheck = Mathf.Abs(body.Joints[JointType.SpineBase].Position.X);

                if (body.IsTracked && lowCheck < currentLow)
                {
                    centerID = body.TrackingId;
                    currentLow = lowCheck;
                }
            }

            if (centerID != 0) _trackedIds.Add(centerID);
            #endregion

            #region Create & Refresh Kinect Bodies
            ChangeUsersFound(_trackedIds.Count);

            foreach (var body in bodyData)
            {
                if (body == null) continue;

                if (body.IsTracked && body.TrackingId == centerID)
                {
                    SensorDataUpdateEvent.Invoke(body);
                }
            }
            #endregion
        }
        else
        {
            // Do something with more users
        }
    }
    #endregion
    #endregion

    /// <summary>
    /// Disposes of objects that no longer need to have connections.
    /// </summary>
    void OnApplicationQuit()
    {
        if (bodyFrameReader != null)
        {
            bodyFrameReader.Dispose();
            bodyFrameReader = null;
        }
        
        if (kinnectSensor != null)
        {
            if (kinnectSensor.IsOpen)
            {
                kinnectSensor.Close();
            }

            kinnectSensor = null;
        }
    }
    #endregion
}
