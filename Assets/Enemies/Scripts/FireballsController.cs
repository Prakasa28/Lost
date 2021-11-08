using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballsController : MonoBehaviour
{

    public GameObject fireballPrefab;
    public Transform positionToSpawnFireball;
    public float fireballSpeed = 80f;


    List<GameObject> fireballs = new List<GameObject>();
    Dictionary<GameObject, float> waitingFireballs = new Dictionary<GameObject, float>();
    private GameObject player;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        MoveFireballs();
    }

    private void MoveFireballs()
    {
        List<GameObject> toRemove = new List<GameObject>();
        foreach (GameObject fireball in fireballs)
        {
            if (fireball == null)
            {
                toRemove.Add(fireball);
                continue;
            }
            float distance = Vector3.Distance(fireball.transform.position, player.transform.position);

            if (distance < 100f)
            {
                fireball.transform.position += fireball.transform.forward * Time.deltaTime * fireballSpeed;
                continue;
            }
            toRemove.Add(fireball);
        }

        foreach (GameObject fireballForDelete in toRemove)
        {
            fireballs.Remove(fireballForDelete);
            Destroy(fireballForDelete);
        }


    }

    public void instantiateFireball(float rotation)
    {
        Vector3 positionToSpawn = positionToSpawnFireball.position;
        positionToSpawn.y = 4;
        GameObject fireballObj = Instantiate(fireballPrefab, positionToSpawn, Quaternion.identity) as GameObject;
        fireballObj.GetComponent<SphereCollider>().enabled = true;

        waitingFireballs.Add(fireballObj, rotation);
    }

    public void fireAllWaitingFireballs(Transform position)
    {
        foreach (KeyValuePair<GameObject, float> fireballEntry in waitingFireballs)
        {
            Vector3 directionToFace = (player.transform.position - fireballEntry.Key.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToFace.x, 0, directionToFace.z));

            lookRotation *= Quaternion.Euler(1, fireballEntry.Value, 1);

            fireballEntry.Key.transform.rotation = Quaternion.Slerp(fireballEntry.Key.transform.rotation, lookRotation, 1);
            fireballs.Add(fireballEntry.Key);

        }
        waitingFireballs.Clear();
    }
}
