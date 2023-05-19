using System.Text;

namespace JackCompiler;

public class VmWriter
{
    private readonly StringBuilder _sb = new();

    public void WritePush(VmMemorySegment segment, int index)
    {
        _sb.AppendLine($"push {segment.ToString().ToLower()} {index}");
    }

    public void WritePop(VmMemorySegment segment, int index)
    {
        _sb.AppendLine($"pop {segment.ToString().ToLower()} {index}");
    }

    public void WriteArithmetic(ArithmeticCommand command)
    {
        switch (command)
        {
            case ArithmeticCommand.Add:
                _sb.AppendLine("add");
                break;
            case ArithmeticCommand.Sub:
                _sb.AppendLine("sub");
                break;
            case ArithmeticCommand.Neg:
                _sb.AppendLine("neg");
                break;
            case ArithmeticCommand.Eq:
                _sb.AppendLine("eq");
                break;
            case ArithmeticCommand.Gt:
                _sb.AppendLine("gt");
                break;
            case ArithmeticCommand.Lt:
                _sb.AppendLine("lt");
                break;
            case ArithmeticCommand.And:
                _sb.AppendLine("and");
                break;
            case ArithmeticCommand.Or:
                _sb.AppendLine("or");
                break;
            case ArithmeticCommand.Not:
                _sb.AppendLine("not");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(command), command, null);
        }
    }

    public void WriteLabel(string label)
    {
        _sb.AppendLine($"label {label}");
    }

    public void WriteGoto(string label)
    {
        _sb.AppendLine($"goto {label}");
    }

    public void WriteIf(string label)
    {
        _sb.AppendLine($"if-goto {label}");
    }

    public void WriteCall(string name, int nArgs)
    {
        _sb.AppendLine($"call {name} {nArgs}");
    }

    public void WriteFunction(string name, int nLocals)
    {
        _sb.AppendLine($"function {name} {nLocals}");
    }

    public void WriteReturn()
    {
        _sb.AppendLine("return");
    }

    public override string ToString() => 
        _sb.ToString();
}

public enum VmMemorySegment
{
    Constant,
    Argument,
    Local,
    Static,
    This,
    That,
    Pointer,
    Temp,
}

public enum ArithmeticCommand
{
    Add,
    Sub,
    Neg,
    Eq,
    Gt,
    Lt,
    And,
    Or,
    Not,
}