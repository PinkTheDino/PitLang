namespace PitLang;

public class Token
{
    TokenType type;
    string lexeme;
    object? literal;
    int line;

    public Token(TokenType type, string lexeme, object? literal, int line)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
    }

    public override string ToString()
    {
        return $"Token(lexeme: \"{lexeme}\",tokenType: {type},line: {line},literal: {literal})";
    }
}


public enum TokenType
{
    //single char
    LEFT_PAREN, RIGHT_PAREN,
    MINUS, PLUS, STAR, SLASH,
    EQUAL, R_ARROW, L_ARROW,
    BANG, SEMICOLON,
    
    //double char
    EQUAL_EQUAL, EQUAL_R_ARROW, EQUAL_L_ARROW,
    
    //literals
    NUMBER, STRING, IDENTIFIER,
    
    //keywords
    IF, ELSE, FOR, WHILE,
    
    EOF, UNKNOWN
}