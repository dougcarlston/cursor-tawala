using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;

using NUnit.Framework;

using Tawala.Projects;
using Tawala.Functions.Runtime;
using TawalaTest.TestingSupport;

namespace TawalaTest.FormDesignerTest
{
	[SetUpFixture]
	public class NamespaceSetUpTearDown
	{
		[SetUp]
		public void BeforeTestsInNamespace()
		{
			CompilationInfo info = FunctionLoader.Rebuild(XmlConstants.ComponentRepository);
			FunctionLoader.Load(info.Path);

		}

		[TearDown]
		public void AfterTestsInNamespace()
		{
		}
	}
}

