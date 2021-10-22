using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Selectable : MonoBehaviour
{

    public ParticleSystem selectedEffect;
    private ParticleSystem spawnedEffect;


    void Update()
    {
        if (spawnedEffect != null)
        {
            Vector3 positionToFollow = transform.position;
            positionToFollow.y += 0.5f;

            spawnedEffect.transform.position = positionToFollow;
        }
    }


    public void AddRing()
    {
        spawnedEffect = Instantiate(selectedEffect);
    }

    public void RemoveRing()
    {
        Destroy(spawnedEffect);
        spawnedEffect = null;
    }

}
