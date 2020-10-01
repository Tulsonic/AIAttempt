using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    public Vector3 inVel = Vector3.zero;
    public Vector3 inAcel = Vector3.zero;

    Vector3 vel = Vector3.zero;
    Vector3 acel = Vector3.zero;

    Rigidbody rb; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        vel = inVel;
        acel = inAcel;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        vel += acel;
        rb.position += vel;
        Debug.Log(rb.position);
    }
}
