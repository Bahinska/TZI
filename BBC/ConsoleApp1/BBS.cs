using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace ConsoleApp1
{
    public static class BBS
    {
        static void Main(string[] args)
        {
            int bitLength = 512;
            BigInteger p = GeneratePrime(bitLength);
            BigInteger q = GeneratePrime(bitLength);
            BigInteger n = p * q;

            BigInteger x = GenerateRandomNumber(n);

            string message = "Hello, world!";
            Console.WriteLine("Original Message: " + message);

            byte[] messageBytes = Encoding.UTF8.GetBytes(message);
            byte[] encryptedBytes = EncryptDecrypt(messageBytes, x, n);
            string encryptedMessage = Convert.ToBase64String(encryptedBytes);
            Console.WriteLine("Encrypted Message: " + encryptedMessage);

            byte[] decryptedBytes = EncryptDecrypt(encryptedBytes, x, n);
            string decryptedMessage = Encoding.UTF8.GetString(decryptedBytes);
            Console.WriteLine("Decrypted Message: " + decryptedMessage);
        }

        static BigInteger GeneratePrime(int bits)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[bits / 8];
            BigInteger number;

            do
            {
                rng.GetBytes(bytes);
                number = new BigInteger(bytes);
            }
            while (!number.IsProbablePrime(10));

            return number;
        }

        static BigInteger GenerateRandomNumber(BigInteger n)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] bytes = n.ToByteArray();
            BigInteger number;

            do
            {
                rng.GetBytes(bytes);
                number = new BigInteger(bytes);
            }
            while (number < 2 || number >= n);

            return number;
        }

        static byte[] EncryptDecrypt(byte[] data, BigInteger x, BigInteger n)
        {
            byte[] result = new byte[data.Length];
            BigInteger state = x;

            for (int i = 0; i < data.Length; i++)
            {
                state = BigInteger.ModPow(state, 2, n);
                byte mask = (byte)(state % 256);
                result[i] = (byte)(data[i] ^ mask);
            }

            return result;
        }

        static bool IsProbablePrime(this BigInteger source, int certainty)
        {
            if (source == 2 || source == 3)
                return true;
            if (source < 2 || source % 2 == 0)
                return false;

            BigInteger d = source - 1;
            int s = 0;

            while (d % 2 == 0)
            {
                d /= 2;
                s += 1;
            }

            Random rand = new Random();
            for (int i = 0; i < certainty; i++)
            {
                BigInteger a = RandomBigInteger(2, source - 2, rand);
                BigInteger x = BigInteger.ModPow(a, d, source);
                if (x == 1 || x == source - 1)
                    continue;

                for (int r = 1; r < s; r++)
                {
                    x = BigInteger.ModPow(x, 2, source);
                    if (x == 1)
                        return false;
                    if (x == source - 1)
                        break;
                }

                if (x != source - 1)
                    return false;
            }

            return true;
        }

        static BigInteger RandomBigInteger(BigInteger min, BigInteger max, Random rand)
        {
            BigInteger result;
            byte[] bytes = max.ToByteArray();
            do
            {
                rand.NextBytes(bytes);
                result = new BigInteger(bytes);
            }
            while (result < min || result > max);

            return result;
        }
    }
}
