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

    /// <summary>
    /// The scene instance of the tutorial video handler.
    /// </summary>
    private static TutorialVideoHandler Instance;

    [Tooltip("The video which calls for the last video to be replayed regardless of what it is")]
    [SerializeField] private VideoClip replayLastVideo;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes the reference to the scene instance of the tutorial video handler.
    /// </summary>
    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// Sets the current video to be played.
    /// </summary>
    /// <param name="video">The identifier for the video to be played.</param>
    public void SetVideo(VideoClip video)
    {
        if (IsntValid(videoPlayer) || videoPlayer.clip == video || video == replayLastVideo) return;

        videoPlayer.clip = video;
        gameObject.SetActive(video != null);
    }

    /// <summary>
    /// Pauses or unpauses the current video being played.
    /// </summary>
    /// <param name="shouldPause">Holds true if the video should be paused.</param>
    public static void PauseVideo(bool shouldPause)
    {
        if (IsntValid(Instance) || Instance.videoPlayer == null) return;

        if (shouldPause)
        {
            Instance.videoPlayer.Pause();
        }
        else
        {
            Instance.videoPlayer.Play();
        }
    }
    #endregion
}
