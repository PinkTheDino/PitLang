using System.Diagnostics;

namespace PitLang;

public abstract class Cursor<TValue, TKey>
{
    public readonly List<TValue> source;
    protected int current { get;  private set; } = 0;

    public Cursor(IEnumerable<TValue> source)
    {
        this.source = source.ToList();
    }
    
    protected virtual void OnAdvance(TValue value) {}
    protected abstract TKey GetKey(TValue value);
    protected abstract TValue EndValue();
    
    protected TValue advance()
    {
        TValue result = source[current++];
        OnAdvance(result);
        return result;
    }
    protected TValue peek(int off = 0)
    {
        Debug.Assert(off >= 0);
        return isEnd(off) ? EndValue() : source[current + off];
    }
    
    protected TValue previous(int off = 0)
    {
        Debug.Assert(off >= 0);
        return isEnd(off) ? EndValue() : source[current - off - 1];
    }

    protected bool match(params TKey[] keys)
    {
        
        TKey k = GetKey(peek());
        foreach (TKey key in keys)
        {
            if (EqualityComparer<TKey>.Default.Equals(k, key))
            {
                advance();
                return true;
            }
        }

        return false;
    }

    protected void expect(TKey key, string errorMsg)
    {
        if (!match(key))
        {
            throw new Exception(errorMsg);
        }
    }
    
    protected bool isEnd(int off = 0)
    {
        return current + off >= source.Count;
    }

    protected void reset()
    {
        current = 0;
    }
}

public class Scanner : Cursor<char, char>
{
    private readonly List<Token> tokens = new();
    private int start;
    private int line;

    private static readonly Dictionary<string, TokenType> keywords = new()
    {
        
        {"false", TokenType.FALSE},
        { "nil", TokenType.NIL},
        {"true", TokenType.TRUE},
        { "if", TokenType.IF },
        { "else", TokenType.ELSE },
        { "while", TokenType.WHILE },
        { "for", TokenType.FOR },
        { "and", TokenType.AND },
        { "or", TokenType.OR },
        { "var", TokenType.VAR},
        { "print", TokenType.PRINT},
        { "mod", TokenType.MOD},
    };

    public Scanner(string s) : base(s) {}

    protected override char EndValue()
    {
        return '\0';
    }

    protected override char GetKey(char value)
    {
        return value;
    }

    protected override void OnAdvance(char value)
    {
        if (value == '\n') line++;
    }


    public List<Token> Tokenize()
    {
        
        while (!isEnd())
        {
            start = current;
            scanToken();
        }
        tokens.Add(new Token(TokenType.EOF, "", null, line+1));
        return tokens;
    }

    private void scanToken()
    {
        char c = advance();
    
        if (char.IsWhiteSpace(c)) return;
        
        
        switch (c) 
        {
            case ';': addToken(TokenType.SEMICOLON);
                return;
            case '(': addToken(TokenType.LEFT_PAREN);
                return;
            case ')': addToken(TokenType.RIGHT_PAREN);
                return;
            case '{': addToken(TokenType.LEFT_BRACE);
                return;
            case '}': addToken(TokenType.RIGHT_BRACE);
                return;
            case '+': addToken(TokenType.PLUS);
                return;
            case '-': addToken(TokenType.MINUS);
                return;
            case '*': addToken(TokenType.STAR);
                return;
            case '/': addToken(TokenType.SLASH);
                return;
            case '!': addToken(match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);    
                return;
            case '=': addToken(match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);   
                return;
            case '<': addToken(match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);    
                return;
            case '>': addToken(match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); 
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
        addToken(TokenType.UNKNOWN, null);
    }

    private void parserError(string errorMsg)
    {
        throw new Exception(errorMsg);
    }
    

    private void numberLiteral()
    {
        while(isNumeric(peek())) advance();

        if (peek() == '.' && isNumeric(peek(1)))
        {
            advance(); // get rid of '.' cause doubles
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
            string str = getLexeme();
            addToken(TokenType.STRING, str.Substring(1, str.Length - 2));
        }
        
        
    }
    

    private bool checkKeyword(string identifier, out TokenType type)
    {
        return keywords.TryGetValue(identifier, out type);
    }

    private string getLexeme()
    {
        return new string(source.GetRange(start, current - start).ToArray());
    }
    
    private void addToken(TokenType type) => addToken(type, null);
    
    private void addToken(TokenType token, object? literal)
    {
        Token t = new Token(token, getLexeme(), literal, line);
        tokens.Add(t);
    }

    public static bool isIdentifier(char c)
    {
        return char.IsLetterOrDigit(c) || c == '_';
    }
    

    private static bool isNumeric(char c)
    {
        return char.IsDigit(c);
    }
}