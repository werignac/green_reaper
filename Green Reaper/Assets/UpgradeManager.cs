using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    public UnityEvent<int> scoreChange = new UnityEvent<int>();

    private GameManager manager;

    private void Start()
    {
        manager = GameManager.instance;
    }




}
