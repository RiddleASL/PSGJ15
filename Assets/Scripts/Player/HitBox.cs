using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    //Important Variables
    [Header("Global Components")]
    public Vector3 dir;
    [HideInInspector] public bool isFriendly = true;
    [HideInInspector] public bool isRanged;
    [HideInInspector] public float rangedDeath = 10f;
    [SerializeField] float destroyTime = 1f;
    [HideInInspector] public bool isPiercing;
    [HideInInspector] public bool isSpreading;
    [HideInInspector] public float spreadSpeed;

    [Header("Attack Stats")]
    public float dmg;
    public float knockback;
    public float speed;

    // Update is called once per frame
    void Update()
    {
        if(isRanged){
            transform.position += dir.normalized * speed * Time.deltaTime;
            if(isSpreading){
                transform.localScale += new Vector3(0, 1, 0) * Time.deltaTime * spreadSpeed;
            }
            Destroy(gameObject, rangedDeath);
        }else{
            Destroy(gameObject, destroyTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Obstacle")){
            if(!isPiercing) Destroy(gameObject);
        }

        if(isFriendly){
            if(other.gameObject.layer == LayerMask.NameToLayer("Enemy")){
                Debug.Log("dir: " + dir);
                other.GetComponent<BaseEnemy>().TakeDamage(dmg, knockback, dir);
                if(!isPiercing) Destroy(gameObject);
            }
        }else{
            if(other.gameObject.layer == LayerMask.NameToLayer("Barrier")){
                other.GetComponent<Barrier>().TakeDamage(dmg);
                Destroy(gameObject);
            } else if(other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Decoy")){
                Debug.Log("Hit Friendly");
                if(other.TryGetComponent(out PlayerMotor pm)){pm.TakeDamage(dmg, knockback, dir);}
                if(other.TryGetComponent(out Decoy decoy)){decoy.TakeDamage(knockback, dir);}
                Destroy(gameObject);
            }
        }
    }
}
