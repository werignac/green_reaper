using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int coins;
    public int questIndex;
    public int[] upgrades;

    public SaveData(int coinsToStore, int currentQuest, int[] upgradesToStore)
    {
        coins = coinsToStore;
        questIndex = currentQuest;
        upgrades = upgradesToStore;
    }
}
