using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;


namespace TawalaTest.FunctionTests
{
    [TestFixture]
	public class ParameterInfoTests : FunctionTestBase
    {
		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			FixtureSetup();
		}

		[Test]
		public void ParameterInfoXmlPropertyIsValid()
		{
            IFunctionInfo info = functionRepository.Functions["record-count"];
			string xml = info.Parameters[0].Xml.OuterXml;
			Assert.IsTrue(xml.StartsWith("<parameter"));
			Assert.IsTrue(xml.EndsWith("</parameter>"));
		}

		[Test]
        public void XmlParametersEquivalentNumberOfIParameterInfo()
        {
            int count = functionRepository.Functions["record-count"].Parameters.Count;
            Assert.AreEqual(2, count);
            count += functionRepository.Functions["popular-choice-correlation-table"].Parameters.Count;
            count += functionRepository.Functions["choice-tally-table"].Parameters.Count;
            Assert.AreEqual(9, count);
        }

		[Test]
		public void XmlEnumerationBecomesPropertyTypeString()
		{
            Assert.AreEqual(typeof(string), functionRepository.Functions["popular-choice-correlation-table"].Parameters[0].PropertyType);
		}

		[Test]
		public void XmlFunctionConditionsBecomesFunctionFilterConditions()
		{
            IFunctionInfo info = functionRepository.Functions["record-count"];
            Assert.AreEqual("Tawala.Projects.FunctionFilterConditions", info.Parameters[1].PropertyType.FullName);
		}

		[Test]
		public void ParameterPropertyInfoTypeSameAsParameterPropertyTypes()
		{
            foreach (IFunctionInfo functionInfo in functionRepository.Functions)
			{
				foreach (IParameterInfo paramInfo in functionInfo.Parameters)
				{
					Assert.IsNotNull(paramInfo.PropertyType);
					Assert.AreSame(paramInfo.PropertyType, paramInfo.PropertyType);
				}
			}
		}

		[Test]
		public void ParameterPropertyHaveDescription()
		{
            foreach (IFunctionInfo functionInfo in functionRepository.Functions)
			{
				foreach (IParameterInfo paramInfo in functionInfo.Parameters)
				{
					Assert.IsTrue(paramInfo.Description.Length > 0);
				}
			}
		}
	}
}
