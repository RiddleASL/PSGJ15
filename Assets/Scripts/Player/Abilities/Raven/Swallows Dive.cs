using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SwallowsDive", menuName = "Abilities/Raven/Swallows Dive")]
public class SwallowsDive : Ability
{
    [Header("Ability Stats")]
    public float dmg = 1f;
    public float knockback = 1f;
    public float speed = 1f;

    public override void Activate(GameObject parent)
    {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
        pm = parent.GetComponent<PlayerMotor>();

        pm.dashAttack = true;
        pm.setDashAttackDmg(dmg * pm.dmgMod);
        pm.setDashAttackKnockback(knockback * pm.knockbackMod);
        pm.setDashAttackSpeed(speed);
        pm.setPrimDurTime(duration);
    }
}
