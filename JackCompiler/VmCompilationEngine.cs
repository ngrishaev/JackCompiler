namespace JackCompiler;

public class VmCompilationEngine
{
    private readonly Tokenizer _tokenizer;
    private readonly VmWriter _vmWriter;
    private SymbolsTable _symbolsTable;

    public VmCompilationEngine(string src)
    {
        _tokenizer = new Tokenizer(src);
        _vmWriter = new VmWriter();

        if (_tokenizer.HasMoreTokens() == false)
            throw new Exception("No tokens");
        _tokenizer.TryAdvance();
    }

    public string CompileClass()
    {
        Eat("class");
        var className = EatTokenOfType<Identifier>();
        _symbolsTable = new SymbolsTable(className);
        
        Eat("{");
        while (_tokenizer.CurrentToken.Value is "static" or "field") 
            CompileClassVarDec();

        while (_tokenizer.CurrentToken.Value is "constructor" or "function" or "method") 
            CompileSubroutine();
        
        Eat("}");
        
        _symbolsTable = new SymbolsTable(className);

        return _vmWriter.ToString();
    }

    public void CompileClassVarDec()
    {
        if(SymbolInfo.IsLocation(_tokenizer.CurrentToken.Value) == false)
            throw new Exception("Expected static or field");
        
        var varLocation = SymbolInfo.GetLocation(_tokenizer.CurrentToken.Value);
        if (varLocation is SymbolInfo.SymbolLocation.Argument or SymbolInfo.SymbolLocation.Local)
            throw new Exception("Expected static or field");
        
        EatAny("static", "field");
        var type = EatTypeDeclaration();
        var varName = EatTokenOfType<Identifier>();
        
        _symbolsTable.Define(varName, type, varLocation);
        
        while (_tokenizer.CurrentToken.Value is ",")
        {
            Eat(",");
            varName = EatTokenOfType<Identifier>();
            _symbolsTable.Define(varName, type, varLocation);
        }

        Eat(";");
    }

    public void CompileSubroutine()
    {
        if (_tokenizer.CurrentToken.Value is "constructor")
            CompileConstructor();
        else if (_tokenizer.CurrentToken.Value is "function")
            CompileFunction();
        // else if (_tokenizer.CurrentToken.Value is "method")
        //     CompileMethod();
        else 
            throw new Exception("Expected constructor, function, or method");
    }

    private void CompileFunction()
    {
        Eat("function");
        EatReturnTypeDeclaration();
        var subroutineName = EatTokenOfType<Identifier>();
        _symbolsTable.StartSubroutine(subroutineName);
        Eat("(");
        CompileParameterList();
        Eat(")");
        
        CompileSubroutineBody();
        
        // If return type is void we still should return 0; 
    }

    private void CompileConstructor()
    {
        throw new NotImplementedException();
        EatAny("constructor", "function", "method");
        EatReturnTypeDeclaration();
        EatTokenOfType<Identifier>();
        Eat("(");
        CompileParameterList();
        Eat(")");
        CompileSubroutineBody();
    }

    private void CompileSubroutineBody()
    {
        Eat("{");
        
        while (_tokenizer.CurrentToken.Value is "var")
        {
            Eat("var");
            var varType = EatTypeDeclaration();
            var varName = EatTokenOfType<Identifier>();

            _symbolsTable.Define(varName, varType, SymbolInfo.SymbolLocation.Local);
            while (_tokenizer.CurrentToken.Value is ",")
            {
                Eat(",");
                varName = EatTokenOfType<Identifier>();
                _symbolsTable.Define(varName, varType, SymbolInfo.SymbolLocation.Local);
            }
            
            Eat(";");
        }
        
        _vmWriter.WriteFunction(
            _symbolsTable.ClassName + "." + _symbolsTable.SubroutineName ?? throw new Exception("Subroutine name is unknown"),
            _symbolsTable.GetCount(SymbolInfo.SymbolLocation.Local)
        );

        CompileStatements();
        Eat("}");
    }

