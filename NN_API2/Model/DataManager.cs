using System.Security.Cryptography;
using Newtonsoft.Json;

namespace NN_API.Model;

public class DataManager
{
    private static SemaphoreSlim semData = new SemaphoreSlim(1);
    public static bool stop { private set; get; }
    private static byte[] key = { 0x15, 0xE2, 0xB0, 0xD3, 0xC3, 0x38, 0x91, 0xEB, 0xB0, 0xF1, 0xEF, 0x60, 0x9E, 0xC4, 0x19, 0x42, 0x0C, 0x20, 0xE3, 0x20, 0xCE, 0x94, 0xC6, 0x5F, 0xBC, 0x8C, 0x33, 0x12, 0x44, 0x8E, 0xB2, 0x25 };
    
    public static bool CheckKey(string input)
    {
        using SHA256 sha256 = SHA256.Create();
        byte[] inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = sha256.ComputeHash(inputBytes);

        if (key.Length != hashBytes.Length) return false;
        
        for (int i = 0; i < key.Length; i++)
        {
            if (hashBytes[i] != key[i]) return false;
        }
        
        return true;
    }
    public static string AddData((int, int[]) input)
    {
        if (stop) return "Dataflow has been stopped";

        semData.Wait();
        try
        {
            string result;
            var newArray = new List <(int, int[])>();
            if(File.Exists(Directory.GetCurrentDirectory() + "/data.txt"))
            {
                using StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/data.txt");
                result = reader.ReadToEnd();
                newArray = JsonConvert.DeserializeObject<List<(int, int[])>>(result);
            }
            
            newArray.Add(input);
            var array = JsonConvert.SerializeObject(newArray);
            
            using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/data.txt", append: false))
            {
                writer.Write(array);
            }
        }
        finally
        {
            semData.Release();
        }

        return "Success";
    }
    
    public static List<(int, int[])> GetData()
    {
        semData.Wait();

        try
        {
            if (!File.Exists(Directory.GetCurrentDirectory() + "/data.txt"))
            {
                return new List<(int, int[])>();
            }
            using StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/data.txt");
            return JsonConvert.DeserializeObject<List<(int, int[])>>(reader.ReadToEnd());
        }
        finally
        {
            semData.Release();
        }
    }

    public static string ChangeStop(bool b, string key)
    {
        if (!CheckKey(key)) return "Wrong Key";
        stop = b;
        return "Success";
    }

    public static string Delete(int index,string key)
    {
        if (!CheckKey(key)) return "Wrong Key";
        if (!stop) return "Stop Dataflow for Datamanipulation";

        semData.Wait();
        try
        {
            string result;
            using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/data.txt"))
            {
                result = reader.ReadToEnd();
            }

            var newArray = JsonConvert.DeserializeObject<List<(int, int[])>>(result);
            if (index >= newArray.Count ||index < 0) return "Index not correct";
            newArray.RemoveAt(index);
            var array = JsonConvert.SerializeObject(newArray);
            
            using (StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "/data.txt", append: false))
            {
                writer.Write(array);
            }
        }
        finally
        {
            semData.Release();
        }

        return "Success";
    }
}