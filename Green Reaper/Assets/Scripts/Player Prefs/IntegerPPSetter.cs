using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegerPPSetter : PlayerPrefSetter<int>
{
    protected override void SetValue(string prefName, int valueToSet)
    {
        PlayerPrefs.SetInt(prefName, valueToSet);
    }
}
