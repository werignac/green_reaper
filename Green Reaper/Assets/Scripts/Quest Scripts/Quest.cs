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
    private int currentlyKilled;

    public void IncrementKillCount()
    {
        currentlyKilled++;
    }

    public void ResetMonster()
    {
        currentlyKilled = 0;
    }

    public int GetKillCount()
    {
        return currentlyKilled;
    }

    public bool CheckCompletion()
    {
        // Default to false, but if completion criteria is met then set to true.
        bool completed = false;

        if (questType == QuestType.GreaterThanOrEqualTo)
        {
            if (currentlyKilled >= numberToKill)
                completed = true;
        }

        if (questType == QuestType.LessThanOrEqualTo)
        {
            if (currentlyKilled <= numberToKill)
                completed = true;
        }

        return completed;
    }
}

[System.Serializable]
public class Quest
{
    public string title;
    public string questText;
    public int goldReward;
    public List<QuestMonster> monsters;

    /// <summary>
    /// Check if the type of plant given is used in the current quest.
    /// If the type matches one of the tracked monsters, the kill count is updated.
    /// </summary>
    /// <param name="type"></param>
    public void CheckAndUpdateKillcounts(PlantType type)
    {
        foreach (QuestMonster monster in monsters)
        {
            if (monster.plantType == type)
            {
                monster.IncrementKillCount();
            }
        }
    }


    /// <returns>Whether or not all criteria for the quest was successfully met.</returns>
    public bool Completed()
    {
        // Default to true, and if one of the quests was incomplete switch to false.
        bool everyRequirementCompleted = true;

        foreach (QuestMonster monster in monsters)
        {
            if (!monster.CheckCompletion())
            {
                everyRequirementCompleted = false;
            }
        }

        return everyRequirementCompleted;
    }

    public void Reset()
    {
        foreach (QuestMonster monster in monsters)
        {
            monster.ResetMonster();
        }
    }
}


