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
    public float chargeTime = 5f;

    private Vector3 startPosition;
    private bool playerInRadius = false;

    private GameObject player;
    private float distanceFromPlayer = Mathf.Infinity;

    private bool canMoveRandomly = true;
    private bool canLookForATarget = true;
    private bool canChase = true;
    private bool canAttack = true;
    private bool canCharge = true;

    private int attackCount = 0;

    List<GameObject> fireballs = new List<GameObject>();
    Dictionary<GameObject, float> waitingFireballs = new Dictionary<GameObject, float>();

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
    }

    void Update()
    {
        if (canLookForATarget)
            StartCoroutine(LookForTarget());

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

        MoveFireballs();
    }

    private void MoveFireballs()
    {
        foreach (GameObject fireball in fireballs)
        {
            float distance = Vector3.Distance(fireball.transform.position, player.transform.position);

            if (distance < 100f)
            {
                fireball.transform.position += fireball.transform.forward * Time.deltaTime * fireballSpeed;
                continue;
            }
            fireballs.Remove(fireball);
            Destroy(fireball);
        }

    }



    private void Attack()
    {

        //count attacks made
        //if 5 do the recharge
        if (attackCount > 2 && canCharge)
            StartCoroutine(Charge());


        if (attackCount <= 2 && canAttack)
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
        canAttack = false;
        float startTime = Time.time;
        animator.SetBool(isCasting, true);

        for (int i = -10; i < 10; i += 5)
        {
            instantiateFireball(i);
        }

        while (Time.time < startTime + castTime)
        {
            FaceEnemy(player.transform.position);
            yield return null;
        }

        animator.SetBool(isCasting, false);
        animator.SetBool(isAttacking, true);
        yield return new WaitForSeconds(.3f);

        fireAllWaitingFireballs(player.transform);

        // fireballs.Add(fireballObj2, dir2);
        // fireballs.Add(fireballObj3, dir3);
        animator.SetBool(isAttacking, false);
        canAttack = true;
        attackCount++;
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
            // currentStage = Stages.Chilling;
            playerInRadius = false;
        }

        yield return new WaitForSeconds(.1f);
        canLookForATarget = true;
    }


    private void instantiateFireball(float rotation)
    {
        Vector3 positionToSpawn = rightHandPosition.position;
        positionToSpawn.y = 0;


        GameObject fireballObj = Instantiate(fireballPrefab, positionToSpawn, Quaternion.identity) as GameObject;

        waitingFireballs.Add(fireballObj, rotation);
    }
    private void fireAllWaitingFireballs(Transform position)
    {
        foreach (KeyValuePair<GameObject, float> fireballEntry in waitingFireballs)
        {
            Vector3 directionToFace = (player.transform.position - fireballEntry.Key.transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(directionToFace.x, 0, directionToFace.z));

            lookRotation *= Quaternion.Euler(0, fireballEntry.Value, 0); // this adds a 90 degrees Y rotation

            fireballEntry.Key.transform.rotation = Quaternion.Slerp(fireballEntry.Key.transform.rotation, lookRotation, 1);
            fireballs.Add(fireballEntry.Key);

        }
        waitingFireballs.Clear();
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
