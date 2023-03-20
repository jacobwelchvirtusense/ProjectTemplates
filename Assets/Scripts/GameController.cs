/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: DefaultCompany
 * Project: Apple Basket
 * Creation Date: 1/6/2023 10:21:52 AM
 * 
 * Description: Handles the state of the game between countdowns,
 *              spawning, and ending the game.
*********************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;
using static SoundPlayer;
using static SettingsManager;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class GameController : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The scene instance of the GameController.
    /// </summary>
    public static GameController Instance;

    [Tooltip("An event that is invoked whenever the gameplay is officially started")]
    public UnityEvent StartGameplay;

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [Tooltip("An event that is invoked whenever the gameplay is officially ended")]
    public UnityEvent EndGameplay;

    #region Score and Combos
    [Header("Score and Combos")]
    [Tooltip("The text prefab for increments to score")]
    [SerializeField] private GameObject scoreText;

    [Range(0.0f, 10.0f)]
    [Tooltip("The rate of points to increase by per combo")]
    [SerializeField] private float comboModifier = 0.25f;

    /// <summary>
    /// The current total of points the player has.
    /// </summary>
    private int currentScore = 0;

    /// <summary>
    /// A getter and setter for updating the current score the user has.
    /// </summary>
    public int CurrentScore
    {
        get => currentScore;
        set
        {
            currentScore = value;
            UIManager.UpdateScore(currentScore);

            if (highestComboReached < currentCombo)
            {
                highestComboReached = currentCombo;
            }
        }
    }

    /// <summary>
    /// The current combo the player has.
    /// </summary>
    private int currentCombo = 0;

    /// <summary>
    /// A getter and setter for updating the current combo the user has.
    /// </summary>
    public int CurrentCombo
    {
        get => currentCombo;
        set
        {
            currentCombo = value;
            UIManager.UpdateCombo(currentCombo);
        }
    }
    #endregion

    #region Timing
    [Header("Timing")]
    [Tooltip("Holds true if there should be a transparent countdown in the last 3 seconds of the training")]
    [SerializeField] private bool showCountdownToEnd = false;

    [Range(0.0f, 5.0f)]
    [Tooltip("The count down time before starting")]
    [SerializeField] private float timeBeforeEnd = 1.0f;

    [Tooltip("The minimum value for the timer in seconds")]
    [SerializeField] private int[] timers = new int[] { 30, 60, 120 };

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]
    #endregion

    #region Lives Mode
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
    #endregion

    #region Sound
    /// <summary>
    /// The AudioSource for game state events.
    /// </summary>
    private AudioSource audioSource;

    [Header("Sound")]
    #region End Sound
    [Tooltip("The sound made when the game ends")]
    [SerializeField]
    private AudioClip endSound;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume of the end sound")]
    [SerializeField]
    private float endSoundVolume = 1.0f;
    #endregion
    #endregion

    #region Output Data
    /// <summary>
    /// The highest combo that the user has reached.
    /// </summary>
    private int highestComboReached = 0;
    #endregion
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Initializes components and starts the game.
    /// </summary>
    private void Awake()
    {
        Instance = this;

        InitializeComponents();
    }

    /// <summary>
    /// Initializes any components needed.
    /// </summary>
    private void InitializeComponents()
    {
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Starts the countdown for the game to begin.
    /// </summary>
    public void StartGameCountdown()
    {
        StartCoroutine(CountdownLoop());
    }
    #endregion

    /* Editor Testing Keys
    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.E))
        {
            UpdateSceneScore(100, Vector2.zero);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            UpdateSceneScore(-100, Vector2.zero);
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            IncreaseCombo();
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ResetCombo();
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            LivesLeft--;
        }

        if (Input.GetKeyDown(KeyCode.V))
        {
            LivesLeft++;
        }
#endif
    }*/

    #region Updating Score
    /// <summary>
    /// Updates the score the player has.
    /// </summary>
    /// <param name="increment">Increments the player's score by this amount.</param>
    public static void UpdateScore(int increment, Vector2 location)
    {
        Instance.UpdateSceneScore(increment, location);
    }

    /// <summary>
    /// Updates the non-static score for the game.
    /// </summary>
    /// <param name="increment">Increments the player's score by this amount.</param>
    public void UpdateSceneScore(int increment, Vector2 location)
    {
        var actualIncrement = ComboModifier(increment);
        CurrentScore += actualIncrement;

        // Spawns text at score location
        var text = Instantiate(scoreText, location, Quaternion.identity);
        text.GetComponent<ScoreIncrementText>().InitializeScore(actualIncrement);

        DisplayGameData();
    }

    /// <summary>
    /// Applies the combo modifier to the score increment.
    /// </summary>
    /// <param name="increment">The score increment.</param>
    /// <returns></returns>
    private int ComboModifier(int increment)
    {
        var amount = increment;

        if(increment > 0) amount += (int)((currentCombo * comboModifier) * increment);

        return amount;
    }

    #region Combo
    /// <summary>
    /// Increase the current combo by 1.
    /// </summary>
    public static void IncreaseCombo()
    {
        Instance.CurrentCombo++;
    }

    /// <summary>
    /// Resets the current combo to zero.
    /// </summary>
    public static void ResetCombo()
    {
        Instance.CurrentCombo = 0;
    }
    #endregion
    #endregion

    #region Countdown
    /// <summary>
    /// Counts down before starting the game again.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CountdownLoop()
    {
        yield return Countdown.CountdownLoop();

        StartGame();
    }

    /// <summary>
    /// Begins the game (spawning of apples and game timer).
    /// </summary>
    private void StartGame()
    {
        if (IsLivesMode())
        {
            StartCoroutine(LivesRoutine());
        }
        else
        {
            StartCoroutine(GameTimer());
        }

        StartGameplay.Invoke();
    }
    #endregion

    #region Timer
    /// <summary>
    /// The timer for the length of the game.
    /// </summary>
    /// <returns></returns>
    private IEnumerator GameTimer()
    {
        float t = GetTimerAmount();

        do
        {
            UIManager.UpdateTimer(t);
            yield return new WaitForEndOfFrame();

            CheckForCountdownToEnd(t);

            t -= Time.deltaTime;
        }
        while (t > 0);

        yield return EndGame();
    }

    /// <summary>
    /// Checks if the the transparent countdown for the end of the training should
    /// be shown.
    /// </summary>
    /// <param name="t">The current time left in the training.</param>
    private void CheckForCountdownToEnd(float t)
    {
        if (showCountdownToEnd && t < 4.0f)
        {
            showCountdownToEnd = false;
            Countdown.ShowTransparentCountdown();
        }
    }

    /// <summary>
    /// Returns the current starting timer that is being used in the game.
    /// </summary>
    /// <returns></returns>
    public static int GetTimerAmount()
    {
        return Instance.timers[GetTimerSlot()];
    }
    #endregion

    #region Lives Mode
    /// <summary>
    /// Waits until the user has run out of lives and then ends the game.
    /// </summary>
    /// <returns></returns>
    private IEnumerator LivesRoutine()
    {
        while(livesLeft != 0)
        {
            yield return new WaitForEndOfFrame();
        }

        yield return EndGame();
    }

    /// <summary>
    /// Returns true if the current gameplay is a lives mode.
    /// </summary>
    /// <returns></returns>
    public static bool IsLivesMode()
    {
        return GetTimerAmount() == -1;
    }
    #endregion

    #region End Game/Output Data
    /// <summary>
    /// Displays end message and outputs the game data.
    /// </summary>
    private IEnumerator EndGame()
    {
        EndGameplay.Invoke();

        // Waits for game to fully complete
        if(timeBeforeEnd != 0)
        yield return new WaitForSeconds(timeBeforeEnd);

        // Displays all data
        UIManager.DisplayEndScreen();
        DisplayGameData();
        OutputData();
        PlaySound(audioSource, endSound, endSoundVolume);
    }

    /// <summary>
    /// Displays all of the game data in the end game displays.
    /// </summary>
    private void DisplayGameData()
    {
        UIManager.UpdateEndGameData(2, "Temp Text");
        UIManager.UpdateEndGameData(3, "Temp Text");
        UIManager.UpdateEndGameData(4, "Temp Text");
        UIManager.UpdateEndGameData(5, "Temp Text");
    }

    /// <summary>
    /// Outputs the data from the players session.
    /// </summary>
    private void OutputData()
    {
        // TODO
        // currentPointTotal;
        // highestComboReached
    }

    /// <summary>
    /// Exits the game both in editor and in builds.
    /// </summary>
    public void ExitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }
    #endregion

    /// <summary>
    /// Calls for the scene to be loaded effectively reloading the game.
    /// </summary>
    public void PlayAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion
}