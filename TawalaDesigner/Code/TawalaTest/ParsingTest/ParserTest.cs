using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Parsing;

namespace TawalaTest.ParsingTest
{
	[TestFixture]
	public class MathParserTest
	{
		[Test]
		public void MathParserString()
		{
			MathParser parser = new MathParser("a + b");

			Assert.IsNotNull(parser);
		}

		[Test]
		public void Balance()
		{
			MathParser parser;

			parser = new MathParser("a + b");
			Assert.AreEqual(0, parser.Balance);

			parser = new MathParser("(a + b)");
			Assert.AreEqual(0, parser.Balance);

			parser = new MathParser("((a + b)");
			Assert.AreEqual(1, parser.Balance);

			parser = new MathParser("((a + b)* c)");
			Assert.AreEqual(0, parser.Balance);

			parser = new MathParser("(a + b)* c)");
			Assert.AreEqual(-1, parser.Balance);
		}

		[Test]
		public void Tokenize()
		{
			MathParser parser;
			MatchCollection matches;
			string[] expMatches;

			// test simple addition tokenization
			parser = new MathParser("a + b");
			matches = parser.Tokenize();

			expMatches = new string [] { "a", "+", "b"};

			for (int i = 0; i < matches.Count; i++)
			{
				Assert.AreEqual(expMatches[i], matches[i].ToString());
			}

			// test  tokenization of parentheses
			parser = new MathParser("(a + b) * c");
			matches = parser.Tokenize();

			expMatches = new string[] { "(", "a", "+", "b", ")", "*", "c" };

			for (int i = 0; i < matches.Count; i++)
			{
				Assert.AreEqual(expMatches[i], matches[i].ToString());
			}

			// test  tokenization of fields
			parser = new MathParser("(<<field1>> + <<Field 2>>) / 2");
			matches = parser.Tokenize();

			expMatches = new string[] { "(", "<<field1>>", "+", "<<Field 2>>", ")", "/", "2" };

			for (int i = 0; i < matches.Count; i++)
			{
				Assert.AreEqual(expMatches[i], matches[i].ToString());
			}

			parser = new MathParser("<<aNy+CHARacteRS.at_ALL>>");
			matches = parser.Tokenize();

			expMatches = new string[] { "<<aNy+CHARacteRS.at_ALL>>" };

			for (int i = 0; i < matches.Count; i++)
			{
				Assert.AreEqual(expMatches[i], matches[i].ToString());
			}

			// test tokenization of numbers
			parser = new MathParser("3 37.5 -90 .5 5. -.5");
			matches = parser.Tokenize();

			expMatches = new string[] { "3", "37.5", "-90", ".5", "5", "-.5" };

			for (int i = 0; i < matches.Count; i++)
			{
				Assert.AreEqual(expMatches[i], matches[i].ToString());
			}

		}

		[Test]
		public void ToPostfix()
		{
			MathParser parser;
			string expString;

			parser = new MathParser("a + b");
			expString = "a b +";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new MathParser("a+b");
			expString = "a b +";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new MathParser("a + b + c");
			expString = "a b + c +";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new MathParser("a + b - c");
			expString = "a b + c -";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new MathParser("a - b - c");
			expString = "a b - c -";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new MathParser("a - (b - c)");
			expString = "a b c - -";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new MathParser("(a + b) * c");
			expString = "a b + c *";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new MathParser("((a + b) * c)");
			expString = "a b + c *";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new MathParser("a + b * c");
			expString = "a b c * +";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new MathParser("a + b + c / d");
			expString = "a b + c d / +";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new MathParser("(a + b + c) / d");
			expString = "a b + c + d /";
			Assert.AreEqual(expString, parser.ToPostfix());
		}

		[Test]
		public void ToPostfixMinusNoSpaces()
		{
			MathParser parser = new MathParser("10-2");
			Assert.AreEqual("10 2 -", parser.ToPostfix());

			parser = new MathParser("10 -2");
			Assert.AreEqual("10 -2", parser.ToPostfix());
		}

		[Test]
		public void ToPostfixMinusParentheses()
		{
			MathParser parser = new MathParser("10+(-2)");
			Assert.AreEqual("10 -2 +", parser.ToPostfix());
		}

		[Test]
		public void ToPostfixMinusMultipleParentheses()
		{
			MathParser parser = new MathParser("10+((-2)-3)");
			Assert.AreEqual("10 -2 3 - +", parser.ToPostfix());

		}
	
		[Test]
		public void ToPostfixNegativeValues()
		{
			MathParser parser = new MathParser("-2");
			Assert.AreEqual("-2", parser.ToPostfix());

			parser = new MathParser("10 - -2");
			Assert.AreEqual("10 -2 -", parser.ToPostfix());
		}

