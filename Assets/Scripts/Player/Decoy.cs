using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoy : MonoBehaviour
{
    public float health = 3f;
    [HideInInspector] public float deathTimer = 5f;

    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, deathTimer);
    }

    public void TakeDamage(float knockback, Vector2 dir)
    {
        health -= 1;
        if(health <= 0)
        {
            Destroy(gameObject);
        }
        GetComponent<Rigidbody2D>().AddForce(dir * knockback, ForceMode2D.Impulse);
    }
}
