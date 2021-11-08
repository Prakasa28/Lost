using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Controller : MonoBehaviour
{

    Animator animator;
    FireballsController fireballController;
    CapsuleCollider capsuleCollider;

    public Transform leftHandPlaceholder;
    public Transform rightHandPlaceholder;
    public ParticleSystem smallFire;
    public ParticleSystem teleportParticle;
    public List<GameObject> teleportLocations;

    public float walkingSpeed = 8f;
    public float runningSpeed = 14f;
    public float walkingRadius = 35f;
    public float aggroRadius = 70f;
    public float attackRadius = 60f;
    public float castTime = 2.2f;
    public float chargeTime = 4f;
    public int attacksBeforeCharge = 5;
    [HideInInspector]
    public bool stunned = false;
    [HideInInspector]
    public bool dead = false;

    private Vector3 startPosition;
    private bool playerInRadius = false;

    private GameObject player;
    private float distanceFromPlayer = Mathf.Infinity;

    private bool canMoveRandomly = true;
    private bool canChase = true;
    private bool canAttack = true;
    private bool canCharge = true;
    private bool canTeleport = true;
    private int attackCount = 0;




    private enum Stages
    {
        Chilling, Chasing, Attacking
    }

    private Stages currentStage = Stages.Chilling;

    int isWalking;
    int isRunning;
    int isCasting;
    int isAttacking;
    int isCharging;

    void Awake()
    {
        animator = GetComponent<Animator>();
        capsuleCollider = GetComponent<CapsuleCollider>();
        fireballController = GetComponent<FireballsController>();
        player = GameObject.FindGameObjectWithTag("Player");

        isWalking = Animator.StringToHash("IsWalking");
        isRunning = Animator.StringToHash("IsRunning");
        isCasting = Animator.StringToHash("IsCasting");
        isAttacking = Animator.StringToHash("IsAttacking");
        isCharging = Animator.StringToHash("IsCharging");

    }

    void Start()
    {
        startPosition = transform.position;

        //spawn particle
        Instantiate(smallFire, rightHandPlaceholder);
        Instantiate(smallFire, leftHandPlaceholder);
    }

    void Update()
    {
        LookForTarget();

        switch (currentStage)
        {
            case Stages.Chilling:
                attackCount = 0;
                if (canMoveRandomly)
                    StartCoroutine(MoveEnemy());
                break;
            case Stages.Chasing:
                if (canChase)
                    StartCoroutine(Chasing());
                break;
            case Stages.Attacking:
                Attack();
                break;
        }
    }


    private void Attack()
    {

        //count attacks made
        //if 5 do the recharge
        if (attackCount > attacksBeforeCharge && canCharge)
            StartCoroutine(Charge());


        if (attackCount <= attacksBeforeCharge && canAttack)
        {
            StartCoroutine(CastSpell());
        }
    }

    IEnumerator Charge()
    {
        canCharge = false;
        float startTime = Time.time;

        animator.SetBool(isAttacking, false);
        animator.SetBool(isCharging, true);
        while (Time.time < startTime + chargeTime)
        {
            yield return null;
        }
        animator.SetBool(isCharging, false);
        canCharge = true;
        canAttack = true;
        attackCount = 0;

    }

    IEnumerator CastSpell()
    {
        if (distanceFromPlayer > attackRadius)
        {
            currentStage = Stages.Chasing;
            yield break;
        }

        if (dead)
            yield break;

        if (distanceFromPlayer < 15 && !stunned)
        {
            if (canTeleport)
            {
                StartCoroutine(Teleport());
            }
            yield break;
        }
        canAttack = false;
        float startTime = Time.time;
        animator.SetBool(isCasting, true);


        while (Time.time < startTime + castTime)
        {
            if (dead)
            {
                break;
            }
            FaceEnemy(player.transform.position);
            yield return null;
        }

        if (dead)
            yield break;

        animator.SetBool(isCasting, false);
        animator.SetBool(isAttacking, true);
        yield return new WaitForSeconds(.7f);
        if (!stunned)
        {
            for (int i = -16; i < 16; i += 8)
            {
                fireballController.instantiateFireball(i);
            }

            fireballController.fireAllWaitingFireballs(player.transform);

        }

        animator.SetBool(isAttacking, false);
        canAttack = true;
        attackCount++;
    }

    IEnumerator Teleport()
    {
        canTeleport = false;

        GameObject farthestTeleportLocation = null;
        float farthestTeleportPosition = 0;

        //find the farthest teleport position
        foreach (GameObject teleportLocation in teleportLocations)
        {
            float distance = Vector3.Distance(transform.position, teleportLocation.transform.position);
            if (distance > farthestTeleportPosition)
            {
                farthestTeleportLocation = teleportLocation;
                farthestTeleportPosition = distance;
            }
        }

        ParticleSystem instantiatedTeleport = Instantiate(teleportParticle);
        Vector3 positionToSpawn = transform.position;
        positionToSpawn.y = transform.position.y + .5f;
        instantiatedTeleport.transform.position = positionToSpawn;

        yield return new WaitForSeconds(1);
        capsuleCollider.enabled = false;

        float axisY = transform.position.y;

        while (axisY > -10)
        {
            transform.position -= new Vector3(0, 10 * Time.deltaTime, 0);
            axisY = transform.position.y;

            yield return null;
        }


        Destroy(instantiatedTeleport.gameObject);
        this.transform.position = farthestTeleportLocation.transform.position;
        capsuleCollider.enabled = true;
        LookForTarget();
        yield return new WaitForSeconds(.2f);
        canTeleport = true;
    }


    IEnumerator Chasing()
    {
        if (distanceFromPlayer > aggroRadius)
        {
            currentStage = Stages.Chilling;
            yield break;
        }

        canChase = false;

        animator.SetBool(isRunning, true);
        while (true)
        {
            FaceEnemy(player.transform.position);
            var offset = player.transform.position - transform.position;
            offset.y = 0;

            if (offset.magnitude < attackRadius)
            {

                animator.SetBool(isRunning, false);
                currentStage = Stages.Attacking;
                canChase = true;
                break;
            }

            offset = offset.normalized * runningSpeed;
            transform.position = Vector3.MoveTowards(transform.position, player.transform.position, runningSpeed * Time.deltaTime);
            yield return null;
        }

    }
    IEnumerator MoveEnemy()
    {
        canMoveRandomly = false;
        Vector3 destination = startPosition + Random.insideUnitSphere * walkingRadius;

        destination.y = 0;

        FaceEnemy(destination);
        animator.SetBool(isWalking, true);
        while (true)
        {
            if (playerInRadius)
            {

                animator.SetBool(isWalking, false);
                currentStage = Stages.Chasing;
                break;
            }

            var offset = destination - transform.position;
            offset.y = 0;

            if (offset.magnitude < 10f)
                break;


            offset = offset.normalized * walkingSpeed;
            transform.position = Vector3.MoveTowards(transform.position, destination, walkingSpeed * Time.deltaTime);

            yield return null;
        }

        animator.SetBool(isWalking, false);
        yield return new WaitForSeconds(2);
        canMoveRandomly = true;
    }
    void LookForTarget()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        distanceFromPlayer = distance;

        // check if player has entered ui range
        if (distance < aggroRadius)
        {
            playerInRadius = true;
        }
        else
        {
            // currentStage = Stages.Chilling;
            playerInRadius = false;
        }

    }



    void FaceEnemy(Vector3 position)
    {
        //face the target
        Vector3 direction = (position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1);
    }
    // draw the range circle
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, walkingRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRadius);

        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, attackRadius);

    }

}
