using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShadowsProtection", menuName = "Abilities/Umbra/ShadowsProtection")]
public class ShadowsProtection : Ability
{
    [Header("Ability Stats")]
    public float barrierHealth = 3f;

    public override void Activate(GameObject parent)
    {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
        pm = parent.GetComponent<PlayerMotor>();

        pm.setSecDurTime(duration);

        GameObject barrier = Instantiate(gs.barrier, parent.transform.position, Quaternion.identity, parent.transform);
        Barrier b = barrier.GetComponent<Barrier>();
        b.barrierHealth = barrierHealth;
        b.barrierDuration = duration;
    }
}
