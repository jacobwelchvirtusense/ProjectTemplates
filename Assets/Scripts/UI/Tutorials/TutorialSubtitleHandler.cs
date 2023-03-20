/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 2/20/2023 9:37:47 AM
 * 
 * Description: Handles displaying of subtitles for a tutorial dialogue.
*********************************/
using TMPro;
using UnityEngine;

public class TutorialSubtitleHandler : MonoBehaviour
{
    #region Fields
    [Tooltip("The text used to display the subtitles")]
    [SerializeField] private TextMeshProUGUI subtitleText;
    #endregion

    #region Functions
    /// <summary>
    /// Sets the subtitle to be displayed (if an empty string this object is disabled).
    /// </summary>
    /// <param name="subtitle">The text to be displayed.</param>
    public void SetSubtitle(string subtitle)
    {
        var subtileValid = subtitle != "";
        gameObject.SetActive(subtileValid);
        subtitleText.text = subtitle;
    }
    #endregion
}
