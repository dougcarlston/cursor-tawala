// $Workfile: FunctionFieldSupportInterfaceTests.cs $
// $Revision: 1 $	$Date: 3/16/07 2:14p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using Tawala.TextEditor;

namespace TawalaTest.TextEditorTest
{
	[TestFixture]
	public class FunctionFieldSupportInterfaceTests : TextEditTestBase
	{
		[Test]
		public void InsertRecordCountFunctionField()
		{
			editor.InsertFunctionField(3001, "RECORD COUNT");

			TXTextControl.TextField tf = txTextControl.TextFields.GetItem(3001);
			Assert.AreEqual(3001, tf.ID);
			Assert.AreEqual(@"FF$3001", tf.Name);
			Assert.AreEqual("RECORD COUNT", tf.Text);
			Assert.IsFalse(tf.Editable);
			Assert.IsFalse(tf.Deleteable);
		}

		[Test]
		public void SelectedFunctionFieldInstanceId()
		{
			editor.InsertFunctionField(2718, "Boo");

			Assert.AreNotEqual(2718, editor.SelectedFunctionFieldInstanceId);

			forceSelectedTextFieldHelper(2718);
			Assert.AreEqual(2718, editor.SelectedFunctionFieldInstanceId);
		}

		[Test]
		public void ChangeSelectedFunctionFieldIdToUniqueId()
		{
			editor.InsertFunctionField(5000, "RECORD COUNT");
			editor.InsertFunctionField(3000, "EXTRA");

			forceSelectedTextFieldHelper(5000);
			editor.ChangeSelectedFunctionFieldIdToUniqueId(5000, 10000);

			Assert.IsFalse(editor.IsFieldIdInUse(5000));
			Assert.IsTrue(editor.IsFieldIdInUse(10000));
		}

		[Test]
		public void UpdateFuntionFieldByUniqueId()
		{
			editor.InsertFunctionField(3001, "RECORD COUNT");

			forceSelectedTextFieldHelper(3001);

			editor.ChangeSelectedFunctionFieldIdToUniqueId(3001, 12000);

			editor.UpdateFunctionFieldByUniqueId(12000, 15000, "SUM");

			TXTextControl.TextField tf = txTextControl.TextFields.GetItem(15000);
			Assert.AreEqual(15000, tf.ID);
			Assert.AreEqual(@"FF$15000", tf.Name);
			Assert.AreEqual("SUM", tf.Text);
		}

		private void forceSelectedTextFieldHelper(int id)
		{
			TXTextControl.TextField tf = txTextControl.TextFields.GetItem(id);
			editor.Select(tf.Start, tf.Start - 2);
		}
	}
}
