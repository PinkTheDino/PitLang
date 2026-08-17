namespace PitLang;

public class Environment
{
    private readonly Dictionary<string, object?> variables = new();
    private readonly HashSet<string> scopeStack = new();
    private readonly Environment? parent;
    
    public Environment(Environment? parent = null)
    {
        this.parent = parent;
    }
    //note: show mom republican jesus video
    private bool variableExists(string name) => variables.ContainsKey(name);

    public void DefineInScope(Token id, object? value)
    {
        if (variableExists(id.lexeme)) throw new Exception("Variable already defined ");
        variables.Add(id.lexeme, value);
    }
    public object? GetVariable(Token id)
    {
        string name = id.lexeme;
        if (variables.TryGetValue(name, out object? value)) return value;
        if (parent != null) return parent.GetVariable(id);
        throw new Exception($"Can't find variable {name}");
    }

    public void Assign(Token id, object? value)
    {
        string name = id.lexeme;
        if (variables.ContainsKey(name))
        {
            variables[name] = value;
            return;
        } 
        if (parent != null)
        {
            parent.Assign(id, value);
            return;
        }
        throw new Exception($"Can't assign variable '{name}'");
    }
}