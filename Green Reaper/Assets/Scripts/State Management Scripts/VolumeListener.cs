using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VolumeListener : MonoBehaviour
{
    public AudioSource source;

    // Start is called before the first frame update
    void Start()
    {
        VolumeManager.instance.VolumeChanged.AddListener(ChangeVolume);
        ChangeVolume(VolumeManager.instance.volume);
    }

    public void ChangeVolume(float newVolume)
    {
        source.volume = newVolume;
    }
}
