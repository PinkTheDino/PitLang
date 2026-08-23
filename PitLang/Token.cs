namespace PitLang;

public class Token
{
    public TokenType type { get; }
    public string lexeme { get; }
    public object? literal  { get; }
    public int line  { get; }

    public Token(TokenType type, string lexeme, object? literal, int line)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
    }

    public override string ToString()
    {
        string s = $"Token(type: {type}, ";
        if (lexeme.Length > 0) s += $"lexeme: {lexeme}, ";
        return s + $"literal: {literal}, line: {line})";
    }
    
}


public enum TokenType
{
    //single char
    LEFT_PAREN, RIGHT_PAREN,
    LEFT_BRACE, RIGHT_BRACE,
    MINUS, PLUS, STAR, SLASH,
    EQUAL, GREATER, LESS,
    BANG, SEMICOLON,
    
    //double char
    EQUAL_EQUAL, GREATER_EQUAL, LESS_EQUAL, BANG_EQUAL,
    
    //literals
    NUMBER, STRING, IDENTIFIER, TRUE, FALSE, NIL,
    
    //keywords
    IF, ELSE, FOR, WHILE, AND, OR, MOD, VAR, 
    PRINT,
    
    EOF, UNKNOWN
}