using System.Numerics;
using System.Security.Cryptography;
using System.Text;

class DigitalSignatureElGamal
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("Виберіть опцію:");
        Console.WriteLine("1. Створити цифровий підпис");
        Console.WriteLine("2. Перевірити цифровий підпис");
        int choice = int.Parse(Console.ReadLine());

        if (choice == 1)
        {
            CreateDigitalSignature();
        }
        else if (choice == 2)
        {
            VerifyDigitalSignature();
        }
        else
        {
            Console.WriteLine("Невірний вибір");
        }
    }

    static void CreateDigitalSignature()
    {
        BigInteger p = GenerateLargePrime(1024);
        BigInteger g = FindPrimitiveRoot(p);

        // Генерація закритого ключа x та обчислення h
        BigInteger x = GeneratePrivateKey(p);
        BigInteger h = BigInteger.ModPow(g, x, p);

        // Друк ключів
        Console.WriteLine("Public Key: p = " + p + ", g = " + g + ", h = " + h);
        Console.WriteLine("Private Key: x = " + x);

        // Зберігання відкритого ключа у файл
        string publicKeyFilePath = "public_key.txt";
        File.WriteAllText(publicKeyFilePath, $"{p} {g} {h}");

        // Читання повідомлення з файлу
        string messageFilePath = "message.txt";
        string message = File.ReadAllText(messageFilePath);

        // Обчислення хешу повідомлення
        byte[] messageHash = ComputeHash(message);

        // Обчислення цифрового підпису
        (BigInteger r, BigInteger s) signature = GenerateSignature(messageHash, p, g, x);

        // Записання підпису у файл
        string signatureFilePath = "signature.txt";
        File.WriteAllText(signatureFilePath, $"{signature.r} {signature.s}");

        Console.WriteLine("Цифровий підпис згенеровано та збережено у " + signatureFilePath);
    }

    static void VerifyDigitalSignature()
    {
        // Читання відкритого ключа з файлу
        string[] publicKey = File.ReadAllText("public_key.txt").Split(' ');
        BigInteger p = BigInteger.Parse(publicKey[0]);
        BigInteger g = BigInteger.Parse(publicKey[1]);
        BigInteger h = BigInteger.Parse(publicKey[2]);

        // Читання повідомлення з файлу
        string messageFilePath = "message.txt";
        string message = File.ReadAllText(messageFilePath);

        // Обчислення хешу повідомлення
        byte[] messageHash = ComputeHash(message);

        // Читання підпису з файлу
        string[] signature = File.ReadAllText("signature.txt").Split(' ');
        BigInteger r = BigInteger.Parse(signature[0]);
        BigInteger s = BigInteger.Parse(signature[1]);

        // Перевірка підпису
        bool isValid = VerifySignature(messageHash, r, s, p, g, h);

        Console.WriteLine("Цифровий підпис дійсний: " + isValid);
    }

    static BigInteger GenerateLargePrime(int bitLength)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[bitLength / 8];
            BigInteger prime;

            do
            {
                rng.GetBytes(bytes);
                prime = new BigInteger(bytes);
                prime = BigInteger.Abs(prime);
            } while (!IsProbablyPrime(prime));

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

    static BigInteger FindPrimitiveRoot(BigInteger p)
    {
        for (BigInteger g = 2; g < p; g++)
        {
            if (IsPrimitiveRoot(g, p))
            {
                return g;
            }
        }
        throw new Exception("Не вдалося знайти первісний корінь для p");
    }

    static bool IsPrimitiveRoot(BigInteger g, BigInteger p)
    {
        if (BigInteger.ModPow(g, (p - 1) / 2, p) == 1)
        {
            return false;
        }

        if (BigInteger.ModPow(g, p - 1, p) != 1)
        {
            return false;
        }

        return true;
    }

    static BigInteger GeneratePrivateKey(BigInteger p)
    {
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[p.ToByteArray().Length];
            BigInteger x;
            do
            {
                rng.GetBytes(bytes);
                x = new BigInteger(bytes);
                x = BigInteger.Abs(x);
            } while (x < 1 || x >= p);

            return x;
        }
    }

    static byte[] ComputeHash(string message)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            return sha256.ComputeHash(Encoding.UTF8.GetBytes(message));
        }
    }

    static (BigInteger, BigInteger) GenerateSignature(byte[] hash, BigInteger p, BigInteger g, BigInteger x)
    {
        BigInteger k, r, s;
        BigInteger hashValue = new BigInteger(hash);
        do
        {
            do
            {
                k = RandomBigInteger(1, p - 1);
            } while (BigInteger.GreatestCommonDivisor(k, p - 1) != 1);

            r = BigInteger.ModPow(g, k, p);
            s = ((hashValue - x * r) * ModInverse(k, p - 1)) % (p - 1);
            if (s < 0)
            {
                s += (p - 1);
            }
        } while (s == 0);

        return (r, s);
    }

    static BigInteger ModInverse(BigInteger a, BigInteger m)
    {
        BigInteger m0 = m, t, q;
        BigInteger x0 = 0, x1 = 1;

        if (m == 1)
            return 0;

        while (a > 1)
        {
            if (m == 0) // Перевірка ділення на нуль
                throw new DivideByZeroException("Divide by zero occurred in ModInverse computation.");

            q = a / m;
            t = m;
            m = a % m;
            a = t;
            t = x0;
            x0 = x1 - q * x0;
            x1 = t;
        }

        if (x1 < 0)
            x1 += m0;

        return x1;
    }

    static bool VerifySignature(byte[] hash, BigInteger r, BigInteger s, BigInteger p, BigInteger g, BigInteger h)
    {
        if (r <= 0 || r >= p || s <= 0 || s >= p - 1) return false;

        BigInteger hashValue = new BigInteger(hash);
        BigInteger v1 = BigInteger.ModPow(h, r, p) * BigInteger.ModPow(r, s, p) % p;
        BigInteger v2 = BigInteger.ModPow(g, hashValue, p) % p;

        return v1 == v2;
    }
}