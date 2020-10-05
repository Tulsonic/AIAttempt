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

    public Matrix<float> outputLayer = Matrix<float>.Build.Dense(1, 4);

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

        Matrix<float> outputWeights = Matrix<float>.Build.Dense(hnCount, 4);
        weights.Add(outputWeights);
        biases.Add(Random.Range(-1f, 1f));

        RandomiseWeights();

    }

    public void RandomiseWeights()
    {
        for (int i = 0; i < weights.Count; i++)
        {
            for (int x = 0; x < weights[i].RowCount; x++)
            {
                for (int y = 0; y < weights[i].ColumnCount; y++)
                {
                    weights[i][x, y] = Random.Range(-1f, 1f);
                }
            }
        }
    }

    Matrix<float> ReluMat(Matrix<float> mat)
    {
        for (int x = 0; x < mat.RowCount; x++)
        {
            for (int y = 0; y < mat.ColumnCount; y++)
            {
                if (mat[x, y] >= 0)
                {
                    mat[x, y] = Mathf.Max(0, mat[x, y]);
                } else
                {
                    mat[x, y] = mat[x, y] * -0.01f;
                }
            }
        }

        return mat;
    }

    float Relu (float inp)
    {
        return Mathf.Min(Mathf.Max(0, inp), 0.25f);
    }

    float classifier (float inp)
    {
        if (inp >= 0)
        {
            return 1;
        } else
        {
            return -1;
        }
    }

    public (float, float, float, float) RunNetwork(List<float> a)
    {
        for (int i = 0; i < 10; i++)
        {
            inputLayer[0, i] = a[i];
        }

        inputLayer = inputLayer.PointwiseTanh();

        hiddenLayers[0] = ReluMat((inputLayer * weights[0]) + biases[0]);

        for (int i = 1; i < hiddenLayers.Count; i++)
        {
            hiddenLayers[i] = ReluMat((hiddenLayers[i - 1] * weights[i]) + biases[i]);
        }

        outputLayer = ((hiddenLayers[hiddenLayers.Count - 1] * weights[weights.Count - 1]) + biases[biases.Count - 1]);

        outputLayer[0, 0] = Relu(outputLayer[0, 0]);
        outputLayer[0, 1] = Relu(outputLayer[0, 1]);
        outputLayer[0, 2] = classifier(outputLayer[0, 2]);
        outputLayer[0, 3] = classifier(outputLayer[0, 3]);

        return (outputLayer[0,0], outputLayer[0,1], outputLayer[0, 2], outputLayer[0, 3]);
    }
}
