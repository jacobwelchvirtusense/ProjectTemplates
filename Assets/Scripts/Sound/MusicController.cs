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

[RequireComponent(typeof(AudioSource))]
public class MusicController : MonoBehaviour
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

    /// <summary>
    /// The scene instance of the Music Controller.
    /// </summary>
    private static MusicController Instance;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes components and fields.
    /// </summary>
    private void Awake()
    {
        Instance = this; 
        audioSource = GetComponent<AudioSource>();

        if(audioSource != null)
        {
            startingVolume = audioSource.volume;
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
        if(audioSource == null) return;

        audioSource.volume = newVolumeScale*startingVolume;
    }
    #endregion
}
