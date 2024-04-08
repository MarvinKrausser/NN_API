using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Enums;
using NeuralNetworkNET.APIs.Interfaces;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.APIs.Results;
using NeuralNetworkNET.APIs.Structs;
using NN_API.Model;
using SixLabors.ImageSharp.PixelFormats;

namespace NN_API2.Model;

public class NeuralNetworkManager
{
    private static INeuralNetwork network = CreateNn();
    private static SemaphoreSlim sem = new SemaphoreSlim(1);
    private static bool isTraining;
    public static bool Train()
    {
        if (isTraining) return false;
        sem.Wait();
        isTraining = true;

        try
        {
            var dataRaw = DataManager.GetData();

            List<(float[], float[])> data = new List<(float[], float[])>();

            for (int i = 0; i < dataRaw.Item1.Count; i++)
            {
                data.Add((dataRaw.Item2[i].Select(j => (float)j).ToArray(), OneHotEncode(dataRaw.Item1[i])));
            }

            Shuffle(data);
            
            Console.WriteLine(data.Count);

            ITrainingDataset dataset = DatasetLoader.Training(data, 100);

            TrainingSessionResult result = NetworkManager.TrainNetwork(
                network, // The network instance to train
                dataset, // The ITrainingDataset instance   
                TrainingAlgorithms.AdaDelta(), // The training algorithm to use
                60, // The expected number of training epochs to run
                0.5f, // Dropout probability
                null, // Optional callback to monitor the training dataset accuracy
                null); // Cancellation token for the training
            Console.WriteLine(result.StopReason);
        }
        finally
        {
            sem.Release(); 
            isTraining = false;
        }

        return true;
    }
    
    private static void Shuffle<T>(List<T> list)
    {
        Random rng = new Random();
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
    
    public static int Predicate(int[] input)
    {
        sem.Wait();
        try
        {
            return IndexOfMax(network.Forward(input.Select(i => (float)i).ToArray()));
        }
        finally
        {
            sem.Release();
        }
    }
    
    private static int IndexOfMax(float[] array)
    {
        if (array == null || array.Length == 0)
            throw new ArgumentException("Array is null or empty");

        float maxValue = array.Max();
        return Array.IndexOf(array, maxValue);
    }
    
    private static float[] OneHotEncode(int digit)
    {
        if (digit < 0 || digit > 9)
        {
            throw new ArgumentException("Digit must be between 0 and 9");
        }

        float[] encodedArray = new float[10];
        encodedArray[digit] = 1;
        return encodedArray;
    }

    private static INeuralNetwork CreateNn()
    {
        return NetworkManager.NewSequential(TensorInfo.Image<Alpha8>(30, 30),
            NetworkLayers.Convolutional((5, 5), 20, ActivationType.Identity),
            NetworkLayers.Pooling(ActivationType.LeakyReLU),
            NetworkLayers.FullyConnected(100, ActivationType.LeCunTanh),
            NetworkLayers.Softmax(10));
    }
}