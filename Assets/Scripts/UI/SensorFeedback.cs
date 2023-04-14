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
using Assets.SensorAdapters;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static ValidCheck;

public class SensorFeedback : SensorDataListener
{
    #region Fields
    #region Data Visualization
    [Header("Sensor Data Visualiation")]
    [Tooltip("The raw image that is use to display sensor data to the user")]
    [SerializeField] private RawImage sensorDataImage;

    [Tooltip("The color of the actively selected user")]
    [SerializeField] private Color activeUserColor = Color.green;

    [Tooltip("The color of any user not selected")]
    [SerializeField] private Color inactiveUserColor = Color.grey;

    [Tooltip("The color of background")]
    [SerializeField] private Color backgroundColor = Color.black;

    /// <summary>
    /// The index of the active user being display in the data visualization.
    /// </summary>
    private short activeUserIndex = -1;

    /// <summary>
    /// The texture that is being applied to the sensorDataImage.
    /// </summary>
    private Texture2D sensorTexture;

    /// <summary>
    /// The array of color being applied to the sensorTexture.
    /// </summary>
    private Color32[] bodyIndexColors;

    /// <summary>
    /// The time of the last body indexes frame data.
    /// </summary>
    private float timeOfLastBodyIndexData = -Mathf.Infinity;

    /// <summary>
    /// The minimum time between frame data in the sensor visualization.
    /// </summary>
    private const float minTimeBetweenFrames = 0.1f;
    #endregion

    #region Text Feedback
    /// <summary>
    /// The text object that is used to display feedback to the user.
    /// </summary>
    private TextMeshProUGUI feedbackText;

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

    #region Sensor Data Visualization
    /// <summary>
    /// Initializes the sensor data image and its texture.
    /// </summary>
    /// <param name="bodyIndexImageSize">The size that the image should be.</param>
    public void InitializeTexture(ImageSize bodyIndexImageSize)
    {
        sensorTexture = new Texture2D(bodyIndexImageSize.Width, bodyIndexImageSize.Height);

        if (sensorDataImage) sensorDataImage.texture = sensorTexture;
    }

    /// <summary>
    /// The event for new body indexes being recieved.
    /// </summary>
    /// <param name="sender">The object sending the message.</param>
    /// <param name="bodyIndexFrame">The event recieved for the body index frame.</param>
    public void OnNewBodyIndexFrame(object sender, GenericEventArgs<BodyIndexFrame> bodyIndexFrame)
    {
        if (!gameObject.activeInHierarchy) return;

        if (bodyIndexFrame.Args != null) BodyIndexReader_FrameArrived(bodyIndexFrame);
    }

    /// <summary>
    /// Sets the sensor data image to show new data.
    /// </summary>
    /// <param name="bodyIndexData">The body index data.</param>
    private void BodyIndexReader_FrameArrived(GenericEventArgs<BodyIndexFrame> bodyIndexData)
    {
        if (timeOfLastBodyIndexData+minTimeBetweenFrames > Time.time) return;

        if (activeUserIndex == -1) return;
        else
        {
            sensorDataImage.color = Color.white;
        }

        if (bodyIndexColors == null || bodyIndexColors.Length != bodyIndexData.Args.PixelCount)
        {
            bodyIndexColors = new Color32[bodyIndexData.Args.PixelCount];
        }

        for (int i = 0; i < bodyIndexColors.Length; i++)
        {
            if (bodyIndexData.Args.Pixels[i] == activeUserIndex)
            {
                bodyIndexColors[i] = activeUserColor;
            }
            else if (bodyIndexData.Args.Pixels[i] != 255)
            {
                bodyIndexColors[i] = inactiveUserColor;
            }
            else
            {
                bodyIndexColors[i] = backgroundColor;
            }
        }

        timeOfLastBodyIndexData = Time.time;
        sensorTexture.SetPixels32(bodyIndexColors);
        sensorTexture.Apply();
    }
    #endregion

    #region Sensor Text Feedback
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

        if (hasFoundSensor && HasNotFoundUser())
        {
            SetText(noUserFound);
        }
    }

    /// <summary>
    /// Uses the user body data to display feedback to them on positioning.
    /// </summary>
    /// <param name="skeleton">The skeleton currently being tracked.</param>
    protected override void UseUserData(Skeleton skeleton)
    {
        if (!gameObject.activeInHierarchy) return;

        activeUserIndex = skeleton.trackingIndex;

        DisplayFeedback(skeleton.joints[(int)JointType.SpineBase].position);
    }

    /// <summary>
    /// Displays feedback to the user based on there position relative to the sensor.
    /// </summary>
    /// <param name="position">The position in the camera space of the sensor.</param>
    private void DisplayFeedback(Vector3 position)
    {
        if (position.z < minZDist)
        {
            SetText(zDistCloseMessage);
        }
        else if (position.z > maxZDist)
        {
            SetText(zDistFarMessage);
        }
        else if (position.x < -maxXDist)
        {
            SetText(moveRightMessage);
        }
        else if (position.x > maxXDist)
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
        feedbackText.text = newText;
    }
    #endregion
    #endregion
}
