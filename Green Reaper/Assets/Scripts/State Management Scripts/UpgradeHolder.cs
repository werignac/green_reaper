using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;
using System;

public class UpgradeHolder
{
    public enum UpgradeType { SPEED = 0, DAMAGE = 1, ATTACKSPEED = 2}

    private Dictionary<UpgradeType, float> multipliers;

    private WeaponController weapon;

    public UpgradeHolder()
    {
        multipliers = new Dictionary<UpgradeType, float>();

        foreach(int type in Enum.GetValues(typeof(UpgradeType)))
            multipliers.Add((UpgradeType)type, 1f);
    }

    public void SetMultiplier(UpgradeType type, float multiplier)
    {
        multipliers[type] = multiplier;
    }

    public Buff<float> GetMultiplierBuff(UpgradeType type)
    {
        return new UpgradeBuff<float>((float baseVal) => baseVal * multipliers[type], (left, right) => { }, BuffType.BUFF, () => { }, "Upgrade: " + type.ToString());
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
