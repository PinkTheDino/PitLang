namespace PitLang;

public class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens = new();
    private int start;
    private int i;
    private int line;
    
    public Scanner(string source)
    {
        this.source = source;
    }
    
    
    
    
    public Token[] Tokenize()
    {
        
        while (!isEnd())
        {
            start = i;
            scanToken();
        }
        tokens.Add(new Token(TokenType.EOF, "", null, line+1));
        return tokens.ToArray();
    }

    private void scanToken()
    {
        char c = advance();

        if (c == '\n')
        {
            line++;
            return;
        }

        if (c == ' ') return;
        
        
        switch (c) 
        {
            case '(': addToken(TokenType.LEFT_PAREN);
                return;
            case ')': addToken(TokenType.RIGHT_PAREN);
                return;
            case '+': addToken(TokenType.PLUS);
                return;
            case '-': addToken(TokenType.MINUS);
                return;
            case '*': addToken(TokenType.STAR);
                return;
            case '/': addToken(TokenType.SLASH);
                return;
        }

        if (isNumeric(c))
        {
            numberLiteral();
            return;
        } 
        if (isIdentifier(c))
        {
            identifier();
            return;
        }

        if (c == '"')
        {
            stringLiteral();
            return;
        }
        tokens.Add(new Token(TokenType.UNKNOWN, "", null, line));
    }

    private void parserError(string errorMsg)
    {
        Console.Error.WriteLine(errorMsg);
    }

    private void numberLiteral()
    {
        while(isNumeric(peek())) advance();

        if (peek() == '.' && isNumeric(peek(1)))
        {
            advance(); // get rid of '.'
            while(isNumeric(peek())) advance();  
        }
        addToken(TokenType.NUMBER, double.Parse(getLexeme()));
    }

    private void identifier()
    {
        while(isIdentifier(peek())) advance();
        if (checkKeyword(getLexeme(), out TokenType type))
        {
            addToken(type);
        }
        else
        {
            addToken(TokenType.IDENTIFIER);
        }
    }

    private void stringLiteral()
    {
        while(peek()  != '"' && !isEnd()) advance();

        if (isEnd())
        {
            parserError("Unterminated string literal");
        }
        else
        {
            advance();
            addToken(TokenType.STRING, getLexeme());
        }
        
        
    }
    

    private bool checkKeyword(string identifier, out TokenType type)
    {
        type = TokenType.EOF;
        switch (identifier)
        {
            case "if": type = TokenType.IF; break;
            case "else": type = TokenType.ELSE; break;
            case "while": type = TokenType.WHILE; break;
            case "for": type = TokenType.FOR; break;
        }

        return type != TokenType.EOF;
    }

    private string getLexeme()
    {
        return source.Substring(start, i - start);
    }
    
    private void addToken(TokenType type) => addToken(type, null);
    
    private void addToken(TokenType token, object? literal)
    {
        Token t = new Token(token, getLexeme(), literal, line);
        tokens.Add(t);
    }
    
    //SCANNER HELPERS
    private char advance()
    {
        return source[i++];
    }

    private char peek(int off = 0)
    {
        if (isEnd(off)) return '\0';
        return source[i+off];
    }

    private bool match(char c)
    {
        if (peek() == c)
        {
            advance();
            return true;
        }

        return false;

    }

    private bool isEnd(int off = 0)
    {
        return i + off >= source.Length;
    }

    public static bool isIdentifier(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }
    
    private static bool isAlphanumeric(char c) // can be used be identifier only after it could be an identifier
    {
        return char.IsLetterOrDigit(c);
    }

    private static bool isNumeric(char c)
    {
        return char.IsDigit(c);
    }

    private static bool isLetter(char c)
    {
        return char.IsLetter(c);
    }
}