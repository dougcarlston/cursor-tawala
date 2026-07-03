using NUnit.Framework;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectXmlRoundTripTest
{
	[TestFixture]
	public class RoundTripF1_12Tests : TestBase
	{
		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			Init(@"RoundTripTestFiles\1.12");
		}

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();
		}


		[Test]
		public void GenericCommunicator()
		{
			RoundTripProject("Generic Communicator.tawala");
		}
	}
}
