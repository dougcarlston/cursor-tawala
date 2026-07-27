using System;
using System.Diagnostics;
using NUnit.Framework;
using Tawala.Common;

namespace TawalaTest.CommonTest
{
	[TestFixture]
	public class RegistryHelperTest
	{
		[Test]
		public void DefaultBrowser()
		{
			string browserApp = RegistryHelper.GetDefaultBrowser();
			Assert.IsNotNull(browserApp, "GetDefaultBrowser() returned null");
			Assert.AreNotEqual(string.Empty, browserApp, "GetDefaultBrowser() returned empty string");
			Assert.IsTrue(browserApp.ToLower().EndsWith(".exe"), "Browser app does not end with \".exe\"");
		}
	}
}
