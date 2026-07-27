// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using NMock2;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms;
using Tawala.ProjectUI;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.DataAccess
{
    [TestFixture]
    public class SharedDataInFunctions2144 : FunctionTestBase
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
            Util.CreateTestDataSources();

            form = Project.Current.AddForm();

            fibItem = new FibItem();
            form.ItemList.Add(fibItem);

            mockery = new Mockery();

            localField = fibItem.BlankList[0];
            localRecordField = FieldUtil.RecordQualifyField(localField);

            sharedField = FieldProviders.ExternalForms["ClientInfo"].GetAllFields()[0] as IPaletteField;
            sharedRecordField = FieldUtil.RecordQualifySharedDataField(sharedField) as IPaletteField;
        }

        #endregion

        private IForm form;
        private FibItem fibItem;
        private Mockery mockery;

        private IPaletteField localField;
        private IPaletteField localRecordField;
        private IPaletteField sharedField;
        private IPaletteField sharedRecordField;

        private const string itemization_table_xml =
			"<itemization-table version=\"2\"><show-print-control>false</show-print-control><show-export-control>false</show-export-control>" +
            "<column><header><string value=\"Test\"/>\r\n</header>" +
            "<contents><field name=\"Record:ClientInfo:Q1:a\" /></contents></column>" +
            "<conditions><form name=\"Form 1\" /></conditions></itemization-table>";

        [Test]
        public void FieldPaletteContainsSharedDataSources()
        {
            var palette = new FieldsPalette();
            TreeView tree = palette.FieldsTreeView;
            FieldsPalette.ConfigureFunctionActive = false;

            foreach (TreeNode node in tree.Nodes)
            {
                Assert.That(FieldProviders.ExternalForms.IndexOf(node.Name) < 0);
            }

            FieldsPalette.ConfigureFunctionActive = false;

            foreach (TreeNode node in tree.Nodes)
            {
                string name = node.Name;
                Assert.That(FieldProviders.ExternalForms.IndexOf(name) >= 0 || name.CompareTo(form.Name) == 0 ||
                            name.CompareTo("Variables") == 0);
            }
        }

        [Test]
        public void FieldUtilGetFieldName()
        {
            Assert.That(FieldUtil.GetFieldName(localField.QualifiedFieldName).CompareTo("Q1:a") == 0);
            Assert.That(FieldUtil.GetFieldName(localRecordField.QualifiedFieldName).CompareTo("Q1:a") == 0);

            Assert.That(FieldUtil.GetFieldName(sharedField.QualifiedFieldName).CompareTo("Q1:a") == 0);
            Assert.That(FieldUtil.GetFieldName(sharedRecordField.QualifiedFieldName).CompareTo("Q1:a") == 0);
        }

        [Test]
        public void FieldUtilGetFormName()
        {
            Assert.That(FieldUtil.GetFormName(localField.QualifiedFieldName).CompareTo("Form 1") == 0);
            Assert.That(FieldUtil.GetFormName(localRecordField.QualifiedFieldName).CompareTo("Form 1") == 0);

            Assert.That(FieldUtil.GetFormName(sharedField.QualifiedFieldName).CompareTo("ClientInfo") == 0);
            Assert.That(FieldUtil.GetFormName(sharedRecordField.QualifiedFieldName).CompareTo("ClientInfo") == 0);
        }

        [Test]
        public void FieldUtilGetRecordName()
        {
            Assert.That(FieldUtil.GetRecordName(localRecordField.QualifiedFieldName).CompareTo("Record") == 0);
            Assert.That(FieldUtil.GetRecordName(sharedRecordField.QualifiedFieldName).CompareTo("Record") == 0);
        }

        [Test]
        public void FieldUtilIsExternalField()
        {
            Assert.That(!FieldUtil.IsExternalField(localField.QualifiedFieldName));
            Assert.That(FieldUtil.IsExternalField(sharedField.QualifiedFieldName));
        }

        [Test]
        public void FieldUtilRecordQualifyField()
        {
            Assert.That(localRecordField is RecordField);
            Assert.That(sharedRecordField is RecordField);
        }

        [Test]
        public void FieldUtilRecordQualifySharedDataField()
        {
            Assert.That(localField is Blank);
            Assert.That(sharedField is Blank);
            Assert.That(sharedRecordField is RecordField);
        }

        [Test]
        public void FormCollectionFormsHaveCorrectAttributeValue()
        {
            var conditions = new FunctionFilterConditions();
            var collection = new FunctionFormCollection();
            collection.AddUnique(form);
            collection.AddUnique(FieldProviders.ExternalForms[0]);
            conditions.Forms = collection;

            Assert.AreEqual("<form name=\"Form 1\" /><form name=\"ClientInfo\" externalSharedData=\"true\" />", collection.ToXml());
        }

        [Test]
        public void ItemizationTableFunctionWithSharedDataParameter()
        {
            try
            {
                functionSetup();

                IFunction function = FunctionLoader.Repository.Functions["itemization-table"].CreateInstance();

                IParameterInfo compositeCollection = function.Info.Parameters["column"];
                var collection = Activator.CreateInstance(compositeCollection.PropertyType) as ICompositeParameterCollection;
                string compositeName = compositeCollection.PropertyType.FullName.Replace("__composite_collection_", "__composite_");

                Type compositeType = Type.GetType(compositeName);
                var composite1 = Activator.CreateInstance(compositeType) as ICompositeParameter;
                composite1["header"] = new FunctionCompoundExpression(new XmlElement("<container><string value=\"Test\"/></container>"));
                composite1["contents"] = new FunctionContentsField(sharedRecordField);

                collection.Add(composite1);

                Assert.AreEqual(1, collection.Count);

                compositeCollection.SetValue(function, collection);

                var functionConditions = new FunctionFilterConditions();
                functionConditions.Forms = new FunctionFormCollection(form);
                function["conditions"] = functionConditions;

                Project.FunctionMapById.AddUnique(function);

                Assert.AreEqual(itemization_table_xml, function.ToXml());
            }
            finally
            {
                functionTearDown();
            }
        }
    }
}