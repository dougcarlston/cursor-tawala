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
	public class RoundTripF1_06Tests : TestBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Init(@"RoundTripTestFiles\1.6");
		}

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void DrugRegistry()
		{
			RoundTripProject("DrugRegistry.tawala");
		}

		[Test]
		public void MultiquestionPoll()
		{
			RoundTripProject("MultiquestionPoll.tawala");
		}

		[Test]
		public void NewMLB()
		{
			RoundTripProject("NewMLB.tawala");
		}

		[Test]
		public void PanAm()
		{
			RoundTripProject("PanAm.tawala");
		}

		[Test]
		public void PigskinPickEm()
		{
			RoundTripProject("PigskinPickEmtest.tawala");
		}

		[Test]
		public void PotluckV3()
		{
			RoundTripProject("Potluck_v.3.tawala");
		}
	}
}
