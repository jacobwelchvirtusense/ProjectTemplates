/*
 * (Jacob Welch)
 * (Singleton)
 * (Singleton and Object Pooling Pattern)
 * (Description: A basic singleton pattern that can be reused by any MonoBehaviour class.)
 */
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T: Singleton<T>
{
    #region Fields
    /// <summary>
    /// The scene instance of this script.
    /// </summary>
    public static T Instance { get; private set; }
    #endregion

    #region Functions
    /// <summary>
    /// Handles initilization of components and other fields before anything else.
    /// </summary>
    protected virtual void Awake()
    {
        if(Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = (T)this;
        }
        else
        {
            DestroyImmediate(gameObject);
            return;
        }
    }

    /// <summary>
    /// Removes the script reference when this object is destroyed.
    /// </summary>
    protected virtual void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
    #endregion
}
