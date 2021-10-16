using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FocusEnemy : MonoBehaviour
{
    // range for the UI
    public float range = 60f;
    private float distance;


    public HealthBar healthBar;

    public List<GameObject> bosses;

    [HideInInspector]
    public GameObject focusedTarget;

    private bool maxHealthSetted = false;



    // Update is called once per frame
    void Update()
    {
        //look for target every .5 seconds
        StartCoroutine("LookForTargets");

        //if focused target not null > show hp canvas

        if (focusedTarget != null)
        {
            //spawn the canvas
            healthBar.gameObject.SetActive(true);

            if (!maxHealthSetted)
            {
                healthBar.SetMaxHealth(100); // set from enemy stats
                maxHealthSetted = true;
            }

            healthBar.SetHealth(30); // set from enemy stats
        }
        else
        {
            maxHealthSetted = false;
            healthBar.gameObject.SetActive(false);
        }
    }

    IEnumerator LookForTargets()
    {
        bool found = false;
        foreach (GameObject boss in bosses)
        {
            distance = Vector3.Distance(transform.position, boss.transform.position);

            // check if player has entered ui range
            if (distance < range)
            {
                focusedTarget = boss;
                found = true;
            }

        }

        if (!found)
            focusedTarget = null;

        yield return new WaitForSeconds(.5f);

    }

    // draw the range circle
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

}
