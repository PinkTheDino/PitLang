using System.Globalization;

namespace PitLang;

public abstract class Statement
{
    public abstract override string ToString();
    public abstract void Execute(Environment env);
}

public class ExpressionStatement : Statement
{
    private readonly Expression expression;
    public ExpressionStatement(Expression expression)
    {
        this.expression = expression;
    }
    
    public override string ToString() => expression.ToString();

    public override void Execute(Environment env)
    {
        expression.Evaluate(env);
    }
}


public class PrintStatement : Statement
{
    readonly Expression expression;

    public PrintStatement(Expression expression)
    {
        this.expression = expression;
    }
    
    public override void Execute(Environment env)
    {
        Console.WriteLine(stringify(expression.Evaluate(env)));
    }

    public override string ToString() => $"print({expression})";

    private static string stringify(object? val)
    {
        switch (val)
        {
            case null: return "nil";
            case bool b: return b ? "true" : "false";
            case double d: return d.ToString(CultureInfo.InvariantCulture);
            default: return val.ToString() ?? "nil";
        }
    } 
}

public class InitVarStatement : Statement // var x = 12.3; 
{
    readonly Expression expression;
    readonly Token identifier;

    public InitVarStatement(Expression expression, Token identifier)
    {
        this.expression = expression;
        this.identifier = identifier;
    }
    
    public override void Execute(Environment env)
    {
        env.DefineInScope(identifier, expression.Evaluate(env));
    }

    public override string ToString() => $"(var= {identifier.lexeme}, {expression})";
}

public class AssignStatement : Statement // var x = 12.3; 
{
    readonly Expression expression;
    readonly Token identifier;

    public AssignStatement(Expression expression, Token identifier)
    {
        this.expression = expression;
        this.identifier = identifier;
    }
    
    public override void Execute(Environment env)
    {
        env.Assign(identifier, expression.Evaluate(env));
    }

    public override string ToString() => $"(= {identifier.lexeme}, {expression})";
    
}