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
                        $"</classVarDec>", result);
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
                        $"</classVarDec>", result);
    }
    
    [Test]
    public void Subroutine()
    {
        var engine = new CompilationEngine("method void run(int x, int y, Point p) {" +
                                           "var int x, y;" +
                                           "var Point p;" +
                                           "}");

        var result = engine.CompileSubroutine();
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