using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int healthLevel = 10;
    public int maxHealth;
    public int CurrentHealth;


    // Start is called before the first frame update
    void Start()
    {
        maxHealth = SetMaxHealthFromHealthLevel();
        CurrentHealth = maxHealth;
    }

    private int SetMaxHealthFromHealthLevel()
    {
        maxHealth = healthLevel * 10;
        return maxHealth;
    }

    public void TakeDamage(int damage)
    {
        CurrentHealth = CurrentHealth - damage;

        Debug.Log("Damage" + CurrentHealth);

        if (CurrentHealth <= 0)
        {
            CurrentHealth = 0;
            Debug.Log("EnemyDead");
        }
    } 

    
}
