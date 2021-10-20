using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AbilitiesController : MonoBehaviour
{

    Animator animator;
    FocusEnemy enemy;
    InputHandler inputHandler;
    CharacterController characterController;
    MovementController movementController;

    [Header("Cooldowns")]
    public float attackCooldown = .2f;
    public float dashCooldown = 1.5f;
    public float chargeCooldown = 6f;
    public float ultimateCooldown = 6f;


    [Header("Dash")]
    public float dashTime = .7f;
    public float dashSpeed = 25;

    [Header("Charge")]
    public float chargeSpeed = 50;
    public float chargeMinRadius = 20;
    public float chargeMaxRadius = 40;
    public float hitTargetPoint = 6f;
    public float particleSize = 2f;
    public float particleHeight = .2f;

    int isDodgingHash;
    int isAttackingHash1;
    int isAttackingHash2;
    int isChargingHash;
    int isEndCharging;
    int isUltimatingHash;

    bool performingAction = false;


    private float attackCooldownCurrent = 0;
    private float dashCooldownCurrent = 0;
    private float chargeCooldownCurrent = 0;
    private float ultimateCooldownCurrent = 0;


    private bool switchAttackAnimations = false;
    private float nextActionTime = 0.0f;
    private float period = .1f; //Update cooldown timer every sec

    [Header("UI")]
    public ParticleSystem particleForCharge;
    public GameObject attackAbilityUIObject;
    public GameObject dashAblilityUIObject;
    public GameObject chargeAblilityUIObject;
    public GameObject ultimateAbilityUIObject;



    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        enemy = GetComponent<FocusEnemy>();
        inputHandler = GetComponent<InputHandler>();
        movementController = GetComponent<MovementController>();

        //spawn particle
        Vector3 particleSpawnPosition = new Vector3(transform.position.x, particleHeight, transform.position.z);
        particleForCharge = Instantiate(particleForCharge, particleSpawnPosition, transform.rotation);
        particleForCharge.transform.parent = transform;
        particleForCharge.transform.localScale = new Vector3(particleSize, particleSize, particleSize);


        isDodgingHash = Animator.StringToHash("IsDodging");
        isAttackingHash1 = Animator.StringToHash("IsAttacking1");
        isAttackingHash2 = Animator.StringToHash("IsAttacking2");
        isChargingHash = Animator.StringToHash("IsCharging");
        isEndCharging = Animator.StringToHash("IsEndCharging");
        isUltimatingHash = Animator.StringToHash("IsUltimating");
    }

    void Start()
    {
        ;
    }

    void Update()
    {
        updateUI();
        cooldownControl();
        handleAbilities();
    }

    void handleAbilities()
    {
        if (!performingAction)
        {
            if (inputHandler.isAttackedPressed && attackCooldownCurrent <= 0)
            {
                StartCoroutine(Attack());
            }

            if (inputHandler.isDodgePressed && dashCooldownCurrent <= 0)
            {
                StartCoroutine(Dash());
            }

            if (enemy.focusedTarget != null)
            {
                float distance = Vector3.Distance(transform.position, enemy.focusedTarget.transform.position);

                if (inputHandler.isChargePressed && chargeCooldownCurrent <= 0 && distance >= chargeMinRadius && distance <= chargeMaxRadius)
                {
                    StartCoroutine(Charge());
                }
            }

            if (inputHandler.isUltimatePressed && ultimateCooldownCurrent <= 0)
            {
                StartCoroutine(Ultimate());
            }

        }
    }
    IEnumerator Charge()
    {

        performingAction = true;
        movementController.canMove = false;

        GameObject focusedEnemy = enemy.focusedTarget;

        particleForCharge.Play();

        animator.SetBool(isChargingHash, true);
        while (true)
        {
            FaceEnemy(focusedEnemy);
            var offset = focusedEnemy.transform.position - transform.position;
            offset.y = 0;

            if (offset.magnitude < hitTargetPoint)
            {
                break;
            }

            offset = offset.normalized * chargeSpeed;
            characterController.Move(offset * Time.deltaTime);


            yield return null;
        }

        animator.SetBool(isChargingHash, false);

        particleForCharge.Stop();

        animator.SetBool(isEndCharging, true);

        yield return new WaitForSeconds(.4f);

        animator.SetBool(isEndCharging, false);

        chargeCooldownCurrent = chargeCooldown;
        movementController.canMove = true;
        performingAction = false;
    }
    IEnumerator Attack()
    {
        performingAction = true;

        if (switchAttackAnimations)
        {
            animator.SetBool(isAttackingHash1, true);
            yield return new WaitForSeconds(.5f);
            animator.SetBool(isAttackingHash1, false);
        }
        else
        {
            animator.SetBool(isAttackingHash2, true);
            yield return new WaitForSeconds(1f);
            animator.SetBool(isAttackingHash2, false);

        }

        switchAttackAnimations = !switchAttackAnimations;
        attackCooldownCurrent = attackCooldown;

        performingAction = false;
    }
    IEnumerator Ultimate()
    {
        performingAction = true;
        movementController.canMove = false;

        animator.SetBool(isUltimatingHash, true);
        yield return new WaitForSeconds(2.3f);
        animator.SetBool(isUltimatingHash, false);
        yield return new WaitForSeconds(0.2f);

        ultimateCooldownCurrent = ultimateCooldown;

        movementController.canMove = true;
        performingAction = false;
    }
    IEnumerator Dash()
    {
        performingAction = true;
        movementController.canMove = false;

        Vector3 dashDir = inputHandler.currentMovement;
        float startTime = Time.time;
        movementController.handleRotation(1);
        animator.SetBool(isDodgingHash, true);
        while (Time.time < startTime + dashTime)
        {
            characterController.Move(dashDir * dashSpeed * Time.deltaTime);
            yield return null;
        }
        animator.SetBool(isDodgingHash, false);

        dashCooldownCurrent = dashCooldown;

        movementController.canMove = true;
        performingAction = false;
    }
    void FaceEnemy(GameObject enemy)
    {
        //face the target
        Vector3 direction = (enemy.transform.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 1);

    }
    void cooldownControl()
    {
        if (Time.time > nextActionTime)
        {
            nextActionTime += period;
            if (dashCooldownCurrent > 0)
                dashCooldownCurrent -= period;

            if (chargeCooldownCurrent > 0)
                chargeCooldownCurrent -= period;

            if (ultimateCooldownCurrent > 0)
                ultimateCooldownCurrent -= period;

            if (attackCooldownCurrent > 0)
                attackCooldownCurrent -= period;

        }

    }
    void updateUIAbility(float cooldown, GameObject uiObject, bool avaliable = true)
    {
        Image image = uiObject.transform.Find("Image").gameObject.GetComponent<Image>();
        GameObject coolDownObject = uiObject.transform.Find("Cooldown").gameObject;

        if (cooldown > 0)
        {
            image.fillAmount -= 1 / cooldown * Time.deltaTime;
            coolDownObject.SetActive(true);

            float coolDownToShow = Mathf.Floor(cooldown);

            if (cooldown < 1)
            {
                coolDownToShow = cooldown;
                coolDownObject.transform.GetComponent<TextMeshProUGUI>().fontSize = 19;
                coolDownObject.transform.GetComponent<TextMeshProUGUI>().text = coolDownToShow.ToString("F1");
            }
            else
            {

                coolDownObject.transform.GetComponent<TextMeshProUGUI>().text = coolDownToShow.ToString();
            }
        }
        else
        {
            if (!avaliable)
            {
                image.fillAmount = 0;
            }
            else
            {
                image.fillAmount = 1;
            }
            coolDownObject.SetActive(false);
        }

    }
    void updateUI()
    {
        bool chargeAvaliable = false;
        if (enemy.focusedTarget != null)
        {
            float distance = Vector3.Distance(transform.position, enemy.focusedTarget.transform.position);

            if (distance >= chargeMinRadius && distance <= chargeMaxRadius)
                chargeAvaliable = true;
        }

        updateUIAbility(dashCooldownCurrent, dashAblilityUIObject);
        updateUIAbility(ultimateCooldownCurrent, ultimateAbilityUIObject);
        updateUIAbility(attackCooldownCurrent, attackAbilityUIObject);
        updateUIAbility(chargeCooldownCurrent, chargeAblilityUIObject, chargeAvaliable);
    }


}
