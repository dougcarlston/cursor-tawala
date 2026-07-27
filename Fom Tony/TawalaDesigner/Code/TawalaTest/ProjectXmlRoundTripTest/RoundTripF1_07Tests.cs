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
	public class RoundTripF1_07Tests : TestBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Init(@"RoundTripTestFiles\1.7");
		}

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void OnlineExamineBuilderV8()
		{
			RoundTripProject("Online_Exam_Builder_v.8.tawala");
		}
	}
}
