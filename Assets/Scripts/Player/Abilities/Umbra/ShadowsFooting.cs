using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShadowsFooting", menuName = "Abilities/Umbra/ShadowsFooting")]
public class ShadowsFooting : Ability
{
    [Header("Ability Stats")]
    public float knockbackResist = 2f;

    public override void Activate(GameObject parent)
    {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
        
        gs.pm.kbResistMod = knockbackResist;
    }
}
