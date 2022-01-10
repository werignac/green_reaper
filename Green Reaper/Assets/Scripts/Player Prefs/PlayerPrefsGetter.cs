using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public abstract class PlayerPrefsGetter<T> : MonoBehaviour
{
    [SerializeField]
    private string playerPrefName;

    [SerializeField]
    private T defaultValue;

    [SerializeField]
    private bool runOnStart;

    [SerializeField]
    private UnityEvent<T> onValueGet;

    private void Start()
    {
        if (runOnStart)
            GetPlayerPref();
    }

    public void GetPlayerPref()
    {
        onValueGet?.Invoke(GetValue(playerPrefName, defaultValue));
    }

    protected abstract T GetValue(string name, T defaultValue);
}
