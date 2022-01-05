using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveRight : MonoBehaviour
{
    [SerializeField]
    private float distance = 1f;
    [SerializeField]
    private float period = 1f;
    private float timePassed;
    private Vector2 initialPosition;
    // Start is called before the first frame update
    void Start()
    {
        initialPosition = transform.position;
    }

    private void FixedUpdate()
    {
        timePassed += Time.fixedDeltaTime;

        if (timePassed > period)
            timePassed -= period;

        transform.position = new Vector2(initialPosition.x + distance * Mathf.Sin(timePassed / period * Mathf.PI * 2), initialPosition.y);
    }
}
