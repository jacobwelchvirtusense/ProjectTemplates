/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/20/2023 10:44:36 AM
 * 
 * Description: The start of a playermovement script for wheel chair 
 *              users.
******************************************************************/
using Assets.SensorAdapters;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using static InspectorValues;
using static UnityEditor.PlayerSettings;
using static ValidCheck;

public class WheelChairMovement : SensorDataListener
{
    #region Fields
    [Header("Sensor Parameters")]
    [Range(0.0f, 3.0f)]
    [Tooltip("The x position the user can be relative to the sensor")]
    [SerializeField] private float maxUserXPos = 5.0f;

    [Range(0.0f, 6.0f)]
    [Tooltip("The minimum z position the user can be relative to the sensor")]
    [SerializeField] private float minUserZPos = 0.5f;

    [Range(0.0f, 6.0f)]
    [Tooltip("The maximum z position the user can be relative to the sensor")]
    [SerializeField] private float maxUserZPos = 5.0f;

    [Header("In-Game Character Parameters")]
    [Range(0.0f, 15.0f)]
    [Tooltip("The maximum x position the in-game character will be based on user XPos")]
    [SerializeField] private float maxCharacterXPos = 10.0f;

    [Range(0.0f, 15.0f)]
    [Tooltip("The maximum Y position the in-game character will be based on user ZPos")]
    [SerializeField] private float maxCharacterYPos = 8.0f;

    [Range(0.0f, 200.0f)]
    [Tooltip("The amount of smoothing to be applied to the movement to reduce jittering")]
    [SerializeField] private float movementSmoothing = 10;

    [Header("Extra movement settings")]
    [Tooltip("Clamps the users movement to be solely on the x axis")]
    [SerializeField] private bool clampToX;

    [Tooltip("Clamps the users movement to be solely on the y axis")]
    [SerializeField] private bool clampToY;

    [Tooltip("Inverts the movement inputs of the user")]
    [SerializeField] private bool invertInput;

    [Header("Data Tracking")]
    [Tooltip("The minimum amount of delta movement before showing an animtion")]
    [SerializeField] private float minimumDataMovementDelta = 0.0f;

    /// <summary>
    /// The position that this object started at.
    /// </summary>
    private Vector3 startingPosition;

    /// <summary>
    /// The time the last frame from the sensor was recieved.
    /// </summary>
    private float timeOfLastDataFrame = 0;

    /// <summary>
    /// The joint to be used in the calculations. Somewhere on the head is likely best for wheel chair users.
    /// </summary>
    private JointType headJoint = JointType.Neck;

    /// <summary>
    /// The last z position of the user.
    /// </summary>
    private Vector2 lastZPosition = Vector2.zero;

    /// <summary>
    /// The total movement for this user.
    /// </summary>
    private float totalMovement = 0.0f;
    #endregion

    #region Functions
    protected override void Awake()
    {
        base.Awake();

        startingPosition = transform.position;
    }

    private void FixedUpdate()
    {
        UseUserData(BodySourceManager.Player1Skeleton);
    }

    /// <summary>
    /// Uses the users data to move their in-game character.
    /// </summary>
    /// <param name="skeleton">The skeleton of the user.</param>
    protected override void UseUserData(Skeleton skeleton)
    {
        if (skeleton == null) return;

        #region Taking input and calculating target position
        var xInput = skeleton.joints[(int)headJoint].position.x;
        var zInput = skeleton.joints[(int)headJoint].position.z;

        var targetPositionLerpX = Mathf.InverseLerp(-maxUserXPos, maxUserXPos, xInput); // Calculates the lerp of the angle
        var targetPositionLerpZ = Mathf.InverseLerp(maxUserZPos, minUserZPos, zInput); // Calculates the lerp of the angle


        if (invertInput)
        {
            var holdXInput = targetPositionLerpX;
            targetPositionLerpX = targetPositionLerpZ;
            targetPositionLerpZ = holdXInput;
        }

        var targetX = Mathf.Lerp(-maxCharacterXPos, maxCharacterXPos, targetPositionLerpX) + startingPosition.x;
        var targetY = Mathf.Lerp(-maxCharacterYPos, maxCharacterYPos, targetPositionLerpZ) + startingPosition.y;
        #endregion

        #region Clamping Inputs
        if (clampToX)
        {
            targetY = transform.position.y;
        }

        if (clampToY)
        {
            targetX = transform.position.x;
        }
        #endregion

        #region Moving In-Game Character
        var pos = transform.position;
        var timeDelta = TimeSinceLastDataFrame();
        pos.x = Mathf.Lerp(pos.x, targetX, timeDelta * movementSmoothing);
        pos.y = Mathf.Lerp(pos.y, targetY, timeDelta * movementSmoothing);
        transform.position = pos;

        timeOfLastDataFrame = Time.time;
        #endregion

        CalculateUserMoveDelta(new Vector2(xInput, zInput));
    }

    private float TimeSinceLastDataFrame()
    {
        return Time.fixedDeltaTime;

        //return Time.time - timeOfLastDataFrame;
    }

    private void CalculateUserMoveDelta(Vector2 newLateralMovement)
    {
        if (!GameController.GameplayActive) return;

        if (lastZPosition != Vector2.zero)
        {
            var xDelta = Mathf.Abs(lastZPosition.x - newLateralMovement.x);
            var yDelta = Mathf.Abs(lastZPosition.y - newLateralMovement.y);

            if (xDelta+yDelta > minimumDataMovementDelta)
            {
                totalMovement += xDelta+yDelta;
                UIManager.UpdateEndGameData(UIManager.EndGameDataType.MOVEMENT, totalMovement.ToString().Substring(0, 4));
            }
        }

        lastZPosition = newLateralMovement;
    }
    #endregion
}
