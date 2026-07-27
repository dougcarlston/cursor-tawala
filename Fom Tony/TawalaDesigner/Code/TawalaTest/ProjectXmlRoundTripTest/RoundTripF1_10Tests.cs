using NUnit.Framework;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectXmlRoundTripTest
{
	[TestFixture]
	public class RoundTripF1_10Tests : TestBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Init(@"RoundTripTestFiles\1.10");
		}

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}


		[Test]
		public void SportsDashboardsTemplateVerson1()
		{
			RoundTripProject("SportsDashboardsTemplateVersion1.tawala");
		}
	}
}
