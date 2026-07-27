using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.RtfSupport;
using NUnit.Framework;

namespace TawalaTest.BugTest
{
    [TestFixture]
    public class ClipboardCopyPasteForm295 : ClipboardTester<IForm>
    {
        private IForm form;

        [SetUp]
        public void SetUp()
        {
            SetUpTest();
            form = project.AddForm();
        }

        [Test]
		public void CopyPasteEmptyForm()
        {
            IForm clipboardCopy = CopyPaste();
            Assert.IsNotNull(clipboardCopy, ErrorMessage);
        }

        protected override IForm GetComponent()
        {
            return Project.Current.FormList[0];
        }

		[Test]
		[Ignore]
		public void CopyPasteFormWithTextItemWithQualifiedField()
		{
			FibItem fibItem = new FibItem();
			form.ItemList.Add(fibItem);

			string rtfString =
				RtfDocument.RtfStringPrefix +
				@"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
				@"\itap0\plain\f0\fs24{\*\txfieldstart\txfieldtype0\txfieldflags219" +
				string.Format(@"\txfielddataval{0}", fibItem.BlankList[0].Id) +
				@"\txfielddata " + RtfUtility.EncodeHexString("TF$Form 1:Q1:a") + "}" +
				@"<<Form 1:Q1:a>>{" +
				@"\*\txfieldend}\par }";

			TextItem textItem = new TextItem();
			textItem.Rtf = rtfString;

			form.ItemList.Add(textItem);

			IForm clipboardCopy = CopyPaste();
			Assert.IsNotNull(clipboardCopy, ErrorMessage);

			TextItem copiedTextItem = clipboardCopy.ItemList[1] as TextItem;
			Assert.IsNotNull(copiedTextItem);

			Paragraph paragraph = (Paragraph)copiedTextItem.Contents[0];

			Assert.IsInstanceOfType(typeof(FormItemNamedField), paragraph.Contents[0]);
			Assert.IsNotNull(((FormItemNamedField)paragraph.Contents[0]).Field);
			Assert.AreSame(fibItem.BlankList[0], ((FormItemNamedField)paragraph.Contents[0]).Field);
		}
	}
}
