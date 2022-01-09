using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    public static UpgradeManager instance;

    public UnityEvent<int> scoreChange = new UnityEvent<int>();

    public UnityEvent successfulPurchase = new UnityEvent();
    public UnityEvent failedPurchase = new UnityEvent();

    private GameManager manager;

    public UnityEvent<int> speedUpdate = new UnityEvent<int>();
    public UnityEvent<int> damageUpdate = new UnityEvent<int>();
    public UnityEvent<int> attackSpeedUpdate = new UnityEvent<int>();
    public UnityEvent<int> aoeUpdate = new UnityEvent<int>();
    public UnityEvent<int> pepperUpdate = new UnityEvent<int>();
    public UnityEvent<int> pumpkinUpdate = new UnityEvent<int>();
    public UnityEvent<int> zuccinniUpdate = new UnityEvent<int>();

    private Dictionary<UpgradeHolder.UpgradeType, int[]> costsOfUpgrades = new Dictionary<UpgradeHolder.UpgradeType, int[]>()
    {
        {UpgradeHolder.UpgradeType.SPEED, new int[]{60, 200, 800}},
        {UpgradeHolder.UpgradeType.DAMAGE, new int[] {150, 350, 700}},
        {UpgradeHolder.UpgradeType.ATTACKSPEED, new int[] {100, 300, 600} },
        {UpgradeHolder.UpgradeType.SCYTHESIZE, new int[]{80, 400, 1000} },
        {UpgradeHolder.UpgradeType.PEPPERPROBABILITY, new int[]{60, 150, 400} },
        {UpgradeHolder.UpgradeType.PUMPKINPROBABILITY, new int[]{60, 200, 500} },
        {UpgradeHolder.UpgradeType.ZUCCINNIPROBABILITY, new int[]{60, 200, 500} },
    };


    private void Start()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);

        manager = GameManager.instance;
        manager.globalScore.valueChanged.AddListener((int score) => scoreChange?.Invoke(score));

        scoreChange.Invoke(manager.globalScore.GetValue());

        UpdateVisuals();
    }

    public void TryPurchaseUpgrade(UpgradeHolder.UpgradeType type)
    {
        int cost = costsOfUpgrades[type][manager.upgrades.GetUpgradeLevel(type)];

        if (TryPurchase(cost))
        {
            manager.upgrades.IncrementUpgradeLevel(type);
            UpdateVisuals();
        }
    }

    public int NextCost(UpgradeHolder.UpgradeType type)
    {
        return costsOfUpgrades[type][manager.upgrades.GetUpgradeLevel(type)];
    }


    private bool TryPurchase(int amountToTry)
    {
        int amountLeft = manager.globalScore.GetValue() - amountToTry;
        if (amountLeft < 0)
        {
            failedPurchase.Invoke();
            return false;
        }
        else
        {
            manager.globalScore.SetValue(amountLeft);
            successfulPurchase.Invoke();
            return true;
        }
    }

    private void UpdateVisuals()
    {
        speedUpdate?.Invoke(manager.upgrades.GetUpgradeLevel(UpgradeHolder.UpgradeType.SPEED));
        damageUpdate?.Invoke(manager.upgrades.GetUpgradeLevel(UpgradeHolder.UpgradeType.DAMAGE));
        attackSpeedUpdate?.Invoke(manager.upgrades.GetUpgradeLevel(UpgradeHolder.UpgradeType.ATTACKSPEED));
        aoeUpdate?.Invoke(manager.upgrades.GetUpgradeLevel(UpgradeHolder.UpgradeType.SCYTHESIZE));
        pepperUpdate?.Invoke(manager.upgrades.GetUpgradeLevel(UpgradeHolder.UpgradeType.PEPPERPROBABILITY));
        pumpkinUpdate?.Invoke(manager.upgrades.GetUpgradeLevel(UpgradeHolder.UpgradeType.PUMPKINPROBABILITY));
        zuccinniUpdate?.Invoke(manager.upgrades.GetUpgradeLevel(UpgradeHolder.UpgradeType.ZUCCINNIPROBABILITY));
    }

    private void OnDestroy()
    {
        if (this == instance)
            instance = null;
    }

    public void NextRound()
    {
        GameManager.instance.LoadFarm();
    }
}
