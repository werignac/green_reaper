using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //Instance of the menu manager that remains alive the entire life of the application.
    public static GameManager instance;

    void Start()
    {
    }

    private void Awake()
    {
        if(instance == null)
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
}
