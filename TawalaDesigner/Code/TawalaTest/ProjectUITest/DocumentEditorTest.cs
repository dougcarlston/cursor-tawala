using System;
using System.IO;
using System.Xml;
using System.Windows.Forms;
using System.Reflection;
using System.Drawing;
using Tawala.Projects.Forms;
using Tawala.ProjectUI;
using Tawala.Documents;
using Tawala.Projects;
using Tawala.Common;
using Tawala.RtfSupport;
using NUnit.Framework;
//using NUnit.Extensions.Forms;

namespace TawalaTest.ProjectUITest
{
	[TestFixture]
	public class DocumentEditorTest// : NUnitFormTest
	{
		private TextItem ti1;
		private FibItem fi1;
		private McqItem mc1;
		private McqItem mc2;
		private IForm form;

		[SetUp]
		public /*override*/ void Setup()
		{
//			base.Setup();

			TestSupport.Util.NewTestProject();

			form = Project.Current.AddForm();

			ti1 = new TextItem();
			fi1 = new FibItem();
			mc1 = new McqItem();
			mc2 = new McqItem();
			mc2.AlternateLabel = "Choice";

			form.ItemList.Add(ti1);
			form.ItemList.Add(fi1);
			form.ItemList.Add(mc1);
			form.ItemList.Add(mc2);
		}

		public /*override*/ void TearDown()
		{
//			base.TearDown();
		}

		[Test]
		[Ignore("Ignored due to reliance on NUnitForms - SB 03/11/2008")]
		public void InsertFormField()
		{
			string rtfString =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags219" + 
				@"\txfielddataval" + mc2.Id +
				@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Choice") + "}" +
				@"<<Form 1:Choice>>{" +
				@"\*\txfieldend}\par }";
					

			System.Windows.Forms.Form f = new System.Windows.Forms.Form();
			f.Controls.Add(new DocumentEditor());
			f.Controls[0].Name = "testDocumentEditor";
//			ControlTester de = new ControlTester("testDocumentEditor");
			f.Show();

			//Assert.AreEqual(1, de.Count);

			//de.Invoke("InsertField", mc2 as IField);
			//object rtf = de.Invoke("GetRTF"); 
			//Assert.AreEqual(true, de["Visible"]);
			//Assert.AreSame(f, de["Parent"]);
			//Assert.AreEqual(rtfString, rtf.ToString());
		}

		[Test]
		public void UpdateFieldNames()
		{
			Blank blank = fi1.BlankList[0];

			string rtfCommon =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				@"\txfielddataval";

			string rtfString =
				rtfCommon + blank.Id +
				@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
				@"<<Form 1:Q1:a>>{" +
				@"\*\txfieldend}\par }";

			DocumentEditor testEditor = new DocumentEditor();
			testEditor.CreateControl();
			testEditor.Show();
			testEditor.InsertField(blank);
			Assert.AreEqual(rtfString, testEditor.GetRTF());
			int oldBlankId = blank.Id;
			form.ItemList.Insert(1, new McqItem());

			string rtfString2 =
				rtfCommon + blank.Id +
				@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q2:a") + "}" +
				@"<<Form 1:Q2:a>>{" +
				@"\*\txfieldend}\par }";

			Assert.AreEqual(rtfString2, testEditor.GetRTF());
			Assert.AreEqual(oldBlankId, blank.Id);
		}

		[Test]
		public void UpdateQualifiedFieldNameWhenFormNameChanges()
		{
			Blank blank = fi1.BlankList[0];

			string rtfCommon =
				@"{\rtf1\ansi\ansicpg1252\uc1\deff0{\fonttbl" + "\r\n" +
				@"{\f0\fswiss\fcharset0\fprq2 Arial;}" + "\r\n" +
				@"{\f1\froman\fcharset2\fprq2 Symbol;}}" + "\r\n" +
				@"{\colortbl;\red0\green0\blue0;\red255\green255\blue255;}" + "\r\n" +
				@"{\stylesheet{\s0\itap0\f0\fs24 [Normal];}{\*\cs10\additive Default Paragraph Font;}}" + "\r\n" +
				@"{\*\generator TX_RTF32 12.0.500.502;}" + "\r\n" +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs20{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				@"\txfielddataval";

			string rtfString =
				rtfCommon + blank.Id +
				@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
				@"<<Form 1:Q1:a>>{" +
				@"\*\txfieldend}\par }";

			DocumentEditor testEditor = new DocumentEditor();
			testEditor.CreateControl();
			testEditor.Show();
			testEditor.InsertField(blank);
			Assert.AreEqual(rtfString, testEditor.GetRTF());

			Assert.AreEqual("Form 1", form.Name);
			Project.Current.RenameForm("Form 1", "Form Renamed");

			string rtfString2 =
				rtfCommon + blank.Id +
				@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form Renamed:Q1:a") + "}" +
				@"<<Form Renamed:Q1:a>>{" +
				@"\*\txfieldend}\par }";

			Assert.AreEqual(rtfString2, testEditor.GetRTF());
		}
	}
}
