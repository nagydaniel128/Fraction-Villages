using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateTowardsCam : MonoBehaviour
{
    Camera cam;
    private void Start()
    {
        cam = Camera.main;
    }
    void LateUpdate()
    {
        transform.LookAt(cam.transform.position);
    }
}
