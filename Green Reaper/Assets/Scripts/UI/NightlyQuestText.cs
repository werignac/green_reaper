using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public enum TextState {Title, Description, RewardValue, Completion, CompletionReward, CoinDifference}

public class NightlyQuestText : MonoBehaviour
{
    public TextState state;
    public Text textToChange;
    private Quest quest;

    private void Awake()
    {
        quest = QuestManager.instance.GetCurrentQuest();
        QuestManager.instance.updateText.AddListener(ModifyText);
    }

    public void ModifyText()
    {
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

            case TextState.Completion:
                if (quest.Completed())
                    textToChange.text = "Completed!";
                else
                    textToChange.text = "Failure!";
                break;

            case TextState.CompletionReward:
                if (quest.Completed())
                    textToChange.text = "" + quest.goldReward;
                else
                    textToChange.text = "0";
                break;

            case TextState.CoinDifference:
                textToChange.text = "" + HarvestState.instance.DifferenceInCoinsThisRound();
                break;

            default:
                break;
        }
    }
}
