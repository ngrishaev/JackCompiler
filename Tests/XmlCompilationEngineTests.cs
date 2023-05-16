using System;
using JackCompiler;
using NUnit.Framework;

namespace Tests;

public class XmlCompilationEngineTests
{
    [Test]
    public void TestEmptyClass()
    {
        var engine = new XmlCompilationEngine("class Main {}");
        var result = engine.CompileClass();

        Assert.AreEqual($"<class>{Environment.NewLine}" +
                        $"<keyword> class </keyword>{Environment.NewLine}" +
                        $"<identifier> Main </identifier>{Environment.NewLine}" +
                        $"<symbol> {{ </symbol>{Environment.NewLine}" +
                        $"<symbol> }} </symbol>{Environment.NewLine}" +
                        $"</class>", result);
    }

    [Test]
    public void TestClassVariableStaticField()
    {
        var engine = new XmlCompilationEngine(@"static int x;");

        var result = engine.CompileClassVarDec();

        Assert.AreEqual($"<classVarDec>{Environment.NewLine}" +
                        $"<keyword> static </keyword>{Environment.NewLine}" +
                        $"<keyword> int </keyword>{Environment.NewLine}" +
                        $"<identifier> x </identifier>{Environment.NewLine}" +
                        $"<symbol> ; </symbol>{Environment.NewLine}" +
                        $"</classVarDec>{Environment.NewLine}", result);
    }

    [Test]
    public void TestClassVariableLocalFieldMultiple()
    {
        var engine = new XmlCompilationEngine("field int x, y;");

        var result = engine.CompileClassVarDec();

        Assert.AreEqual($"<classVarDec>{Environment.NewLine}" +
                        $"<keyword> field </keyword>{Environment.NewLine}" +
                        $"<keyword> int </keyword>{Environment.NewLine}" +
                        $"<identifier> x </identifier>{Environment.NewLine}" +
                        $"<symbol> , </symbol>{Environment.NewLine}" +
                        $"<identifier> y </identifier>{Environment.NewLine}" +
                        $"<symbol> ; </symbol>{Environment.NewLine}" +
                        $"</classVarDec>{Environment.NewLine}", result);
    }
    
    [Test]
    public void ReturnStatement()
    {
        var engine = new XmlCompilationEngine("method int run(int x, int y, Point p) {" +
                                           "var int x, y;" +
                                           "var Point p;" +
                                           "return 15;" +
                                           "}");

        var result = engine.CompileSubroutine();

        Assert.AreEqual($"<subroutineDec>{Environment.NewLine}" +
                        $"<keyword> method </keyword>{Environment.NewLine}" +
                        $"<keyword> int </keyword>{Environment.NewLine}" +
                        $"<identifier> run </identifier>{Environment.NewLine}" +
                        $"<symbol> ( </symbol>{Environment.NewLine}" +
                        $"<parameterList>{Environment.NewLine}" +
                        $"<keyword> int </keyword>{Environment.NewLine}" +
                        $"<identifier> x </identifier>{Environment.NewLine}" +
                        $"<symbol> , </symbol>{Environment.NewLine}" +
                        $"<keyword> int </keyword>{Environment.NewLine}" +
                        $"<identifier> y </identifier>{Environment.NewLine}" +
                        $"<symbol> , </symbol>{Environment.NewLine}" +
                        $"<identifier> Point </identifier>{Environment.NewLine}" +
                        $"<identifier> p </identifier>{Environment.NewLine}" +
                        $"</parameterList>{Environment.NewLine}" +
                        $"<symbol> ) </symbol>{Environment.NewLine}" +
                        $"<subroutineBody>{Environment.NewLine}" +
                        $"<symbol> {{ </symbol>{Environment.NewLine}" +
                        $"<varDec>{Environment.NewLine}" +
                        $"<keyword> var </keyword>{Environment.NewLine}" +
                        $"<keyword> int </keyword>{Environment.NewLine}" +
                        $"<identifier> x </identifier>{Environment.NewLine}" +
                        $"<symbol> , </symbol>{Environment.NewLine}" +
                        $"<identifier> y </identifier>{Environment.NewLine}" +
                        $"<symbol> ; </symbol>{Environment.NewLine}" +
                        $"</varDec>{Environment.NewLine}" +
                        $"<varDec>{Environment.NewLine}" +
                        $"<keyword> var </keyword>{Environment.NewLine}" +
                        $"<identifier> Point </identifier>{Environment.NewLine}" +
                        $"<identifier> p </identifier>{Environment.NewLine}" +
                        $"<symbol> ; </symbol>{Environment.NewLine}" +
                        $"</varDec>{Environment.NewLine}" +
                        $"<statements>{Environment.NewLine}" +
                        $"<returnStatement>{Environment.NewLine}" +
                        $"<keyword> return </keyword>{Environment.NewLine}" +
                        $"<expression>{Environment.NewLine}" +
                        $"<term>{Environment.NewLine}" +
                        $"<integerConstant> 15 </integerConstant>{Environment.NewLine}" +
                        $"</term>{Environment.NewLine}" +
                        $"</expression>{Environment.NewLine}" +
                        $"<symbol> ; </symbol>{Environment.NewLine}" +
                        $"</returnStatement>{Environment.NewLine}" +
                        $"</statements>{Environment.NewLine}" +
                        $"<symbol> }} </symbol>{Environment.NewLine}" +
                        $"</subroutineBody>{Environment.NewLine}" +
                        $"</subroutineDec>{Environment.NewLine}", result);
    }
    
