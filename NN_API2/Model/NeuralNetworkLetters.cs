using NeuralNetworkNET.APIs;
using NeuralNetworkNET.APIs.Enums;
using NeuralNetworkNET.APIs.Interfaces;
using NeuralNetworkNET.APIs.Interfaces.Data;
using NeuralNetworkNET.APIs.Results;
using NeuralNetworkNET.APIs.Structs;
using Newtonsoft.Json;
using NN_API.Model;
using SixLabors.ImageSharp.PixelFormats;

namespace NN_API2.Model;

public class NeuralNetworkDigits
{
    private static INeuralNetwork network = CreateNn();
    private static SemaphoreSlim sem = new (1);
    private static bool isTraining;
    private static string[] charTable = ReadTable();

    private static string[] ReadTable()
    {
        using StreamReader sr = new StreamReader(Directory.GetCurrentDirectory() + "/characters.json");

        return JsonConvert.DeserializeObject<string[]>(sr.ReadToEnd());
    }

    public static bool Train()
    {
        if (isTraining) return false;
        sem.Wait();
        isTraining = true;

        try
        {
            var dataRaw = DataManager.GetData();

            List<(float[], float[])> data = new List<(float[], float[])>();

            for (int i = 0; i < dataRaw.Count; i++)
            {
                float[] target = OneHotEncode(dataRaw[i].Item1);
                float[] input = dataRaw[i].Item2.Select(x => (float)x).ToArray();
                data.Add((input, target));
            }
            
            //DataManipulation.MoveDigits(data);

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
    
    public static string Predicate(int[] input)
    {
        sem.Wait();
        try
        {
            return charTable[IndexOfMax(network.Forward(input.Select(i => (float)i).ToArray()))];
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
        if (digit < 0 || digit > charTable.Length - 1)
        {
            throw new ArgumentException("Digit must be between 0 and 61");
        }

        float[] encodedArray = new float[charTable.Length];
        encodedArray[digit] = 1;
        return encodedArray;
    }

    private static INeuralNetwork CreateNn()
    {
        return NetworkManager.NewSequential(TensorInfo.Image<Alpha8>(30, 30),
            NetworkLayers.Convolutional((5, 5), 20, ActivationType.Identity),
            NetworkLayers.Pooling(ActivationType.LeakyReLU),
            NetworkLayers.FullyConnected(100, ActivationType.LeCunTanh),
            NetworkLayers.Softmax(charTable.Length));
    }
}