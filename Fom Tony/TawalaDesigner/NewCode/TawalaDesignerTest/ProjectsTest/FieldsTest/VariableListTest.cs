using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Test class for VariableList class.
	/// </summary>
	[TestFixture]
	public class VariableListTest
	{
		private VariableList list;

		// execute this before each test method runs
		[SetUp]
		public void SetUp()
		{
			list = new VariableList();

			Variable var1 = new Variable("Variable 1");
			Variable var2 = new Variable("Variable 2");
			Variable var3 = new Variable("Variable 3");

			list.Add(var1);
			list.Add(var2);
			list.Add(var3);

		}

		[Test]
		public void IndexOf() 
		{
			//Assertions 
			Assert.AreEqual(0, list.IndexOf("Variable 1"));
			Assert.AreEqual(1, list.IndexOf("Variable 2"));
			Assert.AreEqual(2, list.IndexOf("Variable 3"));
		}

		[Test]
		public void FieldName()
		{
			Assert.AreEqual("", list.FieldName);
		}

		[Test]
		public void FieldString()
		{
			Assert.AreEqual("", list.FieldString);
		}

		[Test]
		public void AddUnique()
		{
			Assert.AreEqual(3, list.Count);

			list.AddUnique("Variable 4");
			Assert.AreEqual(4, list.Count);

			list.AddUnique("Variable 2");
			Assert.AreEqual(4, list.Count);
		}

		[Test]
		public void AddUniqueWithQualifiedName()
		{
			Assert.AreEqual(3, list.Count);

			list.AddUnique("Record:Variable 4");
			Assert.AreEqual(4, list.Count);

			list.AddUnique("Variable 4");
			Assert.AreEqual(4, list.Count);
		}
	}
}
