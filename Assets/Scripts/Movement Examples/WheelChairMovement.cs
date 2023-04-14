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
    [Tooltip("The x position the user can be relative to the sensor")]
    [SerializeField] private float maxUserXPos = 5.0f;

    [Tooltip("The minimum z position the user can be relative to the sensor")]
    [SerializeField] private float minUserZPos = 0.5f;

    [Tooltip("The maximum z position the user can be relative to the sensor")]
    [SerializeField] private float maxUserZPos = 5.0f;

    [Header("In-Game Character Parameters")]
    [Tooltip("The maximum x position the in-game character will be based on user XPos")]
    [SerializeField] private float maxCharacterXPos = 10.0f;



    [Tooltip("The maximum Y position the in-game character will be based on user ZPos")]
    [SerializeField] private float maxCharacterYPos = 8.0f;

    [Tooltip("The amount of smoothing to be applied to the movement to reduce jittering")]
    [SerializeField] private float movementSmoothing = 10;

    #region ToDO
    [Tooltip("")]
    [SerializeField] private bool clampToX;

    [Tooltip("")]
    [SerializeField] private bool clampToY;
 
    [Tooltip("")]
    [SerializeField] private bool invertInput;
    #endregion

    /// <summary>
    /// The time the last frame from the sensor was recieved.
    /// </summary>
    private float timeOfLastDataFrame = 0;

    /// <summary>
    /// The joint to be used in the calculations. Somewhere on the head is likely best for wheel chair users.
    /// </summary>
    private JointType headJoint = JointType.Neck;
    #endregion

    #region Functions
    protected override void UseUserData(Skeleton skeleton)
    {
        var targetPositionLerpX = Mathf.InverseLerp(-maxUserXPos, maxUserXPos, skeleton.joints[(int)headJoint].position.x); // Calculates the lerp of the angle
        var targetPositionLerpZ = Mathf.InverseLerp(maxUserZPos, minUserZPos, skeleton.joints[(int)headJoint].position.z); // Calculates the lerp of the angle

        var targetX = Mathf.Lerp(-maxCharacterXPos, maxCharacterXPos, targetPositionLerpX);
        var targetY = Mathf.Lerp(-maxCharacterYPos, maxCharacterYPos, targetPositionLerpZ);

        var pos = transform.position;
        var timeDelta = TimeSinceLastDataFrame();
        pos.x = Mathf.Lerp(pos.x, targetX, timeDelta * movementSmoothing);
        pos.y = Mathf.Lerp(pos.y, targetY, timeDelta * movementSmoothing);
        transform.position = pos;

        timeOfLastDataFrame = Time.time;

        print("User X: " + skeleton.joints[(int)headJoint].position.x);
        print("User Z: " + skeleton.joints[(int)headJoint].position.z);
    }

    private float TimeSinceLastDataFrame()
    {
        return Time.time - timeOfLastDataFrame;
    }
    #endregion
}
