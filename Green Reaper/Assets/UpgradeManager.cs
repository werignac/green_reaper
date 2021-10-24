using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class UpgradeManager : MonoBehaviour
{
    public UnityEvent<int> scoreChange = new UnityEvent<int>();

    public UnityEvent successfulPurchase = new UnityEvent();
    public UnityEvent failedPurchase = new UnityEvent();

    private GameManager manager;

    

    private void Start()
    {
        manager = GameManager.instance;
        manager.globalScore.valueChanged.AddListener((int score) => scoreChange?.Invoke(score));

        scoreChange.Invoke(manager.globalScore.GetValue());
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
    


}
