using System.Text;

class LFSR
{
    static void Main(string[] args)
    {
        int state = 0b11001001;
        int length = 8;
        int steps = 256;

        StringBuilder keyStream = new StringBuilder();

        Console.WriteLine("Step\tState\t\tFeedback\tOutput Bit");
        if (state == 0)
        {
            Console.WriteLine("Error: Initial state must not be zero.");
            return;
        }

        for (int i = 0; i < steps; i++)
        {
            int outputBit = state & 1;
            int feedback = ComputeFeedback(state);
            Console.WriteLine($"{i + 1}\t{Convert.ToString(state, 2).PadLeft(length, '0')}\t{feedback}\t\t{outputBit}");

            state = (state >> 1) | (feedback << (length - 1));
            keyStream.Append(outputBit);
            if (state == 0)
            {
                Console.WriteLine("Error: The state has transitioned to all zeros.");
                break;
            }
        }

        string originalText = "Hello, World!";
        Console.WriteLine($"Original Text: {originalText}");

        string encryptedText = EncryptDecrypt(originalText, keyStream.ToString());
        Console.WriteLine($"Encrypted Text: {encryptedText}");

        string decryptedText = EncryptDecrypt(encryptedText, keyStream.ToString());
        Console.WriteLine($"Decrypted Text: {decryptedText}");
    }

    static int ComputeFeedback(int state)
    {
        int bit8 = (state >> 7) & 1;
        int bit4 = (state >> 3) & 1;
        int bit3 = (state >> 2) & 1;
        int bit2 = (state >> 1) & 1;

        return bit8 ^ bit4 ^ bit3 ^ bit2;
    }

    static string EncryptDecrypt(string text, string keyStream)
    {
        StringBuilder result = new StringBuilder();
        for (int i = 0; i < text.Length; i++)
        {
            char c = (char)(text[i] ^ (keyStream[i % keyStream.Length] - '0'));
            result.Append(c);
        }
        return result.ToString();
    }
}