using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeScreenAutoSave : MonoBehaviour
{
    public void Save()
    {
        GameManager.instance.SaveGame(GameManager.instance.GetSaveName());
    }
}
