using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SaveData
{
    public int Coins { get; private set; }
    public int QuestIndex { get; private set; }
    public int[] Upgrades { get; private set; }
    public bool QuestsCompleted { get; private set; }
    public int NumberOfUpgradesPurchased { get; private set; }
    public int MoneySpentOnUpgrades { get; private set; }

    public SaveData(int coinsToStore, int currentQuest, int[] upgradesToStore, bool questsCompletion,
        int numberOfUpgradesPurchased, int moneySpentOnUpgrades)
    {
        Coins = coinsToStore;
        QuestIndex = currentQuest;
        Upgrades = upgradesToStore;
        QuestsCompleted = questsCompletion;
        NumberOfUpgradesPurchased = numberOfUpgradesPurchased;
        MoneySpentOnUpgrades = moneySpentOnUpgrades;
    }
}
