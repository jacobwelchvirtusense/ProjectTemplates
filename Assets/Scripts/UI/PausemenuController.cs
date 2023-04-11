/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 4/10/2023 10:02:13 AM
 * 
 * Description: Handles the opening and closing of the pause menu.
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using static InspectorValues;
using static ValidCheck;

public class PausemenuController : MonoBehaviour
{
    #region Fields
    [Tooltip("The disable parent object of the pausemenu")]
    [SerializeField] private GameObject pauseMenu;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes button events for the pause menu controller.
    /// </summary>
    private void Awake()
    {
        ControllerInput.StartStopEvent.AddListener(ChangeMenuState);        
    }

    /// <summary>
    /// Changes whether the pause menu is open or not.
    /// </summary>
    public void ChangeMenuState()
    {
        if (GameController.GameplayActive || TutorialManager.TutorialActive)
        {
            pauseMenu.SetActive(!pauseMenu.activeSelf);
            Time.timeScale = pauseMenu.activeSelf ? 0.0f : 1.0f;

            TutorialVideoHandler.PauseVideo(pauseMenu.activeSelf);
        }
    }
    #endregion
}
