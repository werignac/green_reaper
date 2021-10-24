using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;
using System;

public class UpgradeHolder
{
    public enum UpgradeType { SPEED = 0, DAMAGE = 1, ATTACKSPEED = 2, SCYTHESIZE = 3, PEPPERFREQUENCY = 4, PUMPKINFREQUENCY = 5, ZUCCINNIFREQUENCY = 6}

    private Dictionary<UpgradeType, int> upgradeLevels;


    private Dictionary<UpgradeType, float[]> upgradeValues = new Dictionary<UpgradeType, float[]>()
    {
        {UpgradeType.SPEED, new float[]{1f, 1.25f, 1.5f, 2f}},
        {UpgradeType.DAMAGE, new float[]{1f, 2f, 3f, 4f}},
        {UpgradeType.ATTACKSPEED, new float[]{1f, 1.4f, 1.8f, 2.2f}},
        {UpgradeType.SCYTHESIZE, new float[] {1f, 2f, 3f, int.MaxValue}},
        {UpgradeType.PEPPERFREQUENCY, new float[] {0f, 0.1f, 0.2f, 0.3f}},
        {UpgradeType.PUMPKINFREQUENCY, new float[] {0f, 0.1f, 0.2f, 0.3f}},
        {UpgradeType.ZUCCINNIFREQUENCY, new float[] {0f, 0.1f, 0.2f, 0.3f}}
    };

    private WeaponController weapon;

    public UpgradeHolder()
    {
        upgradeLevels = new Dictionary<UpgradeType, int>();

        foreach(int type in Enum.GetValues(typeof(UpgradeType)))
            upgradeLevels.Add((UpgradeType)type, 0);
    }

    public void IncrementUpgradeLevel(UpgradeType type)
    {
        upgradeLevels[type] = Mathf.Min(upgradeLevels[type] + 1, upgradeValues[type].Length);
    }

    public int GetUpgradeLevel(UpgradeType type)
    {
        return upgradeLevels[type];
    }

    public bool IsLastLevel(UpgradeType type)
    {
        return upgradeLevels[type] == upgradeValues[type].Length - 1;
    }

    public Buff<float> GetMultiplierBuff(UpgradeType type)
    {
        return new UpgradeBuff<float>((float baseVal) => baseVal * upgradeValues[type][upgradeLevels[type]], (left, right) => { }, BuffType.BUFF, () => { }, "Upgrade: " + type.ToString());
    }

    public float GetMultiplier(UpgradeType type)
    {
        return upgradeValues[type][upgradeLevels[type]];
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
