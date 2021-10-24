using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;
using System;

public class UpgradeHolder
{
    public enum UpgradeType { SPEED = 0, DAMAGE = 1, ATTACKSPEED = 2, SCYTHESIZE = 3, PEPPERPROBABILITY = 4, PUMPKINPROBABILITY = 5, ZUCCINNIPROBABILITY = 6}

    private Dictionary<UpgradeType, int> upgradeLevels;

    private WeaponController[] weapons;

    private Dictionary<UpgradeType, float[]> upgradeValues = new Dictionary<UpgradeType, float[]>()
    {
        {UpgradeType.SPEED, new float[]{1f, 1.25f, 1.5f, 2f}},
        {UpgradeType.DAMAGE, new float[]{1f, 2f, 3f, 4f}},
        {UpgradeType.ATTACKSPEED, new float[]{1f, 1.4f, 1.8f, 2.2f}},
        {UpgradeType.SCYTHESIZE, new float[] {1f, 2f, 3f, int.MaxValue}},
        {UpgradeType.PEPPERPROBABILITY, new float[] {0f, 0.1f, 0.2f, 0.3f}},
        {UpgradeType.PUMPKINPROBABILITY, new float[] {0f, 0.1f, 0.2f, 0.3f}},
        {UpgradeType.ZUCCINNIPROBABILITY, new float[] {0f, 0.1f, 0.2f, 0.3f}}
    };

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

    public WeaponController GetWeapon()
    {
        return weapons[GetUpgradeLevel(UpgradeType.SCYTHESIZE)];
    }

    public void SetWeapons(WeaponController[] _weapons)
    {
        weapons = _weapons;
    }
}
