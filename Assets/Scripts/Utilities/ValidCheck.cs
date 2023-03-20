/*********************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Project Template
 * Creation Date: 3/14/2023 3:34:17 PM
 * 
 * Description: Functions to check if something is or isnt null.
 *              Basically just helps code look nicer with varying
 *              null checks.
*********************************/
using System.Collections.Generic;
using UnityEngine;

public class ValidCheck : MonoBehaviour
{
    #region Functions
    #region Null Checks
    #region Is Valid
    /// <summary>
    /// Takes in an object or struct and returns true if it isn't null.
    /// </summary>
    /// <typeparam name="T">Any generic type.</typeparam>
    /// <param name="obj">The object or struct you want to check.</param>
    /// <returns></returns>
    public static bool IsValid<T>(T obj)
    {
        return !EqualityComparer<T>.Default.Equals(obj, default(T));
    }
    #endregion

    #region Isnt Valid
    /// <summary>
    /// Takes in an object or struct and returns true if it is null.
    /// </summary>
    /// <typeparam name="T">Any generic type.</typeparam>
    /// <param name="obj">The object or struct you want to check.</param>
    /// <returns></returns>
    public static bool IsntValid<T>(T obj)
    {
        return EqualityComparer<T>.Default.Equals(obj, default(T));
    }

    public static bool IsntValid(Object obj)
    {
        return obj == null;
    }
    #endregion
    #endregion
    #endregion
}
