using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : Moveable
{
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private WeaponController weapon;

    [SerializeField]
    private ParticleSystem pepperEffect;

    protected override void Start()
    {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [SerializeField]
    private bool lookRightByDefault;

    private void FixedUpdate()
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

    protected override void Update()
    {
        base.Update();

        if (Input.GetMouseButton(0) || Input.GetKey(KeyCode.Space))
            weapon.Attack(CalculateWeaponAngle());
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
}
