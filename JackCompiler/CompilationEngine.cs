namespace JackCompiler;

public class CompilationEngine
{
    private readonly string _src;
    private readonly Tokenizer _tokenizer;

    public CompilationEngine(string src)
    {
        _src = src;
        _tokenizer = new Tokenizer(src);

        if (_tokenizer.HasMoreTokens() == false)
            throw new Exception("No tokens");
        _tokenizer.TryAdvance();
    }

    public string CompileClass()
    {
        return $"<class>{Environment.NewLine}" +
               $"<keyword> {Eat("class")} </keyword>{Environment.NewLine}" +
               $"<identifier> {EatIdentifier()} </identifier>{Environment.NewLine}" +
               $"<symbol> {Eat("{")} </symbol>" +
               $"{CompileClassVarDecZeroOrMore()}{CompileSubroutineZeroOrMore()}" +
               $"<symbol> {Eat("}")} </symbol>" +
               $"</class>";

        string CompileSubroutineZeroOrMore()
        {
            var result = Environment.NewLine;
        
            while (_tokenizer.CurrentToken.Value is "constructor" or "function" or "method") 
                result += CompileSubroutine() + Environment.NewLine;

            return result.TrimEnd(Environment.NewLine.ToCharArray());
        }

        string CompileClassVarDecZeroOrMore()
        {
            var result = Environment.NewLine;
        
            while (_tokenizer.CurrentToken.Value is "static" or "field") 
                result += CompileClassVarDec() + Environment.NewLine;

            return result.TrimEnd(Environment.NewLine.ToCharArray());
        }
    }

    public string CompileClassVarDec()
    {
        return $"<classVarDec>{Environment.NewLine}" +
               $"<keyword> {EatAny("static", "field")} </keyword>{Environment.NewLine}" +
               $"{CompileType()}{Environment.NewLine}" +
               $"<identifier> {EatIdentifier()} </identifier>" +
               $"{CompileZeroOrMoreVarNameDeclarations()}{Environment.NewLine}" +
               $"<symbol> {Eat(";")} </symbol>{Environment.NewLine}" +
               $"</classVarDec>{Environment.NewLine}";

        string CompileZeroOrMoreVarNameDeclarations()
        {
            var result = Environment.NewLine;
            while (_tokenizer.CurrentToken.Value is ",")
                result += $"<symbol> {Eat(",")} </symbol>"
                          + Environment.NewLine
                          + $"<identifier> {EatIdentifier()} </identifier>"
                          + Environment.NewLine;

            return result.TrimEnd(Environment.NewLine.ToCharArray());
        }
    }

    private string CompileType()
    {
        var tag = _tokenizer.CurrentToken is KeywordToken ? "keyword" : "identifier";
        return $@"<{tag}> {EatType()} </{tag}>";
    }

    private string CompileSubroutine()
    {
        return $"<subroutineDec>{Environment.NewLine}" +
               $"<keyword> {EatAny("constructor", "function", "method")} </keyword>{Environment.NewLine}" +
               $"{CompileTypeAndVoid()}{Environment.NewLine}" +
               $"<identifier>{EatIdentifier()}<identifier>{Environment.NewLine}" +
               $"<symbol>{Eat("(")}</symbol>{CompileParameterList()}{Environment.NewLine}" +
               $"<symbol>{Eat(")")}</symbol>{Environment.NewLine}" +
               $"{CompileSubroutineBody()}{Environment.NewLine}" +
               $"</subroutineDec>{Environment.NewLine}";
    }

    private string CompileSubroutineBody()
    {
        return $"<symbol> {Eat("{")} </symbol>{Environment.NewLine}" +
               
               $"<symbol> {Eat("}")} </symbol>{Environment.NewLine}";
    }

    private string CompileTypeAndVoid()
    {
        var tag = _tokenizer.CurrentToken is KeywordToken ? "keyword" : "identifier";
        return $@"<{tag}> {EatTypeAndVoid()} </{tag}>";
    }

    private string CompileParameterList()
    {
        return "";
    }
    
    private void CompileVarDec()
    {
        
    }
    
    private void CompileStatements()
    {
        
    }
    
    private void CompileDo()
    {
        
    }
    
    private void CompileLet()
    {
        
    }
    
    private void CompileWhile()
    {
        
    }
    
    private void CompileReturn()
    {
        
    }
    
    private void CompileIf()
    {
        
    }
    
    private void CompileExpression()
    {
        
    }

    public string CompileTerm()
    {
        return @"";

    }
    
    private void CompileExpressionList()
    {
        
    }
    
    private string EatIdentifier()
    {
        if (_tokenizer.CurrentToken.Type != TokenType.Identifier)
            throw new Exception($"Expected identifier but got {_tokenizer.CurrentToken.Value}");
        var id = _tokenizer.CurrentToken.Value;
        _tokenizer.TryAdvance();
        return id;
    }

    private string Eat(string tokenValue)
    {
        if (_tokenizer.CurrentToken.Value != tokenValue)
            throw new Exception($"Expected {tokenValue} but got {_tokenizer.CurrentToken.Value}");
        _tokenizer.TryAdvance();
        return tokenValue;
    }
    
    private string EatAny(params string[] tokenValues)
    {
        if (!tokenValues.Contains(_tokenizer.CurrentToken.Value))
            throw new Exception($"Expected {string.Join(" or ", tokenValues)} but got {_tokenizer.CurrentToken.Value}");
        var tokenValue = _tokenizer.CurrentToken.Value;
        _tokenizer.TryAdvance();
        return tokenValue;
    }
    
    private string EatType() => 
        _tokenizer.CurrentToken.Type == TokenType.Identifier ? EatIdentifier() : EatAny("int", "char", "boolean");
    
    private string EatTypeAndVoid() => 
        _tokenizer.CurrentToken.Type == TokenType.Identifier ? EatIdentifier() : EatAny("int", "char", "boolean", "void");
}