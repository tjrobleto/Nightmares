using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    private Transform target;
    public float smoothing = 5.0f;

    private Vector3 offset;

    void Start()
    {
        target = this.transform; // Start at myself until the player arrives
    }

    internal void StartCamera(Transform playerTransform)
    {
        this.target = playerTransform;
        offset = transform.position - target.position;
    }

    internal void StopCamera()
    {
        target = this.transform;
        offset = transform.position - target.position;
    }

    void FixedUpdate()
    {
        Vector3 targetCamPos = target.position + offset;
        transform.position = Vector3.Lerp(transform.position, targetCamPos, smoothing * Time.deltaTime);        
    }
}
