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
        Destroy(gameObject);
    }
}
