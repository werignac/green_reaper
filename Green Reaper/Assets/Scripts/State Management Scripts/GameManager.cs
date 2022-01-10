using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instance of the menu manager that remains alive the entire life of the application.
    public static GameManager instance;
    public GameObject instanceObject;

    public UpgradeHolder upgrades;

    public ValueHolder<int> globalScore;
    public int startingGold;

    [SerializeField]
    private WeaponController[] weapons;

    private GameObject winState;

    private int numberOfUpgradesPurchased = 0;
    private int moneySpentOnUpgrades = 0;
    private string currentSaveName;

    void Start()
    {
        globalScore = new ValueHolder<int>(startingGold);
        upgrades = new UpgradeHolder();
        upgrades.SetWeapons(weapons);

        SceneManager.sceneLoaded += OnLoadScene;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.instanceObject);
        }

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
            //GameObject.Find("Canvas").transform.Find("Win").gameObject.SetActive(true);
            upgrades = new UpgradeHolder();
            upgrades.SetWeapons(weapons);
        }
    }

    public void ReturnToMainMenu()
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

    public void SaveGame(string fileName)
    {
        SaveSystem.SaveGame(globalScore.GetValue(), QuestManager.instance.QuestIndex, fileName, upgrades.UpgradesToArray());
    }
    
    /// <summary>
    /// Loads the current save file.
    /// If the save file did not exist, it is created and then loaded.
    /// </summary>
    /// <param name="fileName">File to open.</param>
    public void LoadGame(string fileName)
    {
        SaveData data = SaveSystem.LoadGame(fileName);
        
        // If the data was null then we want to create a new save, and load that.
        if(data == null)
        {
            SaveGame(fileName);
            data = SaveSystem.LoadGame(fileName);
            // Give new saves starting gold.
            data.coins = startingGold;
        }

        globalScore.SetValue(data.coins);
        QuestManager.instance.SetQuestIndex(data.questIndex);
        upgrades.ArrayToUpgrades(data.upgrades);
        currentSaveName = fileName;

        LoadFarm();
    }

    public void DeleteSave(string fileName)
    {
        SaveSystem.DeleteGame(fileName);
    }

    public string GetSaveName()
    {
        return currentSaveName;
    }
}
