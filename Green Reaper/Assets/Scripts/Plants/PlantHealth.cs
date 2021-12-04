using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Buffs;
using System;

public enum PlantType {CORN = 0, PEPPER = 1, PUMPKIN = 2, CORN2 = 3, CORN3 = 4, ZUCCHINI = 5, ROOTMONSTER = 6}

public class PlantHealth : MonoBehaviour
{
    [SerializeField]
    protected int baseHealth;

    protected ValueHolder<int> health;

    public UnityEvent damageTaken = new UnityEvent();

    public UnityEvent<PlantType> deathEvent = new UnityEvent<PlantType>();

    [SerializeField]
    private PlantType type;

    [SerializeField]
    private bool slowsPlayer;

    private void Start()
    {
        Initialize();
    }

    protected void Initialize()
    {
        if (!GameManager.instance.GetProcedural())
            transform.localScale = transform.localScale * 0.056844f;

        health = new ValueHolder<int>(baseHealth);
        health.valueDecreased.AddListener((int x) =>
        {
            if (x == 0)
            {
                deathEvent?.Invoke(type);
                OnDeath();
            }
            else
                damageTaken?.Invoke();
        });
    }

    public virtual void ChangeHealth(int amountChanged)
    {
        health.SetValue(Mathf.Clamp(health.GetValue() + amountChanged,0,baseHealth));
    }

    protected virtual void OnDeath()
    {
        HarvestState.instance.IncrementScore(baseHealth);
        HarvestState.instance.IncrementCornDeaths(type);
        Destroy(gameObject);
    }

    public PlantType GetPlantType()
    {
        return type;
    }

    protected class PlantBuff : FuncBuff<float>
    {
        private BuffVisualizersManager manager;
        private PlantBuffType plantBuffType;

        public PlantBuff(float effect, Action wipe, PlantBuffType _plantBuffType, BuffVisualizersManager _manager, string name) :
            base((float value) => value * effect, 4f, BuffType.BUFF,
            () => {
                wipe();
                if(_manager.isActiveAndEnabled)
                    _manager.DeactiveVisualizer(_plantBuffType);
            }
            , name)
        {
            manager = _manager;
            plantBuffType = _plantBuffType;
            if(manager.isActiveAndEnabled)
                manager.ActivateVisualizer(plantBuffType);
        }

        public override bool IsActive()
        {
            if(manager.isActiveAndEnabled)
                manager.SetVisualizer(plantBuffType, 1 - timer.GetPercentProgress());
            return base.IsActive();
        }
    }
}
