using System.Globalization;

namespace PitLang;

public abstract class Statement
{
    public abstract override string ToString();
    public abstract void Resolve();
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

    public override void Resolve() { expression.Resolve(); }
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
        Console.WriteLine(Expression.Stringify(expression.Evaluate(env)));
    }

    public override string ToString() => $"print({expression})";

    
    
    public override void Resolve() { expression.Resolve(); }
}

public class DeclareVarStatement : Statement // var x; 
{
    readonly Token identifier;

    public DeclareVarStatement(Token identifier)
    {
        this.identifier = identifier;
    }
    
    public override void Execute(Environment env)
    {
        env.DefineInScope(identifier, null);
    }

    public override string ToString() => $"(var= {identifier.lexeme}, undefined)";

    public override void Resolve()
    {
        ScopeResolver.OnDeclared(identifier);
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

    public override void Resolve()
    {
        ScopeResolver.OnDeclared(identifier);
        expression.Resolve();
        ScopeResolver.OnInitialized(identifier);
    }
}

public class AssignStatement : Statement // x = 12.3; 
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
    public override void Resolve()
    {
        ScopeResolver.OnWrite(identifier);
    }
}

public class BlockStatement : Statement
{
    private readonly  List<Statement> statements;

    public BlockStatement(List<Statement> statements)
    {
        this.statements = statements;
    }

    public override void Execute(Environment env)
    {
        //env is the old environment
        Environment newEnv = new Environment(env);
        foreach (Statement statement in statements)
        {
            statement.Execute(newEnv);
        }
    }

    public override string ToString()
    {
        
        string s = "{\n";
        foreach (Statement statement in statements)
        {
            s += statement.ToString() + "\n";
        }
        return s + "}\n";
    }

    public override void Resolve()
    {
        ScopeResolver.OnBlockStart();
        foreach (Statement statement in statements)
        {
            statement.Resolve();
        }
        ScopeResolver.OnBlockEnd();
    }
}

public class IfStatement : Statement
{
    private readonly Statement statement;
    private readonly Expression condition;
    private readonly Statement? elseState;

    public IfStatement(Statement statement, Expression condition, Statement? elseState = null)
    {
        this.statement = statement;
        this.condition = condition;
        this.elseState = elseState;
    }

    public override void Execute(Environment env)
    {
        object? o = condition.Evaluate(env);
        if (Expression.isTruthyValue(o))
            statement.Execute(env);
        else if (elseState != null)
            elseState.Execute(env);
    }

    public override string ToString()
    {
        if (elseState != null)
        {
            return $"(if {condition}, {statement}, else {elseState})"; 
        }
        return $"(if {condition}, {statement})"; 
    }

    public override void Resolve()
    {
        condition.Resolve();
        statement.Resolve();
        elseState?.Resolve();
    }
}

public class WhileStatement : Statement
{
    private readonly Statement statement;
    private readonly Expression condition;

    public WhileStatement(Statement statement, Expression condition)
    {
        this.statement = statement;
        this.condition = condition;
    }

    public override void Execute(Environment env)
    {
        while (Expression.isTruthyValue(condition.Evaluate(env)))
        {
            statement.Execute(env);
        }
            
    }

    public override string ToString()
    {
        return $"(while {condition}, {statement})"; 
    }

    public override void Resolve()
    {
        condition.Resolve();
        statement.Resolve();
    }
}

