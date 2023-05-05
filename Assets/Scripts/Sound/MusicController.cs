/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/14/2023 2:27:10 PM
 * 
 * Description: Handles events related to music such as changing it's volume or
 *              the music that is currently being played.
*********************************/
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class MusicController : Singleton<MusicController>
{
    #region Fields
    /// <summary>
    /// The volume that the music starts at.
    /// </summary>
    private float startingVolume;

    /// <summary>
    /// The audiosource of the 
    /// </summary>
    private AudioSource audioSource;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes components and fields.
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        if (this != null)
        {
            audioSource = GetComponent<AudioSource>();

            if (audioSource != null)
            {
                startingVolume = audioSource.volume;
            }

            SceneManager.activeSceneChanged += ResetVolume;
        }
    }

    /// <summary>
    /// Scales the musics volume to the new percentage based on its old amount
    /// </summary>
    /// <param name="newVolumeScale">The percentage of its starting volume to set to.</param>
    public static void SetMusicVolume(float newVolumeScale)
    {
        if (Instance == null) return;

        Instance.MusicVolumeUpdater(newVolumeScale);
    }

    /// <summary>
    /// Resets the musics volume when a new scene is entered.
    /// </summary>
    /// <param name="currentScene">The currently active scene before the change.</param>
    /// <param name="nextScene">The next scene that is being loaded.</param>
    private void ResetVolume(Scene currentScene, Scene nextScene)
    {
        audioSource.volume = startingVolume;
    }

    /// <summary>
    /// Sets a new music clip onto the music controller.
    /// </summary>
    /// <param name="newMusic">The new music to be played.</param>
    public static void SetMusic(AudioClip newMusic)
    {
        if (Instance == null || newMusic == null || Instance.audioSource == null) return;

        Instance.audioSource.clip = newMusic;
    }

    /// <summary>
    /// Scales the musics volume to the new percentage based on its old amount
    /// </summary>
    /// <param name="newVolumeScale">The percentage of its starting volume to set to.</param>
    private void MusicVolumeUpdater(float newVolumeScale)
    {
        if (audioSource == null) return;

        audioSource.volume = newVolumeScale * startingVolume;
    }
    #endregion
}