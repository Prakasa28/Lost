using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    private Camera cameraPos;

    private GameObject camera;
    // Update is called once per frame

    private void Awake()
    {
        camera = GameObject.FindGameObjectWithTag("Camera");
        cameraPos = camera.gameObject.GetComponent<Camera>();
    }

    void LateUpdate()
    {
        transform.forward = cameraPos.transform.forward;
    }
}