using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrcController : MonoBehaviour
{

    Animator animator;
    private GameObject player;

    public float walkingSpeed = 8f;
    public float runningSpeed = 14f;
    public float walkingRadius = 35f;
    public float aggroRadius = 70f;
    public float attackRadius = 10f;
    public float chargeTime = 4f;
    public int attacksBeforeCharge = 2;

    private float distanceFromPlayer = Mathf.Infinity;
    private bool playerInRadius = false;

    private bool canMoveRandomly = true;
    private bool canChase = true;
    private bool canAttack = true;
    private bool canCharge = true;
    private int attackCount = 0;

    [HideInInspector]
    public bool stunned = false;
    private Vector3 startPosition;


    private enum Stages
    {
        Chilling, Chasing, Attacking
    }

    private Stages currentStage = Stages.Chilling;

    int isWalking;
    int isRunning;
    int isAttacking;
    int isCharging;

    void Awake()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        isWalking = Animator.StringToHash("IsWalking");
        isRunning = Animator.StringToHash("IsRunning");
        isAttacking = Animator.StringToHash("IsAttacking");
        isCharging = Animator.StringToHash("IsCharging");
    }

    // Start is called before the first frame update
    void Start()
    {

        startPosition = transform.position;
    }

    // Update is called once per frame
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
            FaceEnemy(player.transform.position);
            StartCoroutine(StartAttack());
        }
    }

    IEnumerator StartAttack()
    {
        if (distanceFromPlayer > attackRadius)
        {
            currentStage = Stages.Chasing;
            yield break;
        }

        canAttack = false;

        animator.SetBool(isAttacking, true);

        yield return new WaitForSeconds(3);

        animator.SetBool(isAttacking, false);
        canAttack = true;
        attackCount++;
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

    void FaceEnemy(Vector3 position)
    {
        //face the target
        Vector3 direction = (position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1);
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
