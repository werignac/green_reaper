using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

[RequireComponent(typeof(Rigidbody2D))]
public class Moveable : MonoBehaviour
{
    private Rigidbody2D rigid;

    [SerializeField]
    private float baseMaxSpeed;
    private BuffedValueHolder<float> maxSpeed;

    [SerializeField]
    private float baseMaxVelocityChange;
    private BuffedValueHolder<float> maxVelocityChange;

    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        if (maxSpeed == null)
            maxSpeed = new BuffedValueHolder<float>(baseMaxSpeed);
        if(maxVelocityChange == null)
            maxVelocityChange = new BuffedValueHolder<float>(baseMaxVelocityChange);
    }

    public void MoveInDirection(Vector2 direction)
    {
        direction = Vector2.ClampMagnitude(direction, 1);
        Vector2 goalVelocity = direction * maxSpeed.GetValue();
        Vector2 velocityDifference = Vector2.ClampMagnitude(goalVelocity - rigid.velocity, maxVelocityChange.GetValue());

        rigid.AddForce(PerspectiveUtilities.NormalSpaceToPerspective(velocityDifference * rigid.mass), ForceMode2D.Impulse);
    }

    protected virtual void Update()
    {
        maxSpeed.CheckActiveBuffs();
        maxVelocityChange.CheckActiveBuffs();
    }

    public void BuffMaxSpeed(Buff<float> buff)
    {
        if (maxSpeed == null)
            maxSpeed = new BuffedValueHolder<float>(baseMaxSpeed);
        maxSpeed.AddBuff(buff);
    }

    public void BuffMaxVelocityChange(Buff<float> buff)
    {
        if (maxVelocityChange == null)
            maxVelocityChange = new BuffedValueHolder<float>(baseMaxVelocityChange);
        maxVelocityChange.AddBuff(buff);
    }
}
