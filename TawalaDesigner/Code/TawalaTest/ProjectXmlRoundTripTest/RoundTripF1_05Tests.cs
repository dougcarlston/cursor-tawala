using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Tawala.Common;
using Tawala.Projects;
using Tawala.Functions.Runtime;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectXmlRoundTripTest
{
	[TestFixture]
	public class RoundTripF1_05Tests : TestBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Init(@"RoundTripTestFiles\1.5");
		}

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void SignupSheetV3()
		{
			RoundTripProject("Sign-up Sheet v.3.tawala");
		}
	}
}
