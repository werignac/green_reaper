using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PumpkinExplosion : MonoBehaviour
{
    public int damage;

    private float time = 1.0f;

    private void Update()
    {

        time -= Time.deltaTime;

        if (time <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlantHealth plant = collision.gameObject.GetComponent<PlantHealth>();
        if (plant != null)
            plant.ChangeHealth(-damage);
    }

}
