using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatPPGetter : PlayerPrefsGetter<float>
{
    protected override float GetValue(string name, float defaultValue)
    {
        return PlayerPrefs.GetFloat(name, defaultValue);
    }
}
