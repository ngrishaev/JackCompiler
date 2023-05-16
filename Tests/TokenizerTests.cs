using System;
using NUnit.Framework;

namespace Tests;

public class Tokenizer
{
    [Test]
    public void HasTokensOnStart()
    {
        var tokenizer = new JackCompiler.Tokenizer("42");
        
        Assert.IsTrue(tokenizer.HasMoreTokens());
    }
    
    [Test]
    public void UnimportantTokensOnStart()
    {
        var tokenizer = new JackCompiler.Tokenizer(@"

//comment      //comment


     //     comment
");
        
        Assert.IsFalse(tokenizer.HasMoreTokens());
    }
    
    [Test]
    public void ExhaustTokens()
    {
        var tokenizer = new JackCompiler.Tokenizer("42");

        tokenizer.Advance();
        
        Assert.IsFalse(tokenizer.HasMoreTokens());
    }
    
    [Test]
    public void NumericToken_42()
    {
        var tokenizer = new JackCompiler.Tokenizer("42");
        
        var token = tokenizer.Advance();
        
        Assert.AreEqual("IntConst:42", token.ToString());
    }
    
    [Test]
    public void NumericToken_43()
    {
        var tokenizer = new JackCompiler.Tokenizer("43");
        
        var token = tokenizer.Advance();
        
        Assert.AreEqual("IntConst:43", token.ToString());
    }
    
    [Test]
    public void OneNumericTokenAndSpace()
    {
        var tokenizer = new JackCompiler.Tokenizer("42  ");
        
        var token = tokenizer.Advance();
        
        Assert.AreEqual("IntConst:42", token.ToString());
    }
    
    [Test]
    public void OneNumericTokenAndSpaceAround()
    {
        var tokenizer = new JackCompiler.Tokenizer("    42  ");
        
        var token = tokenizer.Advance();
        
        Assert.AreEqual("IntConst:42", token.ToString());
    }
    
    [Test]
    public void TwoNumericTokenAndSpaceAround()
    {
        var tokenizer = new JackCompiler.Tokenizer("    42  43    ");
        
        var token1 = tokenizer.Advance();
        var token2 = tokenizer.Advance();
        
        Assert.AreEqual("IntConst:42", token1.ToString());
        Assert.AreEqual("IntConst:43", token2.ToString());
    }
    
    [Test]
    public void TwoNumbersSplitByNewLine()
    {
        var tokenizer = new JackCompiler.Tokenizer(@"42
                                        43");
        
        var token1 = tokenizer.Advance();
        var token2 = tokenizer.Advance();
        
        Assert.AreEqual("IntConst:42", token1.ToString());
        Assert.AreEqual("IntConst:43", token2.ToString());
    }
    
    [Test]
    public void TwoNumbersSplitByComment()
    {
        var tokenizer = new JackCompiler.Tokenizer(
            @"42
            //comment
            43");

        var token1 = tokenizer.Advance();
        var token2 = tokenizer.Advance();

        Assert.AreEqual("IntConst:42", token1.ToString());
        Assert.AreEqual("IntConst:43", token2.ToString());
    }
    
    [Test]
    public void TwoNumbersSplitByTwoComments()
    {
        var tokenizer = new JackCompiler.Tokenizer(
            @"42
            //comment
            //comment2
            43");

        var token1 = tokenizer.Advance();
        var token2 = tokenizer.Advance();

        Assert.AreEqual("IntConst:42", token1.ToString());
        Assert.AreEqual("IntConst:43", token2.ToString());
    }
    
    [Test]
    public void NumberStringNumber()
    {
        var tokenizer = new JackCompiler.Tokenizer(
            @"42""string""43");

        var token1 = tokenizer.Advance();
        var token2 = tokenizer.Advance();
        var token3 = tokenizer.Advance();

        Assert.AreEqual("IntConst:42", token1.ToString());
        Assert.AreEqual("StringConst:string", token2.ToString());
        Assert.AreEqual("IntConst:43", token3.ToString());
    }
    [Test]
    public void Number_StringWithNumber_Number()
    {
        var tokenizer = new JackCompiler.Tokenizer(
            @"42""string42""43");

        var token1 = tokenizer.Advance();
        var token2 = tokenizer.Advance();
        var token3 = tokenizer.Advance();

        Assert.AreEqual("IntConst:42", token1.ToString());
        Assert.AreEqual("StringConst:string42", token2.ToString());
        Assert.AreEqual("IntConst:43", token3.ToString());
    }
    
    [Test]
    public void StringToken()
    {
        var tokenizer = new JackCompiler.Tokenizer(@"""string""");

        var token1 = tokenizer.Advance();

        Assert.AreEqual("StringConst:string", token1.ToString());
    }
    
    [Test]
    public void KeywordToken()
    {
        var tokenizer = new JackCompiler.Tokenizer(@"let");

        var token1 = tokenizer.Advance();

        Assert.AreEqual("Keyword:let", token1.ToString());
    }
    
    [Test]
    public void MultilineComment()
    {
        var tokenizer = new JackCompiler.Tokenizer(@"/*let*/ 42");

        var token1 = tokenizer.Advance();

        Assert.AreEqual("IntConst:42", token1.ToString());
    }
    
    [Test]
    public void AllComments()
    {
        var tokenizer = new JackCompiler.Tokenizer($"//{Environment.NewLine}" +
                                                   $"/** Computes */{Environment.NewLine}" +
                                                   $"class");

        var token1 = tokenizer.Advance();

        Assert.AreEqual("Keyword:class", token1.ToString());
    }
    
    [Test]
    public void NumberKeywordStringTokens()
    {
        var tokenizer = new JackCompiler.Tokenizer(@"42 let ""let""");

        var token1 = tokenizer.Advance();
        var token2 = tokenizer.Advance();
        var token3 = tokenizer.Advance();

        Assert.AreEqual("IntConst:42", token1.ToString());
        Assert.AreEqual("Keyword:let", token2.ToString());
        Assert.AreEqual("StringConst:let", token3.ToString());
    }
    
    [Test]
    public void MethodToTokens()
    {
        var tokenizer = new JackCompiler.Tokenizer(@"
                                                    method int getX() {
                                                        var string someString;
                                                        let someString = ""Hello World"";
                                                        return 15;
                                                    }
                                                ");

        var token1 = tokenizer.Advance();
        var token2 = tokenizer.Advance();
        var token3 = tokenizer.Advance();
        var token4 = tokenizer.Advance();
        var token5 = tokenizer.Advance();
        var token6 = tokenizer.Advance();
        var token7 = tokenizer.Advance();
        var token8 = tokenizer.Advance();
        var token9 = tokenizer.Advance();
        var token10 = tokenizer.Advance();
        var token11 = tokenizer.Advance();
        var token12 = tokenizer.Advance();
        var token13 = tokenizer.Advance();
        var token14 = tokenizer.Advance();
        var token15 = tokenizer.Advance();
        var token16 = tokenizer.Advance();
        var token17 = tokenizer.Advance();
        var token18 = tokenizer.Advance();
        var token19 = tokenizer.Advance();
        

        Assert.AreEqual("Keyword:method", token1.ToString());
        Assert.AreEqual("Keyword:int", token2.ToString());
        Assert.AreEqual("Identifier:getX", token3.ToString());
        Assert.AreEqual("Symbol:(", token4.ToString());
        Assert.AreEqual("Symbol:)", token5.ToString());
        Assert.AreEqual("Symbol:{", token6.ToString());
        Assert.AreEqual("Keyword:var", token7.ToString());
        Assert.AreEqual("Identifier:string", token8.ToString());
        Assert.AreEqual("Identifier:someString", token9.ToString());
        Assert.AreEqual("Symbol:;", token10.ToString());
        Assert.AreEqual("Keyword:let", token11.ToString());
        Assert.AreEqual("Identifier:someString", token12.ToString());
        Assert.AreEqual("Symbol:=", token13.ToString());
        Assert.AreEqual("StringConst:Hello World", token14.ToString());
        Assert.AreEqual("Symbol:;", token15.ToString());
        Assert.AreEqual("Keyword:return", token16.ToString());
        Assert.AreEqual("IntConst:15", token17.ToString());
        Assert.AreEqual("Symbol:;", token18.ToString());
        Assert.AreEqual("Symbol:}", token19.ToString());
    }
}