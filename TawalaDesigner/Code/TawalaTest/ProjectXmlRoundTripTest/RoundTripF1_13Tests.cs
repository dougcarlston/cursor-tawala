using NUnit.Framework;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectXmlRoundTripTest
{
	[TestFixture]
	public class RoundTripF1_13Tests : TestBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Init(@"RoundTripTestFiles\1.13");
		}

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}

		[Test]
		public void BoosterDashboards()
		{
			RoundTripProject("BoosterDashboards.tawala");
		}

		[Test]
		public void DixieFranchiseRegistration()
		{
			RoundTripProject("Dixie Franchise Registration.tawala");
		}

		[Test]
		public void SignupSheets()
		{
			RoundTripProject("SignupSheets.tawala");
		}

		[Test]
		public void TawalaOrderForm()
		{
			RoundTripProject("Tawala Order Form.tawala");
		}

		[Test]
		public void SportsDashboardsTemplateVerson2()
		{
			RoundTripProject("SportsDashboardsTemplateVersion2.tawala");
		}
	}
}
