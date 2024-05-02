using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkSounds : MonoBehaviour
{
    [SerializeField] EntityAnimation entityAnimation;
    Audible stepAudibles;
    AudioSource audioSource;
    [SerializeField] AudioClip[] stepSounds;
    [SerializeField] bool isPlayer;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if(isPlayer )
        {
            stepAudibles = GetComponent<Audible>();
        }
    }
    void Step()
    {
        int randomInt = Random.Range(0, stepSounds.Length);
        if(isPlayer )
        {
            stepAudibles.enabled = true;
        }
        if(entityAnimation.running)
        {
            if(isPlayer)
            {
                stepAudibles.range = 7;
            }
            audioSource.volume = 1;      
        }
        else
        {
            if (isPlayer)
            {
                stepAudibles.range = 2.5f;
            }
            audioSource.volume = 0.6f;
        }
        audioSource.clip = stepSounds[randomInt];
        audioSource.Play();
    }
}
