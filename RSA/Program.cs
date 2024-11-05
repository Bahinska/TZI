using System.Numerics;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        BigInteger p = GeneratePrime(1024);
        BigInteger q = GeneratePrime(1024);

        BigInteger n = p * q;
        BigInteger phi = (p - 1) * (q - 1);

        BigInteger e = ChooseE(phi);

        BigInteger d = ModInverse(e, phi);

        Console.WriteLine("Public Key: n = " + n + ", e = " + e);
        Console.WriteLine("Private Key: d = " + d);

        string inputFilePath = "open_message.txt";
        string openMessage = File.ReadAllText(inputFilePath);

        BigInteger[] encryptedMessage = Encrypt(openMessage, e, n);

        string encryptedFilePath = "encrypted_message.txt";
        File.WriteAllText(encryptedFilePath, string.Join(" ", encryptedMessage));

        Console.WriteLine("Shifting complete, result saved to " + encryptedFilePath);

        string[] encryptedData = File.ReadAllText(encryptedFilePath).Split(' ');
        BigInteger[] encryptedBigInts = Array.ConvertAll(encryptedData, BigInteger.Parse);
        string decryptedMessage = Decrypt(encryptedBigInts, d, n);

        string decryptedFilePath = "decrypted_message.txt";
        File.WriteAllText(decryptedFilePath, decryptedMessage);

        Console.WriteLine("Decryption complete, result saved to " + decryptedFilePath);
    }

    static BigInteger GeneratePrime(int bitLength)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] bytes = new byte[bitLength / 8];
            rng.GetBytes(bytes);
            BigInteger prime = new BigInteger(bytes);

            prime = BigInteger.Abs(prime);
            prime |= BigInteger.One << (bitLength - 1);

            while (!IsProbablyPrime(prime))
            {
                prime++;
            }

            return prime;
        }
    }

    static bool IsProbablyPrime(BigInteger n, int k = 10)
    {
        if (n <= 1) return false;
        if (n == 2 || n == 3) return true;
        if (n % 2 == 0) return false;

        BigInteger d = n - 1;
        int r = 0;

        while (d % 2 == 0)
        {
            d /= 2;
            r++;
        }

        for (int i = 0; i < k; i++)
        {
            BigInteger a = 2 + (BigInteger.ModPow(RandomBigInteger(2, n - 2), 1, n));
            BigInteger x = BigInteger.ModPow(a, d, n);

            if (x == 1 || x == n - 1) continue;

            bool isComposite = true;
            for (int j = 0; j < r - 1; j++)
            {
                x = BigInteger.ModPow(x, 2, n);
                if (x == n - 1)
                {
                    isComposite = false;
                    break;
                }
            }

            if (isComposite) return false;
        }

        return true;
    }

    static BigInteger RandomBigInteger(BigInteger min, BigInteger max)
    {
        Random random = new Random();
        byte[] bytes = max.ToByteArray();
        BigInteger result;

        do
        {
            random.NextBytes(bytes);
            result = new BigInteger(bytes);
        } while (result < min || result > max);

        return result;
    }

    static BigInteger ChooseE(BigInteger phi)
    {
        BigInteger e = 65537;
        if (BigInteger.GreatestCommonDivisor(e, phi) == 1)
        {
            return e;
        }

        for (e = 3; e < phi; e += 2)
        {
            if (BigInteger.GreatestCommonDivisor(e, phi) == 1)
            {
                return e;
            }
        }

        throw new Exception("Не вдалося знайти відповідне e.");
    }

    static BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        BigInteger m0 = m, t, q;
        BigInteger x0 = 0, x1 = 1;

        if (m == 1) return 0;

        while (a > 1)
        {
            q = a / m;
            t = m;
            m = a % m;
            a = t;
            t = x0;
            x0 = x1 - q * x0;
            x1 = t;
        }

        if (x1 < 0) x1 += m0;

        return x1;
    }

    static BigInteger[] Encrypt(string message, BigInteger e, BigInteger n)
    {
        byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(message);
        BigInteger[] encrypted = new BigInteger[messageBytes.Length];

        for (int i = 0; i < messageBytes.Length; i++)
        {
            encrypted[i] = BigInteger.ModPow(messageBytes[i], e, n);
        }

        return encrypted;
    }

    static string Decrypt(BigInteger[] encryptedMessage, BigInteger d, BigInteger n)
    {
        byte[] decryptedBytes = new byte[encryptedMessage.Length];

        for (int i = 0; i < encryptedMessage.Length; i++)
        {
            decryptedBytes[i] = (byte)BigInteger.ModPow(encryptedMessage[i], d, n);
        }

        return System.Text.Encoding.UTF8.GetString(decryptedBytes);
    }
}