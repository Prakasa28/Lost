using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusEnemy : MonoBehaviour
{
    // range for the UI
    public float bossRange = 60f;
    public float enemyRange = 60f;
    private float distance;

    private HealthBar BossHealthBar;
    InputHandler inputHandler;

    private GameObject[] bosses;
    private GameObject[] enemies;
    private List<GameObject> enemiesInRange;

    private GameObject focusedBoss;
    [HideInInspector]
    public GameObject focusedTarget;

    private AudioController audioCamera;
    private bool canChangeEnemy = true;



    void Awake()
    {
        inputHandler = GetComponent<InputHandler>();
        bosses = GameObject.FindGameObjectsWithTag("boss");
        enemies = GameObject.FindGameObjectsWithTag("enemy");
        enemiesInRange = new List<GameObject>();
        BossHealthBar = GameObject.FindGameObjectWithTag("bossHealthbar").GetComponent<HealthBar>();
        audioCamera = GameObject.FindGameObjectWithTag("Camera").GetComponentInChildren<AudioController>();
    }


    // Update is called once per frame
    void Update()
    {
        lookForBosses();
        lookForEnemies();
        triggerBossHealthBar();

        if (focusedTarget == null && enemiesInRange.Count > 0 && focusedBoss == null)
        {
            focusedTarget = enemiesInRange[0];
            controlUI();
        }

        if (focusedTarget == null && focusedBoss != null)
        {
            focusedTarget = focusedBoss;
            controlUI();
        }


        //change targets
        if (inputHandler.isChangingEnemy && canChangeEnemy)
        {
            StartCoroutine(ChangeEnemy());
        }
    }

    private void controlUI()
    {

        foreach (GameObject enemy in enemiesInRange)
        {
            if (enemy == focusedTarget)
            {
                enemy.GetComponent<Selectable>().AddRing();
            }
            else
            {
                enemy.GetComponent<Selectable>().RemoveRing();
            }


        }
    }



    IEnumerator ChangeEnemy()
    {
        int enemiesCount = enemiesInRange.Count;
        if (enemiesCount == 0 || enemiesCount == 1)
        {
            yield break;
        }

        canChangeEnemy = false;

        int selectedIndex = enemiesInRange.IndexOf(focusedTarget);

        if (selectedIndex == enemiesCount - 1)
        {
            //its the last element
            focusedTarget = enemiesInRange[0];
        }
        else
        {
            focusedTarget = enemiesInRange[selectedIndex + 1];
        }

        controlUI();
        //if its the last change it to the first
        //otherwise just get the
        yield return new WaitForSeconds(.5f);
        canChangeEnemy = true;

    }

    private void triggerBossHealthBar()
    {
        //if focused target not null > show hp canvas
        if (focusedBoss != null)
        {
            //spawn the canvas
            EnemyStats enemyStats = focusedBoss.GetComponent<EnemyStats>();
            BossHealthBar.gameObject.SetActive(true);
            audioCamera.StartBattle();
            return;
        }
        audioCamera.StopBattle();
        BossHealthBar.gameObject.SetActive(false);
    }

    private void lookForEnemies()
    {
        foreach (GameObject enemy in enemies)
        {
            distance = Vector3.Distance(transform.position, enemy.transform.position);

            // check if player has entered ui range
            if (distance < enemyRange && enemy.GetComponent<Selectable>().isActiveAndEnabled == true)
            {
                if (!enemiesInRange.Contains(enemy))
                    enemiesInRange.Add(enemy);
            }
            else
            {
                if (focusedTarget == enemy)
                    focusedTarget = null;
                enemiesInRange.Remove(enemy);
            }

        }

    }
    private void lookForBosses()
    {
        foreach (GameObject boss in bosses)
        {
            distance = Vector3.Distance(transform.position, boss.transform.position);

            // check if player has entered ui range
            if (distance < bossRange && boss.GetComponent<Selectable>().isActiveAndEnabled == true)
            {
                if (!enemiesInRange.Contains(boss))
                    enemiesInRange.Add(boss);

                focusedBoss = boss;
            }
            else
            {
                enemiesInRange.Remove(boss);
                focusedBoss = null;
            }

        }

    }

    // draw the range circle
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bossRange);
    }



}
