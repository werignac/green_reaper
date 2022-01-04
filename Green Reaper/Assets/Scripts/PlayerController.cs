using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;
using System;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : Moveable
{
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private WeaponController weapon;

    [SerializeField]
    private ParticleSystem pepperEffect;
    [SerializeField]
    private ParticleSystem zuccinniEffect;

    private bool receivingInput = true;
    private bool canAttack = true;

    private bool sprinting = false;

    protected override void Start()
    {
        if (!GameManager.instance.GetProcedural())
            transform.localScale = transform.localScale * 0.056844f;

        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [SerializeField]
    private bool lookRightByDefault;

    private void FixedUpdate()
    {
        if (receivingInput)
        {
            Vector2 movementInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

            //Flip the player sprite in the direction it wants to be moving in.
            if ((lookRightByDefault && movementInput.x > 0 && spriteRenderer.flipX) ||
                (!lookRightByDefault && movementInput.x < 0 && spriteRenderer.flipX))
                spriteRenderer.flipX = false;
            else if ((!lookRightByDefault && movementInput.x > 0 && !spriteRenderer.flipX) ||
                (lookRightByDefault && movementInput.x < 0 && !spriteRenderer.flipX))
                spriteRenderer.flipX = true;

            MoveInDirection(movementInput);
        }
        else
        {
            MoveInDirection(Vector2.zero);
        }
    }

    protected override void Update()
    {
        base.Update();

        weapon?.UpdateStats();

        if (receivingInput)
        {
            if ((Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space)) && canAttack)
                weapon?.Attack(CalculateWeaponAngle());

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                Debug.Log("Spriting");
                sprinting = true;
                BuffMaxSpeed(new SprintBuff(() => sprinting));
                BuffMaxVelocityChange(new SprintBuff(() => sprinting));
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                Debug.Log("Not Spriting");
                sprinting = false;
            }
        }
    }

    private float CalculateWeaponAngle()
    {
        Vector2 targetPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 dir = (targetPos - (Vector2)transform.position);
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    public void SetWeapon(WeaponController newWeapon)
    {
        weapon = newWeapon;
    }

    public void TurnOnPepperEffect()
    {
        pepperEffect.Play();
    }

    public void TurnOffPepperEffect()
    {
        pepperEffect.Stop();
    }

    public void TurnOnZuccinniEffect()
    {
        zuccinniEffect.Play();
    }

    public void TurnOffZuccinniEffect()
    {
        zuccinniEffect.Stop();
    }

    public void SetReceivingInput(bool setReceive)
    {
        receivingInput = setReceive;
    }

    public void TurnOffPlayerAttack()
    {
        canAttack = false;
    }

    public void TurnOnPlayerAttack()
    {
        canAttack = true;
    }

    private class SprintBuff : FuncBuff<float>
    {
        public SprintBuff(Func<bool> useWhile) : base((float x) => x * 10.0f, useWhile, BuffType.BUFF, "Sprint")
        {

        }
    }
}
