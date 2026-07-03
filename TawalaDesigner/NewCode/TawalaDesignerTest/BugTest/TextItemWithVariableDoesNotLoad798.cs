using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using NUnit.Framework;
using TawalaTest.TestingSupport;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Forms.FormItemContents;

namespace TawalaTest.BugTest
{
	[TestFixture]
	public class TextItemWithVariableDoesNotLoad798
	{
		[SetUp]
		public void SetUp()
		{
		}

		[Test]
		public void TextItemContainingVariableLoadsProperly()
		{
			string filePath = Util.GetTestFilePath("VariableInTextItem.tawala");
			string projectXml = File.ReadAllText(filePath);

			ComponentMaker.UseNewComponents(true);
			Project.Create(new XmlElement(projectXml));
			
			IForm form = Project.Current.FormList[0];
			NewTextItem textItem = form.ItemList[0] as NewTextItem;

			IField field = getFieldFromContents(textItem);
			Assert.IsNotNull(field);
			Assert.AreEqual("Var", field.FieldName);
		}

		private static IField getFieldFromContents(NewTextItem textItem)
		{
			FieldReference fieldReference = getFieldReferenceFromContents(textItem);
			return getFieldFromReference(fieldReference);
		}

		private static FieldReference getFieldReferenceFromContents(NewTextItem textItem)
		{
			FormItemContentsCollection fieldReferences = textItem.Contents.GetDescendants(typeof(FieldReference));
			Assert.IsNotNull(fieldReferences);

			return fieldReferences[0] as FieldReference;
		}

		private static IField getFieldFromReference(FieldReference fieldReference)
		{
			return Reflect<FieldReference>.GetField<IField>("field", fieldReference);
		}
	}
}