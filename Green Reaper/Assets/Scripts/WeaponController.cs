using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Buffs;

[RequireComponent(typeof(Animator))]
public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private int baseDamage;

    private BuffedValueHolder<float> damage;
    private BuffedValueHolder<float> attackSpeed;

    public UnityEvent<int> damageEvent;

    [SerializeField]
    private int areaOfEffect = 1;

    [SerializeField]
    private float rotOffset = 90f;

    private bool isDamaging;

    private HashSet<PlantHealth> hitPlants;

    private Animator anim;

    private const string attackAnimationName = "Attack";

    [SerializeField]
    public UnityEvent onSwing = new UnityEvent();


    private void Start()
    {
        hitPlants = new HashSet<PlantHealth>();
        anim = GetComponent<Animator>();
        if (damage == null)
            damage = new BuffedValueHolder<float>(baseDamage);
        if (attackSpeed == null)
            attackSpeed = new BuffedValueHolder<float>(1f);
        attackSpeed.valueChanged.AddListener((float newSpeed) => anim.speed = newSpeed);
    }

    public void StartDamaging()
    {
        isDamaging = true;
        hitPlants = new HashSet<PlantHealth>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDamaging && (hitPlants.Count < areaOfEffect || areaOfEffect < 0))
        {
            PlantHealth health = collision.gameObject.GetComponent<PlantHealth>();
            if (health != null && !hitPlants.Contains(health))
            {
                int damageDealt = (int) damage.GetValue();

                health.ChangeHealth(-damageDealt);

                hitPlants.Add(health);
                damageEvent?.Invoke(damageDealt);
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        OnTriggerEnter2D(collision);
    }

    public void Attack(float rotation)
    {
        if (!isActiveAndEnabled)
        {
            onSwing?.Invoke();

            gameObject.SetActive(true);
            if (anim == null)
                anim = GetComponent<Animator>();

            anim.SetTrigger(attackAnimationName);
            anim.speed = attackSpeed.GetValue();
            isDamaging = false;

            transform.rotation = Quaternion.Euler(0, 0, rotation + rotOffset);
        }
    }

    public void EndAttack()
    {
        gameObject.SetActive(false);
    }

    public void UpdateStats()
    {
        damage.CheckActiveBuffs();
        attackSpeed.CheckActiveBuffs();
    }

    public void AddDamageBuff(Buff<float> damageBuff)
    {
        if (damage == null)
            damage = new BuffedValueHolder<float>(baseDamage);
        damage.AddBuff(damageBuff);
    }

    public void AddSpeedBuff(Buff<float> speedBuff)
    {
        if (attackSpeed == null)
            attackSpeed = new BuffedValueHolder<float>(1f);
        attackSpeed.AddBuff(speedBuff);
    }

    public void SetAOE(int newAOE)
    {
        areaOfEffect = newAOE;
    }
}
