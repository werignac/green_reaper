using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinExplosion : DestroyAfterTime
{
    public int damage;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlantHealth plant = collision.gameObject.GetComponent<PlantHealth>();
        if (plant != null)
            plant.ChangeHealth(-damage);
    }

}