    private void CompileParameterList()
    {
        if (IsType(_tokenizer.CurrentToken))
        {
            var varType = EatTypeDeclaration();
            var varName = EatTokenOfType<Identifier>();
            _symbolsTable.Define(varName, varType, SymbolInfo.SymbolLocation.Argument);
            
            while (_tokenizer.CurrentToken.Value is ",")
            {
                Eat(",");
                varType = EatTypeDeclaration();
                varName = EatTokenOfType<Identifier>();
                _symbolsTable.Define(varName, varType, SymbolInfo.SymbolLocation.Argument);
            }
        }
    }

    private bool IsType(Token token) =>
        token.Value is "int" or "char" or "boolean" ||
        token is Identifier;

    private string CompileStatements()
    {
        while (_tokenizer.CurrentToken.Value is "let" or "if" or "while" or "do" or "return")
        {
            switch (_tokenizer.CurrentToken.Value)
            {
                case "let":
                    CompileLet();
                    break;
                case "if":
                    CompileIf();
                    break;
                case "while":
                    CompileWhile();
                    break;
                case "do":
                    CompileDo();
                    break;
                case "return":
                    CompileReturn();
                    break;
            }
        }

        return "";
    }
    
    private void CompileDo()
    {
        Eat("do");
        var funcName = EatTokenOfType<Identifier>();
        if (_tokenizer.CurrentToken.Value is ".")
        {
            funcName += Eat(".");
            funcName += EatTokenOfType<Identifier>();
        }

        Eat("(");
        var count = CompileExpressionList();
        Eat(")");
        Eat(";");
        
        _vmWriter.WriteCall(funcName, count);
        _vmWriter.WritePop(VmMemorySegment.Temp, 0);
    }
    
