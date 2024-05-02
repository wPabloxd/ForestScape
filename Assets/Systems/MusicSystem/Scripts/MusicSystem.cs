using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicSystem : MonoBehaviour
{
    public enum SongState
    {
        stealth,
        loud
    }
    public SongState songState;

    AudioSource audioSource;
    [SerializeField] AudioClip[] stealthClips;
    int stealthClipsCount;
    [SerializeField] AudioClip[] loudClips;
    int loudClipsCount;
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.volume = 10;
    }
    private void Update()
    {
        switch (GameManager.Instance.songPhase)
        {
            case 0:
                songState = SongState.stealth; 
                break;
            case 1:
                songState = SongState.loud;
                break;
        }
        switch (songState)
        {
            case SongState.stealth:
                if (!audioSource.isPlaying)
                {
                    loudClipsCount = 0;
                    audioSource.clip = stealthClips[stealthClipsCount];
                    audioSource.Play();
                    stealthClipsCount++;
                    if(stealthClipsCount == stealthClips.Length)
                    {
                        stealthClipsCount = 0;
                    }
                }
                break;
            case SongState.loud:
                if (!audioSource.isPlaying)
                {
                    stealthClipsCount = 0;
                    audioSource.clip = loudClips[loudClipsCount];
                    audioSource.Play();
                    loudClipsCount++;
                    if (stealthClipsCount == loudClips.Length)
                    {
                        loudClipsCount = 0;
                    }
                }
                break;
        }
    }
}