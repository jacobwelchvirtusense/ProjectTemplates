/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/6/2023 8:56:59 AM
 * 
 * Description: Gives feedback to the user on where they should stand 
 *              within the space of the sensor.
*********************************/
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Windows.Kinect;
using static ValidCheck;

public class SensorFeedback : SensorDataListener
{
    #region Fields
    /// <summary>
    /// The text object that is used to display feedback to the user.
    /// </summary>
    private TextMeshProUGUI feedbackText;

    /// <summary>
    /// Holds true if messages from here are allowed to be shown.
    /// </summary>
    private bool canShowMessage = true;

    #region Distance from sensor
    [Header("Distance From Sensor")]
    [Range(0.0f, 5.0f)]
    [Tooltip("The minimum z dist the user should be from the sensor")]
    [SerializeField] private float minZDist = 0.4f;

    [Tooltip("The message displayed when the user is too close to the sensor")]
    [SerializeField] private string zDistCloseMessage = "You are too close to the sensor!!!";

    [Space(InspectorValues.SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [Range(0.0f, 5.0f)]
    [Tooltip("The maximum z dist the user should be from the sensor")]
    [SerializeField] private float maxZDist = 1.0f;

    [Tooltip("The message displayed when the user is too far from the sensor")]
    [SerializeField] private string zDistFarMessage = "You are too far from the sensor!!!";
    #endregion

    #region Distance from center
    [Header("Distance From Center")]
    [Range(0.0f, 5.0f)]
    [Tooltip("The maximum x dist the user should be from the center of the sensor")]
    [SerializeField] private float maxXDist = 0.4f;

    [Tooltip("The message displayed when the user is to the right of the center")]
    [SerializeField] private string moveLeftMessage = "Please move left to the center of the sensor";

    [Tooltip("The message displayed when the user is to the left of the center")]
    [SerializeField] private string moveRightMessage = "Please move right to the center of the sensor";
    #endregion

    #region Lack of data
    [Header("Lack of Data Messages")]
    [Tooltip("The message displayed when the kinnect is not detected")]
    [SerializeField] private string noSensorFound = "No sensor detected!!!";

    [Tooltip("The message displayed when the kinnect is not detecting a user")]
    [SerializeField] private string noUserFound = "No user detected!!!";
    #endregion
    #endregion

    #region Functions
    /// <summary>
    /// Initializes components
    /// </summary>
    protected override void Awake()
    {
        feedbackText = GetComponent<TextMeshProUGUI>();

        base.Awake();
    }

    /// <summary>
    /// Sets whether or not the sensor has been found.
    /// </summary>
    /// <param name="hasFoundSensor">Holds true if the sensor has been found.</param>
    protected override void SetSensorFound(bool hasFoundSensor)
    {
        base.SetSensorFound(hasFoundSensor);

        if (!hasFoundSensor)
        {
            SetText(noSensorFound);
        }
        else if (HasNotFoundUser())
        {
            SetText(noUserFound);
        }
    }

    /// <summary>
    /// Sets the current number of users that are being tracked.
    /// </summary>
    /// <param name="currentUsers">The number of users currently being tracked.</param>
    protected override void SetUserFound(int currentUsers)
    {
        base.SetUserFound(currentUsers);

        if (HasNotFoundUser())
        {
            SetText(noUserFound);
        }
    }

    /// <summary>
    /// Uses the user body data to display feedback to them on positioning.
    /// </summary>
    /// <param name="body">The body currently being tracked.</param>
    protected override void UseUserData(Body body)
    {
        DisplayFeedback(body.Joints[JointType.SpineBase].Position);
    }

    /// <summary>
    /// Allows feedback to be shown.
    /// </summary>
    public void ResetFeeback()
    {
        canShowMessage = true;
    }

    /// <summary>
    /// Disables feedback from being displayed.
    /// </summary>
    public void DisableFeedback()
    {
        canShowMessage = false;
    }

    /// <summary>
    /// Displays feedback to the user based on there position relative to the sensor.
    /// </summary>
    /// <param name="position">The position in the camera space of the sensor.</param>
    private void DisplayFeedback(CameraSpacePoint position)
    {
        if (position.Z < minZDist)
        {
            SetText(zDistCloseMessage);
        }
        else if (position.Z > maxZDist)
        {
            SetText(zDistFarMessage);
        }
        else if (position.X < -maxXDist)
        {
            SetText(moveRightMessage);
        }
        else if (position.X > maxXDist)
        {
            SetText(moveLeftMessage);
        }
        else
        {
            SetText("");
        }
    }

    /// <summary>
    /// Sets feedback text to be a desired new string.
    /// </summary>
    /// <param name="newText">The new feedback to be shown.</param>
    private void SetText(string newText)
    {
        if (canShowMessage)
        {
            feedbackText.text = newText;
        }
        else
        {
            feedbackText.text = "";
        }
    }
    #endregion
}
