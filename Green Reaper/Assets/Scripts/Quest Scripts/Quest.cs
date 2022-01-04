using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum QuestType { GreaterThanOrEqualTo, LessThanOrEqualTo }

[System.Serializable]
public class QuestMonster
{
    public PlantType plantType;
    public int numberToKill;
    public QuestType questType;
}

[System.Serializable]
public class Quest
{
    public string title;
    public string questText;
    public int goldReward;
    public List<QuestMonster> monsters;

    private bool isActive;
    private int currentlyKilled;
    private bool completed;


    public void EnableQuest()
    {
        isActive = true;
    }

    public void DisableQuest()
    {
        isActive = false;
    }

    public bool IsActive()
    {
        return isActive;
    }

    public void IncreaseKillCount()
    {
        currentlyKilled++;
    }

    private void CheckCompletion()
    {
        //if (questType == QuestType.GreaterThanOrEqualTo)
        //{
        //    if (currentlyKilled >= numberToKill)
        //        completed = true;
        //}

        //if (questType == QuestType.LessThanOrEqualTo)
        //{
        //    if (currentlyKilled <= numberToKill)
        //        completed = true;
        //}
    }

    public int GetKillCount()
    {
        return currentlyKilled;
    }

    public bool Completed()
    {
        CheckCompletion();
        return completed;
    }
}


