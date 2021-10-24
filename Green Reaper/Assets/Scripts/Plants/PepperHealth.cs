using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

public class PepperHealth : PlantHealth
{
    protected override void OnDeath()
    {
        PlayerController player = HarvestState.instance.currentPlayer;

        player.TurnOnPepperEffect();

        FuncBuff<float> pepperSpeedBuff = new FuncBuff<float>((float speed) => speed * 1.5f, 4f, BuffType.BUFF, () => player.TurnOffPepperEffect(), "Pepper Speed Buff");
        FuncBuff<float> pepperMaxVelChangeBuff = new FuncBuff<float>((float velChange) => velChange * 1.5f, 4f, BuffType.BUFF, "Pepper Vel Change Buff");

        player.BuffMaxSpeed(pepperSpeedBuff);
        player.BuffMaxVelocityChange(pepperMaxVelChangeBuff);
        base.OnDeath();
    }
}
