using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("triggered");
        Destroy(GameObject.FindWithTag("Level2Door"));
        CinemachineShake.Instance.ShakeCamera(5f, 1f);
    }
}
