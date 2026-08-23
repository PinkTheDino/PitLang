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
        if (match(TokenType.WHILE)) return whileStatement();
        if (match(TokenType.IF)) return ifStatement();
        if (match(TokenType.LEFT_BRACE)) return blockStatement();
        if (match(TokenType.PRINT)) return printStatement();
        if (match(TokenType.VAR)) return declarationStatement();
        if (match(TokenType.IDENTIFIER)) return assignmentStatement();
        return expressionStatement();
    }

    private Statement whileStatement()
    {
        expect(TokenType.LEFT_PAREN, "Expected '(' after while statement");
        Expression exp = expression();
        expect(TokenType.RIGHT_PAREN, "Expected ')' after while statement");
        
        Statement block = statement();
        return new WhileStatement(block, exp);
    }

    private Statement ifStatement()
    {
        expect(TokenType.LEFT_PAREN, "Expected '(' after if statement");
        Expression exp = expression();
        expect(TokenType.RIGHT_PAREN, "Expected ')' after if statement");
        
        Statement block = statement();
        if (match(TokenType.ELSE))
        {
            Statement s = statement();
            return new IfStatement(block, exp, s);
        }
        return new IfStatement(block, exp);
    }

    private Statement blockStatement()
    {
        List<Statement> statements = new();
        
        while (!atEoF() && peek().type != TokenType.RIGHT_BRACE) //breaks if '}' or end so we gotta check if its an error or good
        {
            statements.Add(statement());
        }
        
        expect(TokenType.RIGHT_BRACE, "Expected '}' after block");
        
        return new BlockStatement(statements);
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
            return new DeclareVarStatement(id);
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
        compare    → term ( ( "==" | ">=" | "<=" | "!=" | "<" | ">" ) term )* ;         ex: 1 + 1 < 2 + 3 (bool)
        term       → factor ( ( "+" | "-" ) factor )* ;
        factor     → unary  ( ( "*" | "/" ) unary  )* ;
        unary      → "-" unary | primary ;
        primary    → NUMBER | "(" expression ")" ;
                   
     */

    private Expression expression() //start
    {
        return logic();
    }

    private Expression logic()
    {
        Expression exp = compare();
        while (match(TokenType.AND, TokenType.OR))
        {
            Token op = previous();
            Expression right = compare();
            exp = new BinaryExpression(exp, op, right);
        }

        return exp;
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
        while (match(TokenType.STAR, TokenType.SLASH, TokenType.MOD))
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