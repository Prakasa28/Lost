using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    Collider damageCollider;

    public int currentWeaponDamage = 25;

    private void Awake()
    {
        damageCollider = GetComponentInChildren<Collider>();
        damageCollider.gameObject.SetActive(true);
        damageCollider.isTrigger = true;
        damageCollider.enabled = false;
    }

    public void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }

    public void DisableDamageCollider()
    {
        damageCollider.enabled = false;
    }

    //private void ontriggerenter(collider collsion)
    //{
    //    if (collsion.tag == "player")
    //    {
    //        playerstat playerstat = collsion.getcomponent<playerstat>();

    //        if (playerstat != null)
    //        {
    //            playerstat.takedamage(currentweapondamage);
    //        }
    //    }

    //    if (collsion.tag == "enemy")
    //    {
    //        enemystats enemystat = collsion.getcomponent<enemystats>();

    //        if(enemystat != null)
    //        {
    //            enemystat.takedamage(currentweapondamage);
    //        }
    //    }

    //}

   
}
