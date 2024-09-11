using PolybiiAndHill;
class PolybiusSquare : IEncryption
{
    private char[,] square;
    private readonly string alphabet;

    public PolybiusSquare(string alphabet = null)
    {
        this.alphabet = alphabet ?? "ABCDEFGHIJKLMNOPQRSTUVWXYZ ";
    }

    char[,] GetSquare(string key)
    {
        var newAlphabet = alphabet;

        for (int i = 0; i < key.Length; i++)
        {
            newAlphabet = newAlphabet.Replace(key[i].ToString(), "");
        }

        newAlphabet = key + newAlphabet + "0123456789!@#$%^&*)_+-=<>?,.";

        var n = (int)Math.Ceiling(Math.Sqrt(alphabet.Length));

        square = new char[n, n];
        var index = 0;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                if (index < newAlphabet.Length)
                {
                    square[i, j] = newAlphabet[index];
                    index++;
                }
            }
        }

        return square;
    }

    bool FindSymbol(char[,] symbolsTable, char symbol, out int column, out int row)
    {
        var l = symbolsTable.GetUpperBound(0) + 1;
        for (int i = 0; i < l; i++)
        {
            for (int j = 0; j < l; j++)
            {
                if (symbolsTable[i, j] == symbol)
                {
                    row = i;
                    column = j;
                    return true;
                }
            }
        }

        row = -1;
        column = -1;
        return false;
    }

    public string Encrypt(string text, string password)
    {
        var square = GetSquare(password);
        var m = text.Length;
        var coordinates = new int[m * 2];
        for (int i = 0; i < m; i++)
        {
            if (FindSymbol(square, text[i], out int columnIndex, out int rowIndex))
            {
                coordinates[i] = columnIndex;
                coordinates[i + m] = rowIndex;
            }
        }

        for (int shift = 0; shift < 1; shift++)
        {
            coordinates = ShiftCoordinates(coordinates, m);
        }

        var outputText = "";
        for (int i = 0; i < m * 2; i += 2)
        {
            outputText += square[coordinates[i + 1], coordinates[i]];
        }

        return outputText;
    }

    public string Decrypt(string text, string password)
    {
        var square = GetSquare(password);
        var m = text.Length;
        var coordinates = new int[m * 2];
        int j = 0;
        for (int i = 0; i < m; i++)
        {
            if (FindSymbol(square, text[i], out int columnIndex, out int rowIndex))
            {
                coordinates[j] = columnIndex;
                coordinates[j + 1] = rowIndex;
                j += 2;
            }
        }

        for (int shift = 0; shift < 1; shift++)
        {
            coordinates = ReverseShiftCoordinates(coordinates, m);
        }

        var outputText = "";
        for (int i = 0; i < m; i++)
        {
            outputText += square[coordinates[i + m], coordinates[i]];
        }

        return outputText;
    }

    public int[] ShiftCoordinates(int[] coordinates, int length)
    {
        var elemnt = coordinates[0];
        coordinates = coordinates.Skip(1).Append(elemnt).ToArray();
        return coordinates;
    }

    public int[] ReverseShiftCoordinates(int[] coordinates, int length)
    {
        var elemnt = coordinates[^1];
        coordinates = coordinates.Prepend(elemnt).Take(coordinates.Length).ToArray();
        return coordinates;
    }
}