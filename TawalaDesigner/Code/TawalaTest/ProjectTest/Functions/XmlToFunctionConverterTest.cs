// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.ProjectTest
{
    [TestFixture]
    public class XmlToFunctionConverterTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();

            fibItem1 = new FibItem();
            blank1 = fibItem1.BlankList[0];
            fibItem2 = new FibItem();
            blank2 = fibItem2.BlankList[0];

            mcItem1 = new McqItem();
            mcItem2 = new McqItem();

            form.ItemList.Add(fibItem1);
            form.ItemList.Add(fibItem2);
            form.ItemList.Add(mcItem1);
            form.ItemList.Add(mcItem2);

            form.ItemList.Add(new HiddenField());
        }

        #endregion

        protected IFunctionRepository functionRepository;
        private IForm form;
        private FibItem fibItem1, fibItem2;
        private Blank blank1, blank2;
        private McqItem mcItem1, mcItem2;

        [TestFixtureSetUp]
        public void FixtureSetup()
        {
            FunctionLoader.BuildAndLoad(XmlConstants.FunctionRepositoryXml);
            functionRepository = FunctionLoader.Repository;

            Assert.IsNotNull(functionRepository);
        }

        private static readonly string simpleListXml =
            "<simple-list version=\"2\">" +
            "<simple-list-field>Form 1:Q1:a</simple-list-field>" +
            "<conditions></conditions>" +
            "</simple-list>";

        private static readonly string popularChoiceCorrelationTableFunctionXml =
            "<popular-choice-correlation-table version=\"1\">" +
            "<rank>2</rank>" +
            "<choice-available-field-name>Form 1:Q3</choice-available-field-name>" +
            "<choice-preferred-field-name>Form 1:Q2</choice-preferred-field-name>" +
            "<popular-choice-display-field-name>Form 1:Q1:a</popular-choice-display-field-name>" +
            "<conditions><form name=\"Form 1\" /></conditions>" +
            "</popular-choice-correlation-table>";

        // Version 1 Xml has column header as type "text"
        private static readonly string itemizationTableFunctionVersion1Xml =
            "<itemization-table version=\"1\">" +
            "<number-of-columns>2</number-of-columns>" +
            "<column>" +
            "<header>Name</header>" +
            "<contents><field name=\"Record:Form 1:Q1:a\" /></contents>" +
            "</column>" +
            "<column>" +
            "<header>Address</header>" +
            "<contents><field name=\"Record:Form 1:Q2:a\" /></contents>" +
            "</column>" +
            "<conditions><form name=\"Form 1\" />" +
            "<conditions>" + Environment.NewLine +
            "<equals field=\"Form 1:Q1:a\">" + Environment.NewLine +
            "<string value=\"nonsense\"/>" + Environment.NewLine +
            "</equals>" + Environment.NewLine +
            "</conditions>" +
            "</conditions>" +
            "</itemization-table>";

        // Version 2 Xml has column header as type "expression"
        private static readonly string itemizationTableFunctionVersion2Xml =
            "<itemization-table version=\"2\">" +
			"<show-print-control>false</show-print-control><show-export-control>false</show-export-control>" +
            "<column>" +
            "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" +
            "<contents><field name=\"Record:Form 1:Q1:a\" /></contents>" +
            "</column>" +
            "<column>" +
            "<header><string value=\"Address\"/>" + Environment.NewLine + "</header>" +
            "<contents><field name=\"Record:Form 1:Q2:a\" /></contents>" +
            "</column>" +
            "<conditions><form name=\"Form 1\" />" +
            "<conditions>" + Environment.NewLine +
            "<equals field=\"Form 1:Q1:a\">" + Environment.NewLine +
            "<string value=\"nonsense\"/>" + Environment.NewLine +
            "</equals>" + Environment.NewLine +
            "</conditions>" +
            "</conditions>" +
            "</itemization-table>";

        private static readonly string categorizerFunctionVersion2XmlWithFieldInExpression =
            "<categorizer version=\"2\">" +
            "<category-names>Record:Form 2:Q1:a</category-names>" +
            "<category-ids>Record:Form 2:Q2:a</category-ids>" +
            "<category-storage-field>Record:Form 1:Category ID</category-storage-field>" +
            "<navigate-to>Form 1</navigate-to>" +
            "<column>" +
            "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" +
            "<contents><field name=\"Record:Form 1:Q1:a\" /></contents>" +
            "</column>" +
            "<column>" +
            "<header><field name=\"Form 1:Field1\"/>" + Environment.NewLine + "<string value=\" Address\"/>" + Environment.NewLine +
            "</header>" +
            "<contents><field name=\"Record:Form 1:Q2:a\" /></contents>" +
            "</column>" +
            "<conditions>" +
            "<form name=\"Form 2\" /><form name=\"Form 1\" />" +
            "<conditions>" + Environment.NewLine +
            "<doesNotContain field=\"Form 1:Q1:a\">" + Environment.NewLine +
            "<string value=\"nonsense\"/>" + Environment.NewLine +
            "</doesNotContain>" + Environment.NewLine +
            "</conditions>" +
            "</conditions>" +
            "</categorizer>";

        // Version 1 Xml has column header as type "text"
        private static readonly string categorizerFunctionVersion1Xml =
            "<categorizer version=\"2\">" +
            "<category-names>Record:Form 2:Q1:a</category-names>" +
            "<category-ids>Record:Form 2:Q2:a</category-ids>" +
            "<category-storage-field>Record:Form 1:Category ID</category-storage-field>" +
            "<navigate-to>Form 1</navigate-to>" +
            "<number-of-columns>2</number-of-columns>" +
            "<column>" +
            "<header>Name</header>" +
            "<contents><field name=\"Record:Form 1:Q1:a\" /></contents>" +
            "</column>" +
            "<column>" +
            "<header>Address</header>" +
            "<contents><field name=\"Record:Form 1:Q2:a\" /></contents>" +
            "</column>" +
            "<conditions>" +
            "<form name=\"Form 2\" /><form name=\"Form 1\" />" +
            "<conditions>" + Environment.NewLine +
            "<doesNotContain field=\"Form 1:Q1:a\">" + Environment.NewLine +
            "<string value=\"nonsense\"/>" + Environment.NewLine +
            "</doesNotContain>" + Environment.NewLine +
            "</conditions>" +
            "</conditions>" +
            "</categorizer>";

        // Version 2 Xml has column header as type "expression"
        private static readonly string categorizerFunctionVersion2Xml =
            "<categorizer version=\"2\">" +
            "<category-names>Record:Form 2:Q1:a</category-names>" +
            "<category-ids>Record:Form 2:Q2:a</category-ids>" +
            "<category-storage-field>Record:Form 1:Category ID</category-storage-field>" +
            "<navigate-to>Form 1</navigate-to>" +
            "<column>" +
            "<header><string value=\"Name\"/>" + Environment.NewLine + "</header>" +
            "<contents><field name=\"Record:Form 1:Q1:a\" /></contents>" +
            "</column>" +
            "<column>" +
            "<header><string value=\"Address\"/>" + Environment.NewLine + "</header>" +
            "<contents><field name=\"Record:Form 1:Q2:a\" /></contents>" +
            "</column>" +
            "<conditions>" +
            "<form name=\"Form 2\" /><form name=\"Form 1\" />" +
            "<conditions>" + Environment.NewLine +
            "<doesNotContain field=\"Form 1:Q1:a\">" + Environment.NewLine +
            "<string value=\"nonsense\"/>" + Environment.NewLine +
            "</doesNotContain>" + Environment.NewLine +
            "</conditions>" +
            "</conditions>" +
            "</categorizer>";

        [Test]
        public void InitialProjectSetupCheck()
        {
            Assert.AreEqual("Form 1", form.Name);
            Assert.AreEqual("Q1", fibItem1.FieldName);
            Assert.AreEqual("Q1:a", blank1.FieldName);
            Assert.AreEqual("Form 1:Q1:a", blank1.QualifiedFieldName);
        }

        [Test]
        public void RestoreFunctionFromXml()
        {
            var converter = new XmlToFunctionConverter();
            IFunction function = converter.ConvertFrom(new XmlElement(simpleListXml));
            IParameterInfo paramInfo = function.Info.Parameters[0];

            Assert.IsNotNull(function, "Failed to creation IFunction from Xml");
            Assert.IsNotNull(paramInfo.GetValue(function),
                             "Parameter: '" + paramInfo.Name + "' of type: '" + paramInfo.PropertyType.FullName + "' is Null!");

            Assert.AreEqual("simple-list", function.Info.Id, "function id didn't match");

            Assert.AreEqual(simpleListXml, function.ToXml(), "Roundtrip of function XML failed");
        }

        [Test]
        public void RestorePopularChoiceCorrelationTableFunctionFromXml()
        {
            var converter = new XmlToFunctionConverter();
            IFunction function = converter.ConvertFrom(new XmlElement(popularChoiceCorrelationTableFunctionXml));
            IParameterInfoCollection allParams = function.Info.Parameters;
            IParameterInfo paramInfo = allParams[0];

            Assert.IsNotNull(function, "Failed to creation IFunction from Xml");
            Assert.AreEqual("popular-choice-correlation-table", function.Info.Id);

            Assert.AreEqual("2", allParams[0].GetValue(function).ToString());

            Assert.IsNotNull(allParams[4].GetValue(function));
            Assert.AreSame(allParams[4].PropertyType, typeof(FunctionFilterConditions));

            Assert.AreEqual(popularChoiceCorrelationTableFunctionXml, function.ToXml(), "Roundtrip of function XML failed");
        }

        [Test]
        public void RestoringCategorizerTableFunctionFromVersion1XmlProducesVersion2Xml()
        {
            IForm form2 = Project.Current.AddForm();
            form2.ItemList.Add(new FibItem());
            form2.ItemList.Add(new FibItem());

            var converter = new XmlToFunctionConverter();
            IFunction function = converter.ConvertFrom(new XmlElement(categorizerFunctionVersion1Xml));

            Assert.IsNotNull(function, "Failed to creation IFunction from Xml");
            Assert.AreEqual("categorizer", function.Info.Id);

            Assert.AreEqual(6, function.Info.Parameters.Count);
            Assert.AreEqual("column", function.Info.Parameters[4].Id);
            Assert.AreEqual("conditions", function.Info.Parameters[5].Id);

            Assert.AreEqual(categorizerFunctionVersion2Xml, function.ToXml(), "Categorizer V1 Xml failed to convert to Categorizer V2 Xml");
        }

        [Test]
        public void RestoringCategorizerTableFunctionFromVersion2XmlProducesVersion2Xml()
        {
            IForm form2 = Project.Current.AddForm();
            form2.ItemList.Add(new FibItem());
            form2.ItemList.Add(new FibItem());

            var converter = new XmlToFunctionConverter();
            IFunction function = converter.ConvertFrom(new XmlElement(categorizerFunctionVersion2XmlWithFieldInExpression));

            Assert.IsNotNull(function, "Failed to creation IFunction from Xml");
            Assert.AreEqual("categorizer", function.Info.Id);

            Assert.AreEqual(6, function.Info.Parameters.Count);
            Assert.AreEqual("column", function.Info.Parameters[4].Id);
            Assert.AreEqual("conditions", function.Info.Parameters[5].Id);

            Assert.AreEqual(categorizerFunctionVersion2XmlWithFieldInExpression, function.ToXml(), "Roundtrip of function XML failed");
        }

        [Test]
        public void RestoringItemizationTableFunctionFromVersion1XmlProducesVersion2Xml()
        {
            var converter = new XmlToFunctionConverter();
            IFunction function = converter.ConvertFrom(new XmlElement(itemizationTableFunctionVersion1Xml));

            Assert.IsNotNull(function, "Failed to creation IFunction from Xml");
            Assert.AreEqual("itemization-table", function.Info.Id);

            Assert.AreEqual(4, function.Info.Parameters.Count);
            Assert.AreEqual("show-print-control", function.Info.Parameters[0].Id);
			Assert.AreEqual("show-export-control", function.Info.Parameters[1].Id);
			Assert.AreEqual("column", function.Info.Parameters[2].Id);
            Assert.AreEqual("conditions", function.Info.Parameters[3].Id);

            Assert.AreEqual(itemizationTableFunctionVersion2Xml, function.ToXml(), "Roundtrip of function XML failed");
        }
    }
}