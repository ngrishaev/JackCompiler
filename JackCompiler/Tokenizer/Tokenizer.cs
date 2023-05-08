namespace JackCompiler;

public class Tokenizer
{
    private readonly string[] _keywords = new[]
    {
        "class", "constructor", "function", "method", "field", "static", "var", "int", "char", "boolean", "void",
        "true", "false", "null", "this", "let", "do", "if", "else", "while", "return"
    };

    private const string _symbols = "{}()[].,;+-*/&|<>=~";
    
    
    private readonly string _src;
    private int _cursor;

    private readonly List<Func<int, int>> _unimportantTokensSpecification;
    private readonly Dictionary<Func<int, int>, Func<string, Token>> _mainTokensSpecifications;

    public Token CurrentToken { get; private set; }

    public Tokenizer(string src)
    {
        _src = src;
        _cursor = 0;

        _unimportantTokensSpecification = new List<Func<int, int>>()
        {
            SingleComment,
            MultilineComment,
            WhiteSpaces,
            NewLines,
        };
        
        _mainTokensSpecifications = new ()
        {
            {String, (value) => new StringConst(value)},
            {Number, (value) => new IntConst(value)},
            {Keyword, (value) => new Keyword(value)},
            {Symbol, (value) => new Symbol(value)},
            {Identifier, (value) => new Identifier(value)},
        };

        SkipUnimportantTokens();
    }

    public Token Advance()
    {
        foreach (var (tokenExtractor, tokenConstructor) in _mainTokensSpecifications)
        {
            var tokenEnd = tokenExtractor(_cursor);
            if (tokenEnd != _cursor)
            {
                CurrentToken = tokenConstructor(_src.Substring(_cursor, tokenEnd - _cursor));
                _cursor = tokenEnd;
                SkipUnimportantTokens();
                return CurrentToken;
            }
        }

        throw new Exception(
            $"Error while parsing at position {_cursor}. Rest of the string: {_src.Substring(_cursor)}");
    }

    public bool TryAdvance()
    {
        if (!HasMoreTokens())
            return false;
        
        Advance();
        return true;
    }

    private void SkipUnimportantTokens()
    {
        var start = _cursor;
        var end = start;

        do
        {
            start = end;

            foreach (var specification in _unimportantTokensSpecification)
                end = specification(end);
        } while (start != end);

        _cursor = end;
    }

    /// <summary>
    /// Calculates number token end position.
    /// Token should start with digit and end with non-digit character. 
    /// </summary>
    /// <param name="start">From where start token calculation</param>
    /// <returns>Token end index from start (exclusive)</returns>
    private int Number(int start)
    {
        var endToken = start;
        while (endToken < _src.Length && char.IsDigit(_src[endToken]))
            endToken++;
        return endToken;
    }

    private int String(int start)
    {
        if(_src[start] != '"')
            return start;
        var endToken = start + 1;
        while (endToken < _src.Length && _src[endToken] != '"')
            endToken++;
        return ++endToken;
    }
    
    private int Symbol(int start)
    {
        return _symbols.Contains(_src[start]) ? start + 1 : start;
    }
    
    private int Identifier(int start)
    {
        var end = start;
        
        if (char.IsDigit(_src[start]))
            return start;
        
        while (end < _src.Length && (char.IsLetterOrDigit(_src[end]) || _src[end] == '_'))
            end++;

        return end;
    }
    
    private int Keyword(int start)
    {
        var end = start;
        while (end < _src.Length && char.IsLetter(_src[end]))
            end++;
        
        if(end == start)
            return start;
        
        var keyword = _src.Substring(start, end - start);
        return _keywords.Contains(keyword) ? end : start;
    }

    /// <summary>
    /// Calculates comment token end position.
    /// Starts with // and ends with \n
    /// </summary>
    /// <param name="start">Token start calculation index</param>
    /// <returns>Token end index from start (exclusive)</returns>
    private int SingleComment(int start)
    {
        var endToken = start;
        if (_src.Length - endToken < 2)
            return endToken;
        if (_src[endToken] != '/' || _src[endToken + 1] != '/')
            return endToken;
        while (endToken < _src.Length && _src[endToken] != '\n')
            endToken++;
        return endToken;
    }
    
    private int MultilineComment(int start)
    {
        var endToken = start;
        if (_src.Length - endToken < 2)
            return endToken;
        
        if (_src[endToken] != '/' || _src[endToken + 1] != '*')
            return endToken;
        
        endToken += 2;
        
        while (endToken < _src.Length - 1 && !(_src[endToken] == '*' && _src[endToken + 1] == '/'))
            endToken++;

        return endToken + 2;
    }

    /// <summary>
    /// Calculates white spaces token end position.
    /// </summary>
    /// <param name="start">Token start calculation index</param>
    /// <returns>Token end index from start (exclusive)</returns>
    private int WhiteSpaces(int start)
    {
        var end = start;
        while (end < _src.Length && char.IsWhiteSpace(_src[end]))
            end++;
        return end;
    }

    /// <summary>
    /// Calculates new lines token end position. Consist of one or more \n
    /// </summary>
    /// <param name="start"></param>
    /// <returns></returns>
    private int NewLines(int start)
    {
        var end = start;
        while (end < _src.Length && _src[end] == '\n')
            end++;
        return end;
    }

    public bool HasMoreTokens() =>
        _cursor != _src.Length;
}

