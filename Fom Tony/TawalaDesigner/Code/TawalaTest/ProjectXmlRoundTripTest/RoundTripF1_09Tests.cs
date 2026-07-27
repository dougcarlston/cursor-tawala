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
	public class RoundTripF1_09Tests : TestBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Init(@"RoundTripTestFiles\1.9");
		}

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}


		[Test]
		public void Shared_ToDoV8()
		{
			RoundTripProject("Shared To-DoV8.tawala");
		}

		[Test]
		public void MVSC_Communicator()
		{
			RoundTripProject("MVSC Communicator.tawala");
		}

		[Test]
		public void MVSC_4X4_Registration()
		{
			RoundTripProject("MVSC 4X4 Registration.tawala");
		}

		[Test]
		public void MVSC_Fullerton_Summer_Camp()
		{
			RoundTripProject("MVSC Fullerton Summer Camp.tawala");
		}

		[Test]
		public void DirtBowl()
		{
			RoundTripProject("DirtBowl.tawala");
		}
	}
}
