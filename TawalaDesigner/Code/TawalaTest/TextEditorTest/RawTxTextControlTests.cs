// $Workfile: RawTxTextControlTests.cs $
// $Revision: 9 $	$Date: 3/16/07 2:17p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;


namespace TawalaTest.TextEditorTest
{
	[TestFixture]
	public class RawTxTextControlTests : TextEditTestBase
	{
		[Test]
		public void AssumedVersionOfTxTextControl()
		{
			string fullName = "TXTextControl, Version=12.0.400.500, Culture=neutral, PublicKeyToken=6b83fe9a75cfb638";
			Assembly a = typeof(TXTextControl.TextControl).Assembly;
			Assert.AreEqual(fullName, a.FullName);
		}

		[Test]
		public void TextFields_RefsNotEqualButChangesPropagate()
		{
			TXTextControl.TextField tf = new TXTextControl.TextField("text");
			tf.ID = 1001;
			tf.Text = "Test";
			txTextControl.TextFields.Add(tf);

			TXTextControl.TextField tf1 = txTextControl.TextFields.GetItem(1001);
			Assert.IsNotNull(tf1);

			TXTextControl.TextField tf2 = txTextControl.TextFields.GetItem(1001);

			Assert.IsFalse(object.ReferenceEquals(tf, tf1));
			Assert.IsFalse(object.ReferenceEquals(tf1, tf2));
			
			Assert.AreEqual("Test", tf1.Text);
			Assert.AreEqual("Test", tf2.Text);

			tf.Text = "Foo";

			Assert.AreEqual("Foo", tf1.Text);
			Assert.AreEqual("Foo", tf2.Text);

		}

		[Test]
		public void TextFields_TwoWithSameIDRtf()
		{
			Assert.AreEqual(0, txTextControl.TextFields.Count);

			TXTextControl.TextField tf = new TXTextControl.TextField("<<FirstName>>");
			tf.ID = 1001;
			tf.Name = "FirstName";
			tf.ShowActivated = true;
			tf.DoubledInputPosition = true;
			tf.Editable = false;
			tf.Deleteable = false;
			txTextControl.TextFields.Add(tf);

			editor.Selection.Text = "asdf";
			editor.Select(editor.Selection.Start + editor.Selection.Length, 0);

			tf = new TXTextControl.TextField("<<FirstName>>");
			tf.ID = 1001;
			tf.Name = "FirstName";
			tf.ShowActivated = true;
			tf.DoubledInputPosition = true;
			tf.Editable = false;
			tf.Deleteable = false;
			txTextControl.TextFields.Add(tf);
			Assert.AreEqual(2, txTextControl.TextFields.Count);

			string rtfString = 
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" + 
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" + 
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" + 
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" + 
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" + 
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" + 
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" + 
				@"\itap0\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags219\txfielddataval1001" + 
				@"\txfielddata 460069007200730074004e0061006d0065000000}" + 
				@"<<FirstName>>{" + 
				@"\*\txfieldend}\plain\f0\fs20" + 
				@" asdf" + 
				@"\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags219\txfielddataval1001\txfielddata 460069007200730074004e0061006d0065000000}" + 
				@"<<FirstName>>{" + 
				@"\*\txfieldend}\par }";

			Assert.AreEqual(rtfString, editor.GetRTF());
		}

		[Test]
		public void NoIdentity_Table()
		{
			txTextControl.Tables.Add(2, 2, 101);
			TXTextControl.Table ref1 = txTextControl.Tables.GetItem(101);
			TXTextControl.Table ref2 = txTextControl.Tables.GetItem(101);
			Assert.IsNotNull(ref1);
			Assert.AreNotSame(ref1, ref2);
			Assert.AreSame(ref1, ref1);
		}

		[Test]
		public void NoIdentity_HypertextLink()
		{
			TXTextControl.HypertextLink hl = new TXTextControl.HypertextLink("text", "target");
			hl.ID = 2002;
			txTextControl.HypertextLinks.Add(hl);
			TXTextControl.HypertextLink hl2002 = txTextControl.HypertextLinks.GetItem(2002);
			Assert.IsNotNull(hl2002);
			Assert.AreNotSame(hl, hl2002);
			Assert.AreSame(hl2002, hl2002);
		}
	}
}
