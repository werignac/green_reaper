using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

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

    private PlayerMobileControls mobileControls;

    [SerializeField, Range(0, 1)]
    private float joystickThreshold = 0.15f;

    [SerializeField]
    private UnityEvent<float> sendSpeed;

    private void Awake()
    {
        mobileControls = new PlayerMobileControls();
    }

    private void OnEnable()
    {
        mobileControls.Enable();
    }

    private void OnDisable()
    {
        mobileControls.Disable();
    }

    protected override void Start()
    {
      
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    [SerializeField]
    private bool lookRightByDefault;

    private void FixedUpdate()
    {
        if (receivingInput)
        {
            Vector2 movementInput;

            #if UNITY_ANDROID
                movementInput = mobileControls.Mobile.Move.ReadValue<Vector2>();
            #elif UNITY_IOS
                movementInput = mobileControls.Mobile.Move.ReadValue<Vector2>();
            #else
                movementInput = mobileControls.WEBGL.Move.ReadValue<Vector2>(); ;
            #endif

            //Flip the player sprite in the direction it wants to be moving in.
            if ((lookRightByDefault && movementInput.x > 0 && spriteRenderer.flipX) ||
                (!lookRightByDefault && movementInput.x < 0 && spriteRenderer.flipX))
                spriteRenderer.flipX = false;
            else if ((!lookRightByDefault && movementInput.x > 0 && !spriteRenderer.flipX) ||
                (lookRightByDefault && movementInput.x < 0 && !spriteRenderer.flipX))
                spriteRenderer.flipX = true;

            MoveInDirection(movementInput);

            sendSpeed?.Invoke(movementInput.magnitude);
        }
        else
        {
            MoveInDirection(Vector2.zero);
            sendSpeed?.Invoke(0);
        }
    }

    protected override void Update()
    {
        base.Update();

        weapon?.UpdateStats();

        if (receivingInput && canAttack)
        {
            bool isTryingToAttack = false;
            Vector2 weaponAttackDirection;

                #if UNITY_ANDROID
                weaponAttackDirection = mobileControls.Mobile.WeaponDirection.ReadValue<Vector2>();
                isTryingToAttack = weaponAttackDirection.magnitude > joystickThreshold;
                #elif UNITY_IOS
                weaponAttackDirection = mobileControls.Mobile.WeaponDirection.ReadValue<Vector2>();
                isTryingToAttack = weaponAttackDirection.magnitude > joystickThreshold;
                #else
                isTryingToAttack = mobileControls.WEBGL.WeaponFire.ReadValue<float>() > 0;
                Vector2 mousePosition = mobileControls.WEBGL.WeaponDirection.ReadValue<Vector2>();
                Vector2 targetPos = Camera.main.ScreenToWorldPoint(mousePosition);
                weaponAttackDirection = (targetPos - (Vector2)transform.position);
                #endif



            if (isTryingToAttack)
                weapon?.Attack(CalculateWeaponAngle(weaponAttackDirection));
        }
    }

    private float CalculateWeaponAngle(Vector2 directionToAttack)
    {
        return Mathf.Atan2(directionToAttack.y, directionToAttack.x) * Mathf.Rad2Deg;
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
}
