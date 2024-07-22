using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BaseEnemy : MonoBehaviour
{
    [Header("Global Components")]
    public AIPath path;
    GlobalScript gs;
    [SerializeField] bool isRanged;
    Transform player;
    Rigidbody2D rb;
    
    [Header("AI Controls")]
    [SerializeField] float detectionRange = 10f;
    [SerializeField] float attackRange = 1f;
    [SerializeField] float defensiveRange = 1f;
    float defenseRange;
    Transform target;
    ContactFilter2D decoyFilter = new ContactFilter2D();

    [Header("Enemy Stats")]
    public float health = 10f;
    public float speed = 5f;

    [Header("Enemy Attack Stats")]
    [SerializeField] Vector2 hitboxSize = new Vector2(1, 1);
    [SerializeField] Transform attackPos;
    public float dmg = 1f;
    public float knockback = 1f;
    public float attackSpeed = 20f;

    [Header("Enemy Buffers")]
    public float attackBufferTime = .2f;
    public float speedBufferTime = .2f;
    float attackBuffer;
    float speedBuffer;

    // Start is called before the first frame update
    void Start()
    {
        //Component Initialization
        path = GetComponent<AIPath>();
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        rb = GetComponent<Rigidbody2D>();

        //Stat Initialization
        defenseRange = attackRange - defensiveRange;
        path.maxSpeed = speed;

        //Filter Initialization
        decoyFilter.SetLayerMask(LayerMask.GetMask("Decoy"));

        //Buffer Initialization
        attackBuffer = attackBufferTime;
    }

    // Update is called once per frame
    void Update()
    {
        if(health > 0){
            //Run Handle Functions
            handleAI();
            handleBuffers();
        } else {
            Destroy(gameObject);
        }

        //Infinity Frames
        if(rb.velocity.magnitude > 2){
            path.canMove = false;
            rb.freezeRotation = true;
        } else {
            path.canMove = true;
            rb.freezeRotation = false;
        }
    }

    public void TakeDamage(float dmg, float knockback, Vector3 dir){
        if(rb.velocity.magnitude > 2) return;
        health -= dmg;
        Debug.Log("Dir: " + dir + "| Knockback: " + knockback);
        rb.AddForce(dir * knockback, ForceMode2D.Impulse);
    }

    void handleAI(){
        //Decoy checker;
        // if(Physics2D.CircleCast(transform.position, 1, Vector2.zero, Contact LayerMask.LayerToName("Decoy"), RaycastHit2D[] results)){
        //     target = GameObject.FindGameObjectWithTag("Decoy").transform;
        // } else {
        //     target = player;
        // }
        RaycastHit2D[] results = new RaycastHit2D[1];
        if(Physics2D.CircleCast(transform.position, detectionRange, Vector2.zero, decoyFilter, results) > 0){
            Debug.Log(results[0].transform.name);
            target = results[0].transform;
        } else {
            target = player;
        }

        float dist = Vector2.Distance(transform.position, target.position);
        Vector2 dir = (target.position - transform.position).normalized;
        Debug.DrawRay(transform.position, dir * 3, Color.red);

        if(dist <= detectionRange && dist >= defenseRange + 1){
            path.destination = target.position;
            if(isRanged){
                path.endReachedDistance = defenseRange + 3;
                path.slowdownDistance = defenseRange + 3;
            }
        } else if(dist <= defenseRange && isRanged){
            path.destination = transform.position - (Vector3)dir * 3;
            if(isRanged){
                path.endReachedDistance = 2.5f;
                path.slowdownDistance = 5;
            }
        }

        if(speedBuffer > 0){
            if(isRanged) path.maxSpeed = 0;
            else path.maxSpeed = speed / 3;
        } else {
            path.maxSpeed = speed;
        }

        if(Vector2.Distance(transform.position, target.position) <= attackRange && Physics2D.Linecast(transform.position, target.position, 1 << LayerMask.NameToLayer("Obstacle")) == false){
            //rotate towards target.
            Vector3 targetDir = (target.position - transform.position).normalized;
            float angle = Mathf.Atan2(targetDir.y, targetDir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, 0, angle-90), Time.deltaTime * 3);
            attackBuffer -= Time.deltaTime;
            
            if(attackBuffer <= 0 && rb.velocity.magnitude <= 2) Attack();
        } else {
            if(attackBuffer <= attackBufferTime) attackBuffer = attackBufferTime;
        }
    }

    void Attack(){
        attackBuffer = attackBufferTime;
        speedBuffer = speedBufferTime;
        Vector3 hitboxPos = attackPos.position;
        Vector2 dir = (target.position - transform.position).normalized;
        if(isRanged){
            dir = (target.position - transform.position).normalized;
            hitboxPos = transform.position + (Vector3)dir;
        } 
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

        GameObject hitbox = Instantiate(gs.hitbox, hitboxPos, Quaternion.Euler(0, 0, angle));
        HitBox hb = hitbox.GetComponent<HitBox>();
        hb.transform.localScale = new Vector3(hitboxSize.x, hitboxSize.y, 1);
        hb.dmg = dmg;
        hb.knockback = knockback;
        hb.dir = dir;
        hb.isFriendly = false;
        hb.isRanged = isRanged;
        if(isRanged) hb.speed = attackSpeed;
    }

    void handleBuffers(){
        if(speedBuffer > 0) speedBuffer -= Time.deltaTime;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        if(defensiveRange > 0){
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, attackRange - defensiveRange);
        }
    }
}
