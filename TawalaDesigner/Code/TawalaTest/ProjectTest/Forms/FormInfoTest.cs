using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class FormInfoTest
	{
		[Test]
		public void Construct() 
		{
			IForm formInfo = new FormInfo("Test Form");
			Assert.AreEqual("Test Form", formInfo.Name);
		} 

	}
}
