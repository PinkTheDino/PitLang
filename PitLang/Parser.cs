using System.Runtime.InteropServices.JavaScript;

namespace PitLang;

public class Parser : Cursor<Token, TokenType>
{
    public Parser(List<Token> source) : base(source) {}

    protected override Token EndValue()
    {
        return new Token(TokenType.EOF, string.Empty, null, -1);
    }

    protected override TokenType GetKey(Token value)
    {
        return value.type;
    }
    
    private bool atEoF() => peek().type == TokenType.EOF;

    public Expression parseExpression()
    {
        Expression exrp = expression();
        reset();
        return exrp;
    }

    public List<Statement> Parse()
    {
        List<Statement> statements = new();
        while (!atEoF())
        {
            statements.Add(statement());
        }
        return statements;
    }
    

    private Statement statement()
    {
        if (match(TokenType.PRINT)) return printStatement();
        if (match(TokenType.VAR)) return declarationStatement();
        if (match(TokenType.IDENTIFIER)) return assignmentStatement();
        return expressionStatement();
    }

    private Statement expressionStatement()
    {
        Expression exp = expression();
        expect(TokenType.SEMICOLON, "Expected ';'");
        return new ExpressionStatement(exp);
    }

    private Statement declarationStatement()
    {
        expect(TokenType.IDENTIFIER, "Expected identifier");
        Token id = previous();
        if (match(TokenType.SEMICOLON))
        {
            //not initialized ex: var x; 
            return new InitVarStatement(new LiteralExpression(null), id);
        }
        if (match(TokenType.EQUAL))
        {
            Expression exp = expression(); // literal
            expect(TokenType.SEMICOLON, "Expected ';'");
            return new InitVarStatement(exp, id);
        }
        throw new Exception("Expected expression");
    }

    private Statement assignmentStatement()
    {
        Token id = previous();
        if (match(TokenType.EQUAL))
        {
            Expression exp = expression();
            expect(TokenType.SEMICOLON, "Expected ';'");
            return new AssignStatement(exp, id);
        }
        throw new Exception("Expected expression");
    }

    private Statement printStatement()
    {
        expect(TokenType.LEFT_PAREN, "Expected ( after 'print'");
        Expression exp = expression();
        expect(TokenType.RIGHT_PAREN, "Expected ) after print(...");
        expect(TokenType.SEMICOLON, "Expected ';'");
        return new PrintStatement(exp);
    }
    
    
    
    
    
    
    /*
    EXPRESSION GRAMMER
        expression → term ;
        compare    → term ( ( "==" | ">=" | "<=" | "!=" | "<" | ">" ) term )* ;         ex: 1 + 1 < 2 + 3
        term       → factor ( ( "+" | "-" ) factor )* ;
        factor     → unary  ( ( "*" | "/" ) unary  )* ;
        unary      → "-" unary | primary ;
        primary    → NUMBER | "(" expression ")" ;
                   
     */

    private Expression expression() //start
    {
        return compare();
    }

    private Expression compare()
    {
        Expression exp = term();
        while (match(TokenType.EQUAL_EQUAL, TokenType.GREATER_EQUAL, TokenType.LESS_EQUAL, 
                   TokenType.GREATER_EQUAL, TokenType.BANG_EQUAL, TokenType.GREATER, TokenType.LESS,
                   TokenType.AND, TokenType.OR))
        {
            Token op = previous();
            Expression right = term();
            exp = new BinaryExpression(exp, op, right);
        }

        return exp;
    }

    private Expression term()
    {
        Expression exp = factor();
        while (match(TokenType.PLUS, TokenType.MINUS))
        {
            Token op = previous();
            Expression right = factor();
            exp = new BinaryExpression(exp, op, right);
        }
        return exp;
    }

    private Expression factor()
    {
        Expression exp = unary();
        while (match(TokenType.STAR, TokenType.SLASH))
        {
            Token op = previous();
            Expression right = unary();
            exp = new BinaryExpression(exp, op, right);
        }
        return exp;
    }

    private Expression unary()
    {
        if (match(TokenType.MINUS))
        {
            Token op = previous();
            Expression right = unary();
            return new UnaryExpression(op, right);
        }

        return primary();
    }

    private Expression primary()
    {
        if (match(TokenType.NUMBER))
        {
            return new LiteralExpression(previous().literal);
        }

        if (match(TokenType.TRUE))
        {
            return new LiteralExpression(true);
        }

        if (match(TokenType.FALSE))
        {
            return new LiteralExpression(false);
        }

        if (match(TokenType.STRING))
        {
            return new LiteralExpression(previous().literal);
        }

        if (match(TokenType.IDENTIFIER))
        {
            return new VarRefExpression(previous());
        }

        if (match(TokenType.LEFT_PAREN))
        {
            Expression exp = expression();
            expect(TokenType.RIGHT_PAREN, "Expected ')' after expression");
            return exp;
        }

        throw new Exception("Expected expression");
    }
}