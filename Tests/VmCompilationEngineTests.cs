using System;
using JackCompiler;
using NUnit.Framework;

namespace Tests;

[TestFixture]
public class VmCompilationEngineTests
{
    [Test]
    public void TestSimpleClass()
    {
        var engine = new VmCompilationEngine("class Main {" +
                                             "    function void main() {" +
                                             "        do Output.printInt(42);" +
                                             "        return;" +
                                             "    }" +
                                             "}");

        var result = engine.CompileClass();
        Assert.AreEqual($"function main 0{Environment.NewLine}" +
                        $"push constant 42{Environment.NewLine}" +
                        $"call Output.printInt 1{Environment.NewLine}" +
                        $"pop temp 0{Environment.NewLine}" +
                        $"push constant 0{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }

    [Test]
    public void TestSeven()
    {
        var engine = new VmCompilationEngine("class Main {" +
                                             "    function void main() {" +
                                             "        do Output.printInt(1 + (2 * 3));" +
                                             "        return;" +
                                             "    }" +
                                             "}");

        var result = engine.CompileClass();
        Assert.AreEqual($"function Main.main 0{Environment.NewLine}" +
                        $"push constant 1{Environment.NewLine}" +
                        $"push constant 2{Environment.NewLine}" +
                        $"push constant 3{Environment.NewLine}" +
                        $"call Math.multiply 2{Environment.NewLine}" +
                        $"add{Environment.NewLine}" +
                        $"call Output.printInt 1{Environment.NewLine}" +
                        $"pop temp 0{Environment.NewLine}" +
                        $"push constant 0{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }
}