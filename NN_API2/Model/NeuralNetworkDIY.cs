using System;

class NeuralNetwork
{
    private readonly int inputSize;
    private readonly int hiddenSize;
    private readonly int outputSize;
    private readonly double[,] weightsInputHidden;
    private readonly double[,] weightsHiddenOutput;

    public NeuralNetwork(int inputSize, int hiddenSize, int outputSize)
    {
        this.inputSize = inputSize;
        this.hiddenSize = hiddenSize;
        this.outputSize = outputSize;

        // Initialize weights randomly
        weightsInputHidden = InitializeWeights(inputSize, hiddenSize);
        weightsHiddenOutput = InitializeWeights(hiddenSize, outputSize);
    }

    private double[,] InitializeWeights(int rows, int cols)
    {
        var random = new Random();
        var weights = new double[rows, cols];
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                weights[i, j] = random.NextDouble() * 2 - 1; // Random values between -1 and 1
            }
        }
        return weights;
    }

    public double[] Forward(double[] inputs)
    {
        // Forward pass from input to hidden layer
        double[] hiddenOutputs = new double[hiddenSize];
        for (int i = 0; i < hiddenSize; i++)
        {
            double sum = 0;
            for (int j = 0; j < inputSize; j++)
            {
                sum += inputs[j] * weightsInputHidden[j, i];
            }
            hiddenOutputs[i] = Math.Tanh(sum);
        }

        // Forward pass from hidden to output layer
        double[] outputs = new double[outputSize];
        for (int i = 0; i < outputSize; i++)
        {
            double sum = 0;
            for (int j = 0; j < hiddenSize; j++)
            {
                sum += hiddenOutputs[j] * weightsHiddenOutput[j, i];
            }
            outputs[i] = Math.Tanh(sum);
        }

        return outputs;
    }
}