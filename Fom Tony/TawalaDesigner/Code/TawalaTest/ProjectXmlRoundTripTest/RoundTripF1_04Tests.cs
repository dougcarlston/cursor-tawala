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
	public class RoundTripF1_04Tests : TestBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Init(@"RoundTripTestFiles\1.4");
		}

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void OneJustice4All()
		{
			RoundTripProject("1Justice4All.tawala");
		}

		[Test]
		public void MarchMadness()
		{
			RoundTripProject("99MarchMadness.tawala");
		}
		[Test]
		public void Art_Show_Registration()
		{
			RoundTripProject("Art_Show_Registration.tawala");
		}

		[Test]
		public void AWS_Entry_Form()
		{
			RoundTripProject("AWS_Entry_Form.tawala");
		}

		[Test]
		public void FrOpenTennis()
		{
			RoundTripProject("FrOpenTennis.tawala");
		}

		[Test]
		public void GetTogetherV8()
		{
			RoundTripProject("Get Together v.8.tawala");
		}

		[Test]
		public void NBAplayoff()
		{
			RoundTripProject("NBAplayoff.tawala");
		}

		[Test]
		public void OldListBuilderWithoutFunctionsV1()
		{
			RoundTripProject("Old List Builder without functions v.1.tawala");
		}

		[Test]
		public void OnlineExamBuilderV8()
		{
			RoundTripProject("Online Exam Builder v.8.tawala");
		}

		[Test]
		public void OnlinePetitionBuilderV4()
		{
			RoundTripProject("Online Petition Builder v.4.tawala");
		}

		[Test]
		public void PoliticalCampaignToolV7()
		{
			RoundTripProject("Political Campaign Tool v.7.tawala");
		}

		[Test]
		[Description("Downloaded XML has different whitespace than other version")]
		public void Political_Campaign_Tool_V7()
		{
			RoundTripProject("Political_Campaign_Tool_v.7.tawala");
		}

		[Test]
		public void PollOrSurveyV3()
		{
			RoundTripProject("Poll or Survey v.3.tawala");
		}

		[Test]
		public void PotluckV6()
		{
			RoundTripProject("Potluck v.6.tawala");
		}

		[Test]
		public void ProBB()
		{
			RoundTripProject("ProBB.tawala");
		}

		[Test]
		public void SchoolSignupSheetV3()
		{
			RoundTripProject("School Sign-up Sheet v.3.tawala");
		}

		[Test]
		public void SharedToDoV2()
		{
			RoundTripProject("Shared To-Do v.2.tawala");
		}

		[Test]
		public void Simple_Poll_v6()
		{
			RoundTripProject("Simple_Poll_v.6.tawala");
		}

		[Test]
		public void StudentPollOrSurveyV2()
		{
			RoundTripProject("Student Poll or Survey v.2.tawala");
		}

		[Test]
		public void WimbledonTennis()
		{
			RoundTripProject("WimbledonTennis.tawala");
		}
	}
}
