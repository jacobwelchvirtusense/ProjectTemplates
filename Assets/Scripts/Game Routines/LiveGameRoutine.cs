/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 4/14/2023 2:45:33 PM
 * 
 * Description: A routine of gameplay that involves waiting for a
 *              user to run out of lives.
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public class LiveGameRoutine : IGameRoutine
{
    #region Fields
    [Tooltip("The minimum value for the timer in seconds")]
    [SerializeField] private int[] numberOfLives = new int[] { 3, 5, 7 };

    /// <summary>
    /// The current number of lives left for the user.
    /// </summary>
    private int livesLeft = 3;

    /// <summary>
    /// A getter and setter for the number of lives left.
    /// </summary>
    public int LivesLeft
    {
        get => livesLeft;
        set
        {
            livesLeft = value;
            UIManager.UpdateLivesLeft(livesLeft);
        }
    }

    /// <summary>
    /// The current setting slot of lives to be used.
    /// </summary>
    private int currentSlot = 0;

    /// <summary>
    /// The scene instance of the LiveGameRoutine.
    /// </summary>
    private static LiveGameRoutine Instance;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes the scene instance.
    /// </summary>
    private void Awake()
    {
        Instance = this;

        livesLeft = numberOfLives[0];
    }

    /// <summary>
    /// Updates the current life slot within the array of life options.
    /// </summary>
    /// <param name="newLifeSlot">The new slot of the life array to use.</param>
    private void UpdateToNewLifeSlot(int newLifeSlot)
    {
        currentSlot = newLifeSlot;

        if (numberOfLives.Length > currentSlot)
        {
            LivesLeft = numberOfLives[currentSlot];
        }
    }

    /// <summary>
    /// A gameplay routine that involves waiting for the users lives to run out.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator GameplayRoutine()
    {
        while (livesLeft != 0)
        {
            yield return new WaitForEndOfFrame();
        }
    }

    /// <summary>
    /// Returns the current starting lives that is being used in the game.
    /// </summary>
    /// <returns></returns>
    public static int GetStartingLifeAmount()
    {
        if (Instance == null || Instance.numberOfLives.Length <= Instance.currentSlot) return 0;

        return Instance.numberOfLives[Instance.currentSlot];
    }

    /// <summary>
    /// Sets this routine as the active routine and initializes it.
    /// </summary>
    public override void SetActive(bool shouldSetActive)
    {
        if (shouldSetActive)
        {
            UIManager.InitializeTimer(-1);

            SettingsManager.Slot1Update.AddListener(UpdateToNewLifeSlot);
        }
        else
        {
            SettingsManager.Slot1Update.RemoveListener(UpdateToNewLifeSlot);
        }
    }
    #endregion
}
