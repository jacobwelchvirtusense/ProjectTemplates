/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 1/17/2023 3:33:19 PM
 * 
 * Description: Score text that moves upwards and fades away after a short duration.
*********************************/
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class ScoreIncrementText : MonoBehaviour
{
    #region Fields
    #region Serialized Fields
    #region Movement
    [Header("Movement")]
    [Range(0.0f, 5.0f)]
    [Tooltip("The y position offset to spawn this")]
    [SerializeField] private float spawnYOffset = 2.0f;

    [Range(0.0f, 5.0f)]
    [Tooltip("The speed that the text moves upward")]
    [SerializeField] private float speed = 2.0f;
    #endregion

    #region Duration
    [Header("Duration")]
    [Range(0.0f, 5.0f)]
    [Tooltip("The length of time that the text lasts for")]
    [SerializeField] private float duration = 2.0f;

    [Range(0.0f, 5.0f)]
    [Tooltip("The delay before starting to fade out")]
    [SerializeField] private float fadeoutDealy = 0.5f;
    #endregion

    #region Colors
    [Header("Color")]
    [Tooltip("The color to use when gaining points")]
    [SerializeField] private Color goodColor = Color.green;

    [Tooltip("The color to use when losing points")]
    [SerializeField] private Color badColor = Color.red;
    #endregion
    #endregion

    /// <summary>
    /// The current duration left for the score text.
    /// </summary>
    private float currentDuration;

    /// <summary>
    /// The starting color of this score text.
    /// </summary>
    private Color startingColor;

    /// <summary>
    /// The clear color of this score text (Color.Clear behaves weird during lerps so this is better).
    /// </summary>
    private Color clear;

    /// <summary>
    /// The text object that is displaying the score.
    /// </summary>
    private TextMeshPro text;
    #endregion

    #region Functions
    #region Initialization
    /// <summary>
    /// Handles initialization of fields and components.
    /// </summary>
    private void Awake()
    {
        currentDuration = duration;

        var pos = transform.position;
        pos.y += spawnYOffset;
        transform.position = pos;

        InitializeComponents();
    }

    /// <summary>
    /// Handles initialization of components.
    /// </summary>
    private void InitializeComponents()
    {
        text = GetComponent<TextMeshPro>();
    }

    /// <summary>
    /// Initializes the score that should be displayed on this object.
    /// </summary>
    /// <param name="score">The score that was earned.</param>
    public void InitializeScore(int score)
    {
        string scoreString = "";

        if (score > 0)
        {
            scoreString = "+";
        }

        // Sets text onto text object
        scoreString += score.ToString();
        text.text = scoreString;

        // Sets correct color for text
        text.color = score > 0 ? goodColor : badColor;
        startingColor = text.color;
        clear = startingColor;
        clear.a = 0;
    }
    #endregion

    /// <summary>
    /// Handles the movement and updating of the texts duration.
    /// </summary>
    private void FixedUpdate()
    {
        currentDuration -= Time.fixedDeltaTime;

        MoveText();

        UpdateColor();

        CheckDuration();
    }

    /// <summary>
    /// Checks if the text duration is over.
    /// </summary>
    private void CheckDuration()
    {

        if (currentDuration < 0.0f)
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Moves the text upwards.
    /// </summary>
    private void MoveText()
    {
        var pos = transform.position;
        pos.y += speed * Time.fixedDeltaTime;
        transform.position = pos;
    }

    /// <summary>
    /// Lerps the texts color towards being clear.
    /// </summary>
    private void UpdateColor()
    {
        text.color = Color.Lerp(startingColor, clear, Mathf.InverseLerp(duration - fadeoutDealy, 0.0f, currentDuration));

    }
    #endregion
}
