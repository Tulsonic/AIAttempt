using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NeuralNet))]

public class Car : MonoBehaviour
{
    Rigidbody rb;

    float timeSinceStart = 0f;

    public NeuralNet network;

    string state = "moving";

    Vector3 startPosition;
    Vector3 startRotation;

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
    public int layers = 1;
    public int neurons = 7;

    string goal = "A";

    public float fitness = 0f;

    float distanceToTarget = 0f;

    List<float> inputs = new List<float>();

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        for (int i = 0; i < 10; i++)
        {
            inputs.Add(0f);
        }

        startPosition = transform.position;
        startRotation = transform.eulerAngles;

        network = GetComponent<NeuralNet>();
        network.Initilise(layers, neurons);
    }

    void Death()
    {
        GameObject.FindObjectOfType<GenController>().Death(gameObject);
        state = "static";
    }
    

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 8 && state != "static")
        {
            fitness -= fitness * 0.1f;
            Death();
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

        if (timeSinceStart > 10 && Mathf.Abs(fitness) < 100 && state != "static")
        {
            Death();
        }
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

        if(Physics.Raycast(r, out hit, 60, 8))
        {
            inputs[0] = hit.distance / 60;
            //print("sForward: " + sForward);
        }

        r.direction = left;

        if (Physics.Raycast(r, out hit, 60, 8))
        {
            inputs[1] = hit.distance / 60;
            //print("sLeft: " + sLeft);
        }

        r.direction = right;

        if (Physics.Raycast(r, out hit, 60, 8))
        {
            inputs[2] = hit.distance / 60;
            //print("sRight: " + sRight);
        }

        r.direction = back;

        if (Physics.Raycast(r, out hit, 60, 8))
        {
            inputs[3] = hit.distance / 60;
            //print("sBack: " + sBack);
        }

        r.direction = forwardLeft;

        if (Physics.Raycast(r, out hit, 60, 8))
        {
            inputs[4] = hit.distance / 60;
            //print("sFLeft: " + sFLeft);
        }

        r.direction = forwardRight;

        if (Physics.Raycast(r, out hit, 60, 8))
        {
            inputs[5] = hit.distance / 60;
            //print("sFRight: " + sFRight);
        }

        r.direction = backLeft;

        if (Physics.Raycast(r, out hit, 60, 8))
        {
            inputs[6] = hit.distance / 60;
            //print("sBLeft: " + sBLeft);
        }

        r.direction = backRight;

        if (Physics.Raycast(r, out hit, 60, 8))
        {
            inputs[7] = hit.distance / 60;
            //print("sBRight: " + sBRight);
        }

        #endregion

    }

    void MoveCar()
    {
        if (Mathf.Abs(vel) < maxVel)
        {
            vel += acel;
        } else
        {
            if ((vel < 0 && acel > 0) || (vel > 0 && acel < 0))
            {
                vel += acel;
            }
        }

        rb.MovePosition(rb.position += transform.forward * vel);

        if (Mathf.Abs(rVel) < maxRVel)
        {
            rVel += rAcel;
        }
        else
        {
            if ((rVel < 0 && rAcel > 0) || (rVel > 0 && rAcel < 0))
            {
               rVel += rAcel;
            }
        }

        rb.MoveRotation(rb.rotation * Quaternion.Euler(Vector3.up * rVel * vel / maxVel));
    }

    void Reset()
    {
        fitness = 0;
        timeSinceStart = 0f;
        distanceToTarget = 0f;
        transform.position = startPosition;
        transform.eulerAngles = startRotation;
    }

    public void ResetWithNetwork(NeuralNet net)
    {
        network = net;
        Reset();
    } 
    
    void FixedUpdate()
    {
        if (state == "moving")
        {
            InputSensors();

            inputs[8] = vel / maxVel;
            inputs[9] = rVel / maxRVel;

            (float rScal, float vScal, float rDir, float vDir) = network.RunNetwork(inputs);

            acel = vScal * vDir;
            rAcel = rScal * rDir;

            MoveCar();

            timeSinceStart += Time.deltaTime;

            fitnessCalculation();
        }
    }
}
