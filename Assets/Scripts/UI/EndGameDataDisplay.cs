/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 1/30/2023 9:44:58 AM
 * 
 * Description: Handles displays of data at the end of the game.
*********************************/
using TMPro;
using UnityEngine;
using static ValidCheck;

public class EndGameDataDisplay : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The text that is displaying the end game data.
    /// </summary>
    private TextMeshProUGUI endGameDataText;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes components and fields.
    /// </summary>
    private void Awake()
    {
        InitializeText();
    }

    /// <summary>
    /// Initializes the text for the end game data to be displayed on.
    /// </summary>
    private void InitializeText()
    {
        endGameDataText = GetComponent<TextMeshProUGUI>();
    }

    /// <summary>
    /// Updates the data that is to be displayed.
    /// </summary>
    /// <param name="displayText">The new text to display at this spot.</param>
    public void UpdateText(string displayText)
    {
        if (IsntValid(endGameDataText)) InitializeText();

        endGameDataText.text = displayText;
    }
    #endregion
}
