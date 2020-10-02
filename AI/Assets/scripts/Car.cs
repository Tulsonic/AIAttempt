using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    Rigidbody rb;

    float timeSinceStart = 0f;

    [Header("Motion")]
    public float vel = 0f;
    public float acel = 0f;

    public float maxVel = 0f;

    [Header("Rotation")]
    public float rInVel = 0f;
    public float rInAcel = 0f;

    Vector3 rVel = Vector3.zero;
    Vector3 rAcel = Vector3.zero;

    public float maxRVel = 0f;

    [Header("Fitness Values")]
    public float timeMultiplier = -0.2f;
    public float distanceMultiplier = -1.4f;
    public Transform goalA;
    public Transform goalB;

    string goal = "A";

    float fitness = 0f;

    float distanceToTarget = 0f;

    float sForward, sLeft, sRight, sBack, sFLeft, sFRight, sBLeft, sBRight;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        rVel = new Vector3(0, rInVel, 0);
        rAcel = new Vector3(0, rInAcel, 0);
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8)
        {
            Debug.Log(collision);
        }
    }

    void fitnessCalculation()
    {
        Vector3 distA = goalA.position - transform.position;
        Vector3 distB = goalB.position - transform.position;
        Vector3 distAB = goalB.position - goalA.position;

        switch (goal) { 
            case "A":
                distanceToTarget = (distA + distAB).magnitude;
                break;
            case "B":
                distanceToTarget = distB.magnitude;
                break;
        }

        fitness = distanceToTarget * distanceMultiplier + timeSinceStart * timeMultiplier;
        print(fitness);
    }

    void InputSensors()
    {
        Vector3 forward = transform.forward;
        Vector3 left =  -transform.right;
        Vector3 right = transform.right;
        Vector3 back = -transform.forward;
        Vector3 forwardLeft = transform.forward - transform.right;
        Vector3 forwardRight = transform.forward + transform.right;
        Vector3 backLeft = -transform.forward - transform.right;
        Vector3 backRight = -transform.forward + transform.right;

        #region Raycast Checks

        Ray r = new Ray(transform.position, forward);
        RaycastHit hit;

        if(Physics.Raycast(r, out hit, 60))
        {
            sForward = hit.distance / 60;
            //print("sForward: " + sForward);
        }

        r.direction = left;

        if (Physics.Raycast(r, out hit, 60))
        {
            sLeft = hit.distance / 60;
            //print("sLeft: " + sLeft);
        }

        r.direction = right;

        if (Physics.Raycast(r, out hit, 60))
        {
            sRight = hit.distance / 60;
            //print("sRight: " + sRight);
        }

        r.direction = back;

        if (Physics.Raycast(r, out hit, 60))
        {
            sBack = hit.distance / 60;
            //print("sBack: " + sBack);
        }

        r.direction = forwardLeft;

        if (Physics.Raycast(r, out hit, 60))
        {
            sFLeft = hit.distance / 60;
            //print("sFLeft: " + sFLeft);
        }

        r.direction = forwardRight;

        if (Physics.Raycast(r, out hit, 60))
        {
            sFRight = hit.distance / 60;
            //print("sFRight: " + sFRight);
        }

        r.direction = backLeft;

        if (Physics.Raycast(r, out hit, 60))
        {
            sBLeft = hit.distance / 60;
            //print("sBLeft: " + sBLeft);
        }

        r.direction = backRight;

        if (Physics.Raycast(r, out hit, 60))
        {
            sBRight = hit.distance / 60;
            //print("sBRight: " + sBRight);
        }

        #endregion

    }

    void Update()
    {
        Debug.DrawLine(transform.position, transform.position + transform.forward * 2, Color.green);
    }

    void MoveCar()
    {
        if (vel < maxVel)
        {
            vel += acel;
        }

        rb.MovePosition(rb.position += transform.forward * vel);

        if (rVel.magnitude < maxRVel)
        {
            rVel += rAcel;
        }

        Debug.Log(rVel);

        rb.MoveRotation(rb.rotation * Quaternion.Euler(rVel));
    }
    
    void FixedUpdate()
    {
        InputSensors();
        MoveCar();

        timeSinceStart += Time.deltaTime;

        fitnessCalculation();
    }
}
