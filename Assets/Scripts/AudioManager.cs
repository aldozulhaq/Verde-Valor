using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    AudioSource audioSource;

    public AudioClip menu;
    public AudioClip combat;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void PlayBGMMenu()
    {
        audioSource.clip = menu;
        audioSource.Play();
    }

    public void PlayBGMCombat()
    {
        audioSource.clip = combat;
        audioSource.Play();
    }
}