    [Test]
    public void LetStatement()
    {
        var engine = new XmlCompilationEngine("method void run() {" +
                                           "var int x;" +
                                           "let x = 15;" +
                                           "return;" +
                                           "}");

        var result = engine.CompileSubroutine();

        Assert.AreEqual(
            $"<subroutineDec>{Environment.NewLine}" +
            $"<keyword> method </keyword>{Environment.NewLine}" +
            $"<keyword> void </keyword>{Environment.NewLine}" +
            $"<identifier> run </identifier>{Environment.NewLine}" +
            $"<symbol> ( </symbol>{Environment.NewLine}" +
            $"<parameterList>{Environment.NewLine}" +
            $"</parameterList>{Environment.NewLine}" +
            $"<symbol> ) </symbol>{Environment.NewLine}" +
            $"<subroutineBody>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<varDec>{Environment.NewLine}" +
            $"<keyword> var </keyword>{Environment.NewLine}" +
            $"<keyword> int </keyword>{Environment.NewLine}" +
            $"<identifier> x </identifier>{Environment.NewLine}" +
            $"<symbol> ; </symbol>{Environment.NewLine}" +
            $"</varDec>{Environment.NewLine}" +
            $"<statements>{Environment.NewLine}" +
            $"<letStatement>{Environment.NewLine}" +
            $"<keyword> let </keyword>{Environment.NewLine}" +
            $"<identifier> x </identifier>{Environment.NewLine}" +
            $"<symbol> = </symbol>{Environment.NewLine}" +
            $"<expression>{Environment.NewLine}" +
            $"<term>{Environment.NewLine}" +
            $"<integerConstant> 15 </integerConstant>{Environment.NewLine}" +
            $"</term>{Environment.NewLine}" +
            $"</expression>{Environment.NewLine}" +
            $"<symbol> ; </symbol>{Environment.NewLine}" +
            $"</letStatement>{Environment.NewLine}" +
            $"<returnStatement>{Environment.NewLine}" +
            $"<keyword> return </keyword>{Environment.NewLine}" +
            $"<symbol> ; </symbol>{Environment.NewLine}" +
            $"</returnStatement>{Environment.NewLine}" +
            $"</statements>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</subroutineBody>{Environment.NewLine}" +
            $"</subroutineDec>{Environment.NewLine}", result);
    }
    
