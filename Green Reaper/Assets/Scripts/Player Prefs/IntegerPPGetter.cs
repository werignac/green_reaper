using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntegerPPGetter : PlayerPrefsGetter<int>
{
    protected override int GetValue(string name, int defaultValue)
    {
        return PlayerPrefs.GetInt(name, defaultValue);
    }
}
