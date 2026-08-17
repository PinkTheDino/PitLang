namespace PitLang;

public static class Program
{
    public static void Main(string[] args)
    {
        string source = File.ReadAllText(args[0]);
        Console.WriteLine(source);
        
        
        Scanner scanner = new(source);
        List<Token> tokens = scanner.Tokenize();
        foreach (Token token in tokens)
        {
            Console.WriteLine(token.ToString());
        }
        Console.WriteLine();
        
        
        Parser parser = new(tokens);
        /*Expression result = parser.parseExpression();
        Console.WriteLine($"Tree: {result}");
        Environment environment = new();
        Console.WriteLine($"Value: {result.Evaluate(environment)}");
        */
        List<Statement> statements = parser.Parse();
        Console.WriteLine("---Tree---" );
        foreach (Statement statement in statements)
        {
            Console.WriteLine(statement.ToString());
        }
        Console.WriteLine();
        Console.WriteLine("---Running...---" );
        Environment environment = new();
        foreach (Statement statement in statements)
        {
            statement.Execute(environment);
        }
    }
}