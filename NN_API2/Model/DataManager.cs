namespace NN_API.Model;

public class DataManager
{
    public static SemaphoreSlim semData = new SemaphoreSlim(1);
    public static void AddData(string array)
    {
        string result = string.Empty;
        foreach (var c in array)
        {
            if (char.IsDigit(c))
            {
                result += c;
            }
        }

        semData.Wait();
        try
        {
            using StreamWriter writer = new StreamWriter(Directory.GetCurrentDirectory() + "\\data.txt", append: true);
            writer.Write("$" + result);
        }
        finally
        {
            semData.Release();
        }
    }
    
    public static (List<int>, List<List<int>>) GetData()
    {
        List<List<int>> array = new List<List<int>>();
        List<int> digits = new List<int>();

        semData.Wait();

        try
        {
            using StreamReader reader = new StreamReader(Directory.GetCurrentDirectory() + "\\data.txt");
            int charCode;
            bool nextIsDigit = false;
            int counter = -1;

            // Read character by character until end of file
            while ((charCode = reader.Read()) != -1)
            {
                char character = (char)charCode;

                if (character == '$')
                {
                    array.Add(new List<int>());
                    counter++;
                    nextIsDigit = true;
                }
                else if (nextIsDigit)
                {
                    digits.Add(character - 48);
                    nextIsDigit = false;
                }
                else
                {
                    array[counter].Add(character - 48);
                }
            }
        }
        finally
        {
            semData.Release();
        }

        return (digits, array);
    }
}