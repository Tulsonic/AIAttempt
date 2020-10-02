using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using MathNet.Numerics.LinearAlgebra;
using System;

using Random = UnityEngine.Random;

public class NeuralNet : MonoBehaviour
{
    public Matrix<float> inputLayer = Matrix<float>.Build.Dense(1, 10);

    public List<Matrix<float>> hiddenLayers = new List<Matrix<float>>();

    public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 2);

    public List<Matrix<float>> weights = new List<Matrix<float>>();

    public List<float> biases = new List<float>();

    public float fitness;

    public void Initilise(int hlCount, int hnCount)
    {
        inputLayer.Clear();
        hiddenLayers.Clear();
        outputLayer.Clear();
        weights.Clear();
        biases.Clear();

        for (int i = 0; i < hlCount + 1; i++)
        {
            Matrix<float> f = Matrix<float>.Build.Dense(1, hnCount);

            hiddenLayers.Add(f);

            biases.Add(Random.Range(-1f, 1f));

            // Weights

            if (i == 0)
            {
                Matrix<float> inputToH1 = Matrix<float>.Build.Dense(10, hnCount);
                weights.Add(inputToH1);
            } 
         
            Matrix<float> hiddenToHidden = Matrix<float>.Build.Dense(hnCount, hnCount);
            weights.Add(hiddenToHidden);
        }

        Matrix<float> outputWeights = Matrix<float>.Build.Dense(hnCount, 2);
        weights.Add(outputWeights);
        biases.Add(Random.Range(-1f, 1f));

        RandomiseWeights();

    }

    public void RandomiseWeights()
    {
        for (int i = 0; i < weights.Count; i++)
        {
            for (int j = 0; j < weights[i].RowCount; j++)
            {
                for (int k = 0; k < weights[i].ColumnCount; k++)
                {
                    weights[i][j, k] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    public (float, float) RunNetwork(List<float> a)
    {
        for (int i = 0; i < 10; i++)
        {
            inputLayer[0, i] = a[i];
        }

        inputLayer = inputLayer.PointwiseTanh();

        hiddenLayers[0] = ((inputLayer * weights[0]) + biases[0]).PointwiseTanh();

        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ((hiddenLayers[i - 1] * weights[i]) + biases[i]).PointwiseTanh();
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[weights.Count - 1]) + biases[biases.Count - 1]).PointwiseTanh();

        return (outputLayer[0,0], outputLayer[0,1]);
    }
}
