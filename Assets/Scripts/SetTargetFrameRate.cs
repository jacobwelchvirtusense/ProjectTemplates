/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 2/1/2023 8:22:57 AM
 * 
 * Description: Sets the maximum framerate of the application.
*********************************/
using UnityEngine;

public class SetTargetFrameRate : MonoBehaviour
{
    #region Fields
    [Range(0, 240)]
    [Tooltip("The frame rate to limit the user by")]
    [SerializeField] private int targetFrameRate = 120;
    #endregion

    #region Functions
    /// <summary>
    /// Initializes the users frame rate.
    /// </summary>
    private void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = targetFrameRate;
    }
    #endregion
}
