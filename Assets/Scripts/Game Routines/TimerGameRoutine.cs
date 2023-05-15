/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 4/14/2023 2:45:20 PM
 * 
 * Description: TODO
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

public class TimerGameRoutine : IGameRoutine
{
    #region Fields
    [Header("Timing")]
    [Tooltip("Holds true if there should be a transparent countdown in the last 3 seconds of the training")]
    [SerializeField] private bool showCountdownToEnd = false;

    [Tooltip("The minimum value for the timer in seconds")]
    [SerializeField] private int[] timers = new int[] { 30, 60, 120 };

    private int currentSlot = 0;

    /// <summary>
    /// 
    /// </summary>
    private static TimerGameRoutine Instance; 
    #endregion

    #region Functions
    /// <summary>
    /// Initializes the instance and the event reciever.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Updates the current timers slot within the array of timer options.
    /// </summary>
    /// <param name="newTimerSlot">The new slot of the timer array to use.</param>
    private void UpdateToNewTimerSlot(int newTimerSlot)
    {
        currentSlot = newTimerSlot;

        UIManager.InitializeTimer();
    }

    /// <summary>
    /// A gameplay routine where a timer ticks down and then the game ends.
    /// </summary>
    /// <returns></returns>
    public override IEnumerator GameplayRoutine()
    {
        yield return base.GameplayRoutine();

        float t = GetTimerAmount();

        do
        {
            UIManager.UpdateTimer(t);
            yield return new WaitForEndOfFrame();

            CheckForCountdownToEnd(t);

            t -= Time.deltaTime;
        }
        while (t > 0);
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
        if (Instance == null || Instance.timers.Length <= Instance.currentSlot) return 0;

        return Instance.timers[Instance.currentSlot];
    }

    /// <summary>
    /// Sets this routine as the active routine and initializes it.
    /// </summary>
    public override void SetActive(bool shouldSetActive)
    {
        if (shouldSetActive)
        {
            UIManager.InitializeTimer();

            SettingsManager.Slot1OnValueChanged.AddListener(UpdateToNewTimerSlot);
        }
        else
        {
            SettingsManager.Slot1OnValueChanged.RemoveListener(UpdateToNewTimerSlot);
        }
    }
    #endregion
}
