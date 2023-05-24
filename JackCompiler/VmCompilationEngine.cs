namespace JackCompiler;

public class VmCompilationEngine
{
    private readonly Tokenizer _tokenizer;
    private readonly VmWriter _vmWriter;
    private SymbolsTable _symbolsTable;
    private int _branchingCounter = 0;

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
        else if (_tokenizer.CurrentToken.Value is "method")
            CompileMethod();
        else 
            throw new Exception("Expected constructor, function, or method");
    }

    private void CompileMethod()
    {
        Eat("method");
        EatReturnTypeDeclaration();
        var subroutineName = EatTokenOfType<Identifier>();
        _symbolsTable.StartSubroutine(subroutineName);
        _symbolsTable.Define("this", _symbolsTable.ClassName, SymbolInfo.SymbolLocation.Argument);
        Eat("(");
        CompileParameterList();
        Eat(")");

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
        
        _vmWriter.WritePush(VmMemorySegment.Argument, 0);
        _vmWriter.WritePop(VmMemorySegment.Pointer, 0);

        CompileStatements();
        Eat("}");
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

    private void CompileConstructor()
    {
        Eat("constructor");
        EatReturnTypeDeclaration();
        var subroutineName = EatTokenOfType<Identifier>();
        _symbolsTable.StartSubroutine(subroutineName);
        Eat("(");
        CompileParameterList();
        Eat(")");
        
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
        
        _vmWriter.WritePush(VmMemorySegment.Constant, _symbolsTable.GetCount(SymbolInfo.SymbolLocation.Field));
        _vmWriter.WriteCall("Memory.alloc", 1);
        _vmWriter.WritePop(VmMemorySegment.Pointer, 0);

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
        
        var identifier = EatTokenOfType<Identifier>();
        var symbolFound = _symbolsTable.TryGetSymbol(identifier, out var symbol);

        var subroutineName = symbolFound
            ? symbol.Type
            : identifier;
        
        var count = 0;

        if (_tokenizer.CurrentToken.Value is ".")
        {
            subroutineName += Eat(".");
            subroutineName += EatTokenOfType<Identifier>();
            
            if(symbolFound)
            {
                _vmWriter.WritePush(symbol.Location.ToVmSegment(), symbol.Index);
                count++;
            }
        }
        else
        {
            subroutineName = _symbolsTable.ClassName + "." + subroutineName;
            _vmWriter.WritePush(VmMemorySegment.Pointer, 0);
            count++;
        }

        Eat("(");
        count += CompileExpressionList();
        Eat(")");
        Eat(";");


        _vmWriter.WriteCall(subroutineName, count);
        _vmWriter.WritePop(VmMemorySegment.Temp, 0);
    }
    
    private void CompileLet()
    {
        Eat("let");
        var varName = EatTokenOfType<Identifier>();
        
        if(!_symbolsTable.TryGetSymbol(varName, out var symbol))
            throw new Exception($"Symbol {varName} not found");
        
        if (_tokenizer.CurrentToken.Value is "[")
        {
            _vmWriter.WritePush(symbol.Location.ToVmSegment(), symbol.Index);
            Eat("[");
            CompileExpression();
            Eat("]");
            _vmWriter.WriteArithmetic(ArithmeticCommand.Add);
            Eat("=");
            CompileExpression();
            Eat(";");
            
            _vmWriter.WritePop(VmMemorySegment.Temp, 0);
            _vmWriter.WritePop(VmMemorySegment.Pointer, 1);
            _vmWriter.WritePush(VmMemorySegment.Temp, 0);
            _vmWriter.WritePop(VmMemorySegment.That, 0);
        }
        else
        {
            Eat("=");
            CompileExpression();
            Eat(";");
            
            _vmWriter.WritePop(symbol.Location.ToVmSegment(), symbol.Index);
        }

        
            
    }
    
    private void CompileWhile()
    {
        var branchingCounter = _branchingCounter++;
        _vmWriter.WriteLabel($"WHILE_START{branchingCounter}");
        
        Eat("while");
        Eat("(");
        CompileExpression();
        Eat(")");
        
        _vmWriter.WriteArithmetic(ArithmeticCommand.Not);
        _vmWriter.WriteIf($"WHILE_END{branchingCounter}");
        Eat("{");
        CompileStatements();
        Eat("}");
        
        _vmWriter.WriteGoto($"WHILE_START{branchingCounter}");
        _vmWriter.WriteLabel($"WHILE_END{branchingCounter}");
    }

    private void CompileReturn()
    {
        Eat("return");
        
        if (IsTerm(_tokenizer.CurrentToken))
             CompileExpression();
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

    private void CompileIf()
    {
        var branchingCounter = _branchingCounter++;
        Eat("if");
        Eat("(");
        CompileExpression();
        Eat(")");
        _vmWriter.WriteIf($"IF_TRUE{branchingCounter}");
        _vmWriter.WriteGoto($"IF_FALSE{branchingCounter}");
        _vmWriter.WriteLabel($"IF_TRUE{branchingCounter}");
        Eat("{");
        CompileStatements();
        Eat("}");
        
        if(_tokenizer.CurrentToken.Value is "else")
        {
            _vmWriter.WriteGoto($"IF_END{branchingCounter}");
            _vmWriter.WriteLabel($"IF_FALSE{branchingCounter}");
            Eat("else");
            Eat("{");
            CompileStatements();
            Eat("}");
            _vmWriter.WriteLabel($"IF_END{branchingCounter}");
        }
        else
        {
            _vmWriter.WriteLabel($"IF_FALSE{branchingCounter}");
        }
    }

    private int CompileExpression()
    {
        if (!IsTerm(_tokenizer.CurrentToken))
            return 0;

        var counter = CompileTerm();

        while (_tokenizer.CurrentToken.Value is "+" or "-" or "*" or "/" or "&" or "|" or "<" or ">" or "=")
        {
            var op = EatAny("+", "-", "*", "/", "&", "|", "<", ">", "=");
            counter += CompileTerm();
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

    private int CompileTerm()
    {
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
            var stringConst = EatTokenOfType<StringConst>();
            _vmWriter.WritePush(VmMemorySegment.Constant, stringConst.Length);
            _vmWriter.WriteCall("String.new", 1);
            foreach (var c in stringConst)
            {
                _vmWriter.WritePush(VmMemorySegment.Constant, c);
                _vmWriter.WriteCall("String.appendChar", 2);
            }
            counter++;
        }
        else if (token.Value is "true")
        {
            Eat("true");
            _vmWriter.WritePush(VmMemorySegment.Constant, 0);
            _vmWriter.WriteArithmetic(ArithmeticCommand.Not);
            counter++;
        }
        else if (token.Value is "false" or "null")
        {
            EatAny("false", "null"); // false is same as null
            _vmWriter.WritePush(VmMemorySegment.Constant, 0);
            counter++;
        }
        else if (token.Value is "this")
        {
            Eat("this");
            _vmWriter.WritePush(VmMemorySegment.Pointer, 0);
            counter++;
        }
        else if (token is Identifier) // variable or subroutine call
        {
            var identifier = EatTokenOfType<Identifier>();
            
            if (_tokenizer.CurrentToken.Value is "[") // Array
            {
                if(!_symbolsTable.TryGetSymbol(identifier, out var symbol))
                    throw new Exception("Unknown array: " + identifier);
                
                _vmWriter.WritePush(symbol.Location.ToVmSegment(), symbol.Index);
                Eat("[");
                CompileExpression();
                Eat("]");
                _vmWriter.WriteArithmetic(ArithmeticCommand.Add);
                _vmWriter.WritePop(VmMemorySegment.Pointer, 1);
                _vmWriter.WritePush(VmMemorySegment.That, 0);
                counter++;
            } // Subroutine call
            else if (_tokenizer.CurrentToken.Value is "(" or ".") // subroutine on self or other class\variable
            {
                var symbolFound = _symbolsTable.TryGetSymbol(identifier, out var symbol);
                    
                var subroutineName = symbolFound
                    ? symbol.Type
                    : identifier; 
                
                if (_tokenizer.CurrentToken.Value is ".") // other
                {
                    subroutineName += Eat(".");
                    subroutineName += EatTokenOfType<Identifier>();
                    if(symbolFound)
                    {
                        _vmWriter.WritePush(symbol.Location.ToVmSegment(), symbol.Index);
                        counter++;
                    }
                }
                else // self
                {
                    subroutineName = _symbolsTable.ClassName + "." + subroutineName;
                    _vmWriter.WritePush(VmMemorySegment.Pointer, 0);
                    counter++;
                }
                
                Eat("(");
                counter += CompileExpressionList();
                Eat(")");
                _vmWriter.WriteCall(subroutineName, counter);
                counter = 1;
            } 
            else // variable
            {
                if(!_symbolsTable.TryGetSymbol(identifier, out var symbol))
                    throw new Exception("Unknown variable: " + identifier);
                _vmWriter.WritePush(symbol.Location.ToVmSegment(), symbol.Index);
                counter++;
            }
        }
        else if (token.Value is "(")
        {
            Eat("(");
            counter += CompileExpression();
            Eat(")");
        }
        else if (token.Value is "-" or "~")
        {
            var op = EatAny("-", "~");
            counter += CompileTerm();
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

    private int CompileExpressionList()
    {
        var counter = CompileExpression();
        
        while (_tokenizer.CurrentToken.Value is ",")
        {
            Eat(",");
            counter += CompileExpression();
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