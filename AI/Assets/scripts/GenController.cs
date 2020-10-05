using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenController : MonoBehaviour
{
    public Car car;

    public int initialPopulation = 20;
    public float mutationRate = 0.05f;

    List<Car> population = new List<Car>();
    List<Car> matingPool = new List<Car>();
    List<GameObject> deadList = new List<GameObject>();

    public int currentGeneration = 1;

    void Start()
    {
        InitPopulation();
    }

    void InitPopulation()
    {
        for (int i = 0; i < initialPopulation; i++)
        {
            population.Add(Instantiate(car, transform.position, transform.rotation));
        }
    }

    public void Death(GameObject obj) 
    {
        deadList.Add(obj);
    }

    Car CrossOver(Car parentA, Car parentB)
    {
        Car child = Instantiate(car, transform.position, transform.rotation);
        NeuralNet childNet = child.network;
        
        for (int i = 0; i < childNet.weights.Count; i++)
        {
            if (Random.Range(0, 1) <= 0.5f)
            {
                if (i < Mathf.FloorToInt(childNet.weights.Count / 2))
                {
                    childNet.weights[i] = parentA.network.weights[i];
                } else
                {
                    childNet.weights[i] = parentB.network.weights[i];
                }
                
            } else
            {
                if (i < Mathf.FloorToInt(childNet.weights.Count / 2))
                {
                    childNet.weights[i] = parentB.network.weights[i];
                }
                else
                {
                    childNet.weights[i] = parentA.network.weights[i];
                }
            }
        }

        for (int i = 0; i < childNet.biases.Count; i++)
        {
            if (Random.Range(0, 1) <= 0.5f)
            {
                if (i < Mathf.FloorToInt(childNet.biases.Count / 2))
                {
                    childNet.biases[i] = parentA.network.biases[i];
                }
                else
                {
                    childNet.biases[i] = parentB.network.biases[i];
                }

            }
            else
            {
                if (i < Mathf.FloorToInt(childNet.weights.Count / 2))
                {
                    childNet.biases[i] = parentB.network.biases[i];
                }
                else
                {
                    childNet.biases[i] = parentA.network.biases[i];
                }
            }
        }

        return child;
    }

    Car Mutate(Car child)
    {
        NeuralNet network = child.network;

        for (int i = 0; i < network.weights.Count; i++)
        {
            for (int x = 0; x < network.weights[i].RowCount; x++)
            {
                for (int y = 0; y < network.weights[i].ColumnCount; y++)
                {
                    int mult = Mathf.RoundToInt(Random.Range(-1f, 1f));
                    network.weights[i][x, y] += network.weights[i][x, y] * 0.2f * mult;
                }
            }
        }

        for (int i = 0; i < network.biases.Count; i++)
        {
            int mult = Mathf.RoundToInt(Random.Range(-1f, 1f));
            network.biases[i] += network.biases[i] * 0.2f * mult;
        }

        return child;
    }

    void Generate()
    {
        for (int i = 0; i < population.Count; i++)
        {
            bool again = true;
            while (again)
            {
                int a = Mathf.FloorToInt(Random.Range(0, matingPool.Count));
                int b = Mathf.FloorToInt(Random.Range(0, matingPool.Count));
                Car parentA = matingPool[a];
                Car parentB = matingPool[b];
                if (parentA != parentB)
                {
                    Car child = CrossOver(parentA, parentB);
                    if (Random.Range(0f, 1f) < mutationRate)
                    {
                        child = Mutate(child);
                    }
                    population[i] = child;
                    again = false;
                }
            }
        }
        currentGeneration++;
    }

    void Update()
    {

        Debug.DrawLine(transform.position, transform.position + transform.forward * 2, Color.yellow);

        if (deadList.Count > population.Count)
        {
            print("brh");
        }

        float maxFitness = -Mathf.Infinity;
        GameObject maxFitnessCar = population[0].gameObject;

        for (int i = 0; i < population.Count; i++)
        {
            population[i].gameObject.GetComponent<MeshRenderer>().enabled = false;
            if (population[i].fitness > maxFitness)
            {
                maxFitness = population[i].fitness;
                maxFitnessCar = population[i].gameObject;
            }
        }

        maxFitnessCar.GetComponent<MeshRenderer>().enabled = true;
        Transform fitnessCarTransform = maxFitnessCar.transform;
        Debug.DrawLine(fitnessCarTransform.position, fitnessCarTransform.position + fitnessCarTransform.forward * 2, Color.green);

        if (deadList.Count == population.Count)
        {

            matingPool.Clear();

            float minFitness = 0;
            for (int i = 0; i < population.Count; i++)
            {
                if (population[i].fitness < minFitness)
                {
                    minFitness = population[i].fitness;
                }
            }

            float shift = Mathf.Abs(minFitness) + 1;

            maxFitness += shift;

            for (int i = 0; i < population.Count; i++)
            {
                float times = (population[i].fitness + shift) / maxFitness;
                for (int j = 0; j < Mathf.FloorToInt(times * 100); j++)
                {
                    matingPool.Add(population[i]);
                }
            }

            Generate();

            for (int i = 0; i < deadList.Count; i++)
            {
                Destroy(deadList[i]);
            }

            deadList.Clear();
        }
    }
}
