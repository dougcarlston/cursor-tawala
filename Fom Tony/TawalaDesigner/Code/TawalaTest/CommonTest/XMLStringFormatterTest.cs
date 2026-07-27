using System;
using NUnit.Framework;
using Tawala.Common;

namespace TawalaTest.CommonTest
{
	/// <summary>
	/// Test class for the XMLStringFormatter class
	/// </summary>
	[TestFixture]
	public class XMLStringFormatterTest
	{
		[Test]
		public void Instantiate() 
		{ 
			XMLStringFormatter formatter = new XMLStringFormatter();
			Assert.IsNotNull(formatter);
		}

		[Test]
		public void InstantiateWithString()
		{
			XMLStringFormatter formatter = new XMLStringFormatter("x > y");
			Assert.IsNotNull(formatter);
			Assert.AreEqual("x > y", formatter.RawString);
		}

		[Test]
		public void GetFormattedString()
		{
			XMLStringFormatter formatter = new XMLStringFormatter("x > y");
			Assert.AreEqual("x &gt; y", formatter.FormattedString);

			formatter.RawString = "x&c>> \"hello, he's got it!\" <5 & 7 == 4?";
			Assert.AreEqual("x&amp;c&gt;&gt; &quot;hello, he&apos;s got it!&quot; &lt;5 &amp; 7 == 4?", formatter.FormattedString);
		}

		[Test]
		public void EscapeAttributeTextTest()
		{
			string xxx = XMLStringFormatter.EscapeAttributeText("x&c>> \"hello, he's got it!\" <5 & 7 == 4?");
			Assert.AreEqual("x&amp;c&gt;&gt; &quot;hello, he&apos;s got it!&quot; &lt;5 &amp; 7 == 4?", xxx);
		}

		[Test]
		public void EscapeElementTextTest()
		{
			string xxx = XMLStringFormatter.EscapeElementText("x&c>> \"hello, he's got it!\" <5 & 7 == 4?");
			Assert.AreEqual("x&amp;c&gt;&gt; \"hello, he's got it!\" &lt;5 &amp; 7 == 4?", xxx);
		}
	}
}
