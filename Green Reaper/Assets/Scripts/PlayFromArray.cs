using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource)),
    RequireComponent(typeof(VolumeListener))]
public class PlayFromArray : MonoBehaviour
{
    enum PlayOnStartOptions {PLAY_INDEX, PLAY_RANDOM, NO_PLAY}

    private AudioSource audioSource;

    [SerializeField]
    private AudioClip[] clips;

    [SerializeField]
    private PlayOnStartOptions playOnStart;

    [SerializeField]
    private int startIndex = 0;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        switch(playOnStart)
        {
            case PlayOnStartOptions.PLAY_INDEX:
                PlayFromIndex(startIndex);
                break;
            case PlayOnStartOptions.PLAY_RANDOM:
                PlayRandom();
                break;
        }
    }

    public void PlayFromIndex(int index)
    {
        audioSource.clip = clips[index];
        audioSource.Play();
    }

    public void PlayRandom()
    {
        PlayFromIndex(Random.Range(0, clips.Length));
    }
}
