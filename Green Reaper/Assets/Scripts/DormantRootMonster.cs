using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DormantRootMonster : PlantHealth
{
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    protected override void OnDeath()
    {
        deathEvent?.Invoke(PlantType.DORMANTROOTMONSTER);
        QuestManager.instance.PlantDied(GetPlantType());
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();
        if (player != null)
        {
            OnDeath();
        }
    }
}
