/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/6/2023 10:22:42 AM
 * 
 * Description: Handles movement of the basket based on player input.
*********************************/
using Assets.SensorAdapters;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using Joint = Assets.SensorAdapters.Joint;

public class PlayerMovementExample : SensorDataListener
{
    #region Fields
    /// <summary>
    /// The size of the player object.
    /// </summary>
    private float playerSize = 10;

    private BoxCollider2D boxCollider;
    private Light2D appleDetectionLight;

    private static PlayerMovementExample instance;

    #region Movement Type
    [Range(0.0f, 15.0f)]
    [Tooltip("The max distance reaches when at or past the max body angle")]
    [SerializeField] private float maxPos = 8;

    /// <summary>
    /// The types of input for moving the basket.
    /// </summary>
    public enum MovementType { MOVE, LEAN, CATCH, NONE }

    [Tooltip("The current type of input to use for moving the basket")]
    [field:SerializeField] private MovementType currentMovementType { get; set; } = MovementType.LEAN;

    [Tooltip("The modifiers based on the selected movement type")]
    [field:SerializeField] private float[] movementTypeSpeedModifier { get; set; } = new float[3];

    /// <summary>
    /// The types of difficulty for the amount of movement needed by the user.
    /// </summary>
    public enum MovementDifficulty { EASY, MEDIUM, HARD }

    [Tooltip("The current type of difficulty needed for moving the basket")]
    [SerializeField] private MovementDifficulty currentMovementDiffculty = MovementDifficulty.MEDIUM;

    #region Move Mode
    [Header("Move Mode")]
    [Range(0.0f, 10.0f)]
    [Tooltip("The max position to check for move movement")]
    [SerializeField] private float[] maxMovePos = new float[] { 0.5f, 1.0f, 2.0f };

    [Range(0.0f, 100.0f)]
    [Tooltip("The amount of smoothing to apply to movement (higher means less smoothing)")]
    [SerializeField] private float moveMovementSmoothing = 10;
    #endregion

    #region Catch Mode
    [Header("Catch Mode")]
    [Range(0.0f, 180.0f)]
    [Tooltip("The max hand positions to check for catch movement")]
    [SerializeField] private float[] maxCatchPos = new float[] { 0.5f, 1.0f, 1.5f };

    [Range(0.0f, 100.0f)]
    [Tooltip("The amount of smoothing to apply to movement (higher means less smoothing)")]
    [SerializeField] private float catchMovementSmoothing = 10;
    #endregion

    #region Lean Mode
    [Header("Lean Mode")]
    [Range(0.0f, 30.0f)]
    [Tooltip("The max angle needed for the max distance of the basket")]
    [SerializeField] private float[] maxAngle = new float[] { 3, 6, 9 };

    [Range(0.0f, 100.0f)]
    [Tooltip("The amount of smoothing to apply to movement (higher means less smoothing)")]
    [SerializeField] private float leanMovementSmoothing = 10;
    #endregion
    #endregion

    #region Kinnect
    private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
    private BodySourceManager bodyManager;

    private JointType[] leanJoints = new JointType[]
    {
        JointType.SpineBase,
        JointType.SpineShoulder
    };

    private JointType[] moveJoints = new JointType[]
    {
        JointType.SpineBase
    };

