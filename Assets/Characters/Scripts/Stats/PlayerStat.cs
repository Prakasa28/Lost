using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStat : CharacterStats
{
    // Start is called before the first frame update
    void Start()
    {
       
    }

    public override void Die()
    {
        base.Die();
        //kill the player in some way 
        PlayerManager.instance.KillPlayer();
    }


}
