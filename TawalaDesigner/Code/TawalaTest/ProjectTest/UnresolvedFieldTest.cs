using System;
using System.Collections.Generic;
using System.Text;

using Tawala.Proj;
using Tawala.XmlSupport;

using NUnit.Framework;

using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class UnresolvedFieldTest
	{
		private Process process = null;
		private Form form = null;

		[SetUp]
		public void SetUp()
		{
			Util.NewTestProject();

			form = Project.Current.AddForm();
			process = Project.Current.AddProcess();
		}

		private string xmlConditionWithUnresolvableHiddenField =
			"<isBlank field=\"Form 1:HiddenUnresolvable\" />\r\n";

		[Test]
		public void ConditionXmlWithUnresolvalbeHiddenFieldToXml()
		{
			FibCondition condition = new FibCondition(new XmlElement(xmlConditionWithUnresolvableHiddenField), process);

			Assert.IsTrue(condition.Field is UnresolvedField);
			Assert.AreEqual(xmlConditionWithUnresolvableHiddenField, condition.ToXml());
		}

		private string xmlConditionWithUnresolvableVariableField =
			"<isBlank field=\"myVar\" />\r\n";

		[Test]
		public void ConditionXmlWithUnresolvalbeVariableFieldToXml()
		{
			FibCondition condition = new FibCondition(new XmlElement(xmlConditionWithUnresolvableVariableField), process);

			Assert.IsTrue(condition.Field is UnresolvedField);
			Assert.AreEqual(xmlConditionWithUnresolvableVariableField, condition.ToXml());
		}
	}
}
