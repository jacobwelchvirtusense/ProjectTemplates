/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 2/10/2023 2:50:45 PM
 * 
 * Description: Handles the functionality for a setting slot
 *              with multiple indexes.
*********************************/
using UnityEngine;

public class IndexedSettingSlot : SettingsSlot
{
    #region Fields
    [Tooltip("The objects to be used for each index")]
    [SerializeField] private GameObject[] settingsObjects;
    #endregion

    #region Functions
    /// <summary>
    /// Sets the current slot that is being used.
    /// </summary>
    /// <param name="index">The index of the slot being used.</param>
    public void SetCurrentSlotIndex(int index)
    {
        foreach(GameObject obj in settingsObjects)
        {
            obj.SetActive(false);
        }

        settingsObjects[index].SetActive(true);
    }

    /// <summary>
    /// Gets the amount of slots for this setting.
    /// </summary>
    /// <returns></returns>
    public int GetSlotAmount()
    {
        return settingsObjects.Length;
    }
    #endregion
}
