using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2Controller : MonoBehaviour
{

    Animator animator;
    public Transform rightHandPosition;
    public GameObject fireballPrefab;

    public float walkingSpeed = 5f;
    public float runningSpeed = 10f;
    public float walkingRadius = 20f;
    public float aggroRadius = 50f;
    public float attackRadius = 30f;
    public float fireballSpeed = 20f;
    public float castTime = 1f;

    private Vector3 startPosition;
    private bool playerInRadius = false;

    private GameObject player;
    private float distanceFromPlayer = Mathf.Infinity;

    private bool canMoveRandomly = true;
    private bool canLookForATarget = true;
    private bool canChase = true;
    private bool canAttack = true;

    Dictionary<GameObject, Vector3> fireballs = new Dictionary<GameObject, Vector3>();

    private enum Stages
    {
        Chilling, Chasing, Attacking
    }

    private Stages currentStage = Stages.Chilling;

    int isWalking;
    int isRunning;
    int isCasting;
    int isAttacking;

    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");

        isWalking = Animator.StringToHash("IsWalking");
        isRunning = Animator.StringToHash("IsRunning");
        isCasting = Animator.StringToHash("IsCasting");
        isAttacking = Animator.StringToHash("IsAttacking");

    }


    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        if (canLookForATarget)
            StartCoroutine(LookForTarget());

        switch (currentStage)
        {
            case Stages.Chilling:
                Chilling();
                break;
            case Stages.Chasing:
                if (canChase)
                    StartCoroutine(Chasing());
                break;
            case Stages.Attacking:
                Attack();
                break;
        }

        foreach (KeyValuePair<GameObject, Vector3> fireball in fireballs)
        {
            fireball.Key.transform.position += fireball.Key.transform.forward * Time.deltaTime * fireballSpeed;
        }

    }

    private void Attack()
    {
        // if (distanceFromPlayer > attackRadius)
        //     currentStage = Stages.Chasing;

        if (canAttack)
            StartCoroutine(CastSpell());
    }

    IEnumerator CastSpell()
    {
        canAttack = false;
        float startTime = Time.time;
        animator.SetBool(isCasting, true);
        Vector3 positionToSpawn = rightHandPosition.position;
        positionToSpawn.y = 0;
        GameObject fireballObj = Instantiate(fireballPrefab, positionToSpawn, Quaternion.identity) as GameObject;
        GameObject fireballObj2 = Instantiate(fireballPrefab, positionToSpawn, Quaternion.identity) as GameObject;
        GameObject fireballObj3 = Instantiate(fireballPrefab, positionToSpawn, Quaternion.identity) as GameObject;


        while (Time.time < startTime + castTime)
        {
            FaceEnemy(player.transform.position);
            yield return null;
        }
        animator.SetBool(isCasting, false);
        animator.SetBool(isAttacking, true);
        //spawn projectile
        yield return new WaitForSeconds(.3f);

        Vector3 directionToFace = (player.transform.position - fireballObj.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToFace.x, 0, directionToFace.z));
        Quaternion lookRotation2 = lookRotation * Quaternion.Euler(0, 8, 0); // this adds a 90 degrees Y rotation
        Quaternion lookRotation3 = lookRotation * Quaternion.Euler(0, -8, 0); // this adds a 90 degrees Y rotation

        fireballObj.transform.rotation = Quaternion.Slerp(fireballObj.transform.rotation, lookRotation, 1);
        fireballObj2.transform.rotation = Quaternion.Slerp(fireballObj.transform.rotation, lookRotation2, 1);
        fireballObj3.transform.rotation = Quaternion.Slerp(fireballObj.transform.rotation, lookRotation3, 1);
        fireballs.Add(fireballObj, new Vector3(0, 0, 0));
        fireballs.Add(fireballObj2, new Vector3(0, 0, 0));
        fireballs.Add(fireballObj3, new Vector3(0, 0, 0));
        // fireballs.Add(fireballObj2, dir2);
        // fireballs.Add(fireballObj3, dir3);
        animator.SetBool(isAttacking, false);
        canAttack = true;
    }




    IEnumerator Chasing()
    {
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
    private void Chilling()
    {
        if (canMoveRandomly)
        {
            StartCoroutine(MoveEnemy());
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

            if (offset.magnitude < 2f)
                break;

            transform.position = Vector3.MoveTowards(transform.position, destination, walkingSpeed * Time.deltaTime);

            yield return null;
        }

        animator.SetBool(isWalking, false);
        yield return new WaitForSeconds(2);
        canMoveRandomly = true;
    }
    IEnumerator LookForTarget()
    {
        canLookForATarget = false;
        float distance = Vector3.Distance(transform.position, player.transform.position);
        distanceFromPlayer = distance;

        // check if player has entered ui range
        if (distance < aggroRadius)
        {
            playerInRadius = true;
        }
        else
        {
            currentStage = Stages.Chilling;
            playerInRadius = false;
        }

        yield return new WaitForSeconds(.1f);
        canLookForATarget = true;
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


    //detection mode
    //randomly walk around circle
    // if character near by
    // - turn on effects
    // - start attack mode

    //attack mode
    //- if not in range turn chase mode
    //- if getting hit too turn teleport mode
    //- if 10-15sec pass turn special attack mode
    // spawn attack particles

    //chase mode
    //- follow target till you are in some range or if he is to far stop



    //special attack mode
    // turn on special effect
    // for 6 seconds
    // - make the boss invunarable
    // - spawn big circles which when explode take player damage
}
