using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Moveable : MonoBehaviour
{
    private Rigidbody2D rigid;

    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float maxVelocityChange;

    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    public void MoveInDirection(Vector2 direction)
    {
        direction = Vector2.ClampMagnitude(direction, 1);
        Vector2 goalVelocity = direction * maxSpeed;
        Vector2 velocityDifference = Vector2.ClampMagnitude(goalVelocity - rigid.velocity, maxVelocityChange);

        rigid.AddForce(PerspectiveUtilities.NormalSpaceToPerspective(velocityDifference * rigid.mass), ForceMode2D.Impulse);
    }
}
