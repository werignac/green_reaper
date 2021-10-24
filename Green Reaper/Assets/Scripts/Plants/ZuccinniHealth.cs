using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

public class ZuccinniHealth : PlantHealth
{
    protected override void OnDeath()
    {
        PlayerController player = HarvestState.instance.currentPlayer;
        WeaponController weapon = HarvestState.instance.currentWeapon;

        player.TurnOnZuccinniEffect();

        FuncBuff<float> swingBuff = new FuncBuff<float>((float speed) => speed * 2f, 4f, BuffType.BUFF, () => player.TurnOffZuccinniEffect(), "Zuccinni Attack Speed Buff");

        weapon.AddSpeedBuff(swingBuff);        
        base.OnDeath();
    }
}
