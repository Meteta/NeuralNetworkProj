// using System;
// using System.Collections.Generic;
// using UnityEngine;

// public class NerualNetwork
// {
//     private int[] layers;
//     private float[][] neurons;
//     private float[][][] weights;

//     /// <summary>
//     /// Initializes neural network with random weights
//     /// </summary>
//     /// <param name="layers"> layers in the neural network </param>
//     public NerualNetwork(int[] layers)
//     {
//         this.layers = new int[layers.Length];
//         for (int i = 0; i < layers.Length; i++)
//         {
//             this.layers[i] = layers[i];
//         }

//         InitNeurons();
//         InitWeights();
//     }

//     /// <summary>
//     /// Deep copy constructor.
//     /// </summary>
//     /// <param name="copyNetwork"> Network to deep copy</param>
//     public NerualNetwork(NerualNetwork copyNetwork)
//     {
//         this.layers = new int[copyNetwork.layers.Length];
//         for (int i = 0; i < copyNetwork.layers.Length; i++){
//             this.layers[i] = copyNetwork.layers[i];
//         }

//         InitNeurons();
//         InitWeights();
//         CopyWeights(copyNetwork.weights);
//     }

//     private void CopyWeights (float[][][] copyWeights)
//     {
//          for (int i = 0; i < weights.Length; i++)
//         {
//             for (int j = 0; j < weights[i].Length; j++ )
//             {
//                 for (int k = 0; k < weights[i][j].Length; k++){
//                     weights[i][j][k] = copyWeights[i][j][k];
//                 }
//             }
//         }
//     }

//     /// <summary>
//     /// Creates and Initializes neuron Matrix 
//     /// </summary>
//     private void InitNeurons()
//     {
//         //Create a list, then convert the list into a Jagged array
//         List<float[]> neuronsList = new List<float[]>();

//         for (int i = 0; i < layers.Length; i++)
//         {
//             neuronsList.Add(new float[layers[i]]);
//         }

//         neurons = neuronsList.ToArray();
//     }

//     /// <summary>
//     /// Creates and Initializes weight Matrix 
//     /// </summary>
//     private void InitWeights()
//     {
//         List<float[][]> weightsList = new List<float[][]>();

//         //Each layer needs its own weight matrix
//         for (int i = 1; i < layers.Length; i++) 
//         {
//             List<float[]> layerWeightsList = new List<float[]>();

//             int neuronsPrevLayer = layers[i - 1];

//             for (int j = 0; j < neurons[i].Length; j++)
//             {
//                 float[] neuronWeights = new float[neuronsPrevLayer];

//                 //Randomly set weights between given values
//                 for(int k = 0; k < neuronsPrevLayer; k++ )
//                 {
//                     //Give random weights to Neuron between given values
//                     neuronWeights[k] = UnityEngine.Random.Range(-0.5f,0.5f);
//                 }
//                 layerWeightsList.Add(neuronWeights);
//             }

//             weightsList.Add(layerWeightsList.ToArray());
//         }

//         weights = weightsList.ToArray();
//     }

//     /// <summary>
//     /// Feed forward this neural network with a given input array
//     /// Look at Feedforward Neural Network on Wiki (FNN)
//     /// </summary>
//     /// <param name="inputs"> Inputs to network </param>
//     public float[] Feedfoward(float[] inputs)
//     {
//         for (int i = 0; i < inputs.Length; i++)
//         {
//             neurons[0][i] = inputs[i];
//         }

//         // Iterate over every layer starting from the second layer
//         for (int i = 1; i < layers.Length; i++)
//         {
//             //Iterate over every neuron in each of these layers
//             for (int j = 0; j < neurons[i].Length; j++)
//             {
//                 float value = 0.25f; //Constant bias (I believe this is replacing the bias neuron. Not sure tho)

