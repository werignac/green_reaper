using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BooleanPPSetter : PlayerPrefSetter<bool>
{
    protected override void SetValue(string prefName, bool valueToSet)
    {
        PlayerPrefs.SetInt(prefName, (valueToSet) ? 1 : 0);
    }
}
