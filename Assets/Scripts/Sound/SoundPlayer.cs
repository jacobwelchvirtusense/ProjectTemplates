/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/14/2023 4:56:23 PM
 * 
 * Description: Handles base functionality for all null checks related to playing sounds.
 *              Basically I was tired of copying the same function between every script
 *              that has sound.
*********************************/
using static ValidCheck;
using UnityEngine;

public class SoundPlayer : MonoBehaviour
{
    #region Functions
    #region Sound
    /// <summary>
    /// Plays a sound for a specified event (Has null checks built in).
    /// </summary>
    /// <param name="soundClip">The sound clip to be played.</param>
    /// <param name="soundVolume">The volume of the sound to be played.</param>
    public static void PlaySound(AudioSource audioSource, AudioClip soundClip, float soundVolume)
    {
        if (IsntValid(audioSource) || soundClip == null) return;

        audioSource.PlayOneShot(soundClip, soundVolume);
    }
    #endregion
    #endregion
}
