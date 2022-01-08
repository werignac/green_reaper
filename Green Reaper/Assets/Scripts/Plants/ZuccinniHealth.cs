using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

public class ZuccinniHealth : PlantHealth
{
    [SerializeField]
    private float attackSpeedBuffAmount = 2f;

    protected override void OnDeath()
    {
        PlayerController player = HarvestState.instance.currentPlayer;
        WeaponController weapon = HarvestState.instance.currentWeapon;

        player.TurnOnZuccinniEffect();

        FuncBuff<float> swingBuff = new ZucchiniBuff(attackSpeedBuffAmount, player, HarvestState.instance.buffProgresses);

        weapon.AddSpeedBuff(swingBuff);
        base.OnDeath();
    }

    private class ZucchiniBuff : PlantBuff
    {
        public ZucchiniBuff(float effect, PlayerController player, BuffVisualizersManager _manager) : base(effect, () => { player.TurnOffZuccinniEffect(); }, PlantBuffType.FRANKENZINI, _manager, "Zucchini Attack Speed Buff")
        { }
    }
}
