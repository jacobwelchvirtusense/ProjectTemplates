/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/16/2023 9:53:35 AM
 * 
 * Description: Handles the functionality for basic buttons
 *              that use unity's built in button set up.
 *              Buttons are selected and unselected when inputs
 *              are recieved.
*********************************/
using UnityEngine.UI;

public class BasicButtonController : UIButtonController
{
    #region Fields
    /// <summary>
    /// The buttons that are a part of this menu.
    /// </summary>
    protected Button[] buttons;
    #endregion

    #region Functions
    /// <summary>
    /// Gets components and sets their initial states.
    /// </summary>
    protected virtual void Awake()
    {
        buttons = GetComponentsInChildren<Button>();
    }

    /// <summary>
    /// Calls for the currently selected button to be updated.
    /// </summary>
    /// <param name="mod">-1 is down and 1 is up.</param>
    protected override void UpdateSelectedButton(int mod)
    {
        currentButtonSlot = (currentButtonSlot + mod) % buttons.Length;

        if (currentButtonSlot < 0)
        {
            currentButtonSlot = buttons.Length - 1;
        }

        buttons[currentButtonSlot].Select();

        base.UpdateSelectedButton(mod);
    }

    /// <summary>
    /// Performs the click event of the currently selected settings slot.
    /// </summary>
    protected override void ClickSlot()
    {
        buttons[currentButtonSlot].onClick.Invoke();
        base.ClickSlot();
    }

    /// <summary>
    /// Performs an action when the user hits the back button on the remote.
    /// </summary>
    protected override void BackEvent()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Performs an action when the user hits the next button on the remote.
    /// </summary>
    protected override void NextEvent()
    {
        throw new System.NotImplementedException();
    }
    #endregion
}
