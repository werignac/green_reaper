using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerController : Moveable
{
    private SpriteRenderer spriteRenderer;

    [SerializeField]
    private WeaponController weapon;

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

        if ( || )
        {

        }

        MoveInDirection(movementInput);
    }
}
