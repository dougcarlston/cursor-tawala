using System;
using System.Collections.Specialized;
using System.Text;
using NUnit.Framework;
using Tawala.XmlSupport;
using Tawala.RtfSupport;

namespace TawalaTest.RtfSupportTest
{
	[TestFixture]
	public class RtfTokenTest
	{
		private void noAction(string anyText)
		{
		}

		private string accumulatedText;

		private void accumulateText(string text)
		{
			accumulatedText += text;
		}

		private string emittedText;

		private void emitText(string text)
		{
			emittedText = accumulatedText;
		}

		[SetUp]
		public void SetUp()
		{
			accumulatedText = "";
			emittedText = "";
		}

		[Test]
		public void ExecuteText()
		{
			string rtfString1 = @"plain ";
			string rtfString2 = @"text";

			RtfToken token = new RtfToken(rtfString1, accumulateText);
			token.Execute();
			Assert.AreEqual("plain ", accumulatedText);

			token = new RtfToken(rtfString2, accumulateText);
			token.Execute();
			Assert.AreEqual("plain text", accumulatedText);
		}

		[Test]
		public void ExecuteCommand()
		{
			string rtfString1 = @"plain ";
			string rtfString2 = @"text";
			string rtfString3 = @"\par";

			RtfToken token = new RtfToken(rtfString1, accumulateText);
			token.Execute();
			token = new RtfToken(rtfString2, accumulateText);
			token.Execute();
			token = new RtfToken(rtfString3, emitText);
			token.Execute();

			Assert.AreEqual("plain text", emittedText);
		}
	}
}
