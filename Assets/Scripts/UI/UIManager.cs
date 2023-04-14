/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 1/6/2023 10:25:04 AM
 * 
 * Description: Handles the functionality of all
 *              UI assets.
*********************************/
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static ValidCheck;
using static TimerGameRoutine;

public class UIManager : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The instance of the UI manager in the scene.
    /// </summary>
    private static UIManager Instance;

    #region Lives Mode
    [Header("Lives Mode")]
    [Tooltip("The UI parent for the lives based mode")]
    [SerializeField] private GameObject livesUI;

    [Tooltip("The array of lives to be used in the lives based mode")]
    [SerializeField] private GameObject[] hearts;
    #endregion

    #region Timer
    [Header("Timer Mode")]
    [Tooltip("The current timer that the user has left")]
    [SerializeField] private TextMeshProUGUI timerUI;

    [Tooltip("The left image for the timer bar")]
    [SerializeField] private Image timerBar1;

    [Tooltip("The right image for the timer bar")]
    [SerializeField] private Image timerBar2;
    #endregion
    
    [Header("All modes")]
    [Tooltip("The score the user currently has")]
    [SerializeField] private TextMeshProUGUI score;

    [Tooltip("The current combo that the user has")]
    [SerializeField] private TextMeshProUGUI combo;

    [Tooltip("The end screen for the game")]
    [SerializeField] private GameObject endScreen;

    #region End Game Data Displays
    [Tooltip("The end game display for the first slot")]
    [SerializeField] private EndGameDataDisplay endGameDisplay1;

    [Tooltip("The end game display for the second slot")]
    [SerializeField] private EndGameDataDisplay endGameDisplay2;

    [Tooltip("The end game display for the third slot")]
    [SerializeField] private EndGameDataDisplay endGameDisplay3;

    [Tooltip("The end game display for the fourth slot")]
    [SerializeField] private EndGameDataDisplay endGameDisplay4;

    [Tooltip("The end game display for the fifth slot")]
    [SerializeField] private EndGameDataDisplay endGameDisplay5;
    #endregion
    #endregion

    #region Functions
    /// <summary>
    /// Initializes all aspects of the UI manager.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    #region UI Updates
    /// <summary>
    /// Updates the dipslay of the current score.
    /// </summary>
    /// <param name="newScore">The current score the player has.</param>
    public static void UpdateScore(int newScore)
    {
        if (InstanceDoesntExist() || IsntValid(Instance.score)) return;

        // Updates the score UI 
        Instance.score.text = newScore.ToString();
        UpdateScoreEndGame(newScore);
    }

    /// <summary>
    /// Updates the displayed score for at the end of the game.
    /// </summary>
    /// <param name="newScore">The new score the player has.</param>
    private static void UpdateScoreEndGame(int newScore)
    {
        if (IsntValid(Instance.endGameDisplay1)) return;
        Instance.endGameDisplay1.UpdateText(newScore.ToString());
    }

    /// <summary>
    /// Updates the displayed number of lives.
    /// </summary>
    /// <param name="newLives">The number of lives the user currently has.</param>
    public static void UpdateLivesLeft(int newLives)
    {
        for (int i = 0; i < Instance.hearts.Length; i++)
        {
            if (i < Instance.hearts.Length)
            {
                Instance.hearts[i].SetActive(i >= Instance.hearts.Length-newLives);
            }
        }
    }

    #region Timer
    /// <summary>
    /// Sets the timers initial value.
    /// </summary>
    public static void InitializeTimer(int timer = 0)
    {
        if (timer == 0) timer = GetTimerAmount();

        print("Timer: " + timer);

        UpdateTimer(timer);
    }

    /// <summary>
    /// Updates the timer to its current time.
    /// </summary>
    /// <param name="newTime">The current time left of the timer.</param>
    public static void UpdateTimer(float newTime)
    {
        if (InstanceDoesntExist() || IsntValid(Instance.timerUI)) return;

        // Updates the timer UI 
        Instance.timerUI.text = GetTimerValue(newTime);
        Instance.timerUI.gameObject.SetActive(newTime != 0);
        Instance.timerUI.gameObject.SetActive(newTime != -1);

        if (IsValid(Instance.livesUI))
        {
            Instance.livesUI.SetActive(newTime == -1);
        }

        UpdateTimerBars(newTime);
    }

    /// <summary>
    /// Gets the string for displaying how much time is left.
    /// </summary>
    /// <param name="newTime">The current amount of time left</param>
    /// <returns></returns>
    public static string GetTimerValue(float newTime)
    {
        var seconds = (int)newTime;
        var minutes = seconds / 60;
        var leftOverSeconds = (seconds - (minutes * 60));
        string secondsDisplayed = "";

        if (leftOverSeconds < 10) secondsDisplayed += "0";
        secondsDisplayed += leftOverSeconds;

        return minutes.ToString() + ":" + secondsDisplayed;
    }

    /// <summary>
    /// Updates the timer bars to be a percentage of the remaining time left.
    /// </summary>
    /// <param name="newTime">The current amount of time left.</param>
    private static void UpdateTimerBars(float newTime)
    {
        if (IsntValid(Instance.timerBar1) || IsntValid(Instance.timerBar2)) return;

        var isNotInfinite = newTime != -1;
        Instance.timerBar1.gameObject.SetActive(isNotInfinite);
        Instance.timerBar2.gameObject.SetActive(isNotInfinite);

        Instance.timerBar1.fillAmount = newTime / GetTimerAmount();
        Instance.timerBar2.fillAmount = newTime / GetTimerAmount();
    }
    #endregion

    /// <summary>
    /// Updates the displayed combo during the game.
    /// </summary>
    /// <param name="newCombo">The new combo of the player.</param>
    public static void UpdateCombo(int newCombo)
    {
        if (InstanceDoesntExist() || IsntValid(Instance.combo)) return;

        Instance.combo.text = "x" + newCombo.ToString();
    }

    /// <summary>
    /// Displays the message that should appear at the end of the game.
    /// </summary>
    public static void DisplayEndScreen()
    {
        if (InstanceDoesntExist() || IsntValid(Instance.endScreen)) return;

        Instance.endScreen.SetActive(true);
    }
    #endregion

    /// <summary>
    /// Updates end game displays to have certain data.
    /// </summary>
    /// <param name="indexToUpdate">The index between 1-5 to update for end game data.</param>
    /// <param name="newData">The new data to put in the end game display.</param>
    public static void UpdateEndGameData(int indexToUpdate, string newData)
    {
        switch (indexToUpdate)
        {
            case 1:
                Instance.endGameDisplay1.UpdateText(newData);
                break;
            case 2:
                Instance.endGameDisplay2.UpdateText(newData);
                break;
            case 3:
                Instance.endGameDisplay3.UpdateText(newData);
                break;
            case 4:
                Instance.endGameDisplay4.UpdateText(newData);
                break;
            case 5:
                Instance.endGameDisplay5.UpdateText(newData);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Returns true if the Instance of the UIManager does not exist.
    /// </summary>
    /// <returns></returns>
    private static bool InstanceDoesntExist()
    {
        return IsntValid(Instance);
    }
    #endregion
}