    [Test]
    public void DoStatement()
    {
        var engine = new XmlCompilationEngine("method void run() {" +
                                           "do Output.println();" +
                                           "}");

        var result = engine.CompileSubroutine();

        Assert.AreEqual(
            $"<subroutineDec>{Environment.NewLine}" +
            $"<keyword> method </keyword>{Environment.NewLine}" +
            $"<keyword> void </keyword>{Environment.NewLine}" +
            $"<identifier> run </identifier>{Environment.NewLine}" +
            $"<symbol> ( </symbol>{Environment.NewLine}" +
            $"<parameterList>{Environment.NewLine}" +
            $"</parameterList>{Environment.NewLine}" +
            $"<symbol> ) </symbol>{Environment.NewLine}" +
            $"<subroutineBody>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<statements>{Environment.NewLine}" +
            $"<doStatement>{Environment.NewLine}" +
            $"<keyword> do </keyword>{Environment.NewLine}" +
            $"<identifier> Output </identifier>{Environment.NewLine}" +
            $"<symbol> . </symbol>{Environment.NewLine}" +
            $"<identifier> println </identifier>{Environment.NewLine}" +
            $"<symbol> ( </symbol>{Environment.NewLine}" +
            $"<expressionList>{Environment.NewLine}" +
            $"</expressionList>{Environment.NewLine}" +
            $"<symbol> ) </symbol>{Environment.NewLine}" +
            $"<symbol> ; </symbol>{Environment.NewLine}" +
            $"</doStatement>{Environment.NewLine}" +
            $"</statements>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</subroutineBody>{Environment.NewLine}" +
            $"</subroutineDec>{Environment.NewLine}", result);
    }
    
    [Test]
    public void IfStatement()
    {
        var engine = new XmlCompilationEngine("method void run() {" +
                                           "if(true) { return; }" +
                                           "}");

        var result = engine.CompileSubroutine();

        Assert.AreEqual(
            $"<subroutineDec>{Environment.NewLine}" +
            $"<keyword> method </keyword>{Environment.NewLine}" +
            $"<keyword> void </keyword>{Environment.NewLine}" +
            $"<identifier> run </identifier>{Environment.NewLine}" +
            $"<symbol> ( </symbol>{Environment.NewLine}" +
            $"<parameterList>{Environment.NewLine}" +
            $"</parameterList>{Environment.NewLine}" +
            $"<symbol> ) </symbol>{Environment.NewLine}" +
            $"<subroutineBody>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<statements>{Environment.NewLine}" +
            $"<ifStatement>{Environment.NewLine}" +
            $"<keyword> if </keyword>{Environment.NewLine}" +
            $"<symbol> ( </symbol>{Environment.NewLine}" +
            $"<expression>{Environment.NewLine}" +
            $"<term>{Environment.NewLine}" +
            $"<keywordConstant> true </keywordConstant>{Environment.NewLine}" +
            $"</term>{Environment.NewLine}" +
            $"</expression>{Environment.NewLine}" +
            $"<symbol> ) </symbol>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<statements>{Environment.NewLine}" +
            $"<returnStatement>{Environment.NewLine}" +
            $"<keyword> return </keyword>{Environment.NewLine}" +
            $"<symbol> ; </symbol>{Environment.NewLine}" +
            $"</returnStatement>{Environment.NewLine}" +
            $"</statements>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</ifStatement>{Environment.NewLine}" +
            $"</statements>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</subroutineBody>{Environment.NewLine}" +
            $"</subroutineDec>{Environment.NewLine}", result);
    }
    
