using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootableNPC : MonoBehaviour
{

    public float damage = 50f;
    public bool death = false;
    public float health = 100f;
    public float range = 100f;

    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
            Die();
    }
    public void Die()
    {
        death = true;
        Destroy(this.gameObject);
    }

}
