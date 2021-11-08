using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    Collider damageCollider;
    private bool stun = false;

    public int currentWeaponDamage = 25;

    private void Awake()
    {
        damageCollider = GetComponent<Collider>();
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }
    public void DisableDamageCollider()
    {
        stun = false;
        damageCollider.enabled = false;
    }
    public void EnableStun()
    {
        stun = true;
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            //TODO ONLY FOR FIREBALL
            if (this.gameObject.tag == "bullet")
                Destroy(this.gameObject);

            PlayerStats playeStats = collision.GetComponent<PlayerStats>();

            if (playeStats != null)
            {
                playeStats.TakeDamage(currentWeaponDamage);
            }
        }

        if (collision.tag == "enemy" || collision.tag == "boss")
        {
            EnemyStats enemyStats = collision.GetComponent<EnemyStats>();
            Stunnable stunnable = collision.GetComponent<Stunnable>();


            if (stun && stunnable != null)
            {
                stunnable.StunTarget();
            }

            if (enemyStats != null)
            {
                enemyStats.TakeDamage(currentWeaponDamage);
            }
        }

    }
}
