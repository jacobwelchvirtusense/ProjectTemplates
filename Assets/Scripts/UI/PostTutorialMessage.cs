/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/10/2023 12:03:59 PM
 * 
 * Description: Handles the functionality for the post tutorial message.
*********************************/
using UnityEngine;
using static ValidCheck;

public class PostTutorialMessage : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// Holds true if the message should be shown.
    /// </summary>
    public static bool showMessage;

    [Tooltip("The parent object for the whole settings panel")]
    [SerializeField] private GameObject settings;

    [Tooltip("The parent object for the whole post tutorial message")]
    [SerializeField] private GameObject postTutorialMessage;
    #endregion

    #region Functions
    /// <summary>
    /// Checks if the message should be displayed.
    /// </summary>
    private void Start()
    {
        if (showMessage)
        {
            // Sets the message to be active and settings to not be active
            showMessage = false;
            postTutorialMessage.SetActive(true);
            settings.SetActive(false);
        }
    }

    /// <summary>
    /// Enables and disables the post tutorial message.
    /// </summary>
    /// <param name="shouldShow">Holds true if the panel should be enabled.</param>
    public void ShowMessage(bool shouldShow)
    {
        if (IsntValid(postTutorialMessage)) return;

        postTutorialMessage.SetActive(shouldShow);
    }

    /// <summary>
    /// Enables and disables the settings panel.
    /// </summary>
    /// <param name="shouldShow">Holds true if the panel should be enabled.</param>
    public void ShowSettings(bool shouldShow)
    {
        if (IsntValid(settings)) return;

        settings.SetActive(shouldShow);
    }
    #endregion
}
