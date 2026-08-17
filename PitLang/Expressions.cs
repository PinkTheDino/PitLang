using System.Globalization;

namespace PitLang;

public abstract class Expression
{


    public abstract override string ToString();

    public abstract object? Evaluate(Environment env);
    
    
}

public class BinaryExpression : Expression //binary as in two operands, not binary 0101
{
    public Expression left;
    public Token oper;
    public Expression right;

    public BinaryExpression(Expression left, Token oper, Expression right)
    {
        this.left = left;
        this.oper = oper;
        this.right = right;
    }
    
    public override string ToString() => $"({oper.lexeme} {left}, {right})";

    public override object? Evaluate(Environment env)
    {
        switch (oper.type)
        {
            //doubles
            case TokenType.PLUS: return num(left.Evaluate(env)) + num(right.Evaluate(env));
            case TokenType.MINUS: return num(left.Evaluate(env)) - num(right.Evaluate(env));
            case TokenType.STAR: return num(left.Evaluate(env)) * num(right.Evaluate(env));
            case TokenType.SLASH: return num(left.Evaluate(env)) / num(right.Evaluate(env));
            //booleans
            case TokenType.AND: return boolean(left.Evaluate(env)) && boolean(right.Evaluate(env));
            case TokenType.OR:  return boolean(left.Evaluate(env)) || boolean(right.Evaluate(env));
            // a == b
            case TokenType.EQUAL_EQUAL: return left.Evaluate(env) == right.Evaluate(env);
            case TokenType.BANG_EQUAL: return left.Evaluate(env) != right.Evaluate(env);
            
            case TokenType.GREATER_EQUAL: return num(left.Evaluate(env)) >= num(right.Evaluate(env));
            case TokenType.LESS_EQUAL: return num(left.Evaluate(env)) <= num(right.Evaluate(env));
            case TokenType.GREATER: return num(left.Evaluate(env)) < num(right.Evaluate(env));
            case TokenType.LESS: return num(left.Evaluate(env)) > num(right.Evaluate(env));
        }
        throw new Exception($"Unexpected expression type {oper.lexeme}");
    }
    
    private double num(object? v) => v is double d ? d : throw new Exception($"Unexpected expression type {oper.lexeme}");
    private bool boolean(object? v) => v is bool b ? b : throw new Exception($"Unexpected expression type {oper.lexeme}");
}

public class LiteralExpression : Expression
{
    public object? literal;
    public LiteralExpression(object? literal)
    {
        this.literal = literal;
    }

    public override string ToString()
    {
        switch (literal)
        {
            case null: return "null";
            case string s: return s;
            case double d: return d.ToString(CultureInfo.InvariantCulture);
            case bool b: return b ? "true" : "false";
        }
        throw  new Exception($"Unexpected expression type {literal.GetType().Name}");
    }

    public override object? Evaluate(Environment env)
    {
        switch (literal)
        {
            case null: return null;
            case double d: return d;
            case bool b: return b;
            case string s: return s;
        }
        throw new Exception($"Unexpected expression type {literal.GetType().Name}");
    }
}

public class UnaryExpression : Expression
{
    public Token oper;
    public Expression right;
    public UnaryExpression(Token oper, Expression right)
    {
        this.oper = oper;
        this.right = right;
    }
    public override string ToString() => $"{oper.lexeme}{right}";

    public override object? Evaluate(Environment env)
    {
        switch (oper.type)
        {
            case TokenType.MINUS: return -num(right.Evaluate(env));
            case TokenType.BANG: return !boolean(right.Evaluate(env));
        }
        throw new Exception($"Unexpected expression type {oper.lexeme}");
    }
    
    private double num(object? v) => v is double d ? d : throw new Exception($"Unexpected expression type {oper.lexeme}");
    private bool boolean(object? v) => v is bool b ? b : throw new Exception($"Unexpected expression type {oper.lexeme}");
}

public class VarRefExpression : Expression
{
    public Token id;
    public VarRefExpression(Token id)
    {
        this.id = id;
    }
    public override string ToString() => $"{id.lexeme}";

    public override object? Evaluate(Environment env)
    {
        return env.GetVariable(id);
    }
}