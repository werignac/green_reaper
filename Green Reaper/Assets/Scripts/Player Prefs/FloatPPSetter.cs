using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatPPSetter : PlayerPrefSetter<float>
{
    protected override void SetValue(string prefName, float valueToSet)
    {
        PlayerPrefs.SetFloat(prefName, valueToSet);
    }
}
