using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Flight", menuName = "Abilities/Raven/Flight")]
public class Flight : Ability
{
    public override void Activate(GameObject parent)
    {
        pm = parent.GetComponent<PlayerMotor>();
        pm.flight = true;
    }
}
