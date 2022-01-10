using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerPrefSetter <T> : MonoBehaviour
{
    [SerializeField]
    private string prefName;

    public void SetPref(T valueToSet)
    {
        SetValue(prefName, valueToSet);
    }

    protected abstract void SetValue(string prefName, T valueToSet);
    
}
