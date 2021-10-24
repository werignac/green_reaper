using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;
using System;

public class UpgradeHolder
{
    public enum UpgradeType { SPEED = 0, DAMAGE = 1, ATTACKSPEED = 2, SCYTHESIZE = 3, PEPPERFREQUENCY = 4, PUMPKINFREQUENCY = 5, ZUCCINNIFREQUENCY = 6}

    private Dictionary<UpgradeType, float> multipliers;

    private WeaponController weapon;

    public UpgradeHolder()
    {
        multipliers = new Dictionary<UpgradeType, float>();

        foreach(int type in Enum.GetValues(typeof(UpgradeType)))
            multipliers.Add((UpgradeType)type, (type >= 4)? 0.1f : 1f);
    }

    public void SetMultiplier(UpgradeType type, float multiplier)
    {
        multipliers[type] = multiplier;
    }

    public Buff<float> GetMultiplierBuff(UpgradeType type)
    {
        return new UpgradeBuff<float>((float baseVal) => baseVal * multipliers[type], (left, right) => { }, BuffType.BUFF, () => { }, "Upgrade: " + type.ToString());
    }

    public float GetMultiplier(UpgradeType type)
    {
        return multipliers[type];
    }

    public void SetWeapon(WeaponController newWeapon)
    {
        weapon = newWeapon;
    }

    public WeaponController GetWeapon()
    {
        return weapon;
    }
}
