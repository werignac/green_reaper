using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TestIncrement : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<int> timeIncrement;

    private float timepassed = 0;

    private int count = 0;

    private void Update()
    {
        timepassed += Time.deltaTime;

        if (timepassed > 1)
        {
            timeIncrement?.Invoke(++count);
            timepassed = 0;
        }
    }
}
