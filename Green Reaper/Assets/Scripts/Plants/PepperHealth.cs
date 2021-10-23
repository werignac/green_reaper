using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Buffs;

public class PepperHealth : PlantHealth
{
    protected override void OnDeath()
    {
        Debug.Log("Death");

        PlayerController player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();

        player.TurnOnPepperEffect();

        FuncBuff<float> pepperSpeedBuff = new FuncBuff<float>((float speed) => speed * 1.5f, 5f, BuffType.BUFF, () => GameObject.FindWithTag("Player").GetComponent<PlayerController>().TurnOffPepperEffect(), "Pepper Speed Buff");
        FuncBuff<float> pepperMaxVelChangeBuff = new FuncBuff<float>((float velChange) => velChange * 1.5f, 5f, BuffType.BUFF, "Pepper Vel Change Buff");

        player.BuffMaxSpeed(pepperSpeedBuff);
        player.BuffMaxVelocityChange(pepperMaxVelChangeBuff);
        base.OnDeath();
    }
}
