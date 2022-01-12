using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WillsMasterpiece : MonoBehaviour
{

    public AudioSource audioSource;
    public float newSpeed = 1.2f;

    // Start is called before the first frame update
    void Start()
    {
        audioSource.pitch = newSpeed;
        audioSource.outputAudioMixerGroup.audioMixer.SetFloat("Pitch", 1f / newSpeed);
    }
}
