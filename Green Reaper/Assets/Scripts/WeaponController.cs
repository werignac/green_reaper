using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Animator))]
public class WeaponController : MonoBehaviour
{
    [SerializeField]
    private int baseDamage;

    public UnityEvent<int> damageEvent;

    private bool isDamaging;

    private HashSet<GameObject> hitPlants;

    private Animator anim;

    private const string attackAnimationName = "Attack";


    private void Start()
    {
        hitPlants = new HashSet<GameObject>();
        anim = GetComponent<Animator>();
    }

    public void SetDamaging(bool newState)
    {
        isDamaging = newState;
        hitPlants = new HashSet<GameObject>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isDamaging)
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
        gameObject.SetActive(true);
        anim.SetTrigger(attackAnimationName);
    }
}
