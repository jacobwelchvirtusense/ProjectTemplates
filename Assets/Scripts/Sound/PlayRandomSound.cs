/******************************************************************
 * Created by: Jacob Welch
 * Email: jacobw@virtusense.com
 * Company: Virtusense
 * Project: Cannon Fodder
 * Creation Date: 4/25/2023 4:55:11 PM
 * 
 * Description: TODO
******************************************************************/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InspectorValues;
using static ValidCheck;

[RequireComponent(typeof(AudioSource))]
public class PlayRandomSound : MonoBehaviour
{
    #region Fields
    private AudioSource audioSource;

    [SerializeField] private AudioClip[] listOfSounds;
    #endregion

    #region Functions
    // Start is called before the first frame update
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        audioSource.PlayOneShot(listOfSounds[Random.Range(0, listOfSounds.Length)]);
        Destroy(gameObject, 10);
    }
    #endregion
}
