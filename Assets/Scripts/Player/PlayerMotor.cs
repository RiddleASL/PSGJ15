using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    [Header("Global Components")]
    public bool isRanged;
    GlobalScript gs;
    [HideInInspector] public Rigidbody2D rb;
    AbilityHolder ah;

    [Header("Global Character Stats")]
    public float acceleration = 10f;
    [HideInInspector] public bool flight;
    float currSpeed;

    [Header("Basic Character Stats")]
    public float maxHealth = 10f;
    float currHealth;
    public float speed = 10f;
    [HideInInspector] public float speedMod = 1f;
    public float kbResist = 3f;
    [HideInInspector] public float kbResistMod = 1f;
    Vector3 move;
    Vector3 lastMove;

    //Dash Logic
    public float dashDist = 10f;
    [HideInInspector] public float dashMod = 1f;

    [Header("Basic Attack Stats")]
    //Attack Logic
    public Vector2 attackHitbox = new Vector2(1, 1);
    Vector2 attackDir;
    [HideInInspector] public Vector2 hitboxMod = new Vector2(1, 1);
    [SerializeField] GameObject hitbox;
    float attackDist = 1f;
    public float attackSpeed = 1f;
    [HideInInspector] public float attackSpeedMod = 1f;
    public bool dashAttack = false;

    //Attack Stats
    public float dmg = 1f;
    [HideInInspector] public float dmgMod = 1f;
    public float knockback = 1f;
    [HideInInspector] public float knockbackMod = 1f;

    [Header("Buffers")]
    public float attackBuffer = .2f;
    [HideInInspector] public float attackBufferMod = 1;
    float attackBufferTime;
    public float dashBuffer = .2f;
    [HideInInspector] public float dashBufferMod = 1;
    float dashBufferTime;

    //! Final Stats
    float finalSpeed;
    float finalDashDist;
    float finalDmg;
    float finalKnockback;
    float finalAttackSpeed;
    Vector2 finalHitboxSize;
    float finalAttackBuffer;
    float finalDashBuffer;


    //* Ability Stats
    public float primaryAbilityDuration;
    public float secondaryAbilityDuration;

    float dashAttackSpeed = 1f;
    float dashAttackDmg = 1f;
    float dashAttackKnockback = 1f;

    // Start is called before the first frame update
    void Start()
    {
        //Component Initialization
        rb = GetComponent<Rigidbody2D>();
        ah = GetComponent<AbilityHolder>();
        gs = GameObject.FindGameObjectWithTag("Global").GetComponent<GlobalScript>();

        //Variable Initialization
        currHealth = maxHealth;

        //Lock Cursor to Screen
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Handle Functions
        handleMovement();
        handleAttackDirection();
        handleBuffers();
        handleFinalStats();
        
        if(dashAttack && primaryAbilityDuration > 0) DashAttack(attackDir, finalSpeed * dashAttackSpeed);
    }

    public void Movement(InputAction.CallbackContext context)
    {
        if(context.performed) lastMove = context.ReadValue<Vector2>().normalized; //save the last move direction
        move = context.ReadValue<Vector2>().normalized;
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if(!context.performed) return;
        if(dashBufferTime > 0) return;
        dashBufferTime = finalDashBuffer; // Set dash buffer
        StartCoroutine(DashCoroutine());
    }

    public void Attack(InputAction.CallbackContext context)
    {
        if(attackBufferTime > 0) return;
        if(!context.performed) return;
        attackBufferTime = finalAttackBuffer; // Set attack buffer
        GameObject hit = Instantiate(gs.hitbox, (Vector2)transform.position + attackDir.normalized * (attackDist + (finalHitboxSize.x/2)), Quaternion.identity); // Create hitbox
        HitBox hb = hit.GetComponent<HitBox>();
        hb.dmg = finalDmg;  // Set hitbox damage
        hb.knockback = finalKnockback;  // Set hitbox knockback
        hit.transform.localScale = new Vector3(finalHitboxSize.x, finalHitboxSize.y, 1) * hitboxMod;  // Set hitbox size
        hb.dir = attackDir.normalized;

        //If ranged, attack & hitbox is handled differently
        if(isRanged){
            hb.isRanged = true;
            hb.speed = finalAttackSpeed;
        } else{
            hit.transform.parent = transform; // Set melee hitbox parent to player
        }

        //rotate hitbox to face the direction of the attack
        float angle = Mathf.Atan2(attackDir.y, attackDir.x) * Mathf.Rad2Deg;
        hit.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public void TakeDamage(float dmg, float knockback, Vector3 dir)
    {
        currHealth -= dmg;
        rb.AddForce(dir * (knockback), ForceMode2D.Impulse);
    }

    IEnumerator DashCoroutine()
    {
        LayerMask dashMask = new LayerMask();
        if(flight) dashMask = LayerMask.GetMask("Default", "Obstacle");
        else dashMask = LayerMask.GetMask("Default", "TransparentFX", "Obstacle");

        float startTime = Time.time;
        Vector3 startPos = transform.position;
        Vector3 endPos = transform.position + (Vector3)move * finalDashDist;

        RaycastHit2D hit = Physics2D.Raycast(transform.position, move, finalDashDist, dashMask); //Raycast to check if the dash will hit a wall
        if(hit.collider != null) endPos = hit.point; //If the raycast hits a wall, set the end position to the point of the hit

        while (Time.time < startTime + 0.1f)
        {
            rb.MovePosition(Vector3.Lerp(startPos, endPos, (Time.time - startTime) / 0.1f));
            yield return null;
        }
    }

    void DashAttack(Vector2 dir, float speed)
    {
        rb.MovePosition((Vector2)transform.position + dir * (speed * dashAttackSpeed) * Time.fixedDeltaTime);
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(LayerMask.NameToLayer("Enemy"));

        RaycastHit2D hit = Physics2D.CircleCast(transform.position, 1, Vector2.zero, 0, LayerMask.GetMask("Enemy"));
        if(hit.collider != null){
            Vector2 knockbackDir = (hit.collider.transform.position - transform.position).normalized;
            TakeDamage(0, dashAttackKnockback, -knockbackDir);
            hit.collider.GetComponent<BaseEnemy>().TakeDamage(dashAttackDmg, dashAttackKnockback, dir);
            ah.setPrimaryCooldown(ah.primaryAbility.cooldown / 2);

            primaryAbilityDuration = 0;
            dashAttack = false;
        } else if(primaryAbilityDuration <= 0){
            dashAttack = false;

            TakeDamage(0, 10, attackDir);
        };
    }

    void handleMovement()
    {
        //Basic Movement
        if (move != Vector3.zero){
            if(currSpeed + 0.1f > finalSpeed) currSpeed = finalSpeed;
            currSpeed = Mathf.Lerp(currSpeed, finalSpeed, acceleration * Time.fixedDeltaTime);
        } else {
            currSpeed = Mathf.Lerp(currSpeed, 0, acceleration * Time.fixedDeltaTime);
            if(currSpeed < 0.1f) currSpeed = 0;
        }

        if(attackBufferTime > 0) currSpeed = finalSpeed / 4;

        if(rb.velocity.magnitude <= 3) rb.MovePosition(transform.position + move * currSpeed * Time.fixedDeltaTime);
        if(flight) rb.excludeLayers = LayerMask.GetMask("TransparentFX"); else rb.excludeLayers = 0;
    }

    void handleAttackDirection()
    {
        attackDir = (Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()) - transform.position).normalized;
    }

    void handleBuffers()
    {
        if(attackBufferTime > 0) attackBufferTime -= Time.deltaTime; //* Attack Buffer
        if(dashBufferTime > 0) dashBufferTime -= Time.deltaTime; //* Dash Buffer

        if(primaryAbilityDuration > 0) primaryAbilityDuration -= Time.deltaTime; //* Primary Ability Duration
        if(secondaryAbilityDuration > 0) secondaryAbilityDuration -= Time.deltaTime; //* Secondary Ability Duration 
    }

    void handleFinalStats()
    {
        finalSpeed = speed * speedMod;
        finalDashDist = dashDist * dashMod;
        finalDmg = dmg * dmgMod;
        finalKnockback = knockback * knockbackMod;
        finalAttackSpeed = attackSpeed * attackSpeedMod;
        finalHitboxSize = attackHitbox * hitboxMod;
        finalAttackBuffer = attackBuffer * attackBufferMod;
        finalDashBuffer = dashBuffer * dashBufferMod;

        rb.drag = kbResist * kbResistMod;
    }

    private void OnDrawGizmos() {
        
    }

    //Public Get Functions
    public Vector2 getAttackDir(){return attackDir;}

    //Public Set Functions
    public void setDashAttackSpeed(float speed){dashAttackSpeed = speed;}
    public void setDashAttackDmg(float dmg){dashAttackDmg = dmg;}
    public void setDashAttackKnockback(float knockback){dashAttackKnockback = knockback;}
    public void setPrimDurTime(float time){primaryAbilityDuration = time;}
    public void setSecDurTime(float time){secondaryAbilityDuration = time;}
}
