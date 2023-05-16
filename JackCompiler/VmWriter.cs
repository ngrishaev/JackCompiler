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
                throw new NotImplementedException();
                break;
            case ArithmeticCommand.Neg:
                throw new NotImplementedException();
                break;
            case ArithmeticCommand.Eq:
                throw new NotImplementedException();
                break;
            case ArithmeticCommand.Gt:
                throw new NotImplementedException();
                break;
            case ArithmeticCommand.Lt:
                throw new NotImplementedException();
                break;
            case ArithmeticCommand.And:
                throw new NotImplementedException();
                break;
            case ArithmeticCommand.Or:
                throw new NotImplementedException();
                break;
            case ArithmeticCommand.Not:
                throw new NotImplementedException();
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
    Arg,
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