    [Test]
    public void IfElseStatement()
    {
        var engine = new XmlCompilationEngine("method void run() {" +
                                           "if(true) { return; } else { return; }" +
                                           "}");

        var result = engine.CompileSubroutine();

        Assert.AreEqual(
            $"<subroutineDec>{Environment.NewLine}" +
            $"<keyword> method </keyword>{Environment.NewLine}" +
            $"<keyword> void </keyword>{Environment.NewLine}" +
            $"<identifier> run </identifier>{Environment.NewLine}" +
            $"<symbol> ( </symbol>{Environment.NewLine}" +
            $"<parameterList>{Environment.NewLine}" +
            $"</parameterList>{Environment.NewLine}" +
            $"<symbol> ) </symbol>{Environment.NewLine}" +
            $"<subroutineBody>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<statements>{Environment.NewLine}" +
            $"<ifStatement>{Environment.NewLine}" +
            $"<keyword> if </keyword>{Environment.NewLine}" +
            $"<symbol> ( </symbol>{Environment.NewLine}" +
            $"<expression>{Environment.NewLine}" +
            $"<term>{Environment.NewLine}" +
            $"<keywordConstant> true </keywordConstant>{Environment.NewLine}" +
            $"</term>{Environment.NewLine}" +
            $"</expression>{Environment.NewLine}" +
            $"<symbol> ) </symbol>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<statements>{Environment.NewLine}" +
            $"<returnStatement>{Environment.NewLine}" +
            $"<keyword> return </keyword>{Environment.NewLine}" +
            $"<symbol> ; </symbol>{Environment.NewLine}" +
            $"</returnStatement>{Environment.NewLine}" +
            $"</statements>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"<keyword> else </keyword>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<statements>{Environment.NewLine}" +
            $"<returnStatement>{Environment.NewLine}" +
            $"<keyword> return </keyword>{Environment.NewLine}" +
            $"<symbol> ; </symbol>{Environment.NewLine}" +
            $"</returnStatement>{Environment.NewLine}" +
            $"</statements>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</ifStatement>{Environment.NewLine}" +
            $"</statements>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</subroutineBody>{Environment.NewLine}" +
            $"</subroutineDec>{Environment.NewLine}", result);
    }

    [Test]
    public void MultipleSubroutines()
    {
        var engine = new XmlCompilationEngine($"class Main {{{Environment.NewLine}" +
                                           $"function void main () {{{Environment.NewLine}" +
                                           $"return ;{Environment.NewLine}" +
                                           $"}}{Environment.NewLine}" +
                                           $"function void run () {{{Environment.NewLine}" +
                                           $"return ;{Environment.NewLine}" +
                                           $"}}{Environment.NewLine}" +
                                           $"}}{Environment.NewLine}");

        var result = engine.CompileClass();

        Assert.AreEqual(
            $"<class>{Environment.NewLine}" +
            $"<keyword> class </keyword>{Environment.NewLine}" +
            $"<identifier> Main </identifier>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<subroutineDec>{Environment.NewLine}" +
            $"<keyword> function </keyword>{Environment.NewLine}" +
            $"<keyword> void </keyword>{Environment.NewLine}" +
            $"<identifier> main </identifier>{Environment.NewLine}" +
            $"<symbol> ( </symbol>{Environment.NewLine}" +
            $"<parameterList>{Environment.NewLine}" +
            $"</parameterList>{Environment.NewLine}" +
            $"<symbol> ) </symbol>{Environment.NewLine}" +
            $"<subroutineBody>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<statements>{Environment.NewLine}" +
            $"<returnStatement>{Environment.NewLine}" +
            $"<keyword> return </keyword>{Environment.NewLine}" +
            $"<symbol> ; </symbol>{Environment.NewLine}" +
            $"</returnStatement>{Environment.NewLine}" +
            $"</statements>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</subroutineBody>{Environment.NewLine}" +
            $"</subroutineDec>{Environment.NewLine}" +
            $"<subroutineDec>{Environment.NewLine}" +
            $"<keyword> function </keyword>{Environment.NewLine}" +
            $"<keyword> void </keyword>{Environment.NewLine}" +
            $"<identifier> run </identifier>{Environment.NewLine}" +
            $"<symbol> ( </symbol>{Environment.NewLine}" +
            $"<parameterList>{Environment.NewLine}" +
            $"</parameterList>{Environment.NewLine}" +
            $"<symbol> ) </symbol>{Environment.NewLine}" +
            $"<subroutineBody>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<statements>{Environment.NewLine}" +
            $"<returnStatement>{Environment.NewLine}" +
            $"<keyword> return </keyword>{Environment.NewLine}" +
            $"<symbol> ; </symbol>{Environment.NewLine}" +
            $"</returnStatement>{Environment.NewLine}" +
            $"</statements>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</subroutineBody>{Environment.NewLine}" +
            $"</subroutineDec>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</class>", result);
    }

