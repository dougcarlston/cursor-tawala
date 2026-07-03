using System;
using System.Collections.Specialized;
using System.Collections.ObjectModel;
using System.Text;
using NUnit.Framework;
using Tawala.XmlSupport;
using Tawala.RtfSupport;

namespace TawalaTest.RtfSupportTest
{
	[TestFixture]
	public class RtfTokenizerTest
	{
		[Test]
		public void BasicTokenStrings()
		{
			string rtfString = @"{\command \{text\}}";

			string[] expectedTokenStrings = new string[] {
				@"{",
				@"\command",
				@"\{",
				@"text",
				@"\}",
				@"}"
			};

			StringCollection tokens = RtfTokenizer.Tokenize(rtfString);

			for (int i = 0; i < expectedTokenStrings.Length; i++)
			{
				Assert.AreEqual(expectedTokenStrings[i], tokens[i]);
			}

			Assert.AreEqual(expectedTokenStrings.Length, tokens.Count);
		}

		[Test]
		public void BasicLegalTokenStrings()
		{
			string rtfString =
				@"{\rtf1\ansi\deff0"+ 
				@"{\fonttbl" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}}" +
				@"\pard\fs20 Plain Text\par }";

			string[] expectedTokens = new string[] {
				@"{",
				@"\rtf1",
				@"\ansi",
				@"\deff0",
				@"{",
				@"\fonttbl",
				@"{",
				@"\f0",
				@"\fswiss",
				@"\fcharset0",
				@"\fprq2",
				@"Arial;",
				@"}",
				@"}",
				@"\pard",
				@"\fs20",
				@"Plain Text",
				@"\par",
				@"}"
			};

			StringCollection tokens = RtfTokenizer.Tokenize(rtfString);

			for (int i = 0; i < expectedTokens.Length; i++)
			{
				Assert.AreEqual(expectedTokens[i], tokens[i]);
			}

			Assert.AreEqual(19, tokens.Count);
		}
	}
}
