/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 2/1/2023 1:08:22 PM
 * 
 * Description: Handles the inputs into the settings menu and hooks to its changes.
*********************************/
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using static ValidCheck;

public class SettingsManager : UIButtonController
{
    #region Fields
    #region Saved Data
    /// <summary>
    /// The indexed data for settings.
    /// </summary>
    private static int[] indexSettingsData;

    /// <summary>
    /// Holds true if the audio should be on.
    /// </summary>
    private static bool enableAudio = true;

    /// <summary>
    /// Holds true if the settings panel has not been initialized.
    /// </summary>
    private static bool hasNotInitializedSettings = true;

    /// <summary>
    /// Is called when the first setting slot is updated.
    /// </summary>
    public static UnityEvent<int> Slot1Update = new UnityEvent<int>();

    /// <summary>
    /// Is called when the second setting slot is updated.
    /// </summary>
    public static UnityEvent<int> Slot2Update = new UnityEvent<int>();

    /// <summary>
    /// Is called when the third setting slot is updated.
    /// </summary>
    public static UnityEvent<int> Slot3Update = new UnityEvent<int>();

    /// <summary>
    /// Is called when the fourth setting slot is updated.
    /// </summary>
    public static UnityEvent<int> Slot4Update = new UnityEvent<int>();
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

    [Tooltip("The toggle settings slot for muting/unmuting audio")]
    [SerializeField] private IndexedSettingSlot audioToggleSettingSlot;
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Gets components and sets their initial states.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

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

    #region Button Events
    /// <summary>
    /// Calls for the currently selected button to be updated.
    /// </summary>
    /// <param name="mod">-1 is down and 1 is up.</param>
    public override void UpdateSelectedButton(int mod)
    {
        base.UpdateSelectedButton(mod);

        settingsSlots[currentSettingsSlot].SetHover(false);
        currentSettingsSlot = (currentSettingsSlot + mod) % settingsSlots.Length;

        if (currentSettingsSlot < 0)
        {
            currentSettingsSlot = settingsSlots.Length - 1;
        }

        settingsSlots[currentSettingsSlot].SetHover(true);
    }

    /// <summary>
    /// Performs the click event of the currently selected settings slot.
    /// </summary>
    public override void ClickSlot()
    {
        base.ClickSlot();

        settingsSlots[currentSettingsSlot].ClickEvent.Invoke();

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

        CallUpdateEvent(slotIndex);

        RefreshSetting(slotToBeSet, indexSettingsData[slotIndex]);
    }

    /// <summary>
    /// Invokes the update event for that setting slots data.
    /// </summary>
    /// <param name="slotIndex"></param>
    private void CallUpdateEvent(int slotIndex)
    {
        switch (slotIndex)
        {
            case 0:
                Slot1Update.Invoke(indexSettingsData[slotIndex]);
                break;
            case 1:
                Slot2Update.Invoke(indexSettingsData[slotIndex]);
                break;
            case 2:
                Slot3Update.Invoke(indexSettingsData[slotIndex]);
                break;
            case 3:
                Slot4Update.Invoke(indexSettingsData[slotIndex]);
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Sets a specific slot to be at a specific index of it's settings visually.
    /// </summary>
    /// <param name="slotToBeSet">The setting slot that should be set.</param>
    /// <param name="newIndex">The new setting index to set it to.</param>
    private void RefreshSetting(IndexedSettingSlot slotToBeSet, int newIndex)
    {
        newIndex = Mathf.Clamp(newIndex, 0, slotToBeSet.GetSlotAmount()-1);

        slotToBeSet.SetCurrentSlotIndex(newIndex);
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
