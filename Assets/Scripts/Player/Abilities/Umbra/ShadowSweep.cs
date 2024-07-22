using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShadowSweep", menuName = "Abilities/Umbra/Shadow Sweep")]
public class ShadowSweep : Ability
{
    [Header("Ability Stats")]
    public float dmg = 1f;
    public float speed = 10f;
    public float knockback = 25f;
    public float deathTimer = 5f;
    public float spreadSpeed;
    public Vector2 hitboxSize = new Vector2(1, 1);

    public override void Activate(GameObject parent)
    {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
        pm = parent.GetComponent<PlayerMotor>();

        Vector2 direction = parent.GetComponent<PlayerMotor>().getAttackDir();
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //Create Hitbox
        GameObject hitbox = Instantiate(gs.hitbox, (Vector2)parent.transform.position + (direction * 1.5f), Quaternion.identity);
        hitbox.transform.localScale = hitboxSize;
        hitbox.transform.rotation = Quaternion.Euler(0, 0, angle);
        HitBox hb = hitbox.GetComponent<HitBox>();
        hb.dmg = dmg * pm.dmgMod;
        hb.rangedDeath = deathTimer;
        hb.knockback = knockback * pm.knockbackMod;
        hb.speed = speed;
        hb.dir = direction;
        hb.isRanged = true;
        hb.isPiercing = true;
        hb.isSpreading = true;
        hb.spreadSpeed = spreadSpeed;
    }
}