    private JointType[] handJoints = new JointType[]
    {
        JointType.HandLeft,
        JointType.HandRight
    };
    #endregion
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Performs all actions in the awake event.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        instance = this;
        InitializeComponents();
    }

    /// <summary>
    /// Initializes all components for the player.
    /// </summary>
    private void InitializeComponents()
    {
        bodyManager = FindObjectOfType<BodySourceManager>();
        boxCollider = GetComponent<BoxCollider2D>();
        appleDetectionLight = GetComponentInChildren<Light2D>();
    }
    #endregion

    #region Settings
    public static void SetMovementType(int movementType)
    {
        instance.currentMovementType = (MovementType)movementType;
    }

    public static void SetMovementDifficulty(int difficulty)
    {
        instance.currentMovementDiffculty = (MovementDifficulty)difficulty;
    }
    #endregion

    #region Input Handling
    protected override void UseUserData(Skeleton skeleton)
    {
        RefreshBodyObject(skeleton);
    }

    /// <summary>
    /// Refreshes the events from body movement input.
    /// </summary>
    /// <param name="body">The body being checked.</param>
    private void RefreshBodyObject(Skeleton skeleton)
    {
        var targetPos = 0.0f;
        var movementSmoothing = 0.0f;

        switch (currentMovementType)
        {
            #region Move Mode
            case MovementType.MOVE:
                targetPos = CalculateMoveTargetPosition(skeleton.joints[(int)moveJoints[0]].position.x);
                movementSmoothing = moveMovementSmoothing;
                break;
            #endregion

            #region Catch Mode
            case MovementType.CATCH:

                //Vector2 centerXCatch = new Vector2(body.Joints[JointType.SpineShoulder].Position.X, body.Joints[JointType.SpineShoulder].Position.Z);
                var center = GetVector3FromJoint(skeleton.joints[(int)JointType.SpineShoulder]);
                center.x -= 0.0f;
                var handVector1 = GetVector3FromJoint(skeleton.joints[(int)handJoints[0]]);
                var handVector2 = GetVector3FromJoint(skeleton.joints[(int)handJoints[1]]);
                var handVector = (handVector1 + handVector2)/2;
                handVector.y = center.y;

                var handDir = handVector-center;
                var handangle = (Vector3.Angle(handDir, Vector3.forward)-180)*-Mathf.Sign(handVector.x-center.x);
                /*               Vector2 handDir = (new Vector2(handVector.x, handVector.z) - centerXCatch);
                var handangle = Vector2.Angle(handDir, Vector2.right) * Mathf.Sign(handVector.x);*/

                //print("Hand 1: " + handVector1);
                //print("Hand 2: " + handVector2);

                print("dir: " + handDir);
                //print("Angle: " + handangle);

                targetPos = CalculateCatchTargetPosition(handangle);

                /*
                var handVector1 = GetVector3FromJoint(body.Joints[handJoints[0]]);
                Vector2 handDir1 = (new Vector2(handVector1.x, handVector1.z) - centerXCatch);
                var handangle1 = Vector2.Angle(handDir1, Vector2.up) * Mathf.Sign(handVector1.x);
                
                print("dir1: " + handDir1);
                print("Angle1: " + handangle1);

                var handVector2 = GetVector3FromJoint(body.Joints[handJoints[1]]);
                Vector2 handDir2 = (new Vector2(handVector2.x, handVector2.z) - centerXCatch);
                var handangle2 = Vector2.Angle(handDir2, Vector2.up) * Mathf.Sign(handVector2.x);

                print("dir2: " + handDir2);
                print("Angle2: " + handangle2);
                */

                //targetPos = CalculateCatchTargetPosition((handangle1+handangle2)/2);
                //targetPos = CalculateCatchTargetPosition(body.Joints[handJoints[0]].Position.X - centerXCatch, body.Joints[handJoints[1]].Position.X) - centerXCatch;
                movementSmoothing = catchMovementSmoothing;
                break;
            #endregion

            #region Lean Mode
            case MovementType.LEAN:
                var dir = ((Vector2)(GetVector3FromJoint(skeleton.joints[(int)leanJoints[0]]) - GetVector3FromJoint(skeleton.joints[(int)leanJoints[1]])));
                dir.x -= 0.2f;

                var angle = (180 - Vector2.Angle(dir, Vector2.up)) * -Mathf.Sign(dir.x);
                targetPos = CalculateLeanTargetPosition(angle);
                movementSmoothing = leanMovementSmoothing;
                break;
            #endregion

            default:
                break;
        }

        UpdateBasketPosition(targetPos, movementSmoothing);
    }

    /// <summary>
    /// Gets the vector3 position of the joint object.
    /// </summary>
    /// <param name="joint">The joint whose position is being coverted to Vector3.</param>
    /// <returns></returns>
    private Vector3 GetVector3FromJoint(Joint joint)
    {
        return new Vector3(joint.position.x * playerSize, joint.position.y * playerSize, joint.position.z * playerSize);
    }
    #endregion

    #region Calculate Target Position
    /// <summary>
    /// Calculates the current lerp of the players spine angle.
    /// </summary>
    /// <param name="angle">The angle of the users spine.</param>
    /// <returns></returns>
    private float CalculateLeanTargetPosition(float angle)
    {
        var difficulty = (int)currentMovementDiffculty;
        var targetPositionLerp = Mathf.InverseLerp(-maxAngle[difficulty], maxAngle[difficulty], angle); // Calculates the lerp of the angle

        return targetPositionLerp;
    }

    /// <summary>
    /// Calculates the current lerp based on the users x position from the center of the screen.
    /// </summary>
    /// <param name="jointXPos">The x position from the center of the sensor.</param>
    /// <returns></returns>
    private float CalculateMoveTargetPosition(float jointXPos)
    {
        var difficulty = (int)currentMovementDiffculty;
        var targetPositionLerp = Mathf.InverseLerp(-maxMovePos[difficulty], maxMovePos[difficulty], jointXPos);

        return targetPositionLerp;
    }

    /// <summary>
    /// Calculates the current lerp based on the position of the users hands.
    /// </summary>
    /// <param name="hand1Pos">The position of the users left hand.</param>
    /// <param name="hand2Pos">The position of the users right hand.</param>
    /// <returns></returns>
    private float CalculateCatchTargetPosition(float angle)
    {
        var difficulty = (int)currentMovementDiffculty;
        var targetLerp = Mathf.InverseLerp(-maxCatchPos[difficulty], maxCatchPos[difficulty], angle);

        return targetLerp;
    }

    /*
    /// <summary>
    /// Calculates the current lerp based on the position of the users hands.
    /// </summary>
    /// <param name="hand1Pos">The position of the users left hand.</param>
    /// <param name="hand2Pos">The position of the users right hand.</param>
    /// <returns></returns>
    private float CalculateCatchTargetPosition(float hand1Pos, float hand2Pos)
    {
        var difficulty = (int)currentMovementDiffculty;
        var targetPosition1 = Mathf.InverseLerp(-maxCatchPos[difficulty], maxCatchPos[difficulty], hand1Pos);
        var targetPosition2 = Mathf.InverseLerp(-maxCatchPos[difficulty], maxCatchPos[difficulty], hand2Pos);
        var targetPositionLerp = (targetPosition1 + targetPosition2) / 2;

        return targetPositionLerp;
    }*/
    #endregion

    /// <summary>
    /// Updates the position of the basket to move towards the current lerp value based on movement smoothing.
    /// </summary>
    /// <param name="targetPositionLerp">The current target lerp for the basket position.</param>
    /// <param name="movementSmoothing">The amount to smooth the basket movement.</param>
    private void UpdateBasketPosition(float targetPositionLerp, float movementSmoothing)
    {
        var pos = transform.position;
        var targetPosition = Mathf.Lerp(-maxPos, maxPos, targetPositionLerp);  // Calculates desired position based on angle

        // Sets the positions of the basket
        pos.x = Mathf.Lerp(pos.x, targetPosition, Time.fixedDeltaTime*movementSmoothing);
        transform.position = pos;
    }

    public static float SpeedGameMod()
    {
        return instance.movementTypeSpeedModifier[(int)instance.currentMovementType];
    }

    public static void LockMovement()
    {
        Vector3 pos = instance.transform.position;
        pos.x = 0;

        instance.transform.position = pos;
        instance.currentMovementType = MovementType.NONE;
    }
    #endregion
}
