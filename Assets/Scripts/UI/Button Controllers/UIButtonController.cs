/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/16/2023 9:42:07 AM
 * 
 * Description: A base class for taking in all inputs to menus and handling
 *              moving between buttons in them.
*********************************/
using UnityEngine;

public abstract class UIButtonController : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The current button slot that is selected.
    /// </summary>
    protected int currentButtonSlot;
    #endregion

    #region Functions
    /// <summary>
    /// Resets the selected button to be the first one.
    /// </summary>
    protected virtual void OnEnable()
    {
        currentButtonSlot = 0;
        UpdateSelectedButton(0);
    }

    #region Input
    /// <summary>
    /// Gets keyboard inputs for testing purposes.
    /// </summary>
    private void Update()
    {
        KeyboardInput();
    }

    /// <summary>
    /// Gets keyboard input for navigating buttons
    /// </summary>
    private void KeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            UpdateSelectedButton(1);
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            UpdateSelectedButton(-1);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ClickSlot();
        }
    }
    #endregion

    #region Input Events
    /// <summary>
    /// Calls for the currently selected button to be updated.
    /// </summary>
    /// <param name="mod">-1 is down and 1 is up.</param>
    protected virtual void UpdateSelectedButton(int mod)
    {
        if(mod != 0)
        {
            UISoundManager.PlayHoverSound();
        }
    }

    /// <summary>
    /// Performs the click event of the currently selected button.
    /// </summary>
    protected virtual void ClickSlot()
    {
        UISoundManager.PlayHoverSound();
    }

    /// <summary>
    /// Performs an action when the user hits the back button on the remote.
    /// </summary>
    protected abstract void BackEvent();

    /// <summary>
    /// Performs an action when the user hits the next button on the remote.
    /// </summary>
    protected abstract void NextEvent();
    #endregion
    #endregion
}
