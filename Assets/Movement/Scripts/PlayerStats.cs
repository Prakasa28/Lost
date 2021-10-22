using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    Animator animator;
    MovementController movementController;
    AbilitiesController abilitiesController;
    HealthBar healthBar;

    private int isDyingHash;
    private int isHitHash;


    void Awake()
    {
        animator = GetComponent<Animator>();
        movementController = GetComponent<MovementController>();
        abilitiesController = GetComponent<AbilitiesController>();
        isDyingHash = Animator.StringToHash("IsDying");
        isHitHash = Animator.StringToHash("IsHit");

        healthBar = GameObject.FindGameObjectWithTag("playerHealthbar").GetComponent<HealthBar>();
    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth); // set from enemy stats
    }

    private void SetMaxHealth(int health)
    {
        maxHealth = health;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);

        if (currentHealth <= 0)
        {
            movementController.enabled = false;
            abilitiesController.enabled = false;
            animator.SetBool(isDyingHash, true);
            return;
            //FINISH
        }

        StartCoroutine(HitAnimation());

    }

    IEnumerator HitAnimation()
    {
        animator.SetBool(isHitHash, true);
        yield return new WaitForSeconds(.5f);
        animator.SetBool(isHitHash, false);
    }

}
