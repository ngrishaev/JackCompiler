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
        
        Assert.AreEqual(token.ToString(), "IntConst:42");
    }
    
    [Test]
    public void NumericToken_43()
    {
        var tokenizer = new JackCompiler.Tokenizer("43");
        
        var token = tokenizer.Advance();
        
        Assert.AreEqual(token.ToString(), "IntConst:43");
    }
    
    [Test]
    public void OneNumericTokenAndSpace()
    {
        var tokenizer = new JackCompiler.Tokenizer("42  ");
        
        var token = tokenizer.Advance();
        
        Assert.AreEqual(token.ToString(), "IntConst:42");
    }
    
    [Test]
    public void OneNumericTokenAndSpaceAround()
    {
        var tokenizer = new JackCompiler.Tokenizer("    42  ");
        
        var token = tokenizer.Advance();
        
        Assert.AreEqual(token.ToString(), "IntConst:42");
    }
    
    [Test]
    public void TwoNumericTokenAndSpaceAround()
    {
        var tokenizer = new JackCompiler.Tokenizer("    42  43    ");
        
        var token1 = tokenizer.Advance();
        var token2 = tokenizer.Advance();
        
        Assert.AreEqual(token1.ToString(), "IntConst:42");
        Assert.AreEqual(token2.ToString(), "IntConst:43");
    }
    
    [Test]
    public void TwoNumbersSplitByNewLine()
    {
        var tokenizer = new JackCompiler.Tokenizer(@"42
                                        43");
        
        var token1 = tokenizer.Advance();
        var token2 = tokenizer.Advance();
        
        Assert.AreEqual(token1.ToString(), "IntConst:42");
        Assert.AreEqual(token2.ToString(), "IntConst:43");
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

        Assert.AreEqual(token1.ToString(), "IntConst:42");
        Assert.AreEqual(token2.ToString(), "IntConst:43");
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

        Assert.AreEqual(token1.ToString(), "IntConst:42");
        Assert.AreEqual(token2.ToString(), "IntConst:43");
    }
}