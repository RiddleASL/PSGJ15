using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShadowsGust", menuName = "Abilities/Raven/Shadows Gust")]
public class ShadowsGust : Ability
{
    [Header("Ability Stats")]
    public float range = 10f;
    public float knockback = 10f;

    public override void Activate(GameObject parent)
    {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();

        RaycastHit2D[] hit = Physics2D.CircleCastAll(parent.transform.position, range, Vector2.zero); 
        foreach(RaycastHit2D h in hit)
        {
            if(h.collider.gameObject.layer != LayerMask.NameToLayer("Enemy")) continue;
            Vector2 dir = (h.collider.transform.position - parent.transform.position).normalized;

            BaseEnemy enemy = h.collider.GetComponent<BaseEnemy>();
            enemy.TakeDamage(0, knockback, dir);
        }
    }
}
