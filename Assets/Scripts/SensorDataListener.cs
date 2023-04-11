/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/16/2023 3:44:11 PM
 * 
 * Description: A base listener for all scripts that should use data from
 *              sensors.
*********************************/
using Assets.SensorAdapters;
using UnityEngine;
using Windows.Kinect;
using static ValidCheck;

public abstract class SensorDataListener : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// Holds true if the sensor has been found.
    /// </summary>
    protected bool hasFoundSensor = false;

    /// <summary>
    /// The current number of users being tracked by the sensor.
    /// </summary>
    protected int currentUsers = 0;

    /// <summary>
    /// The manager of all body sources for the scene.
    /// </summary>
    private BodySourceManager bodySourceManager;
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Finds body source manager of the scene and listens for it's events.
    /// </summary>
    protected virtual void Awake()
    {
        bodySourceManager = FindObjectOfType<BodySourceManager>();
        LinkBodySourceEvents();
    }

    /// <summary>
    /// Links functions in this script to the input user events.
    /// </summary>
    private void LinkBodySourceEvents()
    {
        if (IsntValid(bodySourceManager)) return;

        bodySourceManager.SensorFoundEvent.AddListener(SetSensorFound);
        bodySourceManager.UsersFoundEvent.AddListener(SetUserFound);
        bodySourceManager.SensorDataUpdateEvent.AddListener(SetUserData);
    }
    #endregion

    #region Data Listening Events
    /// <summary>
    /// Sets whether or not the sensor has been found.
    /// </summary>
    /// <param name="hasFoundSensor">Holds true if the sensor has been found.</param>
    protected virtual void SetSensorFound(bool hasFoundSensor)
    {
        this.hasFoundSensor = hasFoundSensor;
    }

    /// <summary>
    /// Records the number of users that have been found.
    /// </summary>
    /// <param name="currentUsers">The current number of users being tracked.</param>
    protected virtual void SetUserFound(int currentUsers)
    {
        this.currentUsers = currentUsers;
    }

    /// <summary>
    /// Calls for user data to be used.
    /// </summary>
    /// <param name="body">The body of the user being tracked.</param>
    private void SetUserData(Skeleton skeleton)
    {
        if (hasFoundSensor && HasFoundUser())
        {
            UseUserData(skeleton);
        }
    }

    /// <summary>
    /// Uses the users data for some action or feedback.
    /// </summary>
    /// <param name="body">The body of the user being tracked.</param>
    protected abstract void UseUserData(Skeleton skeleton);
    #endregion

    /// <summary>
    /// Returns true if no users have been found.
    /// </summary>
    /// <returns></returns>
    protected bool HasNotFoundUser()
    {
        return currentUsers == 0;
    }

    /// <summary>
    /// Returns true if at least one user has been found.
    /// </summary>
    /// <returns></returns>
    protected bool HasFoundUser()
    {
        return currentUsers > 0;
    }
    #endregion
}
