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
        Assert.AreEqual($"function Main.main 0{Environment.NewLine}" +
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
    
    [Test]
    public void TestNegate()
    {
        var engine = new VmCompilationEngine("class Main {" +
                                             "    function void main() {" +
                                             "        do Output.printInt(-1);" +
                                             "        return;" +
                                             "    }" +
                                             "}");

        var result = engine.CompileClass();
        Assert.AreEqual($"function Main.main 0{Environment.NewLine}" +
                        $"push constant 1{Environment.NewLine}" +
                        $"neg{Environment.NewLine}" +
                        $"call Output.printInt 1{Environment.NewLine}" +
                        $"pop temp 0{Environment.NewLine}" +
                        $"push constant 0{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }
    
    [Test]
    public void TestLet()
    {
        var engine = new VmCompilationEngine("class Main {"+
                                             "    function void main() {"+
                                             "        var int value;"+
                                             "        let value = Memory.peek(8000);" +
                                             "        return;"+
                                             "    }"+
                                             "}");
        

        var result = engine.CompileClass();
        Assert.AreEqual($"function Main.main 1{Environment.NewLine}" +
                        $"push constant 8000{Environment.NewLine}" +
                        $"call Memory.peek 1{Environment.NewLine}" +
                        $"pop local 0{Environment.NewLine}" +
                        $"push constant 0{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }
    
    [Test]
    public void TestLetMultipleVariables()
    {
        var engine = new VmCompilationEngine("class Main {" +
                                             "    function void main() {" +
                                             "        var int value, value2;" +
                                             "        let value = 5;" +
                                             "        let value2 = value + 1;" +
                                             "        return;" +
                                             "    }" +
                                             "}"
        );
        

        var result = engine.CompileClass();
        Assert.AreEqual($"function Main.main 2{Environment.NewLine}"+
                        $"push constant 5{Environment.NewLine}"+
                        $"pop local 0{Environment.NewLine}"+
                        $"push local 0{Environment.NewLine}"+
                        $"push constant 1{Environment.NewLine}"+
                        $"add{Environment.NewLine}"+
                        $"pop local 1{Environment.NewLine}"+
                        $"push constant 0{Environment.NewLine}"+
                        $"return{Environment.NewLine}", result);
    }
    
    [Test]
    public void TestIf()
    {
        var engine = new VmCompilationEngine("class Main {" +
                                             "  function void main() {"+
                                             "      if(4 > 5)"+
                                             "      {"+
                                             "          do Output.printInt(100);"+
                                             "      }"+
                                             "      else"+ 
                                             "      {"+
                                             "          do Output.printInt(200);"+
                                             "      }"+
                                             "  return;" +
                                             "  }"+
                                             "}");
        
        var result = engine.CompileClass();
        Assert.AreEqual($"function Main.main 0{Environment.NewLine}" +
                        $"push constant 4{Environment.NewLine}" +
                        $"push constant 5{Environment.NewLine}" +
                        $"gt{Environment.NewLine}" +
                        $"if-goto IF_TRUE0{Environment.NewLine}" +
                        $"goto IF_FALSE0{Environment.NewLine}" +
                        $"label IF_TRUE0{Environment.NewLine}" +
                        $"push constant 100{Environment.NewLine}" +
                        $"call Output.printInt 1{Environment.NewLine}" +
                        $"pop temp 0{Environment.NewLine}" +
                        $"goto IF_END0{Environment.NewLine}" +
                        $"label IF_FALSE0{Environment.NewLine}" +
                        $"push constant 200{Environment.NewLine}" +
                        $"call Output.printInt 1{Environment.NewLine}" +
                        $"pop temp 0{Environment.NewLine}" +
                        $"label IF_END0{Environment.NewLine}" +
                        $"push constant 0{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }
    
    [Test]
    public void TestWhile()
    {
        var engine = new VmCompilationEngine("class Main {" +
                                             "    function void main() {" +
                                             "        var int i;" +
                                             "        let i = 0;" +
                                             "        while(i < 3){" +
                                             "            let i = i + 1;" +
                                             "            do Output.printInt(i);" +
                                             "        }" +
                                             "        return;" +
                                             "    }" +
                                             "}");
        

        var result = engine.CompileClass();
        Assert.AreEqual($"function Main.main 1{Environment.NewLine}" +
                        $"push constant 0{Environment.NewLine}" +
                        $"pop local 0{Environment.NewLine}" +
                        $"label WHILE_START0{Environment.NewLine}" +
                        $"push local 0{Environment.NewLine}" +
                        $"push constant 3{Environment.NewLine}" +
                        $"lt{Environment.NewLine}" +
                        $"not{Environment.NewLine}" +
                        $"if-goto WHILE_END0{Environment.NewLine}" +
                        $"push local 0{Environment.NewLine}" +
                        $"push constant 1{Environment.NewLine}" +
                        $"add{Environment.NewLine}" +
                        $"pop local 0{Environment.NewLine}" +
                        $"push local 0{Environment.NewLine}" +
                        $"call Output.printInt 1{Environment.NewLine}" +
                        $"pop temp 0{Environment.NewLine}" +
                        $"goto WHILE_START0{Environment.NewLine}" +
                        $"label WHILE_END0{Environment.NewLine}" +
                        $"push constant 0{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }
    
        [Test]
    public void TestStringConst()
    {
        var engine = new VmCompilationEngine("class Main {" +
                                             "    function void main() {" +
                                             "        var string str;" +
                                             "        let str = \"Hello!\";" +
                                             "        do Output.printInt(str);" +
                                             "        return;" +
                                             "    }" +
                                             "}");
        
        var result = engine.CompileClass();
        Assert.AreEqual($"function Main.main 1{Environment.NewLine}" +
                        $"push constant 6{Environment.NewLine}" +
                        $"call String.new 1{Environment.NewLine}" +
                        $"push constant 72{Environment.NewLine}" +
                        $"call String.appendChar 2{Environment.NewLine}" +
                        $"push constant 101{Environment.NewLine}" +
                        $"call String.appendChar 2{Environment.NewLine}" +
                        $"push constant 108{Environment.NewLine}" +
                        $"call String.appendChar 2{Environment.NewLine}" +
                        $"push constant 108{Environment.NewLine}" +
                        $"call String.appendChar 2{Environment.NewLine}" +
                        $"push constant 111{Environment.NewLine}" +
                        $"call String.appendChar 2{Environment.NewLine}" +
                        $"push constant 33{Environment.NewLine}" +
                        $"call String.appendChar 2{Environment.NewLine}" +
                        $"pop local 0{Environment.NewLine}" +
                        $"push local 0{Environment.NewLine}" +
                        $"call Output.printInt 1{Environment.NewLine}" +
                        $"pop temp 0{Environment.NewLine}" +
                        $"push constant 0{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }
    
    [Test]    
    public void TestOtherObjectMethodCall()
    {
        var engine = new VmCompilationEngine("class Main {" +
                                             "    function void main() {" +
                                             "        var Test test;" +
                                             "        let test = Test.new();" +
                                             "        do test.bar();" +
                                             "        return;" +
                                             "    }" +
                                             "}");
        
        var result = engine.CompileClass();
        Assert.AreEqual($"function Main.main 1{Environment.NewLine}" +
                        $"call Test.new 0{Environment.NewLine}" +
                        $"pop local 0{Environment.NewLine}" +
                        $"push local 0{Environment.NewLine}" +
                        $"call Test.bar 1{Environment.NewLine}" +
                        $"pop temp 0{Environment.NewLine}" +
                        $"push constant 0{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }
    
    [Test]    
    public void TestConstructor()
    {
        var engine = new VmCompilationEngine("class Test {" +
                                             "    field int x, y;" +
                                             "    static int s;" +
                                             "    constructor Test new() {" +
                                             "        return this;" +
                                             "    }" +
                                             "}");
        
        var result = engine.CompileClass();
        Assert.AreEqual($"function Test.new 0{Environment.NewLine}" +
                        $"push constant 2{Environment.NewLine}" +
                        $"call Memory.alloc 1{Environment.NewLine}" +
                        $"pop pointer 0{Environment.NewLine}" +
                        $"push pointer 0{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }
    
    [Test]    
    public void TestMethod()
    {
        var engine = new VmCompilationEngine("class Test {" +
                                             "    field int x, y;" +
                                             "    static int s;" +
                                             "    method int bar(int x, int y) {" +
                                             "        return x + y;" +
                                             "    }" +
                                             "}");
        
        var result = engine.CompileClass();
        Assert.AreEqual($"function Test.bar 0{Environment.NewLine}" +
                        $"push argument 0{Environment.NewLine}" +
                        $"pop pointer 0{Environment.NewLine}" +
                        $"push argument 1{Environment.NewLine}" +
                        $"push argument 2{Environment.NewLine}" +
                        $"add{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }
    
    [Test]    
    public void TestOtherObjectMethodCallAsPartOfExpression()
    {
        var engine = new VmCompilationEngine("class Main {" +
                                             "    function void main() {" +
                                             "        var Test test;" +
                                             "        let test = Test.new();" +
                                             "        do Output.printInt(test.bar());" +
                                             "        return;" +
                                             "    }" +
                                             "}");
        
        var result = engine.CompileClass();
        Assert.AreEqual($"function Main.main 1{Environment.NewLine}" +
                        $"call Test.new 0{Environment.NewLine}" +
                        $"pop local 0{Environment.NewLine}" +
                        $"push local 0{Environment.NewLine}" +
                        $"call Test.bar 1{Environment.NewLine}" +
                        $"call Output.printInt 1{Environment.NewLine}" +
                        $"pop temp 0{Environment.NewLine}" +
                        $"push constant 0{Environment.NewLine}" +
                        $"return{Environment.NewLine}", result);
    }
    
        [Test]    
        public void TestSelfMethodCallInExpression()
        {
            var engine = new VmCompilationEngine("class Test {" +
                                                 "    field int x, y;" +
                                                 "    static int s;" +
                                                 "    constructor Test new() {" +
                                                 //"        do Output.printInt(bar(3, 4));" +
                                                 "        return this;" +
                                                 "    }" +
                                                 "    method int bar(int x, int y){" +
                                                 "        return x + y;" +
                                                 "    }" +
                                                 "}");
            
            var result = engine.CompileClass();
            Assert.AreEqual($"function Test.new 0{Environment.NewLine}" +
                            $"push constant 2{Environment.NewLine}" +
                            $"call Memory.alloc 1{Environment.NewLine}" +
                            $"pop pointer 0{Environment.NewLine}" +
                            $"push pointer 0{Environment.NewLine}" +
                            $"push constant 3{Environment.NewLine}" +
                            $"push constant 4{Environment.NewLine}" +
                            $"call Test.bar 3{Environment.NewLine}" +
                            $"call Output.printInt 1{Environment.NewLine}" +
                            $"pop temp 0{Environment.NewLine}" +
                            $"push pointer 0{Environment.NewLine}" +
                            $"return{Environment.NewLine}" +
                            $"function Test.bar 0{Environment.NewLine}" +
                            $"push argument 0{Environment.NewLine}" +
                            $"pop pointer 0{Environment.NewLine}" +
                            $"push argument 1{Environment.NewLine}" +
                            $"push argument 2{Environment.NewLine}" +
                            $"add{Environment.NewLine}" +
                            $"return{Environment.NewLine}", result);
        }
        
        [Test]    
        public void TestSelfMethodCall()
        {
            var engine = new VmCompilationEngine("class Test {" +
                                                 "    field int x, y;" +
                                                 "    static int s;" +
                                                 "    constructor Test new() {" +
                                                 "        do bar(3, 4);" +
                                                 "        return this;" +
                                                 "    }" +
                                                 "    method int bar(int x, int y){" +
                                                 "        return x + y;" +
                                                 "    }" +
                                                 "}");
            
            var result = engine.CompileClass();
            Assert.AreEqual($"function Test.new 0{Environment.NewLine}" +
                            $"push constant 2{Environment.NewLine}" +
                            $"call Memory.alloc 1{Environment.NewLine}" +
                            $"pop pointer 0{Environment.NewLine}" +
                            $"push pointer 0{Environment.NewLine}" +
                            $"push constant 3{Environment.NewLine}" +
                            $"push constant 4{Environment.NewLine}" +
                            $"call Test.bar 3{Environment.NewLine}" +
                            $"pop temp 0{Environment.NewLine}" +
                            $"push pointer 0{Environment.NewLine}" +
                            $"return{Environment.NewLine}" +
                            $"function Test.bar 0{Environment.NewLine}" +
                            $"push argument 0{Environment.NewLine}" +
                            $"pop pointer 0{Environment.NewLine}" +
                            $"push argument 1{Environment.NewLine}" +
                            $"push argument 2{Environment.NewLine}" +
                            $"add{Environment.NewLine}" +
                            $"return{Environment.NewLine}", result);
        }
}