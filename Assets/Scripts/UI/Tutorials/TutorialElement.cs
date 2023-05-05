/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/16/2023 10:55:49 AM
 * 
 * Description: A data type for single elements within a tutorial.
*********************************/
using UnityEngine;
using UnityEngine.Video;
using static InspectorValues;

[System.Serializable]
public class TutorialElement
{
    [field: Tooltip("The name shown for the tutorial in editor")]
    [field: SerializeField] public string TutorialName { get; private set; }

    [field: Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [field: TextArea]
    [field: Tooltip("The subtitle text that is displayed for the tutorial")]
    [field: SerializeField] public string SubtitleText { get; private set; }

    [field: Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [field: Tooltip("The videoclip to play during this tutorial (set the same as last clip to keep tutorial playing)")]
    [field: SerializeField] public VideoClip VideoClip { get; private set; }

    #region Dialogue
    [field: Tooltip("The dialogue that is played for the tutorial")]
    [field: SerializeField] public AudioClip AudioDialogue { get; private set; }

    [field: Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [field: Range(0.0f, 1.0f)]
    [field: Tooltip("The volume of the dialogue for the tutorial")]
    [field: SerializeField] public float DialogueVolume { get; private set; } = 1.0f;
    #endregion

    #region Delays
    [field: Range(0.0f, 5.0f)]
    [field: Tooltip("The delay before showing the tutorial element")]
    [field: SerializeField] public float DelayBefore { get; private set; } = 0.0f;

    [field: Range(0.0f, 5.0f)]
    [field: Tooltip("The delay after the tutorial has finished before contining to the next tutorial element")]
    [field: SerializeField] public float DelayAfter { get; private set; } = 1.0f;
    #endregion

    #region Extra Loop Features
    #region Branch Tutorial
    /// <summary>
    /// Holds a reason for branching between a set of options.
    /// </summary>
    public enum TutorialBranching { NONE }

    [field: Space(SPACE_BETWEEN_EDITOR_ELEMENTS)]

    [field: Tooltip("The reason for branching in the tutorial")]
    [field: SerializeField] public TutorialBranching TutorialBranchReason { get; private set; }
    #endregion

    #region Dialogue Interruption
    /// <summary>
    /// Events that interrupt the dialogue from happening.
    /// </summary>
    public enum InterruptDialogue { NONE }

    [field: Tooltip("The interruption type for the tutorial")]
    [field: SerializeField] public InterruptDialogue DialogueInterruption { get; private set; }
    #endregion

    #region Pre Tutorial Action
    /// <summary>
    /// Events that take place before the dialogue has been performed.
    /// </summary>
    public enum PreTutorialAction { NONE }

    [field: Tooltip("The interruption type for the tutorial")]
    [field: SerializeField] public PreTutorialAction[] PreTutorialEvent { get; private set; }
    #endregion

    #region Post Tutorial Action
    /// <summary>
    /// Events that take place after the dialogue has been performed.
    /// </summary>
    public enum PostTutorialAction { NONE }

    [field: Tooltip("The interruption type for the tutorial")]
    [field: SerializeField] public PostTutorialAction[] PostTutorialEvent { get; private set; }
    #endregion
    #endregion
}