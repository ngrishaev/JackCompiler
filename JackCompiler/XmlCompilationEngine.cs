namespace JackCompiler;

public class XmlCompilationEngine
{
    private readonly Tokenizer _tokenizer;

    public XmlCompilationEngine(string src)
    {
        _tokenizer = new Tokenizer(src);

        if (_tokenizer.HasMoreTokens() == false)
            throw new Exception("No tokens");
        _tokenizer.TryAdvance();
    }

    public string CompileClass()
    {
        return $"<class>{Environment.NewLine}" +
               $"<keyword> {Eat("class")} </keyword>{Environment.NewLine}" +
               $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}" +
               $"<symbol> {Eat("{")} </symbol>{Environment.NewLine}" +
               $"{CompileClassVarDecZeroOrMore()}{CompileSubroutineZeroOrMore()}" +
               $"<symbol> {Eat("}")} </symbol>{Environment.NewLine}" +
               $"</class>";

        string CompileSubroutineZeroOrMore()
        {
            var result = "";
        
            while (_tokenizer.CurrentToken.Value is "constructor" or "function" or "method") 
                result += CompileSubroutine();
            
            result = result.TrimEnd(Environment.NewLine.ToCharArray());
            return result == "" ? result : result + Environment.NewLine;
        }

        string CompileClassVarDecZeroOrMore()
        {
            var result = "";
        
            while (_tokenizer.CurrentToken.Value is "static" or "field") 
                result += CompileClassVarDec();

            return result;
        }
    }

    public string CompileClassVarDec()
    {
        return $"<classVarDec>{Environment.NewLine}" +
               $"<keyword> {EatAny("static", "field")} </keyword>{Environment.NewLine}" +
               $"{CompileType()}{Environment.NewLine}" +
               $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}" +
               $"{CompileZeroOrMoreVarNameDeclarations()}" +
               $"<symbol> {Eat(";")} </symbol>{Environment.NewLine}" +
               $"</classVarDec>{Environment.NewLine}";
    }
    
    private string CompileZeroOrMoreVarNameDeclarations()
    {
        var result = "";
        
        while (_tokenizer.CurrentToken.Value is ",")
            result += $"<symbol> {Eat(",")} </symbol>"
                      + Environment.NewLine
                      + $"<identifier> {EatTokenOfType<Identifier>()} </identifier>"
                      + Environment.NewLine;

        result = result.TrimEnd(Environment.NewLine.ToCharArray());

        return result == "" ? result : result + Environment.NewLine;
    }

    private string CompileType()
    {
        var tag = _tokenizer.CurrentToken is Keyword ? "keyword" : "identifier";
        return $@"<{tag}> {EatTypeDeclaration()} </{tag}>";
    }

    public string CompileSubroutine()
    {
        return $"<subroutineDec>{Environment.NewLine}" +
               $"<keyword> {EatAny("constructor", "function", "method")} </keyword>{Environment.NewLine}" +
               $"{CompileTypeAndVoid()}{Environment.NewLine}" +
               $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}" +
               $"<symbol> {Eat("(")} </symbol>{Environment.NewLine}" +
               $"{CompileParameterList()}" +
               $"<symbol> {Eat(")")} </symbol>{Environment.NewLine}" +
               $"{CompileSubroutineBody()}" +
               $"</subroutineDec>{Environment.NewLine}";
    }

    private string CompileSubroutineBody()
    {
        return $"<subroutineBody>{Environment.NewLine}" +
               $"<symbol> {Eat("{")} </symbol>{Environment.NewLine}" +
               $"{CompileVarDecOneOrMore()}" +
               $"{CompileStatements()}" +
               $"<symbol> {Eat("}")} </symbol>{Environment.NewLine}" +
               $"</subroutineBody>{Environment.NewLine}";
        
        string CompileVarDecOneOrMore()
        {
            var result = "";

            while (_tokenizer.CurrentToken.Value is "var")
                result += $"<varDec>{Environment.NewLine}" +
                          $"<keyword> {Eat("var")} </keyword>{Environment.NewLine}" +
                          $"{CompileType()}{Environment.NewLine}" +
                          $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}" +
                          $"{CompileZeroOrMoreVarNameDeclarations()}" +
                          $"<symbol> {Eat(";")} </symbol>{Environment.NewLine}" +
                          $"</varDec>{Environment.NewLine}"; 

            result = result.TrimEnd(Environment.NewLine.ToCharArray());
            return result == "" ? result : result + Environment.NewLine;
        }
    }

    private string CompileTypeAndVoid()
    {
        var tag = _tokenizer.CurrentToken is Keyword ? "keyword" : "identifier";
        return $@"<{tag}> {EatReturnTypeDeclaration()} </{tag}>";
    }

    private string CompileParameterList()
    {
        var result = $"<parameterList>{Environment.NewLine}";
        if (IsType(_tokenizer.CurrentToken))
        {
            result += $"{CompileType()}{Environment.NewLine}" +
                      $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}" +
                      $"{CompileParametersVariablesNames()}";
        }
                     
        return result + $"</parameterList>{Environment.NewLine}";
        
        string CompileParametersVariablesNames()
        {
            var variablesCompilation = "";
            while (_tokenizer.CurrentToken.Value is ",")
                variablesCompilation += $"<symbol> {Eat(",")} </symbol>{Environment.NewLine}" +
                                        $"{CompileType()}{Environment.NewLine}" +
                                        $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}";
            
            
            variablesCompilation = variablesCompilation.TrimEnd(Environment.NewLine.ToCharArray());
            return variablesCompilation == "" ? variablesCompilation : variablesCompilation + Environment.NewLine;
        }

    }

    private bool IsType(Token token) =>
        token.Value is "int" or "char" or "boolean" ||
        token is Identifier;

    private string CompileStatements()
    {
        var result = $"<statements>{Environment.NewLine}";
        while (_tokenizer.CurrentToken.Value is "let" or "if" or "while" or "do" or "return")
        {
            switch (_tokenizer.CurrentToken.Value)
            {
                case "let":
                    result += CompileLet();
                    break;
                case "if":
                    result += CompileIf();
                    break;
                case "while":
                    result += CompileWhile();
                    break;
                case "do":
                    result += CompileDo();
                    break;
                case "return":
                    result += CompileReturn();
                    break;
            }
        }
        return result + $"</statements>{Environment.NewLine}";
    }
    
    private string CompileDo()
    {
        return $"<doStatement>{Environment.NewLine}" +
               $"<keyword> {Eat("do")} </keyword>{Environment.NewLine}" +
               $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}" +
               $"{CompileSubroutineCallBody()}" +
               $"<symbol> {Eat(";")} </symbol>{Environment.NewLine}" +
               $"</doStatement>{Environment.NewLine}";
    }
    
    private string CompileLet()
    {
        var result = $"<letStatement>{Environment.NewLine}" +
                     $"<keyword> {Eat("let")} </keyword>{Environment.NewLine}" +
                     $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}";
        
        if (_tokenizer.CurrentToken.Value is "[")
            result += $"<symbol> {Eat("[")} </symbol>{Environment.NewLine}" +
                      $"{CompileExpression()}" +
                      $"<symbol> {Eat("]")} </symbol>{Environment.NewLine}";
        
        result += $"<symbol> {Eat("=")} </symbol>{Environment.NewLine}" +
                  $"{CompileExpression()}" +
                  $"<symbol> {Eat(";")} </symbol>{Environment.NewLine}";

        return result + $"</letStatement>{Environment.NewLine}";
    }
    
    private string CompileWhile()
    {
        return $"<whileStatement>{Environment.NewLine}" +
               $"<keyword> {Eat("while")} </keyword>{Environment.NewLine}" +
               $"<symbol> {Eat("(")} </symbol>{Environment.NewLine}" +
               $"{CompileExpression()}" +
               $"<symbol> {Eat(")")} </symbol>{Environment.NewLine}" +
               $"<symbol> {Eat("{")} </symbol>{Environment.NewLine}" +
               $"{CompileStatements()}" +
               $"<symbol> {Eat("}")} </symbol>{Environment.NewLine}" +
               $"</whileStatement>{Environment.NewLine}";
    }

    private string CompileReturn()
    {
        var result = $"<returnStatement>{Environment.NewLine}" +
                     $"<keyword> {Eat("return")} </keyword>{Environment.NewLine}" +
                     $"{CompileReturnExpression()}";

        return result +
               $"<symbol> {Eat(";")} </symbol>{Environment.NewLine}" +
               $"</returnStatement>{Environment.NewLine}";

        string CompileReturnExpression() => 
            IsTerm(_tokenizer.CurrentToken) ? CompileExpression() : "";
    }

    private bool IsTerm(Token token)
    {
        // integerConstant | stringConstant | varName | varName '[' expression ']' | subroutineCall 
        if (token is IntConst or StringConst or Identifier)
            return true;
        
        // keywordConstant
        if (token.Value is "true" or "false" or "null" or "this")
            return true;
        
        // '(' expression ')' 
        if (token.Value is "(")
            return true;

        // unaryOp term
        if (token.Value is "-" or "~")
            return true;

        return false;
    }

    private string CompileIf()
    {
        var result = $"<ifStatement>{Environment.NewLine}" +
                     $"<keyword> {Eat("if")} </keyword>{Environment.NewLine}" +
                     $"<symbol> {Eat("(")} </symbol>{Environment.NewLine}" +
                     $"{CompileExpression()}" +
                     $"<symbol> {Eat(")")} </symbol>{Environment.NewLine}" +
                     $"<symbol> {Eat("{")} </symbol>{Environment.NewLine}" +
                     $"{CompileStatements()}" +
                     $"<symbol> {Eat("}")} </symbol>{Environment.NewLine}";
        
        if(_tokenizer.CurrentToken.Value is "else")
            result += $"<keyword> {Eat("else")} </keyword>{Environment.NewLine}" +
                      $"<symbol> {Eat("{")} </symbol>{Environment.NewLine}" +
                      $"{CompileStatements()}" +
                      $"<symbol> {Eat("}")} </symbol>{Environment.NewLine}";

        return result + $"</ifStatement>{Environment.NewLine}";
    }
    
    private string CompileExpression()
    {
        if (!IsTerm(_tokenizer.CurrentToken))
            return "";
        
        var result = $"<expression>{Environment.NewLine}" +
                     $"{CompileTerm()}";
        
        if(_tokenizer.CurrentToken.Value is "+" or "-" or "*" or "/" or "&" or "|" or "<" or ">" or "=")
            result += $"<symbol> {EatAny("+", "-", "*", "/", "&", "|", "<", ">", "=")} </symbol>{Environment.NewLine}" +
                      $"{CompileTerm()}";
        
        return result + $"</expression>{Environment.NewLine}";
    }

    private string CompileTerm()
    {
        var result = $"<term>{Environment.NewLine}";
        var token = _tokenizer.CurrentToken;
        // integerConstant | stringConstant | varName | varName '[' expression ']' | subroutineCall 
        if (token is IntConst)
            result += $"<integerConstant> {EatTokenOfType<IntConst>()} </integerConstant>{Environment.NewLine}";
        else if (token is StringConst)
            result += $"<stringConstant> {EatTokenOfType<StringConst>()} </stringConstant>{Environment.NewLine}";
        else if (token.Value is "true" or "false" or "null" or "this")
            result += $"<keywordConstant> {EatAny("true", "false", "null", "this")} </keywordConstant>{Environment.NewLine}";
        else if (token is Identifier)
        {
            result += $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}";

            if (_tokenizer.CurrentToken.Value is "[")
                result += $"<symbol> {Eat("[")} </symbol>{Environment.NewLine}" +
                          $"{CompileExpression()}" +
                          $"<symbol> {Eat("]")} </symbol>{Environment.NewLine}";
            else if (_tokenizer.CurrentToken.Value is "(" or ".")
                result += $"{CompileSubroutineCallBody()}";
            
        }
        else if (token.Value is "(")
            result += $"<symbol> {Eat("(")} </symbol>{Environment.NewLine}" +
                      $"{CompileExpression()}" +
                      $"<symbol> {Eat(")")} </symbol>{Environment.NewLine}";
        else if (token.Value is "-" or "~")
            result += $"<symbol> {EatAny("-", "~")} </symbol>{Environment.NewLine}" +
                      $"{CompileTerm()}";
        
        return result + $"</term>{Environment.NewLine}";
    }

    private string CompileSubroutineCallBody()
    {
        if(_tokenizer.CurrentToken.Value is "(")
            return $"<symbol> {Eat("(")} </symbol>{Environment.NewLine}" +
                   $"{CompileExpressionList()}" +
                   $"<symbol> {Eat(")")} </symbol>{Environment.NewLine}";
        if (_tokenizer.CurrentToken.Value is ".")
            return $"<symbol> {Eat(".")} </symbol>{Environment.NewLine}" +
                   $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}" +
                   $"<symbol> {Eat("(")} </symbol>{Environment.NewLine}" +
                   $"{CompileExpressionList()}" +
                   $"<symbol> {Eat(")")} </symbol>{Environment.NewLine}";
        throw new Exception($"Expected '(' or '.' but got {_tokenizer.CurrentToken.Value}");
    }

    private string CompileExpressionList()
    {
        var result = $"<expressionList>{Environment.NewLine}" +
                     $"{CompileExpression()}";
        
        while (_tokenizer.CurrentToken.Value is ",")
            result += $"<symbol> {Eat(",")} </symbol>{Environment.NewLine}" +
                      $"{CompileExpression()}";
        
        return result + $"</expressionList>{Environment.NewLine}";
    }

    private string EatTokenOfType<TToken>()
        where TToken : Token
    {
        if (_tokenizer.CurrentToken is not TToken)
            throw new Exception($"Expected {typeof(TToken)} but got {_tokenizer.CurrentToken.Value}");
        var token = _tokenizer.CurrentToken.Value;
        _tokenizer.TryAdvance();
        return token;
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
    
    private string EatTypeDeclaration() => 
        _tokenizer.CurrentToken is Identifier ? EatTokenOfType<Identifier>() : EatAny("int", "char", "boolean");
    
    private string EatReturnTypeDeclaration() => 
        _tokenizer.CurrentToken is Identifier ? EatTokenOfType<Identifier>() : EatAny("int", "char", "boolean", "void");
}