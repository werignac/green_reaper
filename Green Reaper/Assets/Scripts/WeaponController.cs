using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Buffs;
using System;

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

    private SortedSet<PlantHitEntry> hitThisFrame;

    private Animator anim;

    private const string attackAnimationName = "Attack";

    [SerializeField]
    public UnityEvent onSwing = new UnityEvent();


    private Coroutine checkHitProcess;

    private void Start()
    {
        hitPlants = new HashSet<PlantHealth>();
        anim = GetComponent<Animator>();
        if (damage == null)
            damage = new BuffedValueHolder<float>(baseDamage);
        if (attackSpeed == null)
            attackSpeed = new BuffedValueHolder<float>(1f);
        attackSpeed.valueChanged.AddListener((float newSpeed) => anim.speed = newSpeed);

        hitThisFrame = new SortedSet<PlantHitEntry>();
    }

    public void StartDamaging()
    {
        isDamaging = true;
        hitPlants = new HashSet<PlantHealth>();

        hitThisFrame.Clear();

        checkHitProcess = StartCoroutine(CheckPlantsHit());
    }

    private void OnTriggerEnter2D(Collider2D plantCollider)
    {
        if (isDamaging && CanHitPlants())
        {
            PlantHealth health = plantCollider.gameObject.GetComponent<PlantHealth>();
            if (health != null)
            {
                PlantHitEntry entry = new PlantHitEntry(health, this);

                if(!hitPlants.Contains(health) && !hitThisFrame.Contains(entry))
                {
                    hitThisFrame.Add(entry);
                }
            }
        }
    }

    private void OnTriggerStay2D(Collider2D plantCollider)
    {
        OnTriggerEnter2D(plantCollider);
    }

    public void Attack(float rotation)
    {
        if (!isActiveAndEnabled)
        {
            gameObject.SetActive(true);
            onSwing?.Invoke();
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
        StopCoroutine(checkHitProcess);
        ProcessPlantsHit();
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

    private IEnumerator CheckPlantsHit()
    {
        yield return new WaitForSecondsRealtime(0.05f);

        if (isActiveAndEnabled)
        {
            ProcessPlantsHit();

            yield return CheckPlantsHit();
        }
    }

    private void ProcessPlantsHit()
    {
        foreach (PlantHitEntry entry in hitThisFrame)
        {
            if (!CanHitPlants())
                break;

            PlantHealth plant = entry.GetPlant();

            DamagePlant(plant);
        }

        hitThisFrame.Clear();
    }

    private void DamagePlant(PlantHealth plant)
    {
        if (!hitPlants.Contains(plant))
        {
            int damageDealt = (int)damage.GetValue();

            plant.ChangeHealth(-damageDealt);

            hitPlants.Add(plant);
            damageEvent?.Invoke(damageDealt);
        }
    }

    private bool CanHitPlants()
    {
        return hitPlants.Count < areaOfEffect || areaOfEffect < 0;
    }

    private class PlantHitEntry : IComparable
    {
        private PlantHealth plant;
        private PlantType plantType;
        private float distanceFromHitBoxCenter;
        private int Priority
        {
            get
            {
                if (plantTypePriority.TryGetValue(plantType, out int priority))
                    return priority;
                return 0;
            }
        }

        private static readonly Dictionary<PlantType, int> plantTypePriority = new Dictionary<PlantType, int>() {{PlantType.PUMPKIN, 4}, { PlantType.ZUCCHINI, 3}, { PlantType.PEPPER, 2 }, { PlantType.ROOTMONSTER, 1 }};

        public PlantHitEntry(PlantHealth _plant, WeaponController thisWeapon)
        {
            plant = _plant;

            plantType = _plant.GetPlantType();

            Collider2D weaponCollider = thisWeapon.GetComponentInChildren<Collider2D>();

            distanceFromHitBoxCenter = Vector2.Distance(_plant.transform.position, weaponCollider.transform.TransformVector(weaponCollider.bounds.center));
        }


        public int CompareTo(object obj)
        {
            if (!(obj is PlantHitEntry))
                throw new ArgumentException("Can only compare PlantHitEntry to type PlantHitEntry. Got an object of type: " + obj.GetType());

            PlantHitEntry other = obj as PlantHitEntry;

            int typeDifference = other.Priority - Priority;

            if (typeDifference != 0)
                return typeDifference;

            float distanceComparison = distanceFromHitBoxCenter - other.distanceFromHitBoxCenter;

            if (distanceComparison < 0)
                return -1;
            else if (distanceComparison > 0)
                return 1;
            
            return 0;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is PlantHitEntry))
                throw new ArgumentException("Can only compare PlantHitEntry to type PlantHitEntry. Got an object of type: " + obj.GetType());

            PlantHitEntry other = obj as PlantHitEntry;

            return plant.Equals(other.plant);
        }

        public override int GetHashCode()
        {
            return plant.GetHashCode();
        }

        public PlantHealth GetPlant()
        {
            return plant;
        }
    }
}
