using System;
using JackCompiler;
using NUnit.Framework;

namespace Tests;

public class CompilationEngineTests
{
    [Test]
    public void TestEmptyClass()
    {
        var engine = new CompilationEngine("class Main {}");
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
        var engine = new CompilationEngine(@"static int x;");

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
        var engine = new CompilationEngine("field int x, y;");

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
    public void ReturnSubroutine()
    {
        var engine = new CompilationEngine("method int run(int x, int y, Point p) {" +
                                           "var int x, y;" +
                                           "var Point p;" +
                                           "return 15;" +
                                           "}");

        var result = engine.CompileSubroutine();

        Assert.AreEqual($"<subroutineDec>{Environment.NewLine}" +
                        $"<keyword> method </keyword>{Environment.NewLine}" +
                        $"<keyword> int </keyword>{Environment.NewLine}" +
                        $"<identifier> run <identifier>{Environment.NewLine}" +
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
                        $"<keyword> var <keyword>{Environment.NewLine}" +
                        $"<keyword> int </keyword>{Environment.NewLine}" +
                        $"<identifier> x </identifier>{Environment.NewLine}" +
                        $"<symbol> , </symbol>{Environment.NewLine}" +
                        $"<identifier> y </identifier>{Environment.NewLine}" +
                        $"<symbol> ; </symbol>{Environment.NewLine}" +
                        $"</varDec>{Environment.NewLine}" +
                        $"<varDec>{Environment.NewLine}" +
                        $"<keyword> var <keyword>{Environment.NewLine}" +
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
    public void TestClassWithFields()
    {
        var engine = new CompilationEngine($"class Main {{{Environment.NewLine}" +
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
}