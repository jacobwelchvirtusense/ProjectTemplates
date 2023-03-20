/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 2/17/2023 4:37:45 PM
 * 
 * Description: Handles the funcitonality of buttons that get underlined text.
*********************************/
using System.Collections.Generic;
using TMPro;
using UnityEngine.UI;

public class UnderlinedButtonController : BasicButtonController
{
    #region Fields
    /// <summary>
    /// The list of texts that need to be underlined.
    /// </summary>
    private List<TextMeshProUGUI> buttonTexts = new List<TextMeshProUGUI>();
    #endregion

    #region Functions
    /// <summary>
    /// Gets components and sets their initial states.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        foreach(Button button in buttons) 
        {
            buttonTexts.Add(button.transform.parent.gameObject.GetComponentInChildren<TextMeshProUGUI>());
        }
    }

    /// <summary>
    /// Calls for the currently selected button to be updated.
    /// </summary>
    /// <param name="mod">-1 is down and 1 is up.</param>
    protected override void UpdateSelectedButton(int mod)
    {
        var previousButtonSlot = currentButtonSlot;

        base.UpdateSelectedButton(mod);

        if (currentButtonSlot < 0 || buttonTexts.Count <= previousButtonSlot || buttonTexts.Count <= currentButtonSlot) return;

        buttonTexts[previousButtonSlot].fontStyle = FontStyles.Normal;
        buttonTexts[currentButtonSlot].fontStyle = FontStyles.Underline;
    }
    #endregion
}
