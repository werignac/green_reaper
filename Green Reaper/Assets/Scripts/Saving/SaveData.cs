using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int coins;
    public int questIndex;

    public SaveData(int coinsToStore, int currentQuest)
    {
        coins = coinsToStore;
        questIndex = currentQuest;
    }
}
