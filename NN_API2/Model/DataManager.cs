using Newtonsoft.Json;

namespace NN_API.Model;

public class DataManager
{
    public static SemaphoreSlim semData = new SemaphoreSlim(1);
    public static void AddData((int, int[]) input)
    {
        

        semData.Wait();
        try
        {
            string result;
            using (StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/data.txt"))
            {
                result = reader.ReadToEnd();
            }

            var newArray = JsonConvert.DeserializeObject<List<(int, int[])>>(result);
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
    }
    
    public static List<(int, int[])> GetData()
    {

        semData.Wait();

        try
        {
            using StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "/data.txt");
            return JsonConvert.DeserializeObject<List<(int, int[])>>(reader.ReadToEnd());
        }
        finally
        {
            semData.Release();
        }
    }
}