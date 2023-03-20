/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/14/2023 1:14:33 PM
 * 
 * Description: Handles the playing of sound for UI events.
*********************************/
using UnityEngine;
using static ValidCheck;
using static InspectorValues;

[RequireComponent(typeof(AudioSource))]
public class UISoundManager : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The audiosource for all UI sounds.
    /// </summary>
    private AudioSource audioSource;

    /// <summary>
    /// The scenes instance of the UISoundManager.
    /// </summary>
    private static UISoundManager Instance;

    [Header("Sound")]
    #region Hover Sound
    [Tooltip("The sound made when a UI element is hovered over")]
    [SerializeField] private AudioClip hoverSound;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume of the hover sound")]
    [SerializeField] private float hoverSoundVolume = 1.0f;
    #endregion

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    #region Click Sound
    [Tooltip("The sound made when a UI element is clicked on")]
    [SerializeField] private AudioClip clickSound;

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume of the click sound")]
    [SerializeField] private float clickSoundVolume = 1.0f;
    #endregion
    #endregion

    #region Functions
    /// <summary>
    /// Initializes Components
    /// </summary>
    private void Awake()
    {
        Instance = this;
        audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Plays the hover sound.
    /// </summary>
    public static void PlayHoverSound()
    {
        if (IsntValid(Instance)) return;

        Instance.PlaySound(Instance.hoverSound, Instance.hoverSoundVolume);
    }

    /// <summary>
    /// Plays the click sound
    /// </summary>
    public static void PlayClickSound()
    {
        if (IsntValid(Instance)) return;

        Instance.PlaySound(Instance.clickSound, Instance.clickSoundVolume);
    }

    #region Sound
    /// <summary>
    /// Plays a sound for a specified event (Has null checks built in).
    /// </summary>
    /// <param name="soundClip">The sound clip to be played.</param>
    /// <param name="soundVolume">The volume of the sound to be played.</param>
    private void PlaySound(AudioClip soundClip, float soundVolume)
    {
        if (IsntValid(audioSource) || IsntValid(soundClip)) return;

        audioSource.PlayOneShot(soundClip, soundVolume);
    }
    #endregion
    #endregion
}
