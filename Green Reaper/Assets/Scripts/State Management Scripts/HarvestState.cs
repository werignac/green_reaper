using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class HarvestState : MonoBehaviour
{
    public static HarvestState instance;

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
    [SerializeField]
    public UnityEvent<float> timePercentUpdate = new UnityEvent<float>();

    public PlayerController currentPlayer;
    public WeaponController currentWeapon;

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

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
                timePercentUpdate.Invoke(1 - (timeRemaining / startTime));
            }
            else
            {
                EndGame();
            }

            
        }
        
    }

    public void StartHarvesting()
    {
        beginHarvesting.gameObject.SetActive(false);
        decreaseTime = true;

        InstatiatePlayer();
    }

    private void InstatiatePlayer()
    {
        GameObject playerInstance = Instantiate(player.gameObject);
        GameObject weaponInstance = Instantiate(GameManager.instance.upgrades.GetWeapon().gameObject, playerInstance.transform);

        Camera.main.transform.parent = playerInstance.transform;

        Camera.main.transform.localPosition = new Vector3(0, 0, Camera.main.transform.localPosition.z);

        weaponInstance.SetActive(false);

        playerInstance.transform.position = startLocation;

        PlayerController pCont = playerInstance.GetComponent<PlayerController>();
        WeaponController wCont = weaponInstance.GetComponent<WeaponController>();

        currentPlayer = pCont;
        currentWeapon = wCont;

        pCont.BuffMaxSpeed(GameManager.instance.upgrades.GetMultiplierBuff(UpgradeHolder.UpgradeType.SPEED));
        pCont.SetWeapon(wCont);
        wCont.AddDamageBuff(GameManager.instance.upgrades.GetMultiplierBuff(UpgradeHolder.UpgradeType.DAMAGE));
        wCont.AddSpeedBuff(GameManager.instance.upgrades.GetMultiplierBuff(UpgradeHolder.UpgradeType.ATTACKSPEED));
    }

    private void EndGame()
    {
        //timeRemainingText.text = "Score: " + score;
        returnToHouse.gameObject.SetActive(true);
        currentPlayer.SetReceivingInput(false);
    }

    public void ReturnToHouse()
    {
        GameManager.instance.LoadHouse();
    }

    private void OnDestroy()
    {
        if (this == instance)
            instance = null;
    }
}
