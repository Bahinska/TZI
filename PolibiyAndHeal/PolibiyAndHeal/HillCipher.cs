
using PolybiiAndHill;

class HillCipher : IEncryption
{
    private static char[] _alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
    private static int _mod = _alphabet.Length;

    public static int CharToIndex(char character) => Array.IndexOf(_alphabet, character);

    public static char IndexToChar(int index)
    {
        index = index % _mod;
        if (index < 0) index += _mod; // Ensure that the index is non-negative
        return _alphabet[index];
    }

    public static int[,] GenerateKeyMatrix(string keyword)
    {
        int n = (int)Math.Sqrt(keyword.Length);
        if (n * n != keyword.Length)
            throw new Exception("Довжина ключового слова має бути квадратом цілого числа.");

        int[,] keyMatrix = new int[n, n];
        for (int i = 0; i < keyword.Length; i++)
        {
            keyMatrix[i / n, i % n] = CharToIndex(keyword[i]);
        }

        return keyMatrix;
    }

    public static int ModInverse(int a, int m)
    {
        a = a % m;
        for (int x = 1; x < m; x++)
        {
            if ((a * x) % m == 1)
            {
                return x;
            }
        }
        return -1;
    }

    public static int GetDeterminant(int[,] matrix, int modulus)
    {
        int n = matrix.GetLength(0);

        if (n == 1)
        {
            return matrix[0, 0];
        }

        if (n == 2)
        {
            return ((matrix[0, 0] * matrix[1, 1]) - (matrix[0, 1] * matrix[1, 0])) % modulus;
        }

        int determinant = 0;
        for (int c = 0; c < n; c++)
        {
            int[,] subMatrix = new int[n - 1, n - 1];
            for (int i = 1; i < n; i++)
            {
                int subMatrixCol = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == c)
                    {
                        continue;
                    }
                    subMatrix[i - 1, subMatrixCol] = matrix[i, j];
                    subMatrixCol++;
                }
            }
            determinant = (determinant + (int)Math.Pow(-1, c) * matrix[0, c] * GetDeterminant(subMatrix, modulus)) % modulus;
        }

        if (determinant < 0)
        {
            determinant = (determinant + modulus) % modulus;
        }

        return determinant;
    }

    public static int[,] AdjugateMatrix(int[,] matrix, int modulus)
    {
        int n = matrix.GetLength(0);
        int[,] adjugate = new int[n, n];

        if (n == 1)
        {
            adjugate[0, 0] = 1;
            return adjugate;
        }

        int sign = 1;
        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                int[,] subMatrix = new int[n - 1, n - 1];
                int subi = 0;
                for (int x = 0; x < n; x++)
                {
                    if (x == i)
                    {
                        continue;
                    }

                    int subj = 0;
                    for (int y = 0; y < n; y++)
                    {
                        if (y == j)
                        {
                            continue;
                        }

                        subMatrix[subi, subj] = matrix[x, y];
                        subj++;
                    }

                    subi++;
                }

                adjugate[j, i] = (sign * GetDeterminant(subMatrix, modulus) % modulus + modulus) % modulus;
                sign = -sign;
            }
        }

        return adjugate;
    }

    public static int[,] InvertMatrix(int[,] matrix, int modulus)
    {
        int n = matrix.GetLength(0);
        int det = GetDeterminant(matrix, modulus);
        int invDet = ModInverse(det, modulus);

        if (invDet == -1)
        {
            throw new Exception("Матриця не має оберненої.");
        }

        int[,] adjugate = AdjugateMatrix(matrix, modulus);
        int[,] inverse = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                inverse[i, j] = (adjugate[i, j] * invDet) % modulus;
                if (inverse[i, j] < 0)
                {
                    inverse[i, j] += modulus;
                }
            }
        }

        return inverse;
    }

    public static List<int[]> DivideTextIntoBlocks(string text, int blockSize)
    {
        List<int[]> blocks = new List<int[]>();
        int[] block = new int[blockSize];
        int blockIndex = 0;

        foreach (char c in text)
        {
            block[blockIndex++] = CharToIndex(c);
            if (blockIndex == blockSize)
            {
                blocks.Add(block);
                block = new int[blockSize];
                blockIndex = 0;
            }
        }

        if (blockIndex > 0)
            blocks.Add(block);

        return blocks;
    }

    public string Encrypt(string text, string pass)
    {
        int[,] keyMatrix = GenerateKeyMatrix(pass);
        int n = keyMatrix.GetLength(0);
        var blocks = DivideTextIntoBlocks(text, n);

        List<char> encryptedText = new List<char>();

        foreach (var block in blocks)
        {
            int[] encryptedBlock = new int[n];
            for (int i = 0; i < n; i++)
            {
                int sum = 0;
                for (int j = 0; j < n; j++)
                {
                    sum += keyMatrix[i, j] * block[j];
                }
                encryptedBlock[i] = sum % _mod;
            }
            encryptedText.AddRange(encryptedBlock.Select(IndexToChar));
        }

        return new string(encryptedText.ToArray());
    }

    public string Decrypt(string text, string pass)
    {
        int[,] keyMatrix = GenerateKeyMatrix(pass);
        int[,] invKeyMatrix = InvertMatrix(keyMatrix, _mod);
        int n = invKeyMatrix.GetLength(0);
        var blocks = DivideTextIntoBlocks(text, n);

        List<char> decryptedText = new List<char>();

        foreach (var block in blocks)
        {
            int[] decryptedBlock = new int[n];
            for (int i = 0; i < n; i++)
            {
                int sum = 0;
                for (int j = 0; j < n; j++)
                {
                    sum += invKeyMatrix[i, j] * block[j];
                }
                decryptedBlock[i] = sum % _mod;
            }
            decryptedText.AddRange(decryptedBlock.Select(IndexToChar));
        }

        return new string(decryptedText.ToArray());
    }
}