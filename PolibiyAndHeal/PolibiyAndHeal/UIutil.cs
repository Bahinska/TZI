namespace PolybiiAndHill
{
    public static class UIutil
    {
        public static void Start(IEncryption encryption)
        {
            while (true)
            {
                Console.WriteLine("Виберіть дію:");
                Console.WriteLine("1. Зашифрувати текст");
                Console.WriteLine("2. Розшифрувати текст");
                Console.WriteLine("3. Вихід");
                Console.Write("Ваш вибір: ");
                var choice = Console.ReadLine();

                if (choice == "3")
                    break;

                Console.WriteLine("\nВиберіть спосіб вводу:");
                Console.WriteLine("1. З консолі");
                Console.WriteLine("2. З файлу");
                Console.Write("Ваш вибір: ");
                var inputChoice = Console.ReadLine();

                string text = "";
                if (inputChoice == "1")
                {
                    Console.Write("\nВведіть текст: ");
                    text = Console.ReadLine().ToUpper();
                }
                else if (inputChoice == "2")
                {
                    Console.Write("\nВведіть шлях до файлу: ");
                    var filePath = Console.ReadLine();
                    text = File.ReadAllText(filePath).ToUpper();
                }

                Console.Write("\nВведіть пароль: ");
                var pass = Console.ReadLine().ToUpper();

                if (choice == "1")
                {
                    var cipherText = encryption.Encrypt(text, pass);
                    Console.WriteLine("\nЗашифрований текст: {0}", cipherText);

                    Console.WriteLine("\nВиберіть спосіб збереження:");
                    Console.WriteLine("1. В консоль");
                    Console.WriteLine("2. В файл");
                    Console.Write("Ваш вибір: ");
                    var outputChoice = Console.ReadLine();

                    if (outputChoice == "1")
                    {
                        Console.WriteLine("\nЗашифрований текст: {0}", cipherText);
                    }
                    else if (outputChoice == "2")
                    {
                        Console.Write("\nВведіть шлях до файлу: ");
                        var outputPath = Console.ReadLine();
                        File.WriteAllText(outputPath, cipherText);
                    }
                }
                else if (choice == "2")
                {
                    var plainText = encryption.Decrypt(text, pass);
                    Console.WriteLine("\nРозшифрований текст: {0}", plainText);

                    Console.WriteLine("\nВиберіть спосіб збереження:");
                    Console.WriteLine("1. В консоль");
                    Console.WriteLine("2. В файл");
                    Console.Write("\nВаш вибір: ");
                    var outputChoice = Console.ReadLine();

                    if (outputChoice == "1")
                    {
                        Console.WriteLine("\nРозшифрований текст: {0}", plainText);
                    }
                    else if (outputChoice == "2")
                    {
                        Console.Write("\nВведіть шлях до файлу: ");
                        var outputPath = Console.ReadLine();
                        File.WriteAllText(outputPath, plainText);
                    }
                }
                else
                {
                    Console.WriteLine("Невірний вибір. Спробуйте ще раз.");
                }
                Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");
            }

        }
    }
}