		[Test]
		public void ToPostfixField()
		{
			MathParser parser = new MathParser("<<x>> + 2");
			Assert.AreEqual("<<x>> 2 +", parser.ToPostfix());

			parser = new MathParser("<<x>> - 2");
			Assert.AreEqual("<<x>> 2 -", parser.ToPostfix());
		}

		[Test]
		public void ToPostfixFieldNoSpaces()
		{
			MathParser parser = new MathParser("<<x>>+2");
			Assert.AreEqual("<<x>> 2 +", parser.ToPostfix());

			parser = new MathParser("<<x>>+2");
			Assert.AreEqual("<<x>> 2 +", parser.ToPostfix());
		}

		[Test]
		public void ToPostfixUnaryMinus()
		{
			MathParser parser = new MathParser("-2");
			Assert.AreEqual("-2", parser.ToPostfix());
		}

		[Test]
		public void ToXml()
		{
			string expString;
			MathParser parser;

			parser = new MathParser("a + b");
			//postfix: "a b +"
			expString =
				"<add>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"</add>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new MathParser("a + b + c");
			//postfix: "a b + c +"
			expString =
				"<add>\r\n" +
				"<add>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"</add>\r\n" +
				"<operand value=\"c\"/>\r\n" +
				"</add>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new MathParser("a + b - c");
			//postfix: "a b + c -"
			expString =
				"<sub>\r\n" +
				"<add>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"</add>\r\n" +
				"<operand value=\"c\"/>\r\n" +
				"</sub>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new MathParser("a - b - c");
			//postfix: "a b - c -"
			expString =
				"<sub>\r\n" +
				"<sub>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"</sub>\r\n" +
				"<operand value=\"c\"/>\r\n" +
				"</sub>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new MathParser("a - (b - c)");
			//postfix: "a b c - -"
			expString =
				"<sub>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<sub>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"<operand value=\"c\"/>\r\n" +
				"</sub>\r\n" +
				"</sub>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new MathParser("(a + b) * c");
			//postfix: "a b + c *"
			expString =
				"<mul>\r\n" +
				"<add>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"</add>\r\n" +
				"<operand value=\"c\"/>\r\n" +
				"</mul>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new MathParser("a + b * c");
			//postfix: "a b c * +"
			expString =
				"<add>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<mul>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"<operand value=\"c\"/>\r\n" +
				"</mul>\r\n" +
				"</add>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new MathParser("a + b + c / d");
			//postfix: "a b + c d / +"
			expString =
				"<add>\r\n" +
				"<add>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"</add>\r\n" +
				"<div>\r\n" +
				"<operand value=\"c\"/>\r\n" +
				"<operand value=\"d\"/>\r\n" +
				"</div>\r\n" +
				"</add>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new MathParser("(a + b + c) / d");
			//postfix: "a b + c + d /"
			expString =
				"<div>\r\n" +
				"<add>\r\n" +
				"<add>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"</add>\r\n" +
				"<operand value=\"c\"/>\r\n" +
				"</add>\r\n" +
				"<operand value=\"d\"/>\r\n" +
				"</div>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new MathParser("(<<Score 1>> + <<Score 2>> + <<Score 3>>) / 3");
			//postfix: "a b + c + d /"
			expString =
				"<div>\r\n" +
				"<add>\r\n" +
				"<add>\r\n" +
				"<operand field=\"Score 1\"/>\r\n" +
				"<operand field=\"Score 2\"/>\r\n" +
				"</add>\r\n" +
				"<operand field=\"Score 3\"/>\r\n" +
				"</add>\r\n" +
				"<operand value=\"3\"/>\r\n" +
				"</div>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			// check for invalid XML characters
			parser = new MathParser("(<<It's <bad> 1>> + <<& \"bad\" 2>>");
			expString =
				"<add>\r\n" +
				"<operand field=\"It&apos;s &lt;bad&gt; 1\"/>\r\n" +
				"<operand field=\"&amp; &quot;bad&quot; 2\"/>\r\n" +
				"</add>\r\n";
			Assert.AreEqual(expString, parser.ToXml());
		}

		[Test]
		public void ToXmlField()
		{
			MathParser parser = new MathParser("<<x>> + 2");

			string expString =
				"<add>\r\n" +
				"<operand field=\"x\"/>\r\n" +
				"<operand value=\"2\"/>\r\n" +
				"</add>\r\n";
			Assert.AreEqual(expString, parser.ToXml());
		}

		[Test]
		public void ToXmlFieldNoSpaces()
		{
			MathParser parser = new MathParser("<<x>>+2");

			string expString =
				"<add>\r\n" +
				"<operand field=\"x\"/>\r\n" +
				"<operand value=\"2\"/>\r\n" +
				"</add>\r\n";
			Assert.AreEqual(expString, parser.ToXml());
		}
	}
}
