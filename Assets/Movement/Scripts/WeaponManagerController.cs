using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponManagerController : MonoBehaviour
{
    public GameObject leftHandWeaponPlaceholder;
    public GameObject rightHandWeaponPlaceholder;

    DamageCollider damageCollider;

    private bool withStun = false;

    public void EnableStun()
    {
        withStun = true;
    }

    public void LoadLeftHandCollider()
    {
        damageCollider = leftHandWeaponPlaceholder.GetComponentInChildren<DamageCollider>();

        if (damageCollider != null)
        {
            damageCollider.EnableDamageCollider();
            if (withStun)
                damageCollider.EnableStun();
        }
    }

    public void LoadRightHandCollider()
    {
        damageCollider = rightHandWeaponPlaceholder.GetComponentInChildren<DamageCollider>();

        if (damageCollider != null)
        {
            damageCollider.EnableDamageCollider();
            if (withStun)
                damageCollider.EnableStun();

        }
    }


    public void DisableLeftHandCollider()
    {
        damageCollider = leftHandWeaponPlaceholder.GetComponentInChildren<DamageCollider>();

        if (damageCollider != null)
            damageCollider.DisableDamageCollider();

        withStun = false;
    }

    public void DisableRightHandCollider()
    {
        damageCollider = rightHandWeaponPlaceholder.GetComponentInChildren<DamageCollider>();

        if (damageCollider != null)
            damageCollider.DisableDamageCollider();

        withStun = false;
    }



}
