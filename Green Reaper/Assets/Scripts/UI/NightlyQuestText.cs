using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum TextState {Title, Description, RewardValue}

public class NightlyQuestText : MonoBehaviour
{
    public TextState state;
    public Text textToChange;

    // Start is called before the first frame update
    void Start()
    {
        Quest quest = QuestManager.instance.GetCurrentQuest();

        switch (state)
        {
            case TextState.Title:
                textToChange.text = quest.title;
                break;

            case TextState.Description:
                textToChange.text = quest.questText;
                break;

            case TextState.RewardValue:
                textToChange.text = "" + quest.goldReward;
                break;

            default:
                break;
        }
    }
}
