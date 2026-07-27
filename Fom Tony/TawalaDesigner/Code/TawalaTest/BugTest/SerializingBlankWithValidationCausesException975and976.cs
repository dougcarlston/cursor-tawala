using System;
using System.IO;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.BugTest
{
	/// <summary>
	/// Test for Mantis issues:
	///		# 975 (Exception Error if Adding SET statement with Validated Interger twice)
	///		# 976 (Exception Error if Adding SET statement with Validated Email FIB twice)
	/// </summary>
	[TestFixture]
	public class SerializingBlankWithValidationCausesException975and976 : FunctionTestBase
	{
		[SetUp]
		public void SetUp()
		{
			functionSetup();

			Util.NewTestProject();
		}

		[TearDown]
		public void TearDown()
		{
			functionTearDown();
		}

		private readonly string blankWithIntegerValidationXml =
			@"<blank label=""a"" length=""20"" required=""false"">" +
			@"<validator>" +
			@"<integer-range-validator version=""1"">" +
			@"<error-message><string value=""This number is not valid.""/>" + Environment.NewLine +
			@"</error-message>" +
			@"<lower-limit></lower-limit><upper-limit></upper-limit>" +
			@"</integer-range-validator>" +
			@"</validator>" +
			@"</blank>";

		private readonly string blankWithEmailValidationXml =
			@"<blank label=""a"" length=""20"" required=""false"">" +
			@"<validator>" +
			@"<email-validator version=""1"">" +
			@"<error-message><string value=""This email is not valid.""/>" + Environment.NewLine +
			@"</error-message>" +
			@"</email-validator>" +
			@"</validator>" +
			@"</blank>";

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			RuntimeTypeResolver.Init();
			FunctionLoader.BuildAndLoad(XmlConstants.FunctionRepositoryXml);
		}

		[Test]
		public void BlankWithEmailValidationIsSerializedAndRegenerated976()
		{
			var blank = new Blank(new XmlElement(blankWithEmailValidationXml), new FibItem());

			// necessary to initially generate validation function in the Blank class
			Project.Events.RaiseProjectOpenedEvent(new ProjectEventArgs("Untitled"));

			Assert.AreEqual(blankWithEmailValidationXml, blank.ToXml());

			var dataObject = new DataObject();
			using (var ms = new MemoryStream())
			{
				var bf = new BinaryFormatter();
				bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
				bf.Serialize(ms, blank);
				ms.Seek(0, SeekOrigin.Begin);
				dataObject.SetData(typeof (Blank), bf.Deserialize(ms));
			}

			var regeneratedBlank = dataObject.GetData(typeof (Blank)) as Blank;

			Assert.AreEqual(blankWithEmailValidationXml, regeneratedBlank.ToXml());
		}

		[Test]
		public void BlankWithIntegerValidationIsSerializedAndRegenerated975()
		{
			var blank = new Blank(new XmlElement(blankWithIntegerValidationXml), new FibItem());

			// necessary to initially generate validation function in the Blank class
			Project.Events.RaiseProjectOpenedEvent(new ProjectEventArgs("Untitled"));

			Assert.AreEqual(blankWithIntegerValidationXml, blank.ToXml());

			var dataObject = new DataObject();
			using (var ms = new MemoryStream())
			{
				var bf = new BinaryFormatter();
				bf.AssemblyFormat = FormatterAssemblyStyle.Simple;
				bf.Serialize(ms, blank);
				ms.Seek(0, SeekOrigin.Begin);
				dataObject.SetData(typeof (Blank), bf.Deserialize(ms));
			}

			var regeneratedBlank = dataObject.GetData(typeof (Blank)) as Blank;

			Assert.AreEqual(blankWithIntegerValidationXml, regeneratedBlank.ToXml());
		}
	}
}