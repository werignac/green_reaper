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
    private ValueHolder<int> score;

    [SerializeField]
    private PlayerController player;
    [SerializeField]
    private Vector3 startLocation;
    [SerializeField]
    public UnityEvent<float> timePercentUpdate = new UnityEvent<float>();

    public PlayerController currentPlayer { get; private set; }
    public WeaponController currentWeapon { get; private set; }

    public UnityEvent<int> roundEnd = new UnityEvent<int>();
    public UnityEvent<int> scoreIncrement = new UnityEvent<int>();

    [SerializeField]
    public int maxNumberOfPowerUps;
    [SerializeField]
    public Vector2 PowerUpRangeX;
    [SerializeField]
    public Vector2 PowerUpRangeY;
    [SerializeField]
    private GameObject pepperPrefab;
    [SerializeField]
    private GameObject zucchiniPrefab;
    [SerializeField]
    private GameObject pumpkinPrefab;

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

        score = new ValueHolder<int>(0);
        score.valueChanged.AddListener((int x) => scoreIncrement?.Invoke(x));
        GeneratePowerups();
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
            else
            {
                EndRound();
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

        wCont.damageEvent.AddListener(IncrementScore);
    }

    private void EndRound()
    {
        returnToHouse.gameObject.SetActive(true);
        currentPlayer.SetReceivingInput(false);
        GameManager.instance.globalScore.SetValue(GameManager.instance.globalScore.GetValue() + score.GetValue());
        roundEnd?.Invoke(score.GetValue());
    }

    private void EndGame()
    {
        GameManager.instance.LoadMainMenu(); // TODO: Switch to Endscreen
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
        score.SetValue(score.GetValue() + amount);
    }

    private void GeneratePowerups()
    {
        Dictionary<UpgradeHolder.UpgradeType, float> powerUps = new Dictionary<UpgradeHolder.UpgradeType, float>();
        
        float totalWeight = SumWeightsAndPopulateDictionary(powerUps);
        float randomWeight;
        
        for (int i = 0; i < maxNumberOfPowerUps; i++)
        {
            randomWeight = Random.Range(0, totalWeight);
            foreach (UpgradeHolder.UpgradeType upType in powerUps.Keys)
            {
                if (randomWeight <= powerUps[upType] && powerUps[upType] != 0)
                {
                    DeterminePowerupToSpawn(upType);
                    break;
                }

                randomWeight -= powerUps[upType];
            }
        }
        
    }

    private void DeterminePowerupToSpawn(UpgradeHolder.UpgradeType type)
    {
        switch (type)
        {
            case UpgradeHolder.UpgradeType.PEPPERFREQUENCY:
                SpawnPowerup(pepperPrefab);
                break;
            
            case UpgradeHolder.UpgradeType.ZUCCINNIFREQUENCY:
                SpawnPowerup(zucchiniPrefab);
                break;
            
            case UpgradeHolder.UpgradeType.PUMPKINFREQUENCY:
                SpawnPowerup(pumpkinPrefab);
                break;
            
            default:
                break;
        }
    }

    private void SpawnPowerup(GameObject powerup)
    {
        float xPos;
        float yPos;

        xPos = Random.Range(PowerUpRangeX.x, PowerUpRangeX.y);
        yPos = Random.Range(PowerUpRangeY.x, PowerUpRangeY.y);

        // Creates the object at the random position.
        GameObject pw = Instantiate(powerup, new Vector3(xPos, yPos, 0), transform.rotation);
    }

    private float SumWeightsAndPopulateDictionary(Dictionary<UpgradeHolder.UpgradeType, float> powerUps)
    {
        float totalWeight = 0;

        //Get the weights for all powerups. 
        for (int i = 4; i < 7; i++)
        {
            // Determine type and weight, then add the weight to the total.
            UpgradeHolder.UpgradeType upgradeType = (UpgradeHolder.UpgradeType)i;
            float upgradeWeight = GameManager.instance.upgrades.GetMultiplier(upgradeType);
            totalWeight += upgradeWeight;


            powerUps.Add(upgradeType, upgradeWeight);
        }

        return totalWeight;
    }
}
