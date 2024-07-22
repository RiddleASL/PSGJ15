using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PiercingShadow", menuName = "Abilities/Nyx/PiercingShadow")]
public class PiercingShadow : Ability
{

    [Header("Ability Stats")]
    public float dmg = 1f;
    public float speed = 10f;
    public float knockback = 1f;
    public Vector2 hitboxSize = new Vector2(1, 1);

    public override void Activate(GameObject parent)
    {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
        pm = parent.GetComponent<PlayerMotor>();
        Vector2 direction = parent.GetComponent<PlayerMotor>().getAttackDir();

        //Create Hitbox
        GameObject hitbox = Instantiate(gs.hitbox, parent.transform.position + ((Vector3)direction * 2), Quaternion.identity);
        hitbox.transform.localScale = hitboxSize;
        HitBox hb = hitbox.GetComponent<HitBox>();
        hb.dmg = dmg * pm.dmgMod;
        hb.knockback = knockback * pm.knockbackMod;
        hb.speed = speed;
        hb.dir = direction;
        hb.isRanged = true;
        hb.isPiercing = true;
    }
}
