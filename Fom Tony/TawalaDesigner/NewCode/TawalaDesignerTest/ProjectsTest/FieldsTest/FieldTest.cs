using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest
{
	/// <summary>
	/// Class to test Tawala.Common.Field class.
	/// </summary>
	[TestFixture]
	public class FieldTest
	{
		[Test]
		public void NewField() 
		{
			Field field;

			// create field from name
			field = new Field("First Name");

			Assert.AreEqual("First Name", field.FieldName);
		} 

		[Test]
		public void TestToString() 
		{ 
			Field field = new Field("First Name");

			//Assertions 
			Assert.AreEqual("First Name", field.ToString());
		}

		[Test]
		public void Compare() 
		{ 
			Field field1 = new Field("First Name");
			Field field2 = new Field("Last Name");
			Field field3 = new Field("First Name");

			int result1 = field1.CompareTo(field2);
			int result2 = field1.CompareTo(field3);

			//Assertions 
			Assert.IsTrue(result1 < 0);
			Assert.IsTrue(result2 == 0);
		} 
	}
}
