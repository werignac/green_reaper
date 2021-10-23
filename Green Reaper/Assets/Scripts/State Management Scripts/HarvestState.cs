using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HarvestState : MonoBehaviour
{
    public Text timeRemainingText;
    public Button beginHarvesting;
    public Button returnToHouse;
    public float startTime;

    private float timeRemaining;
    private bool decreaseTime = false;
    private int score;

    // Start is called before the first frame update
    void Start()
    {
        ChangeTimerText(startTime);
        beginHarvesting.gameObject.SetActive(true);
        returnToHouse.gameObject.SetActive(false);
        timeRemaining = startTime;
    }
    private void Update()
    {
        if (decreaseTime)
        {   
            if(timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                ChangeTimerText(timeRemaining);
            }
            else
            {
                EndGame();
            }

            
        }
        
    }

    private void ChangeTimerText(float time)
    {
        timeRemainingText.text = "Time Remaining: " + (int)time;
    }

    public void StartHarvesting()
    {
        beginHarvesting.gameObject.SetActive(false);
        decreaseTime = true;
    }

    private void EndGame()
    {
        timeRemainingText.text = "Score: " + score;
        returnToHouse.gameObject.SetActive(true);
    }

    public void ReturnToHouse()
    {
        GameManager.instance.LoadHouse();
    }
}
