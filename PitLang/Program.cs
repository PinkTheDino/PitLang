using PitLang;

public class Program
{
    public static void Main(string[] args)
    {
        string source = File.ReadAllText(args[0]);
        Console.WriteLine(source);
        Scanner scanner = new(source);
        Token[] tokens = scanner.Tokenize();
        foreach (Token token in tokens)
        {
            Console.WriteLine(token.ToString());
        }
    }
}