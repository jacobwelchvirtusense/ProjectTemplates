/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 4/14/2023 2:44:20 PM
 * 
 * Description: An abstract functionality for all routines that 
 *              could be had for gameplay.
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using static InspectorValues;
using static ValidCheck;

public abstract class IGameRoutine : MonoBehaviour
{
    #region Fields
    public UnityEvent StartGameEvent = new UnityEvent();
    #endregion

    #region Functions
    /// <summary>
    /// Sets this routine as the active routine and initializes it.
    /// </summary>
    public abstract void SetActive(bool shouldBeActive);

    /// <summary>
    /// The routine of gameplay to be handled by this class.
    /// </summary>
    /// <returns></returns>
    public virtual IEnumerator GameplayRoutine()
    {
        StartGameEvent.Invoke();

        if (false)
        {
            yield return null;
        }
    }
    #endregion
}
