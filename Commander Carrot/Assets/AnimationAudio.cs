using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationAudio : MonoBehaviour
{
    AudioSource audioSource;
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    void Footstep()
    {
        if(audioSource == null) return;

        Debug.Log("SHOULD BE MAKING FOOTSTEP SOUND!");

        audioSource.Play();
    }
}