    private void CompileLet()
    {
        Eat("let");
        var varName = EatTokenOfType<Identifier>();

        // TODO: Handle array
        if (_tokenizer.CurrentToken.Value is "[")
        {
            Eat("[");
            CompileExpressionNum();
            Eat("]");
        }

        Eat("=");
        CompileExpressionNum();
        Eat(";");
        
        if(!_symbolsTable.TryGetSymbol(varName, out var symbolInfo))
            throw new Exception($"Symbol {varName} not found");
        
        _vmWriter.WritePop(symbolInfo.Location.ToVmSegment(), symbolInfo.Index);
            
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

    private void CompileReturn()
    {
        Eat("return");
        
        if (IsTerm(_tokenizer.CurrentToken))
             CompileExpressionNum();
        else
            _vmWriter.WritePush(VmMemorySegment.Constant, 0);
        
        _vmWriter.WriteReturn();
        Eat(";");
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

        CompileTerm();
        
        if(_tokenizer.CurrentToken.Value is "+" or "-" or "*" or "/" or "&" or "|" or "<" or ">" or "=")
        {
            EatAny("+", "-", "*", "/", "&", "|", "<", ">", "=");
            CompileTerm();
        }

        return "";
    }
    
    private int CompileExpressionNum()
    {
        if (!IsTerm(_tokenizer.CurrentToken))
            return 0;

        var counter = CompileTermNum();

        if (_tokenizer.CurrentToken.Value is "+" or "-" or "*" or "/" or "&" or "|" or "<" or ">" or "=")
        {
            var op = EatAny("+", "-", "*", "/", "&", "|", "<", ">", "=");
            counter += CompileTermNum();
            switch (op)
            {
                case "+":
                    _vmWriter.WriteArithmetic(ArithmeticCommand.Add);
                    break;
                case "-":
                    _vmWriter.WriteArithmetic(ArithmeticCommand.Sub);
                    break;
                case "*":
                    _vmWriter.WriteCall("Math.multiply", 2);
                    break;
                case "/":
                    _vmWriter.WriteCall("Math.divide", 2);
                    break;
                case "&":
                    _vmWriter.WriteArithmetic(ArithmeticCommand.And);
                    break;
                case "|":
                    _vmWriter.WriteArithmetic(ArithmeticCommand.Or);
                    break;
                case "<":
                    _vmWriter.WriteArithmetic(ArithmeticCommand.Lt);
                    break;
                case ">":
                    _vmWriter.WriteArithmetic(ArithmeticCommand.Gt);
                    break;
                case "=":
                    _vmWriter.WriteArithmetic(ArithmeticCommand.Eq);
                    break;
                default:
                    throw new Exception("Unexpected operand: " + op);
            }

            counter = counter - 2 + 1; // Arithmetic command takes 2 arguments and returns 1
        }

        return counter;
    }

    private string CompileTerm()
    {
        var result = "";
        var token = _tokenizer.CurrentToken;
        if (token is IntConst)
        {
            var intConst = int.Parse(EatTokenOfType<IntConst>());
            _vmWriter.WritePush(VmMemorySegment.Constant, intConst);
        }
        else if (token is StringConst)
        {
            result += $"<stringConstant> {EatTokenOfType<StringConst>()} </stringConstant>{Environment.NewLine}";
        }
        else if (token.Value is "true" or "false" or "null" or "this")
        {
            result +=
                $"<keywordConstant> {EatAny("true", "false", "null", "this")} </keywordConstant>{Environment.NewLine}";
        }
        else if (token is Identifier)
        {
            result += $"<identifier> {EatTokenOfType<Identifier>()} </identifier>{Environment.NewLine}";

            if (_tokenizer.CurrentToken.Value is "[")
            {
                result += $"<symbol> {Eat("[")} </symbol>{Environment.NewLine}" +
                          $"{CompileExpression()}" +
                          $"<symbol> {Eat("]")} </symbol>{Environment.NewLine}";
            }
            else if (_tokenizer.CurrentToken.Value is "(" or ".")
            {
                result += $"{CompileSubroutineCallBody()}";
            }
        }
        else if (token.Value is "(")
        {
            result += $"<symbol> {Eat("(")} </symbol>{Environment.NewLine}" +
                      $"{CompileExpression()}" +
                      $"<symbol> {Eat(")")} </symbol>{Environment.NewLine}";
        }
        else if (token.Value is "-" or "~")
        {
            result += $"<symbol> {EatAny("-", "~")} </symbol>{Environment.NewLine}" +
                      $"{CompileTerm()}";
        }

        return "";
    }

    
    private int CompileTermNum()
    {
        var result = "";
        var counter = 0;
        var token = _tokenizer.CurrentToken;
        if (token is IntConst)
        {
            var intConst = int.Parse(EatTokenOfType<IntConst>());
            _vmWriter.WritePush(VmMemorySegment.Constant, intConst);
            counter++;
        }
        else if (token is StringConst)
        {
            result += $"<stringConstant> {EatTokenOfType<StringConst>()} </stringConstant>{Environment.NewLine}";
        }
        else if (token.Value is "true" or "false" or "null" or "this")
        {
            result +=
                $"<keywordConstant> {EatAny("true", "false", "null", "this")} </keywordConstant>{Environment.NewLine}";
        }
        else if (token is Identifier)
        {
            var name = EatTokenOfType<Identifier>();

            if (_tokenizer.CurrentToken.Value is "[")
            {
                // TODO: handle array
                Eat("[");
                counter += CompileExpressionNum();
                Eat("]");
            }
            else if (_tokenizer.CurrentToken.Value is "(" or ".")
            {
                if (_tokenizer.CurrentToken.Value is ".")
                {
                    name += Eat(".");
                    name += EatTokenOfType<Identifier>();
                }

                Eat("(");
                counter += CompileExpressionList();
                Eat(")");
                _vmWriter.WriteCall(name, counter);
                counter = 0;
            }
        }
        else if (token.Value is "(")
        {
            Eat("(");
            counter += CompileExpressionNum();
            Eat(")");
        }
        else if (token.Value is "-" or "~")
        {
            var op = EatAny("-", "~");
            counter += CompileTermNum();
            switch (op)
            {
                case "-":
                    _vmWriter.WriteArithmetic(ArithmeticCommand.Neg);
                    break;
                case "~":
                    _vmWriter.WriteArithmetic(ArithmeticCommand.Not);
                    break;
                default:
                    throw new Exception("Unknown operand: " + op);
            }
        }

        return counter;
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

    private int CompileExpressionList()
    {
        var counter = CompileExpressionNum();
        
        while (_tokenizer.CurrentToken.Value is ",")
        {
            Eat(",");
            counter = CompileExpressionNum();
        }

        return counter;
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