/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/24/2023 2:28:33 PM
 * 
 * Description: Sends out events based on button inputs from the
 *              user.
******************************************************************/
using UnityEngine;
using UnityEngine.Events;

public class ControllerInput : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// Event fires when ever the up or down buttons are pressed on the controller.
    /// 1 is for up
    /// -1 is for down
    /// </summary>
    public static UnityEvent<int> UpDownEvent = new UnityEvent<int>();

    /// <summary>
    /// Event fires when ever the ok button is pressed on the controller.
    /// </summary>
    public static UnityEvent OkEvent = new UnityEvent();

    /// <summary>
    /// Event fires when ever the start/stop button is pressed on the controller.
    /// </summary>
    public static UnityEvent StartStopEvent = new UnityEvent();

    /// <summary>
    /// Event fires when ever the rewind button is pressed on the controller.
    /// </summary>
    public static UnityEvent RewindEvent = new UnityEvent();

    /// <summary>
    /// Event fires when ever the next button is pressed on the controller.
    /// </summary>
    public static UnityEvent NextEvent = new UnityEvent();
    #endregion

    #region Functions
    /// <summary>
    /// Gets keyboard inputs for testing purposes.
    /// </summary>
    private void Update()
    {
        KeyboardInput();
        ControllerInputs();
    }

    /// <summary>
    /// Gets inputs from the remote Virtusense remote control.
    /// </summary>
    private void ControllerInputs()
    {
        if (Input.GetKeyDown(KeyCode.F9))
        {
            UpDownEvent.Invoke(1);
        }

        if (Input.GetKeyDown(KeyCode.F6))
        {
            UpDownEvent.Invoke(-1);
        }

        if (Input.GetKeyDown(KeyCode.PageDown))
        {
            OkEvent.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F8))
        {
            NextEvent.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.PageUp))
        {
            StartStopEvent.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            RewindEvent.Invoke();
        }
    }

    /// <summary>
    /// Gets inputs from the keyboard, mainly for testing purposes.
    /// </summary>
    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            UpDownEvent.Invoke(1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpDownEvent.Invoke(-1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            OkEvent.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            NextEvent.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            StartStopEvent.Invoke();
        }

        if (Input.GetKeyDown(KeyCode.Backspace))
        {
            RewindEvent.Invoke();
        }
    }
    #endregion
}
