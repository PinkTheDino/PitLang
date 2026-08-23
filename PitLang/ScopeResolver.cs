namespace PitLang;
using Scope = Dictionary<string, VariableStatus>;

public enum VariableStatus : int
{
    Undefined = 0, // doesnt exist at all in the code
    Declared = 2, // var x;
    Initialized = 4, // var x = 10;
}


public static class ScopeResolver
{
    private static bool initialized = false;
    private static List<Scope> scopes = new();
    
    private static int currentScopeIndex => scopes.Count - 1;
    private static Scope currentScope => scopes[currentScopeIndex];
    
    public static void init()
    {
        if (initialized) return;
        scopes.Add(new Scope()); //first scope cause its an empty list
        initialized = true;
    }

    public static void reset()
    {
        scopes = new();
        initialized = false;
    }

    public static void OnBlockStart()
    {
        scopes.Add(new Scope());
    }

    public static void OnBlockEnd()
    {
        if (scopes.Count <= 0) throw new Exception("Resolver: Scope list is empty Something when wrong wtih a block");
        scopes.RemoveAt(currentScopeIndex);
    }

    public static void OnDeclared(Token id)
    {
        if (getVariableStatus(id.lexeme) == VariableStatus.Undefined)
        {
            currentScope.Add(id.lexeme, VariableStatus.Declared);
            return;
        }

        throw new Exception($"Resolver: Variable '{id.lexeme}' already declared in extended scope");
    }

    public static void OnInitialized(Token id)
    {
        currentScope[id.lexeme] = VariableStatus.Initialized;
    }

    public static void OnRead(Token id)
    {
        VariableStatus state = getVariableStatus(id.lexeme);
        if (state == VariableStatus.Undefined) throw new Exception($"Reference of undefined variable '{id.lexeme}'");
        if (state == VariableStatus.Declared) throw new Exception($"Attempted read of a variable '{id.lexeme}' with undefined value ");
    }

    public static void OnWrite(Token id)
    {
        if (getVariableStatus(id.lexeme) == VariableStatus.Undefined)
            throw new Exception($"Reference of undefined variable '{id.lexeme}'");
        
    }

    private static VariableStatus getVariableStatus(string name)
    {
        for (int i = scopes.Count - 1; i >= 0; i--)
            if (scopes[i].TryGetValue(name, out VariableStatus state)) return state;
        return VariableStatus.Undefined;
    }
}