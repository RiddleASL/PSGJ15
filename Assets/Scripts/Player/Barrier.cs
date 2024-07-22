using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrier : MonoBehaviour
{
    public float barrierHealth = 3f;
    [HideInInspector] public float barrierDuration = 5f;

    void Update()
    {
        Destroy(gameObject, barrierDuration);
    }

    public void TakeDamage(float damage)
    {
        barrierHealth -= damage;
        if (barrierHealth <= 0)
        {
            Destroy(gameObject);
        }
    }
}
