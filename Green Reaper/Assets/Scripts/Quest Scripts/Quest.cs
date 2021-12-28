using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum QuestType { GreaterThanOrEqualTo, LessThanOrEqualTo}

[System.Serializable]
public class Quest
{
    public string title;
    public string description;
    public int numberToKill;
    public int goldReward;
    public PlantType plantType;
    public QuestType questType;

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
        if(questType == QuestType.GreaterThanOrEqualTo)
        {
            if (currentlyKilled >= numberToKill)
                completed = true;
        }

        if (questType == QuestType.LessThanOrEqualTo)
        {
            if (currentlyKilled <= numberToKill)
                completed = true;
        }
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
