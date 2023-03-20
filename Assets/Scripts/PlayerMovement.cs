/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/20/2023 10:44:36 AM
 * 
 * Description: The start of a playermovement script.
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Windows.Kinect;
using static InspectorValues;
using static ValidCheck;

public class PlayerMovement : SensorDataListener
{
    #region Fields
    #region Kinnect
    private JointType[] jointsToUse = new JointType[]
    {
        JointType.SpineBase,
        JointType.SpineShoulder
    };
    #endregion
    #endregion

    #region Functions
    protected override void UseUserData(Body body)
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
