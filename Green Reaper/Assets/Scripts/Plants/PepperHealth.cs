using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

public class PepperHealth : PlantHealth
{
    [SerializeField]
    private float speedBuffAmount = 100f;

    protected override void OnDeath()
    {
        PlayerController player = HarvestState.instance.currentPlayer;

        player.TurnOnPepperEffect();

        FuncBuff<float> pepperSpeedBuff = new PepperSpeedBuff(speedBuffAmount, player, HarvestState.instance.buffProgresses);
        FuncBuff<float> pepperMaxVelChangeBuff = new PepperSpeedBuff(speedBuffAmount, player, HarvestState.instance.buffProgresses);

        player.MoveableBlackList(BuffType.DEBUFF);

        player.BuffMaxSpeed(pepperSpeedBuff);
        player.BuffMaxVelocityChange(pepperMaxVelChangeBuff);
        base.OnDeath();
    }

    private class PepperSpeedBuff : PlantBuff
    {
        public PepperSpeedBuff(float effectToAdd, PlayerController player, BuffVisualizersManager _manager) : base((value) => value + effectToAdd, () => { player.MoveableUnblackList(BuffType.DEBUFF); player.TurnOffPepperEffect(); }, PlantBuffType.GHOSTPEPPER, _manager, "Ghost Pepper Speed Buff")
        { }
    }
}
