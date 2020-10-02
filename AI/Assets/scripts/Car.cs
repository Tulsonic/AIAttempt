using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NeuralNet))]

public class Car : MonoBehaviour
{
    Rigidbody rb;

    float timeSinceStart = 0f;

    NeuralNet network;

    [Header("Motion")]
    float vel = 0f;
    float acel = 0f;

    public float maxVel = 0f;

    [Header("Rotation")]
    float rVel = 0f;
    float rAcel = 0f;

    public float maxRVel = 0f;

    [Header("Fitness Values")]
    public float timeMultiplier = -0.2f;
    public float distanceMultiplier = -1.4f;
    public Transform goalA;
    public Transform goalB;

    [Header("NN options")]
    public int layers = 4;
    public int neurons = 15;

    string goal = "A";

    float fitness = 0f;

    float distanceToTarget = 0f;

    List<float> inputs = new List<float>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < 10; i++)
        {
            inputs.Add(0f);
        }

        network = GetComponent<NeuralNet>();
        network.Initilise(layers, neurons);
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
            inputs[0] = hit.distance / 60;
            //print("sForward: " + sForward);
        }

        r.direction = left;

        if (Physics.Raycast(r, out hit, 60))
        {
            inputs[1] = hit.distance / 60;
            //print("sLeft: " + sLeft);
        }

        r.direction = right;

        if (Physics.Raycast(r, out hit, 60))
        {
            inputs[2] = hit.distance / 60;
            //print("sRight: " + sRight);
        }

        r.direction = back;

        if (Physics.Raycast(r, out hit, 60))
        {
            inputs[3] = hit.distance / 60;
            //print("sBack: " + sBack);
        }

        r.direction = forwardLeft;

        if (Physics.Raycast(r, out hit, 60))
        {
            inputs[4] = hit.distance / 60;
            //print("sFLeft: " + sFLeft);
        }

        r.direction = forwardRight;

        if (Physics.Raycast(r, out hit, 60))
        {
            inputs[5] = hit.distance / 60;
            //print("sFRight: " + sFRight);
        }

        r.direction = backLeft;

        if (Physics.Raycast(r, out hit, 60))
        {
            inputs[6] = hit.distance / 60;
            //print("sBLeft: " + sBLeft);
        }

        r.direction = backRight;

        if (Physics.Raycast(r, out hit, 60))
        {
            inputs[7] = hit.distance / 60;
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
        if (Mathf.Abs(vel) < maxVel)
        {
            print(vel);
            vel += acel;
        } else
        {
            vel = maxVel;
        }

        rb.MovePosition(rb.position += transform.forward * vel);

        if (Mathf.Abs(rVel) < maxRVel)
        {
            rVel += rAcel;
        } else
        {
            rVel = maxRVel;
        }

        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * rVel));
    }
    
    void FixedUpdate()
    {
        InputSensors();

        inputs[8] = vel;
        inputs[9] = rVel;

        (rAcel, acel) = network.RunNetwork(inputs);

        rAcel /= 10;
        acel /= 10;

        MoveCar();

        timeSinceStart += Time.deltaTime;

        fitnessCalculation();
    }
}