//                 //Iterate over every neuron in previous layer
//                 for (int k = 0; k < neurons[i-1].Length; k++)
//                 {
//                     value += weights[i-1][j][k]* neurons[i-1][k];
//                 }

//                 neurons[i][j] = (float)Math.Tanh(value);
//             }
//         }

//         //Return output layer (the last layer)
//         return neurons[neurons.Length - 1];
//     }


//     // Aesexual Mutation Woooooooooooo
//     // TODO: Replace with sexual reproduction. This will allow certain traits to be gained or lost based on who else is still alive.
//     ///<summary>
//     /// Mutate neural network weights
//     ///</summary>
//     public void Mutate()
//     {
//         for (int i = 0; i < weights.Length; i++)
//         {
//             for (int j = 0; j < weights[i].Length; j++ )
//             {
//                 for (int k = 0; k < weights[i][j].Length; k++){
//                     float weight = weights[i][j][k];

//                     // Mutate the weight value
//                     // Also coincidentally, the mutation chance when this method is called. 8% chance of mutation.
//                     float randomNumber = UnityEngine.Random.Range(0f,100f);

//                     // Several types of Mutations that can occur to the Neural Network
//                     if(randomNumber <= 2f){
//                         weight *= -1f; //flip sign of weight
//                     } else if( randomNumber <= 4f){
//                         weight = UnityEngine.Random.Range(-0.5f,0.5f); //pick rand between -1 and 1
//                     } else if( randomNumber <= 6f){
//                         float factor = UnityEngine.Random.Range(0f,1f) + 1f; //randomly increase by 0% to 100%
//                         weight *= factor;
//                     } else if( randomNumber <= 8f){
//                         float factor = UnityEngine.Random.Range(0f,1f); //randomly decrease by 0% to 100%
//                         weight *= factor;
//                     }

//                     weights[i][j][k] = weight;
//                 }
//             }
//         }
//     }
// }

// Code Below by John Sorrentino
// To be used by Actors

using UnityEngine;

public class NerualNetwork : MonoBehaviour
{
    public Layer [] layers;
    // Defines the shape of the neural network.
    public int [] networkShape = {2,4,4,2};

    public class Layer
    {
        public float[,] weightsArray;
        public float[] biasesArray;
        public float[] nodeArray;

        private int n_nodes;
        private int n_inputs;

        public Layer(int n_inputs, int n_nodes)
        {
            this.n_nodes = n_nodes;
            this.n_inputs = n_inputs;

            weightsArray = new float [n_nodes, n_inputs];
            biasesArray = new float [n_nodes];
            nodeArray = new float [n_nodes];
        }

        public void Forward(float [] inputsArray)
        {
            nodeArray = new float [n_nodes];

            for(int i = 0;i < n_nodes ; i++)
            {
                //sum of weights times inputs
                for(int j = 0; j < n_inputs; j++)
                {
                    nodeArray[i] += weightsArray[i,j] * inputsArray[j];
                }

                //add the bias
                nodeArray[i] += biasesArray[i];
            }
        }

        public void Activation()
        {
            for(int i = 0; i < n_nodes; i++)
            {
                if(nodeArray[i] < 0)
                {
                    nodeArray[i] = 0;
                }
            }
        }
    }

    public void Awake()
    {
        layers = new Layer[networkShape.Length - 1];
        for(int i = 0; i < layers.Length; i++)
        {
            layers[i] = new Layer(networkShape[i], networkShape[i + 1]);
        }

    }

    public float[] Brain(float [] inputs)
    {
        for(int i = 0; i < layers.Length; i++)
        {
            if(i == 0)
            {
                layers[i].Forward(inputs);
                layers[i].Activation();
            } 
            else if(i == layers.Length - 1)
            {
                layers[i].Forward(layers[i - 1].nodeArray);
            }
            else
            {
                layers[i].Forward(layers[i - 1].nodeArray);
                layers[i].Activation();
            }    
        }

        return(layers[layers.Length - 1].nodeArray);
    }
}