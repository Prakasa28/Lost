using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bilboard : MonoBehaviour
{
    private Camera cameraPos;

    private GameObject gameCamera;
    // Update is called once per frame

    private void Awake()
    {
        gameCamera = GameObject.FindGameObjectWithTag("Camera");
        cameraPos = gameCamera.gameObject.GetComponent<Camera>();
    }

    void LateUpdate()
    {
        transform.forward = cameraPos.transform.forward;
    }
}