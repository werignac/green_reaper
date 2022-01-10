using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringPPSetter : PlayerPrefSetter<string>
{
    protected override void SetValue(string prefName, string valueToSet)
    {
        PlayerPrefs.SetString(prefName, valueToSet);
    }
}
