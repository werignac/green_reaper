using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterTime : MonoBehaviour
{
    [SerializeField]
    private float time = 1.0f;

    private void Update()
    {
        time -= Time.deltaTime;

        if (time <= 0)
            Destroy(gameObject);
    }
}
