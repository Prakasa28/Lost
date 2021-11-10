using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    FMOD.Studio.EventInstance Event;

    public void PlayEvent(string path)
    {
        FMODUnity.RuntimeManager.PlayOneShotAttached(path, gameObject);
    }

    void PlayerAttackEvent(string path)
    {
        Event = FMODUnity.RuntimeManager.CreateInstance(path);
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(Event, transform, GetComponent<Rigidbody>());
        Event.start();
        Event.release();
    }

    void StopAttackEvent()
    {
        Event.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }
}
