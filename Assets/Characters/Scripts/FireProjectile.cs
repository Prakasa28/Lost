using System;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public GameObject projectile;
    public Player player;
    private List<GameObject> projectiles;


    private void Awake()
    {
        projectiles = new List<GameObject>();
    }

    void Update()
    {
        GameObject newProjectile = Instantiate(projectile, transform.position, Quaternion.identity);
        projectiles.Add(newProjectile);
        fireProjectile();
    }

    void fireProjectile()
    {
        foreach (GameObject projectile in projectiles)
        {
            projectile.transform.position =
                Vector3.MoveTowards(projectile.transform.position, player.transform.position, 10f * Time.deltaTime);
        }
    }
}