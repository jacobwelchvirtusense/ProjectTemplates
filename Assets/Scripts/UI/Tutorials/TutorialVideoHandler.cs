/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 2/21/2023 2:41:44 PM
 * 
 * Description: Handles the playing and disabling of tutorial videos.
*********************************/
using UnityEngine;
using UnityEngine.Video;
using static ValidCheck;

public class TutorialVideoHandler : MonoBehaviour
{
    #region Fields
    [Tooltip("The player of all tutorial videos")]
    [SerializeField] private VideoPlayer videoPlayer;
    #endregion

    #region Functions
    /// <summary>
    /// Sets the current video to be played.
    /// </summary>
    /// <param name="video">The identifier for the video to be played.</param>
    public void SetVideo(VideoClip video)
    {
        if (IsntValid(videoPlayer) || videoPlayer.clip == video) return;

        videoPlayer.clip = video;
        gameObject.SetActive(video != null);
    }
    #endregion
}
