using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{


    public ParticleSystem explosion;
    public ParticleSystem thunder;
    public ParticleSystem circle;
    public List<GameObject> firstLevelEnemies;

    private AbilitiesController abilitiesController;
    private MovementController movementController;
    private Animator animator;
    private GameObject player;
    private GameObject stone;
    private Collider boxCollider;
    private bool performBreaking = false;
    private bool canPerform = true;
    private bool spawned = false;

    int isCastingHash;

    private void Awake()
    {
        player = GameObject.FindWithTag("Player");
        stone = GameObject.FindWithTag("Level2Door");
        animator = player.GetComponent<Animator>();
        boxCollider = GetComponent<Collider>();

        abilitiesController = player.GetComponent<AbilitiesController>();
        movementController = player.GetComponent<MovementController>();
        isCastingHash = Animator.StringToHash("IsCasting");

        boxCollider.enabled = false;
    }

    private void Update()
    {
        if (CheckIfLevelCompleted() && !spawned)
        {
            Instantiate(circle, this.transform.position, Quaternion.identity);
            boxCollider.enabled = true;
            spawned = true;
        }

        if (performBreaking)
        {
            StartCoroutine(BreakWall());
        }
    }

    private bool CheckIfLevelCompleted()
    {
        bool allDead = true;
        foreach (GameObject enemy in firstLevelEnemies)
        {
            EnemyStats stats = enemy.GetComponent<EnemyStats>();
            if (stats.getCurrentHealth() > 0)
            {
                allDead = false;
                break;
            }
        }

        return allDead;
    }

    IEnumerator BreakWall()
    {
        performBreaking = false;
        canPerform = false;

        abilitiesController.enabled = false;
        movementController.enabled = false;

        animator.SetBool(isCastingHash, true);
        yield return new WaitForSeconds(1);

        Vector3 spawnEffectPosition = stone.transform.position;
        FaceDoor(spawnEffectPosition);
        Vector3 spawnThunderPosition = player.transform.position;
        spawnThunderPosition.y = 1.5f;
        ParticleSystem spawnedThunder = Instantiate(thunder, spawnThunderPosition, Quaternion.identity);
        spawnedThunder.Play();

        CinemachineShake.Instance.ShakeCamera(5f, 1f);
        yield return new WaitForSeconds(1.7f);

        animator.SetBool(isCastingHash, false);

        Instantiate(explosion.transform, spawnEffectPosition, Quaternion.identity);
        explosion.Play();
        thunder.Stop();
        Destroy(spawnedThunder.gameObject);
        Destroy(stone);
        Destroy(this.gameObject);
        abilitiesController.enabled = true;
        movementController.enabled = true;



        yield return null;
    }



    private void OnTriggerEnter(Collider collider)
    {
        if (canPerform && collider.gameObject.tag == "Player")
        {
            performBreaking = true;
        }
    }


    void FaceDoor(Vector3 position)
    {
        Vector3 direction = (position - player.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        player.transform.rotation = Quaternion.Slerp(player.transform.rotation, lookRotation, 1);
    }
}
