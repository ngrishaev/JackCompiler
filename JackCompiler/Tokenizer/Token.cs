namespace JackCompiler;

public abstract class Token
{
    public virtual string Value { get; }
    public TokenType Type { get; init; }

    protected Token(string value) => 
        Value = value;

    public override string ToString() => 
        $"{Type}:{Value}";

    public abstract string ToXml();
}

public class Keyword : Token
{
    public Keyword(string value) : base(value) => 
        Type = TokenType.Keyword;

    public override string ToXml() => 
        $"<keyword> {Value} </keyword>";
}

public class Symbol : Token
{
    public Symbol(string value) : base(value) => 
        Type = TokenType.Symbol;

    public override string ToXml() => 
        $"<symbol> {Value} </symbol>";
}

public class Identifier : Token
{
    public Identifier(string value) : base(value) => 
        Type = TokenType.Identifier;

    public override string ToXml() => 
        $"<identifier> {Value} </identifier>";
}

public class IntConst : Token
{
    public IntConst(string value) : base(value) => 
        Type = TokenType.IntConst;

    public override string ToXml() => 
        $"<integerConstant> {Value} </integerConstant>";
}

public class StringConst : Token
{
    public override string Value => 
        base.Value[1..^1];

    public StringConst(string value) : base(value) => 
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
    StringConst,
}