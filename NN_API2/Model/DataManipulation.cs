namespace NN_API2.Model;

public class DataManipulation
{
    public static void MoveDigits(List<(float[], float[])> data)
    {
        int size = data.Count;
        for (int i = 0; i < size; i++)
        {
            for(int z = 0; z < 50; z++){
                float[,] square = ConvertToSquare2D(data[i].Item1);

                (int, int) direction = (new Random().Next(-1, 2), new Random().Next(-1, 2));

                float[,] newArray = (float[,])square.Clone();
                int distance;
                int moveDistance;

                switch (direction.Item1)
                {
                    case -1:
                        distance = GetNearestOneDistance(square, 0, 0, "left");

                        moveDistance = WeightedRandom(distance);

                        newArray = MoveArrayContent(square, "left", moveDistance);
                        break;
                    case 1:
                        distance = GetNearestOneDistance(square, 0, 0, "right");

                        moveDistance = WeightedRandom(distance);

                        newArray = MoveArrayContent(square, "right", moveDistance);
                        break;
                }

                switch (direction.Item1)
                {
                    case -1:
                        distance = GetNearestOneDistance(square, 0, 0, "top");

                        moveDistance = WeightedRandom(distance);

                        newArray = MoveArrayContent(square, "top", moveDistance);
                        break;
                    case 1:
                        distance = GetNearestOneDistance(square, 0, 0, "bottom");

                        moveDistance = WeightedRandom(distance);

                        newArray = MoveArrayContent(square, "bottom", moveDistance);
                        break;
                }

                data.Add((Flatten2DArray(newArray), data[i].Item2));
            }
        }
    }
    
    private static int WeightedRandom(int max)
    {
        // Generate a random number between 0 and the sum of numbers from 1 to max
        int totalWeight = max * (max + 1) / 2;
        int randomNumber = new Random().Next(totalWeight);

        // Calculate which number the random number corresponds to
        int sum = 0;
        for (int i = 1; i <= max; i++)
        {
            sum += i;
            if (randomNumber < sum)
            {
                return i;
            }
        }

        // This should never happen, but just in case
        return max;
    }
    
    private static float[] Flatten2DArray(float[,] array)
    {
        int numRows = array.GetLength(0);
        int numCols = array.GetLength(1);
        
        // Use LINQ to flatten the 2D array
        return Enumerable.Range(0, numRows)
            .SelectMany(row => Enumerable.Range(0, numCols)
                .Select(col => array[row, col]))
            .ToArray();
    }
    
    private static float[,] MoveArrayContent(float[,] array, string direction, int distance)
    {
        var newArray = new float[array.GetLength(0), array.GetLength(1)];

        for (int i = 0; i < newArray.GetLength(0); i++)
        {
            for (int j = 0; j < newArray.GetLength(1); j++)
            {
                newArray[i, j] = 0;
            }
        }
        
        switch (direction.ToLower())
        {
            case "top":
                for (int i = distance; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        newArray[i - distance, j] = array[i, j];
                    }
                }
                break;
            case "bottom":
                for (int i = 0; i < array.GetLength(0) - distance; i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        newArray[i + distance, j] = array[i, j];
                    }
                }
                break;
            case "left":
                for (int i = distance; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        newArray[j, i - distance] = array[j, i];
                    }
                }
                break;
            case "right":
                for (int i = 0; i < array.GetLength(0) - distance; i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        newArray[j, i + distance] = array[j, i];
                    }
                }
                break;
            default:
                throw new ArgumentException("Invalid direction.");
        }
        

        return newArray;
    }



    
    private static int GetNearestOneDistance(float[,] array, int startRow, int startCol, string direction)
    {
        int[,] directions = { { -1, 0 }, { 1, 0 }, { 0, -1 }, { 0, 1 } };
        int numRows = array.GetLength(0);
        int numCols = array.GetLength(1);

        // Check if the starting position is within the array bounds
        if (startRow < 0 || startRow >= numRows || startCol < 0 || startCol >= numCols)
            throw new ArgumentException("Invalid starting position.");

        int distance = 0;
        
        switch (direction.ToLower())
        {
            case "left":
                for (int i = 0; i < array.GetLength(1); i++)
                {
                    for (int j = 0; j < array.GetLength(0); j++)
                    {
                        if (array[j, i] == 1) return distance;
                    }

                    distance++;
                }
                break;
            case "right":
                for (int i = array.GetLength(1) - 1; i >= 0; i--)
                {
                    for (int j = 0; j < array.GetLength(0); j++)
                    {
                        if (array[j, i] == 1) return distance;
                    }

                    distance++;
                }
                break;
            case "top":
                for (int i = 0; i < array.GetLength(0); i++)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        if (array[i,j] == 1) return distance;
                    }

                    distance++;
                }
                break;
            case "bottom":
                for (int i = array.GetLength(0) - 1; i >= 0 ; i--)
                {
                    for (int j = 0; j < array.GetLength(1); j++)
                    {
                        if (array[i,j] == 1) return distance;
                    }

                    distance++;
                }
                break;
            default:
                throw new ArgumentException("Invalid direction.");
        }

        return -1;
    }
    
    private static T[,] ConvertToSquare2D<T>(T[] array)
    {
        int size = (int)Math.Sqrt(array.Length);
        if (size * size != array.Length)
        {
            throw new ArgumentException("Input array length is not a perfect square.");
        }

        T[,] squareArray = new T[size, size];
        for (int i = 0; i < array.Length; i++)
        {
            int row = i / size;
            int col = i % size;
            squareArray[row, col] = array[i];
        }

        return squareArray;
    }
}