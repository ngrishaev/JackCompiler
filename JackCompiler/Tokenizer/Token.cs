namespace JackCompiler;

public abstract class Token
{
    public readonly string Value;
    public TokenType Type { get; init; }

    protected Token(string value) => 
        Value = value;

    public override string ToString() => 
        $"{Type}:{Value}";

    public abstract string ToXml();
}

public class KeywordToken : Token
{
    public KeywordToken(string value) : base(value) => 
        Type = TokenType.Keyword;

    public override string ToXml() => 
        $"<keyword> {Value} </keyword>";
}

public class SymbolToken : Token
{
    public SymbolToken(string value) : base(value) => 
        Type = TokenType.Symbol;

    public override string ToXml() => 
        $"<symbol> {Value} </symbol>";
}

public class IdentifierToken : Token
{
    public IdentifierToken(string value) : base(value) => 
        Type = TokenType.Identifier;

    public override string ToXml() => 
        $"<identifier> {Value} </identifier>";
}

public class IntConstToken : Token
{
    public IntConstToken(string value) : base(value) => 
        Type = TokenType.IntConst;

    public override string ToXml() => 
        $"<integerConstant> {Value} </integerConstant>";
}

public class StringConstToken : Token
{
    public StringConstToken(string value) : base(value) => 
        Type = TokenType.StringConst;

    public override string ToXml() => 
        $"<stringConstant> {Value} </stringConstant>";
}

public enum TokenType
{
    Keyword,
    Symbol,
    Identifier,
    IntConst,
    StringConst
}