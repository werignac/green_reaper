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

    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private Vector3 startLocation;

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

    private void InstatiatePlayer()
    {
        GameObject playerInstance = Instantiate(player.gameObject);
        GameObject weaponInstance = Instantiate(GameManager.instance.upgrades.GetWeapon().gameObject, playerInstance.transform);

        playerInstance.transform.position = startLocation;

        PlayerController pCont = playerInstance.GetComponent<PlayerController>();
        WeaponController wCont = weaponInstance.GetComponent<WeaponController>();

        pCont.BuffMaxSpeed(GameManager.instance.upgrades.GetMultiplierBuff(UpgradeHolder.UpgradeType.SPEED));
        wCont.AddDamageBuff(GameManager.instance.upgrades.GetMultiplierBuff(UpgradeHolder.UpgradeType.DAMAGE));
        wCont.AddSpeedBuff(GameManager.instance.upgrades.GetMultiplierBuff(UpgradeHolder.UpgradeType.ATTACKSPEED));
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