    [Test]
    public void TestClassWithFields()
    {
        var engine = new XmlCompilationEngine($"class Main {{{Environment.NewLine}" +
                                           $"static int x, y;{Environment.NewLine}" +
                                           $"field Point p;}}{Environment.NewLine}");

        var result = engine.CompileClass();

        Assert.AreEqual($"<class>{Environment.NewLine}" +
                        $"<keyword> class </keyword>{Environment.NewLine}" +
                        $"<identifier> Main </identifier>{Environment.NewLine}" +
                        $"<symbol> {{ </symbol>{Environment.NewLine}" +
                        $"<classVarDec>{Environment.NewLine}" +
                        $"<keyword> static </keyword>{Environment.NewLine}" +
                        $"<keyword> int </keyword>{Environment.NewLine}" +
                        $"<identifier> x </identifier>{Environment.NewLine}" +
                        $"<symbol> , </symbol>{Environment.NewLine}" +
                        $"<identifier> y </identifier>{Environment.NewLine}" +
                        $"<symbol> ; </symbol>{Environment.NewLine}" +
                        $"</classVarDec>{Environment.NewLine}" +
                        $"<classVarDec>{Environment.NewLine}" +
                        $"<keyword> field </keyword>{Environment.NewLine}" +
                        $"<identifier> Point </identifier>{Environment.NewLine}" +
                        $"<identifier> p </identifier>{Environment.NewLine}" +
                        $"<symbol> ; </symbol>{Environment.NewLine}" +
                        $"</classVarDec>{Environment.NewLine}" +
                        $"<symbol> }} </symbol>{Environment.NewLine}" +
                        "</class>", result);
    }
    
    [Test]
    public void TestClassWithSubroutines()
    {
        var engine = new XmlCompilationEngine($"class Main {{{Environment.NewLine}" +
                                           $"function void main () {{{Environment.NewLine}" +
                                           $"return ;{Environment.NewLine}" +
                                           $"}}{Environment.NewLine}" +
                                           $"}}{Environment.NewLine}");

        var result = engine.CompileClass();

        Assert.AreEqual(
            $"<class>{Environment.NewLine}" +
            $"<keyword> class </keyword>{Environment.NewLine}" +
            $"<identifier> Main </identifier>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<subroutineDec>{Environment.NewLine}" +
            $"<keyword> function </keyword>{Environment.NewLine}" +
            $"<keyword> void </keyword>{Environment.NewLine}" +
            $"<identifier> main </identifier>{Environment.NewLine}" +
            $"<symbol> ( </symbol>{Environment.NewLine}" +
            $"<parameterList>{Environment.NewLine}" +
            $"</parameterList>{Environment.NewLine}" +
            $"<symbol> ) </symbol>{Environment.NewLine}" +
            $"<subroutineBody>{Environment.NewLine}" +
            $"<symbol> {{ </symbol>{Environment.NewLine}" +
            $"<statements>{Environment.NewLine}" +
            $"<returnStatement>{Environment.NewLine}" +
            $"<keyword> return </keyword>{Environment.NewLine}" +
            $"<symbol> ; </symbol>{Environment.NewLine}" +
            $"</returnStatement>{Environment.NewLine}" +
            $"</statements>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</subroutineBody>{Environment.NewLine}" +
            $"</subroutineDec>{Environment.NewLine}" +
            $"<symbol> }} </symbol>{Environment.NewLine}" +
            $"</class>", result);
    }
}