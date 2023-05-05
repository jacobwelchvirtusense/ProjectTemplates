/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/16/2023 9:42:07 AM
 * 
 * Description: A base class for taking in all inputs to menus and handling
 *              moving between buttons in them.
*********************************/
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public abstract class UIButtonController : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The current button slot that is selected.
    /// </summary>
    protected int currentButtonSlot;

    [Tooltip("An event that is fire when the next button is pressed on the controller")]
    [SerializeField]
    private UnityEvent nextEvent = new UnityEvent();

    [Tooltip("An event that is fire when the rewind button is pressed on the controller")]
    [SerializeField]
    private UnityEvent rewindEvent = new UnityEvent();

    [Tooltip("An event that is fire when the start stop button is pressed on the controller")]
    [SerializeField]
    private UnityEvent startStopEvent = new UnityEvent();
    #endregion

    #region Functions
    /// <summary>
    /// Sets initial events for a UI button controller.
    /// </summary>
    protected virtual void Awake()
    {
        ControllerInput.UpDownEvent.AddListener(UpdateSelectedButtonReciever);
        ControllerInput.OkEvent.AddListener(ClickSlotReciever);

        ControllerInput.StartStopEvent.AddListener(StartStopEvent);
        ControllerInput.RewindEvent.AddListener(RewindEvent);
        ControllerInput.NextEvent.AddListener(NextEvent);
    }

    /// <summary>
    /// Resets the selected button to be the first one.
    /// </summary>
    protected virtual void OnEnable()
    {
        currentButtonSlot = 0;
        UpdateSelectedButtonReciever(0);
    }

    #region Input Events
    /// <summary>
    /// Ensures only the active element is called.
    /// </summary>
    /// <param name="mod">-1 is down and 1 is up.</param>
    protected async void UpdateSelectedButtonReciever(int mod)
    {
        if (!gameObject.activeInHierarchy) return;

        await Task.Delay(1); // Needed to ensure execution order does not cause event to fire on multiple button controllers

        UpdateSelectedButton(mod);
    }

    /// <summary>
    /// Calls for the currently selected button to be updated.
    /// </summary>
    /// <param name="mod">-1 is down and 1 is up.</param>
    public virtual void UpdateSelectedButton(int mod, bool shouldPlaySound = true)
    {
        if (mod != 0)
        {
            UISoundManager.PlayHoverSound();
        }
    }

    /// <summary>
    /// Ensures only the active element is called.
    /// </summary>
    protected async void ClickSlotReciever()
    {
        if (!gameObject.activeInHierarchy) return;

        await Task.Delay(1); // Needed to ensure execution order does not cause event to fire on multiple button controllers

        ClickSlot();
    }

    /// <summary>
    /// Performs the click event of the currently selected button.
    /// </summary>
    public virtual void ClickSlot()
    {
        UISoundManager.PlayClickSound();
    }

    /// <summary>
    /// Performs an action when the user hits the back button on the remote.
    /// </summary>
    protected virtual async void RewindEvent()
    {
        if (!gameObject.activeInHierarchy) return;

        await Task.Delay(1); // Needed to ensure execution order does not cause event to fire on multiple button controllers

        rewindEvent.Invoke();
    }

    /// <summary>
    /// Performs an action when the user hits the next button on the remote.
    /// </summary>
    protected virtual async void NextEvent()
    {
        if (!gameObject.activeInHierarchy) return;

        await Task.Delay(1); // Needed to ensure execution order does not cause event to fire on multiple button controllers

        nextEvent.Invoke();
    }

    /// <summary>
    /// Fires an event when the start stop button has been pressed.
    /// </summary>
    protected virtual async void StartStopEvent()
    {
        if (!gameObject.activeInHierarchy) return;

        await Task.Delay(1); // Needed to ensure execution order does not cause event to fire on multiple button controllers

        startStopEvent.Invoke();
    }
    #endregion
    #endregion
}
