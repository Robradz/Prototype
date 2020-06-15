using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

[DisallowMultipleComponent]
public class Rotator : MonoBehaviour
{

    [SerializeField] float xAngle = 0.001f;
    [SerializeField] float yAngle = 0.0f;
    [SerializeField] float zAngle = 0.0f;


    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(xAngle, yAngle, zAngle);
    }
}
