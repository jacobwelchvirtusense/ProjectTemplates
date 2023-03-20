/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/7/2023 4:01:28 PM
 * 
 * Description: Handles extra functionality for the tutorial prompt.
*********************************/

public class TutorialPromptButtonsController : BasicButtonController
{
    #region Functions
    /// <summary>
    /// Opens the tutorial prompt.
    /// </summary>
    public void OpenTutorialPrompt()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// Closes the tutorial prompt.
    /// </summary>
    public void CloseTutorialPrompt()
    {
        gameObject.SetActive(false);
    }
    #endregion
}
