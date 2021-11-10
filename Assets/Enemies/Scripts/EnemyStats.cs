using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyStats : MonoBehaviour
{
    public int maxHealth = 200;
    private int currentHealth;

    Animator animator;
    Boss2Controller controller;
    OrcController controllerOrc;
    Selectable selectable;
    Stunnable stunnable;
    HealthBar healthBar;

    private int isDyingHash;
    private int isHitHash;

    private bool maxHealthSetted;

    private void Awake()
    {

        animator = GetComponent<Animator>();
        controller = GetComponent<Boss2Controller>();
        controllerOrc = GetComponent<OrcController>();
        selectable = GetComponent<Selectable>();
        stunnable = GetComponent<Stunnable>();
        isDyingHash = Animator.StringToHash("IsDead");
        isHitHash = Animator.StringToHash("IsHit");

    }

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;

        //if boss healthbar == the big one
        if (transform.tag == "boss")
        {
            healthBar = GameObject.FindGameObjectWithTag("bossHealthbar").GetComponent<HealthBar>();
        }

        if (transform.tag == "enemy")
        {
            healthBar = GetComponentInChildren<HealthBar>();
        }

        healthBar.SetMaxHealth(maxHealth); // set from enemy stats
    }

    private void SetMaxHealth(int health)
    {
        maxHealth = health;
    }

    public int getCurrentHealth()
    {
        return currentHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;

        healthBar.SetHealth(getCurrentHealth());

        if (currentHealth <= 0)
        {

            animator.SetBool(isDyingHash, true);

            if (stunnable != null)
                stunnable.dead = true;

            if (controller != null)
            {

                controller.enabled = false;
                controller.dead = true;
            }
            if (controllerOrc != null)
                controllerOrc.enabled = false;
            
            if (healthBar != null)
                healthBar.gameObject.SetActive(false);

            if (selectable != null)
            {
                selectable.RemoveRing();
                selectable.enabled = false;
            }

            if (this.gameObject.CompareTag("boss"))
                SceneManager.LoadScene(3);
            return;
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
