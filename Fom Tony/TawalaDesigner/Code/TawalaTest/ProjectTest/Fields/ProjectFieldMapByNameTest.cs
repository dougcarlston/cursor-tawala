using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace TawalaTest.ProjectTest
{
	[TestFixture]
	public class ProjectFieldMapByNameTest
	{
		private ProjectFieldMapByName fieldMap;
		private IForm form;
		private FibItem fibItem;
		private Blank fibItemBlank;
		private McqItem mcItem;
		private IChoice mcItemChoice;
		private const string inviteeId = "_InviteeID";

		[SetUp]
		public void SetUp()
		{
			form = null;
			fibItem = null;
			fibItemBlank = null;
			mcItem = null;
			mcItemChoice = null;

			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();

			fieldMap = Project.FieldMapByName;
		}

		private void addBlank()
		{
			fibItem = new FibItem();
			form.ItemList.Add(fibItem);
			fibItemBlank = fibItem.BlankList[0];
		}

		private void addMCItem()
		{
			mcItem = new McqItem();
			form.ItemList.Add(mcItem);
			mcItemChoice = mcItem.Choices[0];
		}

		[Test]
		public void FieldMapInitiallyContainsInviteeID()
		{
			Assert.AreEqual(1, fieldMap.Count);
			Assert.IsNotNull(fieldMap[inviteeId]);
		}

		[Test]
		public void AddingBlankWithoutAlternateLabelCreatesOneMapEntry()
		{
			addBlank();

			Assert.AreEqual(2, fieldMap.Count);
			Assert.IsNotNull(fieldMap[inviteeId]);
			Assert.AreSame(fibItemBlank, fieldMap["Form 1:Q1:a"]);
		}

		[Test]
		public void AddingBlankWithAlternateLabelCreatesOneMapEntry()
		{
			addBlank();
			fibItemBlank.AlternateLabel = "AlternateLabel";

			Assert.AreEqual(2, fieldMap.Count);
			Assert.IsNotNull(fieldMap[inviteeId]);
			Assert.AreSame(fibItemBlank, fieldMap["Form 1:AlternateLabel"]);
		}
		[Test]
		public void AddingBlankTwiceDoesNotCreateMoreMapEntries()
		{
			addBlank();
			fieldMap.AddUnique(fibItemBlank);
			fieldMap.AddUnique(fibItemBlank);

			Assert.AreEqual(2, fieldMap.Count);
			Assert.IsNotNull(fieldMap[inviteeId]);
		}

		[Test]
		public void AddingMCItemWithoutAlternateLabelCreatesOneMapEntry()
		{
			addMCItem();

			Assert.AreEqual(2, fieldMap.Count);
			Assert.IsNotNull(fieldMap[inviteeId]);
			Assert.AreSame(mcItem, fieldMap["Form 1:Q1"]);
		}

		[Test]
		public void AddingMCItemWithAlternateLabelCreatesOneMapEntry()
		{
			addMCItem();
			mcItem.AlternateLabel = "AlternateLabel";

			Assert.AreEqual(2, fieldMap.Count);
			Assert.IsNotNull(fieldMap[inviteeId]);
			Assert.AreSame(mcItem, fieldMap["Form 1:AlternateLabel"]);
		}
		[Test]
		public void AddingMCItemTwiceDoesNotCreateMoreMapEntries()
		{
			addMCItem();
			fieldMap.AddUnique(mcItem);
			fieldMap.AddUnique(mcItem);

			Assert.AreEqual(2, fieldMap.Count);
			Assert.IsNotNull(fieldMap[inviteeId]);
		}

		[Test]
		public void RemovingFieldLowersMapCount()
		{
			addBlank();
			Assert.AreEqual(2, fieldMap.Count);

			fieldMap.Remove(fibItemBlank);

			Assert.AreEqual(1, fieldMap.Count);
			Assert.IsNotNull(fieldMap[inviteeId]);
		}

	}

}
