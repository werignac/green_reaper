using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum PlantType {CORN = 0, PEPPER = 1, PUMPKIN = 2}

public class PlantHealth : MonoBehaviour
{
    [SerializeField]
    private int baseHealth;

    private ValueHolder<int> health;

    public UnityEvent<PlantType> deathEvent = new UnityEvent<PlantType>();

    [SerializeField]
    private PlantType type;

    private void Start()
    {
        health = new ValueHolder<int>(baseHealth);
        health.valueDecreased.AddListener((int x) =>
        {
            if (x == 0)
            {
                deathEvent?.Invoke(type);
                OnDeath();
            }
        });
    }

    public void ChangeHealth(int amountChanged)
    {
        health.SetValue(Mathf.Clamp(health.GetValue() + amountChanged,0,baseHealth));
    }

    protected virtual void OnDeath()
    {
        gameObject.SetActive(false);
    }

}
