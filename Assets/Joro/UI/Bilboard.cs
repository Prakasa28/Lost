using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    public Camera _camera;
    // Update is called once per frame
    void LateUpdate()
    {
       transform.forward = _camera.transform.forward; 
    }
}
