using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Teleport : MonoBehaviour
{
    public GameObject enemy;
    public List<GameObject> teleportPoints;

    void Update()
    {
        teleportEnemy();
    }

    void teleportEnemy()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            var number = Random.Range(0, teleportPoints.Count - 1);
            enemy.transform.position = teleportPoints[number].transform.position;
        }
    }
}