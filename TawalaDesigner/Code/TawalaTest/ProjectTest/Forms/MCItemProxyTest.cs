using System;
using System.Collections;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Expressions;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class MCItemProxyTest
	{
		[Test]
		public void Construct()
		{
			MCItemProxy proxy = new MCItemProxy();
			Assert.AreEqual("(selection)", proxy.FieldName);
		}

		[Test]
		public void OperatorDataSource()
		{
			MCItemProxy proxy = new MCItemProxy();
			Assert.AreEqual(MCOneOperator.List.DataSource, proxy.OperatorDataSource);
		}

	}
}
