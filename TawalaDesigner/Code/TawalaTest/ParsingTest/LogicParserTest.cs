using System;
using NUnit.Framework;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.Parsing;

namespace TawalaTest.ParsingTest
{
	[TestFixture]
	public class LogicParserTest
	{
		[Test]
		public void LogicParserString()
		{
			MathParser parser = new MathParser("a AND b");

			Assert.IsNotNull(parser);
		}

		[Test]
		public void Tokenize()
		{
			LogicParser parser;
			MatchCollection matches;
			string[] expMatches;

			// test simple AND tokenization
			parser = new LogicParser("a AND b");
			matches = parser.Tokenize();

			expMatches = new string [] { "a", "AND", "b"};

			for (int i = 0; i < matches.Count; i++)
			{
				Assert.AreEqual(expMatches[i], matches[i].ToString());
			}

			// test  tokenization of parentheses
			parser = new LogicParser("(a AND b) OR c");
			matches = parser.Tokenize();

			expMatches = new string[] { "(", "a", "AND", "b", ")", "OR", "c" };

			for (int i = 0; i < matches.Count; i++)
			{
				Assert.AreEqual(expMatches[i], matches[i].ToString());
			}
		}

		[Test]
		public void TokenizeComparisonOperators()
		{
			LogicParser parser;
			MatchCollection matches;
			string[] expMatches;

			// test comparison operator tokenization
			parser = new LogicParser("<<Name>> begins with S AND <<Age>> is greater than or equal to 21");
			matches = parser.Tokenize();

			expMatches = new string[] { "<<Name>>", "begins with",  "S", "AND", "<<Age>>", "is greater than or equal to", "21" };

			for (int i = 0; i < matches.Count; i++)
			{
				Assert.AreEqual(expMatches[i], matches[i].ToString());
			}

		}

		[Test]
		public void ToPostfix()
		{
			LogicParser parser;
			string expString;

			parser = new LogicParser("a AND b");
			expString = "a b AND";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new LogicParser("a AND b AND c");
			expString = "a b AND c AND";
			Assert.AreEqual(expString, parser.ToPostfix());

			parser = new LogicParser("a AND b OR c");
			expString = "a b AND c OR";
			Assert.AreEqual(expString, parser.ToPostfix());
		}

		[Test]
		public void ToXml()
		{
			string expString;
			LogicParser parser;

			parser = new LogicParser("a AND b");
			//postfix: "a b AND"
			expString =
				"<and>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"</and>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new LogicParser("a AND b AND c");
			//postfix: "a b AND c AND"
			expString =
				"<and>\r\n" +
				"<and>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"</and>\r\n" +
				"<operand value=\"c\"/>\r\n" +
				"</and>\r\n";
			Assert.AreEqual(expString, parser.ToXml());

			parser = new LogicParser("a AND b OR c");
			//postfix: "a b AND c OR"
			expString =
				"<or>\r\n" +
				"<and>\r\n" +
				"<operand value=\"a\"/>\r\n" +
				"<operand value=\"b\"/>\r\n" +
				"</and>\r\n" +
				"<operand value=\"c\"/>\r\n" +
				"</or>\r\n";
			Assert.AreEqual(expString, parser.ToXml());
		}
	}
}
