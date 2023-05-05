/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 2/20/2023 8:27:06 AM
 * 
 * Description: Manages all of the functionality for displaying
 *              a tutorial to the user. This includes the whole
 *              tutorial loop and calling sound, subtitles and videos.
*********************************/
using System.Collections;
using static InspectorValues;
using static ValidCheck;
using static SoundPlayer;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class TutorialManager : MonoBehaviour
{
    #region Fields
    /// <summary>
    /// The AudioSource for tutorial dialogue.
    /// </summary>
    private AudioSource audioSource;

    #region Serialized Fields
    [Tooltip("The event called when the tutorial is finished")]
    public UnityEvent EndTutorialEvent = new UnityEvent();

    [Tooltip("These tutorials are played in the order they are set")]
    [SerializeField] TutorialElement[] tutorialElements = new TutorialElement[0];

    [Range(0.0f, 1.0f)]
    [Tooltip("The volume for the music to be while the tutorial is playing")]
    [SerializeField] private float tutorialMusicVolume = 0.05f;

    [Tooltip("Holds true if the tutorial should have a countdown")]
    [SerializeField] private bool tutorialHasCountdown = true;

    [Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    #region Handlers
    [Tooltip("The video handler used for tutorials")]
    [SerializeField] private TutorialVideoHandler videoHandler;

    [Tooltip("The subtitle handler used for tutorials")]
    [SerializeField] private TutorialSubtitleHandler subtitleHandler;
    #endregion
    #endregion

    #region Statics
    /// <summary>
    /// Holds true if the tutorial is currently being played.
    /// </summary>
    public static bool IsPlaying = false;

    /// <summary>
    /// The static instance of the tutorial manager
    /// </summary>
    private static TutorialManager Instance;

    /// <summary>
    /// The routine that the tutorial is running on.
    /// </summary>
    private Coroutine tutorialRoutine;

    /// <summary>
    /// Returns true if the tutorial is currently running
    /// </summary>
    public static bool TutorialActive
    {
        get
        {
            if (IsntValid(Instance) || IsntValid(Instance.tutorialRoutine)) return false;

            return true;
        }
    }
    #endregion
    #endregion

    #region Functions
    /// <summary>
    /// Iniailizes components and fields.
    /// </summary>
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        Instance = this;
    }

    /// <summary>
    /// Starts the tutorial with initialization and its loop.
    /// </summary>
    public void StartTutorial()
    {
        InitializeTutorialValues(true);

        tutorialRoutine = StartCoroutine(TutorialLoop());
    }

    /// <summary>
    /// Stops the tutorial with uninitialization and its loop.
    /// </summary>
    public static void StopTutorial()
    {
        if (IsntValid(Instance)) return;

        Instance.InitializeTutorialValues(false);
        Instance.StopAllCoroutines();
        Instance.tutorialRoutine = null;
    }

    /// <summary>
    /// Sets the initialization values for the tutorial starting or being stopped.
    /// </summary>
    /// <param name="shouldInitialize">Holds true if the tutorial is being started.</param>
    private void InitializeTutorialValues(bool shouldInitialize)
    {
        var volume = shouldInitialize ? tutorialMusicVolume : 1.0f;
        MusicController.SetMusicVolume(volume);

        IsPlaying = shouldInitialize;
        PostTutorialMessage.showMessage = shouldInitialize;
    }

    /// <summary>
    /// A loop for handling all currently set tutorial elements and their branches.
    /// </summary>
    /// <returns></returns>
    private IEnumerator TutorialLoop()
    {
        if (tutorialHasCountdown)
        {
            yield return Countdown.CountdownLoop();
        }

        foreach (var tutorial in tutorialElements)
        {
            if (CheckTutorialBranch(tutorial.TutorialBranchReason))
            {
                yield return PlayTutorial(tutorial);
            }
        }

        tutorialRoutine = null;
        EndTutorialEvent.Invoke();
        IsPlaying = false;
    }

    /// <summary>
    /// Checks to see if a specific condition is met or not for a branched tutorial.
    /// </summary>
    /// <param name="branchReason">The reason to branch the tutorial if any.</param>
    /// <returns></returns>
    private bool CheckTutorialBranch(TutorialElement.TutorialBranching branchReason)
    {
        switch (branchReason)
        {
            #region Example
            /* Branching between different settings example
             
            private int movementIndex = 0;
            private int timingIndex = 0;
            
            case TutorialElement.TutorialBranching.MOVEMENTYPE:
                if(movementIndex == SettingsManager.inputType)
                {
                    movementIndex++;
                    return true;
                }
                else
                {
                    movementIndex++;
                    return false;
                }

            case TutorialElement.TutorialBranching.TIMING:
                if((timingIndex == 0 && !GameController.IsInfinite()) || (timingIndex == 1 && GameController.IsInfinite()))
                {
                    timingIndex++;
                    return true;
                }
                else
                {
                    timingIndex++;
                    return false;
                }*/
            #endregion

            case TutorialElement.TutorialBranching.NONE:
            default:
                return true;
        }
    }

    #region Play Tutorial Element
    /// <summary>
    /// Plays the tutorial element with all of its specified events.
    /// </summary>
    /// <param name="tutorial">The tutorial element to be played.</param>
    /// <returns></returns>
    private IEnumerator PlayTutorial(TutorialElement tutorial)
    {
        #region Before Audio
        if (tutorial.DelayBefore != 0) yield return new WaitForSeconds(tutorial.DelayBefore); // Even if 0, calling a WaitForSeconds will run for at least 1 frame so we check first

        foreach (var preEvent in tutorial.PreTutorialEvent)
        {
            if (preEvent != TutorialElement.PreTutorialAction.NONE) yield return PreTutorialAction(preEvent);
        }

        videoHandler.SetVideo(tutorial.VideoClip);
        #endregion

        yield return DialoguePlayer(tutorial.SubtitleText, tutorial.AudioDialogue, tutorial.DialogueVolume);

        #region After Audio
        foreach (var postEvent in tutorial.PostTutorialEvent)
        {
            if (postEvent != TutorialElement.PostTutorialAction.NONE) yield return PostTutorialAction(postEvent);
        }

        if (tutorial.DelayAfter != 0) yield return new WaitForSeconds(tutorial.DelayAfter); // Even if 0, calling a WaitForSeconds will run for at least 1 frame so we check first
        #endregion
    }

    #region Helper Functions
    /// <summary>
    /// Plays an audio file with subtitles and waits for it to complete itself.
    /// </summary>
    /// <param name="subtitle">The subtitle to be displayed with the audio.</param>
    /// <param name="dialogue">The dialogue of audio to be played.</param>
    /// <param name="dialogueVolume">The volume to play the audio at.</param>
    /// <returns></returns>
    private IEnumerator DialoguePlayer(string subtitle, AudioClip dialogue, float dialogueVolume)
    {
        // Plays and displays audio
        subtitleHandler.SetSubtitle(subtitle);
        PlaySound(audioSource, dialogue, dialogueVolume);

        // Waits for audio duration
        if (dialogue != null && dialogue.length != 0.0f) yield return new WaitForSeconds(dialogue.length);
    }

    /// <summary>
    /// Does an action before the audio is started for the tutorial element.
    /// </summary>
    /// <param name="action">The action to be performed.</param>
    /// <returns></returns>
    private IEnumerator PreTutorialAction(TutorialElement.PreTutorialAction action)
    {
        switch (action)
        {
            #region Examples
            /* Calls for specific Events
             * Doesn't work with this code base (from Apple Frenzy)
            #region Spawn Apples
            case TutorialElement.PreTutorialAction.SPAWNSIDEAPPLES:
                AppleSpawner.SpawnSideApples();
                yield break;

            case TutorialElement.PreTutorialAction.SPAWNGOODAPPLE:
                BasketMovement.LockMovement();
                AppleSpawner.SpawnGoodApple();
                yield break;
            
            case TutorialElement.PreTutorialAction.SPAWNBADAPPLE:
                AppleSpawner.SpawnBadApple();
                yield break;
            #endregion*/

            /* Calls for changing a settings
             * Doesn't work with this code base (from Apple Frenzy)
            #region Set Movement Types
            case TutorialElement.PreTutorialAction.SETMOVE:
                BasketMovement.SetMovementType(0);
                yield break;
            case TutorialElement.PreTutorialAction.SETLEAN:
                BasketMovement.SetMovementType(1);
                yield break;
            case TutorialElement.PreTutorialAction.SETHANDS:
                BasketMovement.SetMovementType(2);
                yield break;
            #endregion*/
            #endregion

            case TutorialElement.PreTutorialAction.NONE:
            default:
                yield break;
        }
    }

    /// <summary>
    /// Does an action after the audio is finished for a tutorial element.
    /// </summary>
    /// <param name="action">The action to be performed.</param>
    /// <returns></returns>
    private IEnumerator PostTutorialAction(TutorialElement.PostTutorialAction action)
    {
        switch (action)
        {
            #region Examples
            /* Example of waiting for something to happen
             * Doesn't work with this code base (From Apple Frenzy)
            case TutorialElement.PostTutorialAction.WAITFORAPPLES:
                
                while (GameObject.FindGameObjectsWithTag("Good").Length != 0 || GameObject.FindGameObjectsWithTag("Bad").Length != 0)
                {
                    yield return new WaitForEndOfFrame();
                }

                yield break; */
            #endregion

            case TutorialElement.PostTutorialAction.NONE:
            default:
                yield break;
        }
    }
    #endregion
    #endregion
    #endregion
}