using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    //Instance of the manager that remains alive the entire life of the application.
    public static QuestManager instance;

    public int QuestIndex;
    public List<Quest> quests;
    
    private Quest currentQuest;

    void Start()
    {
        if(QuestIndex >= 0 && QuestIndex < quests.Count)
            currentQuest = quests[QuestIndex];
    }

    private void Awake()
    {
        instance = this;
    }

    public void PlantDied(PlantType type)
    {
        if (currentQuest == null)
            return;

        currentQuest.CheckAndUpdateKillcounts(type);
    }

    public bool QuestComplete()
    {
        if (currentQuest == null)
            return false;

        return currentQuest.Completed();
    }

    public void ResetCurrentQuest()
    {
        currentQuest.Reset();
    }

    public int GetQuestRewards()
    {
        if (currentQuest == null)
            return 0;

        return currentQuest.goldReward;
    }
}
