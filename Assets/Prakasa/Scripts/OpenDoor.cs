using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private AbilitiesController abilitiesController;
    private MovementController movementController;
    private GameObject player;
    public ParticleSystem Explosion;
    public ParticleSystem Thunder;
    private GameObject Stone;
 

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        Stone = GameObject.FindWithTag("Level2Door");
        
        abilitiesController = player.GetComponent<AbilitiesController>();
        movementController = player.GetComponent<MovementController>();
    }



    private void OnTriggerEnter(Collider collider)
    {
       

        abilitiesController.enabled = false;
        movementController.enabled = false;
        Vector3 spawnEffectPosition = Stone.transform.position;
        FaceDoor(spawnEffectPosition);
        Instantiate(Explosion.transform, spawnEffectPosition, Quaternion.identity);

        Explosion.Play();
        Destroy(Stone);
        CinemachineShake.Instance.ShakeCamera(5f, 1f);
    }
     

    void FaceDoor(Vector3 position)
    {
        Vector3 direction = (position - player.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookRotation, 1);
    }
}
