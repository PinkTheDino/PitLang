namespace PitLang;

public class ScopeResolver
{
    private readonly List<Dictionary<string, bool>> scopeStack = new();
    public bool IsGlobalScope => scopeStack.Count == 0;
    
    public void BeginScope()
    {
        scopeStack.Add(new Dictionary<string, bool>());
    }

    public void EndScope()
    {
        scopeStack.RemoveAt(scopeStack.Count - 1);
    }

    public bool VariableExists(Token id)
    {
        if (IsGlobalScope) return false;
        foreach (Dictionary<string, bool> scope in scopeStack)
        {
            if (scope.ContainsKey(id.lexeme)) return true;
        }

        return false;
    }

    public void Declare(Token id)
    {
        if (IsGlobalScope) return;
        if (VariableExists(id)) throw new Exception($"Variable '{id.lexeme}' is already defined");
        scopeStack[scopeStack.Count - 1][id.lexeme] = true;
    }

    public void Define(Token id)
    {
        
    }
}