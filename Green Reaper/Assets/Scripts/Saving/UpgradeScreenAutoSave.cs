using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeScreenAutoSave : MonoBehaviour
{
    public void AutoSave()
    {
        GameManager.instance.SaveGame(GameManager.instance.GetSaveName());
    }
}
