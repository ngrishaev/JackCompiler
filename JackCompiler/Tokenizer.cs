﻿namespace JackCompiler;

public class Tokenizer
{
    private readonly string _src;
    private int _cursor;

    private readonly List<Func<int, int>> _skippableTokensSpecification;

    public Tokenizer(string src)
    {
        _src = src;
        _cursor = 0;

        _skippableTokensSpecification = new List<Func<int, int>>()
        {
            Comment,
            WhiteSpaces,
            NewLines,
        };

        SkipUnimportantTokens();
    }

    public Token Advance()
    {
        var numberEnd = Number(_cursor);
        if (numberEnd != _cursor)
        {
            var token = new Token(TokenType.IntConst, _src.Substring(_cursor, numberEnd - _cursor));
            _cursor = numberEnd;
            SkipUnimportantTokens();
            return token;
        }
        
        var stringEnd = String(_cursor);
        if (stringEnd != _cursor)
        {
            var token = new Token(TokenType.StringConst, _src.Substring(_cursor, stringEnd - _cursor));
            _cursor = stringEnd;
            SkipUnimportantTokens();
            return token;
        }

        throw new Exception(
            $"Error while parsing at position {_cursor}. Rest of the string: {_src.Substring(_cursor)}");
    }

    private void SkipUnimportantTokens()
    {
        var start = _cursor;
        var end = start;

        do
        {
            start = end;

            foreach (var specification in _skippableTokensSpecification)
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

    /// <summary>
    /// Calculates comment token end position.
    /// Starts with // and ends with \n
    /// </summary>
    /// <param name="start">Token start calculation index</param>
    /// <returns>Token end index from start (exclusive)</returns>
    private int Comment(int start)
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

public class Token
{
    public readonly TokenType Type;
    public readonly string Value;

    public Token(TokenType type, string value)
    {
        Type = type;
        Value = value;
    }

    public override string ToString()
    {
        return $"{Type}:{Value}";
    }
}

public enum TokenType
{
    Keyword,
    Symbol,
    Identifier,
    IntConst,
    StringConst
}