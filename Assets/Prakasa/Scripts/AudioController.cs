using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class AudioController : MonoBehaviour
{

    private bool battleMusic = false;
    FMOD.Studio.EventInstance Music;
    //FMOD.Studio.paramterInstance combatVal;

    // Use this for initialization
    void Start()
     {
      
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/GameSound/Juhani Junkala - Adaptive Game Music");
        Music.start();

    }

    // Update is called once per frame
    void Update()
    {
        if (battleMusic == true)
        {
            Music.setParameterByName("State", 1.0f, true);
        }
        else
        {
            Music.setParameterByName("State", 0f, true);
        }
          
    }

    public void StartBattle()
    {
        battleMusic = true;
    }

    public void StopBattle()
    {
        battleMusic = false;
    }
}
