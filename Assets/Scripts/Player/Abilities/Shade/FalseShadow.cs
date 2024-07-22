using UnityEngine;

[CreateAssetMenu(fileName = "FalseShadow", menuName = "Abilities/Shade/FalseShadow")]
public class FalseShadow : Ability
{
    public override void Activate(GameObject parent)
    {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();

        Vector2 spawnDir = parent.GetComponent<PlayerMotor>().getAttackDir().normalized;
        GameObject ShadowDecoy = Instantiate(gs.decoy, (Vector2)parent.transform.position + (spawnDir * 2), Quaternion.identity);
        ShadowDecoy.GetComponent<Decoy>().deathTimer = duration;
    }
}
