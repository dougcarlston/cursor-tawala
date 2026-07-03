using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class ProjectFieldMapByIdTest
	{
		[SetUp]
		public void Setup()
		{
			Util.NewTestProject();
		}
		
		[Test]
		public void GetQualifiedFieldsDictionary()
		{
			Dictionary<int, string> dictionary = Project.FieldMapById.GetQualifiedFieldDictionary();
			Assert.AreEqual(1, dictionary.Count);	// Note:  Project.privateInvitationInviteeID = ID #1

			IForm form = Project.Current.AddForm();
			McqItem mcItem = new McqItem();
			form.ItemList.Add(mcItem);
			FibItem fib = new FibItem();
			form.ItemList.Add(fib);

			dictionary = Project.FieldMapById.GetQualifiedFieldDictionary();
			Assert.AreEqual(5, dictionary.Count);
			Assert.AreEqual(mcItem.QualifiedFieldName, dictionary[mcItem.Id]);
			Assert.AreEqual(fib.QualifiedFieldName, dictionary[fib.Id]);
			Assert.AreEqual(fib.BlankList[0].QualifiedFieldName, dictionary[fib.BlankList[0].Id]);
		}
	}
}
