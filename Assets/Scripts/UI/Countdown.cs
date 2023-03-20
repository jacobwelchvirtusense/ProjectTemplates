/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 2/20/2023 8:49:34 AM
 * 
 * Description: Handles the funcitonality of a countdown.
*********************************/
using System.Collections;
using UnityEngine;
using static ValidCheck;
using static SoundPlayer;
using static InspectorValues;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class Countdown : MonoBehaviour
{
    #region Fields
    #region Serialized Fields
    [Tooltip("The object used to show the countdown")]
    [SerializeField] private GameObject countdownObject;

    [Range(0.0f, 1.0f)]
    [Tooltip("The amount transparency for the countdown to have during transparent countdowns")]
    [SerializeField] private float translucentAmount = 0.5f;

    #region Sound
    [Header("Sound")]
    #region Countdown Sound
    [Tooltip("The sound made with each change of the countdown")]
    [SerializeField] private AudioClip countDownSound;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume of the count down sound")]
    [SerializeField] private float countDownSoundVolume = 1.0f;
    #endregion

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    #region Start Sound
    [Tooltip("The sound made when it says go")]
    [SerializeField]
    private AudioClip startSound;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume of the start sound")]
    [SerializeField]
    private float startSoundVolume = 1.0f;
    #endregion
    #endregion
    #endregion

    /// <summary>
    /// The AudioSource for game state events.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// The instance of the countdown in the scene.
    /// </summary>
    private static Countdown Instance;

    /// <summary>
    /// The amount of time needed before starting.
    /// </summary>
    private const int timeBeforeStart = 3;

    /// <summary>
    /// The image that displays the countdown.
    /// </summary>
    private static Image countdownImage;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes components.
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Instance = this;

        if (IsValid(countdownObject))
        {
            countdownImage = countdownObject.GetComponent<Image>();
        }
    }

    /// <summary>
    /// Counts down before starting the game again.
    /// </summary>
    /// <returns></returns>
    public static IEnumerator CountdownLoop()
    {
        if (IsntValid(Instance)) yield break;

        int t = timeBeforeStart;
        Instance.countdownObject.SetActive(true);

        yield return new WaitForSeconds(0.25f);

        do
        {
            #region Sound
            if (t != 0)
            {
                PlaySound(Instance.audioSource, Instance.countDownSound, Instance.countDownSoundVolume);
            }
            else
            {
                PlaySound(Instance.audioSource, Instance.startSound, Instance.startSoundVolume);
            }
            #endregion

            if (t >= 0) yield return new WaitForSeconds(1);
        }
        while (t-- > 0);

        Instance.countdownObject.SetActive(false);
    }

    /// <summary>
    /// Shows the countdown as a transparent image with no sound.
    /// </summary>
    public static void ShowTransparentCountdown()
    {
        if (IsntValid(Instance) || IsntValid(Instance.countdownObject)) return;

        Instance.countdownObject.SetActive(true);
        Instance.ChangeTransparency();

        Instance.StartCoroutine(Instance.DisableTransparentCountdown());
    }

    /// <summary>
    /// Disables the countdown object.
    /// </summary>
    /// <returns></returns>
    private IEnumerator DisableTransparentCountdown()
    {
        yield return new WaitForSeconds(3.0f);
        countdownObject.SetActive(false);
    }

    /// <summary>
    /// Sets the transparency of the countdown.
    /// </summary>
    private void ChangeTransparency()
    {
        if (IsntValid(countdownImage)) return;

        var color = Color.white;
        color.a = translucentAmount;

        countdownImage.color = color;
    }
    #endregion
}
