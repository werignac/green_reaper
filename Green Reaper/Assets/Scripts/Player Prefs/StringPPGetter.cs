using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringPPGetter : PlayerPrefsGetter<string>
{
    protected override string GetValue(string name, string defaultValue)
    {
        return PlayerPrefs.GetString(name, defaultValue);
    }
}
