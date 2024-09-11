using PolybiiAndHill;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        var hillCipher = new HillCipher();
        UIutil.Start(hillCipher);

        //var polybius = new PolybiusSquare();
        //UIutil.Start(polybius);
    }
}

/*
 Hill:
 in: HELLOWORLDAA
 out: TFJIPIJSGSNI
 pass: GYBNQKURP

 */