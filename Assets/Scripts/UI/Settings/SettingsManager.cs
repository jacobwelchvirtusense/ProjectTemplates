/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 2/1/2023 1:08:22 PM
 * 
 * Description: Handles the inputs into the settings menu and hooks to its changes.
*********************************/
using com.rfilkov.kinect;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using static ValidCheck;

public class SettingsManager : UIButtonController
{
    #region Fields
    #region Saved Data
    public static int[] indexSettingsData;

    /// <summary>
    /// Holds true if the audio should be on.
    /// </summary>
    private static bool enableAudio = true;

    /// <summary>
    /// Holds true if the settings panel has not been initialized.
    /// </summary>
    private static bool hasNotInitializedSettings = true;
    #endregion

    /// <summary>
    /// The scene instance of the SettingsManager.
    /// </summary>
    private static SettingsManager Instance;

    /// <summary>
    /// The current settings slot hovered over.
    /// </summary>
    private int currentSettingsSlot = 0;

    /// <summary>
    /// The array of all child settings slots.
    /// </summary>
    private SettingsSlot[] settingsSlots;

    /// <summary>
    /// All of the indexed slots being used.
    /// </summary>
    private List<IndexedSettingSlot> indexedSettingSlots = new List<IndexedSettingSlot>();

    [Range(0, 4)]
    [Tooltip("The initial setting to be used for each setting slot")]
    [SerializeField] private int[] initialSettingSlotsValues = new int[0];

    [Tooltip("The setting slot for timers")]
    [SerializeField] private IndexedSettingSlot timerSettingSlot;

    [Tooltip("The toggle settings slot for muting/unmuting audio")]
    [SerializeField] private IndexedSettingSlot audioToggleSettingSlot;
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Gets components and sets their initial states.
    /// </summary>
    private void Awake()
    {
        settingsSlots = GetComponentsInChildren<SettingsSlot>();
        Instance = this;

        indexedSettingSlots = GetComponentsInChildren<IndexedSettingSlot>().ToList();

        if(IsntValid(indexSettingsData))
        indexSettingsData = new int[indexedSettingSlots.Count];
    }

    /// <summary>
    /// Calls for all settings to be initialized.
    /// </summary>
    private void Start()
    {
        InitializeSettings();
    }

    /// <summary>
    /// Sets the initial state of the settings.
    /// </summary>
    private void InitializeSettings()
    {
        for(int i = 0; i < initialSettingSlotsValues.Length; i++)
        {
            if (hasNotInitializedSettings)
            {
                indexSettingsData[i] = initialSettingSlotsValues[i];
            }

            RefreshSetting(indexedSettingSlots[i], indexSettingsData[i]);
        }

        if (hasNotInitializedSettings)
        {
            hasNotInitializedSettings = false;
        }
    }
    #endregion

    /// <summary>
    /// Turns off the settings menu.
    /// </summary>
    public void DisableSettingsMenu()
    {
        gameObject.SetActive(false);
    }

    #region Show Sensor Data
    /// <summary>
    /// Displays sensor data if this menu is enabled.
    /// </summary>
    protected override void OnEnable()
    {
        base.OnEnable();

        DisplaySensorData(true);
    }

    /// <summary>
    /// Disables sensor data if this menu is enabled.
    /// </summary>
    private void OnDisable()
    {
        DisplaySensorData(false);
    }

    /// <summary>
    /// Enables or disables the sensor data feedback.
    /// </summary>
    /// <param name="shouldShow">Holds true if the data should be shown.</param>
    private void DisplaySensorData(bool shouldShow)
    {
        var kinnectManager = FindObjectOfType<KinectManager>();

        if (IsntValid(kinnectManager)) return;

        kinnectManager.shouldDisplaySensorData = shouldShow;
    }
    #endregion

    #region Button Events
    /// <summary>
    /// Calls for the currently selected button to be updated.
    /// </summary>
    /// <param name="mod">-1 is down and 1 is up.</param>
    protected override void UpdateSelectedButton(int mod)
    {
        settingsSlots[currentSettingsSlot].SetHover(false);
        currentSettingsSlot = (currentSettingsSlot+mod) % settingsSlots.Length;

        if(currentSettingsSlot < 0)
        {
            currentSettingsSlot = settingsSlots.Length - 1;
        }

        settingsSlots[currentSettingsSlot].SetHover(true);

        base.UpdateSelectedButton(mod);
    }

    /// <summary>
    /// Performs the click event of the currently selected settings slot.
    /// </summary>
    protected override void ClickSlot()
    {
        settingsSlots[currentSettingsSlot].ClickEvent.Invoke();
        base.ClickSlot();
    }

    /// <summary>
    /// Performs an action when the user hits the back button on the remote.
    /// </summary>
    protected override void BackEvent()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Performs an action when the user hits the next button on the remote.
    /// </summary>
    protected override void NextEvent()
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region Settings Updates
    /// <summary>
    /// Sets a specific slot to be the next index of its settings.
    /// </summary>
    /// <param name="slotToBeSet">The setting slot that should be set.</param>
    public void SetSlot(IndexedSettingSlot slotToBeSet)
    {
        var slotIndex = indexedSettingSlots.IndexOf(slotToBeSet);

        indexSettingsData[slotIndex]++;
        indexSettingsData[slotIndex] %= slotToBeSet.GetSlotAmount();

        RefreshSetting(slotToBeSet, indexSettingsData[slotIndex]);
    }

    /// <summary>
    /// Sets a specific slot to be at a specific index of it's settings visually.
    /// </summary>
    /// <param name="slotToBeSet">The setting slot that should be set.</param>
    /// <param name="newIndex">The new setting index to set it to.</param>
    private void RefreshSetting(IndexedSettingSlot slotToBeSet, int newIndex)
    {
        newIndex = Mathf.Clamp(newIndex, 0, slotToBeSet.GetSlotAmount()-1);

        // Initializes the timer
        if(slotToBeSet == timerSettingSlot)
        {
            UIManager.InitializeTimer();
        }

        slotToBeSet.SetCurrentSlotIndex(newIndex);
    }

    /// <summary>
    /// Returns the index of timer setting.
    /// </summary>
    /// <returns></returns>
    public static int GetTimerSlot()
    {
        if(IsntValid(Instance)) return 0;

        var index = Instance.indexedSettingSlots.IndexOf(Instance.timerSettingSlot);

        return indexSettingsData[index];
    }

    #region Audio
    /// <summary>
    /// Toggles the audio on and off.
    /// </summary>
    public void SetAudio()
    {
        enableAudio = !enableAudio;
        RefreshAudio();
    }

    /// <summary>
    /// Refreshed the display setting and its hook.
    /// </summary>
    private void RefreshAudio()
    {
        var index = enableAudio ? 0 : 1;
        audioToggleSettingSlot.SetCurrentSlotIndex(index);
        AudioListener.volume = System.Convert.ToInt32(enableAudio);
    }
    #endregion
    #endregion
    #endregion
}
