using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooleanPPGetter : PlayerPrefsGetter<bool>
{
    protected override bool GetValue(string name, bool defaultValue)
    {
        if (PlayerPrefs.HasKey(name))
            return PlayerPrefs.GetInt(name) != 0;
        else
            return defaultValue;
    }
}
