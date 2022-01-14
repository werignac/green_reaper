using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QuestManager : MonoBehaviour
{
    //Instance of the manager that remains alive the entire life of the application.
    public static QuestManager instance;
    public GameObject instanceObject;

    public int QuestIndex;
    public List<Quest> quests;
    
    private Quest currentQuest;
    public UnityEvent updateText;

    void Start()
    {
        BoundsCheckQuestIndex();
    }

    private void BoundsCheckQuestIndex()
    {
        if (QuestIndex >= 0 && QuestIndex < quests.Count)
            currentQuest = quests[QuestIndex];

        if(QuestIndex < 0)
        {
            currentQuest = quests[0];
        }

        if (QuestIndex >= quests.Count)
        {
            currentQuest = quests[quests.Count - 1];
        }
    }

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(instance.instanceObject);
        }
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

    public Quest GetCurrentQuest()
    {
        return currentQuest;
    }

    public int GetCurrentQuestIndex()
    {
        return QuestIndex;
    }

    public void ContinueToNextQuest()
    {
        QuestIndex++;
        BoundsCheckQuestIndex();
    }

    public void SetQuestIndex(int index)
    {
        QuestIndex = index;
        BoundsCheckQuestIndex();
    }
}
