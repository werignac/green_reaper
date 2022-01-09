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
    public GameObject nightStart;
    public GameObject nightEnd;
    public float startTime;

    private float timeRemaining;
    private bool decreaseTime = false;
    private bool endRound = false;
    private int roundStartMoney;
    private int roundEndMoney;

    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private Vector3 startLocation;
    [SerializeField]
    public UnityEvent<float> timePercentUpdate = new UnityEvent<float>();

    public PlayerController currentPlayer { get; private set; }
    public GameObject playerInstance { get; private set; }
    public WeaponController currentWeapon { get; private set; }
    public BuffVisualizersManager buffProgresses;

    public UnityEvent<int> roundEnd = new UnityEvent<int>();
    public UnityEvent<int> scoreIncrement = new UnityEvent<int>();

    // Start is called before the first frame update
    void Start()
    {
        if (instance != null)
            Destroy(gameObject);
        else
            instance = this;

        beginHarvesting.gameObject.SetActive(true);
        returnToHouse.gameObject.SetActive(false);
        nightStart.gameObject.SetActive(true);
        nightEnd.gameObject.SetActive(false);
        QuestManager.instance.updateText.Invoke();
        timeRemaining = startTime;

        scoreIncrement?.Invoke(GameManager.instance.globalScore.GetValue());
        ResumeGame();
    }

    private void Update()
    {
        if (decreaseTime)
        {
            if (timeRemaining > 0)
            {
                timeRemaining -= Time.deltaTime;
                timePercentUpdate.Invoke(1 - (timeRemaining / startTime));
            }
            else if (!endRound)
            {
                EndRound();
            }
        }

    }

    public void StartHarvesting()
    {
        beginHarvesting.gameObject.SetActive(false);
        nightStart.gameObject.SetActive(false);
        decreaseTime = true;
        roundStartMoney = GameManager.instance.globalScore.GetValue();

        InstatiatePlayer();
        QuestManager.instance.ResetCurrentQuest();
    }

    private void InstatiatePlayer()
    {
        playerInstance = Instantiate(player.gameObject);
        GameObject weaponInstance = Instantiate(GameManager.instance.upgrades.GetWeapon().gameObject, new Vector3(0, 0, 0.01f), Quaternion.Euler(Vector3.zero), playerInstance.transform);

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
        wCont.SetAOE((int)GameManager.instance.upgrades.GetMultiplier(UpgradeHolder.UpgradeType.SCYTHESIZE));
    }

    private void EndRound()
    {
        returnToHouse.gameObject.SetActive(true);
        currentPlayer.SetReceivingInput(false);
        roundEnd?.Invoke(GameManager.instance.globalScore.GetValue());
        nightEnd.gameObject.SetActive(true);

        if (QuestManager.instance.QuestComplete())
        {
            IncrementScore(QuestManager.instance.GetQuestRewards());
        }
        roundEndMoney = GameManager.instance.globalScore.GetValue();
        QuestManager.instance.updateText.Invoke();

        // Tragically this check must be done again.
        if (QuestManager.instance.QuestComplete())
        {
            QuestManager.instance.ContinueToNextQuest();
        }

        endRound = true;
    }

    public int DifferenceInCoinsThisRound()
    {
        return roundEndMoney - roundStartMoney;
    }

    public void ReturnToMenu()
    {
        endRound = true;
        GameManager.instance.ReturnToMainMenu();
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

    public void IncrementScore(int amount)
    {
        GameManager.instance.globalScore.SetValue(GameManager.instance.globalScore.GetValue() + amount);
        scoreIncrement?.Invoke(GameManager.instance.globalScore.GetValue());
    }

    public int DecrementScore(int amount)
    {
        // Store initial value of the score.
        int differenceBetweenValues = GameManager.instance.globalScore.GetValue();

        // Subtract from score.
        int subtractedTotal = GameManager.instance.globalScore.GetValue() - amount;
        // Minimum value is 0. 
        if (subtractedTotal < 0)
            subtractedTotal = 0;
        GameManager.instance.globalScore.SetValue(subtractedTotal);
        scoreIncrement?.Invoke(GameManager.instance.globalScore.GetValue());

        // Find the difference
        differenceBetweenValues -= GameManager.instance.globalScore.GetValue();
        return differenceBetweenValues;
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void VolumeChanged(float newVolume)
    {
        VolumeManager.instance.SetVolume(newVolume);
    }
}
