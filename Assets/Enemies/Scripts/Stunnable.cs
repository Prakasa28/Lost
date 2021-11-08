using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stunnable : MonoBehaviour
{
    Animator animator;

    public ParticleSystem stunnedParticle;
    public Transform stunnedPlaceholder;
    public int duration = 4;

    private bool justStunned = false;
    private int isStunned;
    [HideInInspector]
    public bool dead;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        isStunned = Animator.StringToHash("IsStunned");
    }

    // Update is called once per frame
    void Update()
    {
        if (justStunned)
        {
            StartCoroutine(Stun());
        }
    }

    IEnumerator Stun()
    {
        justStunned = false;
        animator.SetBool(isStunned, true);
        ParticleSystem spawnedParticle = Instantiate(stunnedParticle, stunnedPlaceholder);
        spawnedParticle.Play();

        AbilitiesController abilitiesController = GetComponent<AbilitiesController>();
        FireballsController fireballsController = GetComponent<FireballsController>();
        MovementController movementController = GetComponent<MovementController>();
        Boss2Controller boss2Controller = GetComponent<Boss2Controller>();
        OrcController orcController = GetComponent<OrcController>();

        if (movementController != null)
            movementController.enabled = false;
        if (abilitiesController != null)
            abilitiesController.enabled = false;
        if (orcController != null)
            orcController.enabled = false;
        // if (fireballsController != null)
            // fireballsController.enabled = false;


        if (boss2Controller != null)
        {
            boss2Controller.stunned = true;
            boss2Controller.enabled = false;
        }


        yield return new WaitForSeconds(duration);

        if (spawnedParticle != null)
            Destroy(spawnedParticle);

        animator.SetBool(isStunned, false);

        if (!dead)
        {
            if (movementController != null)
                movementController.enabled = true;
            if (abilitiesController != null)
                abilitiesController.enabled = true;
            if (orcController != null)
                orcController.enabled = true;
            // if (fireballsController != null)
                // fireballsController.enabled = true;

            if (boss2Controller != null)
            {
                boss2Controller.stunned = false;
                boss2Controller.enabled = true;

            }

        }


    }

    public void StunTarget()
    {
        justStunned = true;
    }
}
