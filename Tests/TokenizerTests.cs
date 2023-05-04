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
        Assert.AreEqual("StringConst:\"string\"", token2.ToString());
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
        Assert.AreEqual("StringConst:\"string42\"", token2.ToString());
        Assert.AreEqual("IntConst:43", token3.ToString());
    }
    
    [Test]
    public void StringToken()
    {
        var tokenizer = new JackCompiler.Tokenizer(@"""string""");

        var token1 = tokenizer.Advance();

        Assert.AreEqual("StringConst:\"string\"", token1.ToString());
    }
}