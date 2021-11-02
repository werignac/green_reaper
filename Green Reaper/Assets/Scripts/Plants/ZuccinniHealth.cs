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

        FuncBuff<float> swingBuff = new ZucchiniBuff(player, HarvestState.instance.buffProgresses);

        weapon.AddSpeedBuff(swingBuff);
        base.OnDeath();
    }

    private class ZucchiniBuff : PlantBuff
    {
        public ZucchiniBuff(PlayerController player, BuffVisualizersManager _manager) : base(2f, () => { player.TurnOffZuccinniEffect(); }, PlantBuffType.FRANKENZINI, _manager, "Zucchini Attack Speed Buff")
        { }
    }
}
