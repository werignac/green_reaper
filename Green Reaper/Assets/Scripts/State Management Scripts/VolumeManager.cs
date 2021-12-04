using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class VolumeManager : MonoBehaviour
{
    public float volume;
    public UnityEvent<float> VolumeChanged;

    //Instance of the menu manager that remains alive the entire life of the application.
    public static VolumeManager instance;

    public void SetVolume(float newVol)
    {
        volume = newVol;
        VolumeChanged.Invoke(newVol);
    }

    private void Awake()
    {
        instance = this;
    }
}
