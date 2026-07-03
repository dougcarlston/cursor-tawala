using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Tawala.Proj;
using Tawala.Functions.Runtime;
using TawalaTest.TestSupport;

namespace TawalaTest.FormDesignerTest
{
	[SetUpFixture]
	public class NamespaceSetUpTearDown
	{
		[SetUp]
		public void BeforeTestsInNamespace()
		{
			ComponentMaker.UseNewComponents(true);
			CompilationInfo info = FunctionLoader.Rebuild(XmlConstants.ComponentRepository);
			FunctionLoader.Load(info.Path);

		}

		[TearDown]
		public void AfterTestsInNamespace()
		{
		}
	}
}

