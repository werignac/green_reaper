using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instance of the menu manager that remains alive the entire life of the application.
    public static GameManager instance;

    public UpgradeHolder upgrades;

    public ValueHolder<int> globalScore;
    public int startingGold;

    [SerializeField]
    private WeaponController[] weapons;

    private GameObject winState;

    private int numberOfUpgradesPurchased = 0;
    private int moneySpentOnUpgrades = 0;

    void Start()
    {
        globalScore = new ValueHolder<int>(startingGold);
        upgrades = new UpgradeHolder();
        upgrades.SetWeapons(weapons);

        SceneManager.sceneLoaded += OnLoadScene;
    }

    private void Awake()
    {
        instance = this;
    }

    /// <summary>
    /// Loads the Farm level.
    /// </summary>
    public void LoadFarm()
    { 
     SceneManager.LoadScene(1);
    }

    public void LoadHouse()
    {
        SceneManager.LoadScene(2);
    }

    /// <summary>
    /// Loads the Farm level.
    /// </summary>
    public void LoadMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    private void OnLoadScene(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            GameObject.Find("Canvas").transform.Find("Win").gameObject.SetActive(true);
            upgrades = new UpgradeHolder();
            upgrades.SetWeapons(weapons);
            Destroy(gameObject);
        }
    }

    public void EndGame()
    {
        LoadMainMenu();
    }

    public void SpendMoneyOnUpgrade(int amountToSpend)
    {
        globalScore.SetValue(globalScore.GetValue() - amountToSpend);
        numberOfUpgradesPurchased++;
        moneySpentOnUpgrades += amountToSpend;
    }
    
    public int GetNumberOfUpgradesPurchased()
    {
        return numberOfUpgradesPurchased;
    }

    public int GetMoneySpentOnUpgrades()
    {
        return moneySpentOnUpgrades;
    }
  
}
