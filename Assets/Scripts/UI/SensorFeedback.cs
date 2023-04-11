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
using UnityEngine;
using UnityEngine.UI;
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

    // TODO
    #region Data Visualization
    [SerializeField] private RawImage sensorDataImage;
    private Texture2D sensorTexture;
    Color32[] bodyIndexColors;
    #endregion

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

    #region Sensor Data Visualization
    public void InitializeTexture(ImageSize bodyIndexImageSize)
    {
        sensorTexture = new Texture2D(bodyIndexImageSize.Width, bodyIndexImageSize.Height);

        if (sensorDataImage)
            sensorDataImage.texture = sensorTexture;
    }

    public void OnNewBodyIndexFrame(object sender, GenericEventArgs<BodyIndexFrame> e)
    {
        if (e.Args != null) BodyIndexReader_FrameArrived(e);
    }

    void BodyIndexReader_FrameArrived(GenericEventArgs<BodyIndexFrame> bodyIndexData)
    {

        var bodyColor = new Color32(0, 255, 0, 255);

        if (bodyIndexColors == null || bodyIndexColors.Length != bodyIndexData.Args.PixelCount)
        {
            bodyIndexColors = new Color32[bodyIndexData.Args.PixelCount];
        }

        for (int i = 0; i < bodyIndexColors.Length; i++)
        {
            byte index = bodyIndexData.Args.Pixels[i];

            if (index != 255)
            {
                bodyIndexColors[i] = new Color32(0, 255, 0, 255); // green; displays most of the time

            }
            else
            {
                bodyIndexColors[i] = new Color32(32, 32, 32, 50); // black for background
            }
        }

        sensorTexture.SetPixels32(bodyIndexColors);
        sensorTexture.Apply();
    }
    #endregion

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
    protected override void UseUserData(Skeleton skeleton)
    {
        DisplayFeedback(skeleton.joints[(int)JointType.SpineBase].position);
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
