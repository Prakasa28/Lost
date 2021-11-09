using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;
using FMODUnity;

public class AudioController : MonoBehaviour
{
    

    FMOD.Studio.EventInstance Music;
    //FMOD.Studio.paramterInstance combatVal;

    // Use this for initialization
    void Start()
     {
      
        Music = FMODUnity.RuntimeManager.CreateInstance("event:/Juhani Junkala - Adaptive Game Music");
        Music.start();

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            Music.setParameterByName("State", 1.0f, true);
        }
        if (Input.GetKeyDown("k"))
        {
            Music.setParameterByName("State", 0f, true);
        }
    }
}
