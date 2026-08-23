namespace PitLang;

public static class Program
{

    private enum ExitCode : int
    {
        ExitUsage = 64,
        ExitDataError = 65,
        ExitNoInput = 66,
    }
    
    public static int Main(string[] args)
    {
        bool debug = false;
        string? path = null;
        
        
        
        if (args.Length == 0) { usageMessage(); return (int)ExitCode.ExitNoInput; }
        
        foreach (string arg in args)
        {
            //Console.Error.WriteLine(arg);
            if (arg == "-h" || arg == "--help") { usageMessage(); return 0; }
            if (path == null)
            {
                path = arg;
                continue;
            }

            if (arg == "-d" || arg == "--debug")
            {
                debug = true; 
                continue;
            }

            if (arg.StartsWith('-'))
            {
                Console.Error.WriteLine($"Unknown option: '{arg}'"); 
                usageMessage();
                return (int)ExitCode.ExitUsage;
            }
            
        }

        if (!File.Exists(path))
        {
            Console.Error.WriteLine($"Could not open file '{path}'");
            return (int)ExitCode.ExitNoInput;
        }

        if (debug)
        {
            Console.Error.WriteLine($"Running '{Path.GetFileName(path)}' in debug mode");
        }
        else
        {
            Console.Error.WriteLine($"Running '{Path.GetFileName(path)}'");
        }
        /*
        try
        {
            run(File.ReadAllText(path), debug);
        }
        catch (Exception error)
        {
            Console.Error.WriteLine("---Error---");
            Console.Error.WriteLine(error.Message);
            return (int)ExitCode.ExitDataError;
        }*/
        run(File.ReadAllText(path), debug);

        return 0;
    }

    private static void run(string source, bool debug)
    {
            Scanner scanner = new(source);
            List<Token> tokens = scanner.Tokenize();
            if (debug)
            {
                Console.Error.WriteLine("---Tokens (Scanner.cs)---" );
                foreach (Token token in tokens) Console.Error.WriteLine(token.ToString());
            }

            Parser parser = new(tokens);
            List<Statement> statements = parser.Parse();
            if (debug)
            {
                Console.Error.WriteLine("---Tree (Parser.cs)---" );
                foreach (Statement statement in statements) Console.Error.WriteLine(statement.ToString());
            }
            
            ScopeResolver.init();
            
            foreach (Statement statement in statements)
            {
                statement.Resolve();
            }
            if (debug) Console.Error.WriteLine("---Input Output---" );
            Environment environment = new();
            foreach (Statement statement in statements)
            {
                statement.Execute(environment);
            }
    }

    public static void usageMessage()//usage of program
    {
        Console.Error.WriteLine("Usage: pitlang <entryScript.pit> [flags...]");
        Console.Error.WriteLine();
        Console.Error.WriteLine("Optional flags: ");
        Console.Error.WriteLine("   -d/--debug    ->    Dumps tokens and parse tree to terminal");
        Console.Error.WriteLine("   -h/--help     ->    Shows the usage of the program");
    }
}