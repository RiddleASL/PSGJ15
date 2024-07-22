using UnityEngine;

[CreateAssetMenu(fileName = "SpreadingShadow", menuName = "Abilities/Nyx/SpreadingShadow")]
public class SpreadingShadow : Ability
{
    [Header("Ability Stats")]
    public int arrowCount = 9;
    public float spreadAngle = 15;
    public float dmg = 1f;
    public float speed = 10f;
    public float knockback = 1f;
    public Vector2 hitboxSize = new Vector2(1, 1);

    public override void Activate(GameObject parent)
    {
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
        pm = parent.GetComponent<PlayerMotor>();
        Vector2 baseDir = parent.GetComponent<PlayerMotor>().getAttackDir();

        for (int i = -((arrowCount-1)/2); i <= ((arrowCount-1)/2); i++)
        {
            Vector3 dir = (Quaternion.Euler(0, 0, (spreadAngle/arrowCount) * i) * baseDir).normalized;
            GameObject hitbox = Instantiate(gs.hitbox, parent.transform.position + (dir * 1.5f), Quaternion.identity);

            hitbox.transform.localScale = hitboxSize;
            HitBox hb = hitbox.GetComponent<HitBox>();
            hb.dmg = dmg * pm.dmgMod;
            hb.knockback = knockback * pm.knockbackMod;
            hb.speed = speed;
            hb.dir = dir;
            hb.isRanged = true;
        }
    }


}
