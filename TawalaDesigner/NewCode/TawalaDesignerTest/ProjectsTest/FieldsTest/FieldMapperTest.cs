using System;
using NUnit.Framework;
using Tawala.Projects;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class FieldMapperTest
	{
		[Test]
		public void Empty() 
		{
			FieldMapper mapper = new FieldMapper();

			mapper.Add("key 1", new Field("key 1"));
			mapper.Fields.Add(new Field("key 1"));
			mapper.Qualifiers.Add("Qualifier 1");

			Assert.AreEqual(1, mapper.Count);
			Assert.AreEqual(1, mapper.Fields.Count);
			Assert.AreEqual(1, mapper.Qualifiers.Count);

			mapper.Empty();

			Assert.AreEqual(0, mapper.Count);
			Assert.AreEqual(0, mapper.Fields.Count);
			Assert.AreEqual(0, mapper.Qualifiers.Count);
		}

		[Test]
		public void Map()
		{
			FieldMapper mapper = new FieldMapper();

			mapper.Fields.Add(new Field("Q1:a"));
			mapper.Fields.Add(new Field("Q1:b"));
			mapper.Fields.Add(new Field("Q2"));
			mapper.Qualifiers.Add("Record1");
			mapper.Qualifiers.Add("Record2");

			mapper.Map();

			Assert.IsTrue(mapper.ContainsKey("Q1:a"));
			Assert.IsTrue(mapper.ContainsKey("Q1:b"));
			Assert.IsTrue(mapper.ContainsKey("Q2"));
			Assert.IsTrue(mapper.ContainsKey("Record1:Q1:a"));
			Assert.IsTrue(mapper.ContainsKey("Record1:Q1:b"));
			Assert.IsTrue(mapper.ContainsKey("Record1:Q2"));
			Assert.IsTrue(mapper.ContainsKey("Record2:Q1:a"));
			Assert.IsTrue(mapper.ContainsKey("Record2:Q1:b"));
			Assert.IsTrue(mapper.ContainsKey("Record2:Q2"));

			Assert.AreEqual("Q1:a", mapper["Q1:a"].FieldName);
			Assert.AreEqual("Q1:b", mapper["Q1:b"].FieldName);
			Assert.AreEqual("Q2", mapper["Q2"].FieldName);
			Assert.AreEqual("Record1:Q1:a", mapper["Record1:Q1:a"].FieldName);
			Assert.AreEqual("Record1:Q1:b", mapper["Record1:Q1:b"].FieldName);
			Assert.AreEqual("Record1:Q2", mapper["Record1:Q2"].FieldName);
			Assert.AreEqual("Record2:Q1:a", mapper["Record2:Q1:a"].FieldName);
			Assert.AreEqual("Record2:Q1:b", mapper["Record2:Q1:b"].FieldName);
			Assert.AreEqual("Record2:Q2", mapper["Record2:Q2"].FieldName);
		}

	}
}
