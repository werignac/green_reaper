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

    private BuffedValueHolder<int> damage;
    private BuffedValueHolder<float> attackSpeed;

    public UnityEvent<int> damageEvent;

    [SerializeField]
    private bool areaOfEffect;

    private bool isDamaging;

    private HashSet<GameObject> hitPlants;

    private Animator anim;

    private const string attackAnimationName = "Attack";


    private void Start()
    {
        hitPlants = new HashSet<GameObject>();
        anim = GetComponent<Animator>();

        damage = new BuffedValueHolder<int>(baseDamage);
        attackSpeed = new BuffedValueHolder<float>(1f);
    }

    public void SetDamaging(bool newState)
    {
        isDamaging = newState;
        hitPlants = new HashSet<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDamaging && (areaOfEffect || hitPlants.Count == 0))
        {
            if (hitPlants.Contains(collision.gameObject))
            {
                int damageDealt = baseDamage;

                //Deal damage to plant based off of baseDamage (plant will handle special effects)

                hitPlants.Add(collision.gameObject);
                damageEvent.Invoke(damageDealt);
            }
        }
    }

    public void Attack()
    {
        if (!isActiveAndEnabled)
        {
            gameObject.SetActive(true);
            anim.SetTrigger(attackAnimationName);
        }
    }

    public void EndAttack()
    {
        gameObject.SetActive(false);
    }

    private void FixedUpdate()
    {
        damage.CheckActiveBuffs();
        attackSpeed.CheckActiveBuffs();
    }
}
