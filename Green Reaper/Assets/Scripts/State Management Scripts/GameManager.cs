using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instance of the menu manager that remains alive the entire life of the application.
    public static GameManager instance;

    public UpgradeHolder upgrades;

    public ValueHolder<int> globalScore = new ValueHolder<int>(0);

    [SerializeField]
    private WeaponController[] weapons;

    private GameObject winState;

    void Start()
    {
        upgrades = new UpgradeHolder();
        upgrades.SetWeapons(weapons);
    }

    private void Awake()
    {
        if (instance == null)
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
        GameObject.Find("Win");
        upgrades = new UpgradeHolder();
        upgrades.SetWeapons(weapons);
    }

    public void EndGame()
    {
        LoadMainMenu();
        winState.SetActive(true);
    }    
}
