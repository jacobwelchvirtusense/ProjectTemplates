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
    protected override void Awake()
    {
        base.Awake();

        buttons = GetComponentsInChildren<Button>();
    }

    /// <summary>
    /// Calls for the currently selected button to be updated.
    /// </summary>
    /// <param name="mod">-1 is down and 1 is up.</param>
    public override void UpdateSelectedButton(int mod, bool shouldPlaySound = true)
    {
        base.UpdateSelectedButton(mod, shouldPlaySound);

        currentButtonSlot = (currentButtonSlot + mod) % buttons.Length;

        if (currentButtonSlot < 0)
        {
            currentButtonSlot = buttons.Length - 1;
        }

        buttons[currentButtonSlot].Select();
    }

    /// <summary>
    /// Performs the click event of the currently selected settings slot.
    /// </summary>
    public override void ClickSlot()
    {
        base.ClickSlot();

        buttons[currentButtonSlot].onClick.Invoke();
    }
    #endregion
}
