// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms;
using Tawala.Projects.Function;
using Tawala.RtfSupport;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Functions
{
    [TestFixture]
    public class DesignerInsertsInitialFunctionFieldInDocument2002 : FunctionTest
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();

            fibItem = new FibItem();
            mcItem1 = new McqItem();
            mcItem2 = new McqItem();

            form.ItemList.Add(fibItem);
            form.ItemList.Add(mcItem1);
            form.ItemList.Add(mcItem2);
        }

        #endregion

        private IForm form;
        private FibItem fibItem;
        private McqItem mcItem1;
        private McqItem mcItem2;

        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            functionSetup();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            functionTearDown();
        }

        private IFunction createRecordCountFunction()
        {
            IFunction function = functions["record-count"].CreateInstance();

            function.SetValue("form-name", form);

            var functionConditions = new FunctionFilterConditions();
            functionConditions.Forms = new FunctionFormCollection(form);
            function.SetValue("conditions", functionConditions);

            Project.FunctionMapById.AddUnique(function);

            return function;
        }

        private IFunction createPopularChoiceTableFunction()
        {
            IFunction function = functions["popular-choice-correlation-table"].CreateInstance();

            function.SetValue("rank", "2");

            var functionMCItem1 = new FunctionMCItem(new RecordField(new Record("Record"), mcItem1));
            function.SetValue("choice-available-field-name", functionMCItem1);

            var functionMCItem2 = new FunctionMCItem(new RecordField(new Record("Record"), mcItem2));
            function.SetValue("choice-preferred-field-name", functionMCItem2);

            var functionBlank = new FunctionBlank(new RecordField(new Record("Record"), fibItem.BlankList[0]));
            function.SetValue("popular-choice-display-field-name", functionBlank);

            Project.FunctionMapById.AddUnique(function);

            return function;
        }

        [Test]
        public void FunctionFieldRtfProducesFunctionFieldXml()
        {
            IFunction function = functions["record-count"].CreateInstance();

            var functionConditions = new FunctionFilterConditions();
            functionConditions.Forms = new FunctionFormCollection(form);
            function.SetValue("conditions", functionConditions);

            string encryptedFunctionData = RtfUtility.EncodeHexString(@"FF$" + function.InstanceId, false);

            string rtfString = rtfPrefixString + @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                               @"\itap0\plain\f0\fs24" + @"{\*\txfieldstart\txfieldtype0\txfieldflags216" + @"\txfielddataval" +
                               function.InstanceId + @"\txfielddata " + encryptedFunctionData + "}" + @"<<FORM RECORD COUNT>>{" +
                               @"\*\txfieldend}\par }";

            var parser = new RtfParser(rtfString);
            parser.Parse();

            string expectedXml = "<paragraph indent=\"0\" align=\"left\">" + "<functionField instanceId=\"" + function.InstanceId +
                                 "\"/></paragraph>";

            Assert.AreEqual(expectedXml, parser.ToXml());
        }

        [Test]
        public void PopularChoiceTableComponentProducesExpectedDisplayString()
        {
            IFunction function = createPopularChoiceTableFunction();

            string expectedString = "<<RANKED MULTIQUESTION RESPONSE LIST(2, Record:Form 1:Q2, ...)>>";

            Assert.AreEqual(expectedString, function.ToDisplayString());
        }

        [Test]
        public void PopularChoiceTableComponentProducesPopularChoiceTableXml()
        {
            IFunction function = createPopularChoiceTableFunction();

            string expectedString = "<popular-choice-correlation-table version=\"1\">" + "<rank>2</rank>" +
                                    "<choice-available-field-name>Record:Form 1:Q2</choice-available-field-name>" +
                                    "<choice-preferred-field-name>Record:Form 1:Q3</choice-preferred-field-name>" +
                                    "<popular-choice-display-field-name>Record:Form 1:Q1:a</popular-choice-display-field-name>" +
                                    "<conditions></conditions>" + "</popular-choice-correlation-table>";

            Assert.AreEqual(expectedString, function.ToXml());
        }

        [Test]
        public void PopularChoiceTableComponentProducesPopularChoiceTableXmlWithDefaultValues()
        {
            IFunction function = functions["popular-choice-correlation-table"].CreateInstance();

            string expectedString = "<popular-choice-correlation-table version=\"1\">" + "<rank>1</rank>" +
                                    "<choice-available-field-name></choice-available-field-name>" +
                                    "<choice-preferred-field-name></choice-preferred-field-name>" +
                                    "<popular-choice-display-field-name></popular-choice-display-field-name>" + "<conditions></conditions>" +
                                    "</popular-choice-correlation-table>";

            Assert.AreEqual(expectedString, function.ToXml());
        }

        [Test]
        public void PopularChoiceTableFunctionFieldRtfProducesPopularChoiceTableFunctionXml()
        {
            IFunction function = createPopularChoiceTableFunction();

            string encryptedFunctionData = RtfUtility.EncodeHexString(@"FF$" + function.InstanceId, false);

            string rtfString = rtfPrefixString + @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                               @"\itap0\plain\f0\fs24" + @"{\*\txfieldstart\txfieldtype0\txfieldflags216" + @"\txfielddataval" +
                               function.InstanceId + @"\txfielddata " + encryptedFunctionData + "}" + @"<<FORM RECORD COUNT>>{" +
                               @"\*\txfieldend}\par }";

            var parser = new RtfParser(rtfString);
            parser.Parse();

            IXmlElement element = new XmlElement(parser.ToXml());

            var paragraph = new Paragraph(element);

            string expectedXml = "<paragraph indent=\"0\" align=\"left\">" + "<popular-choice-correlation-table version=\"1\">" +
                                 "<rank>2</rank>" + "<choice-available-field-name>Record:Form 1:Q2</choice-available-field-name>" +
                                 "<choice-preferred-field-name>Record:Form 1:Q3</choice-preferred-field-name>" +
                                 "<popular-choice-display-field-name>Record:Form 1:Q1:a</popular-choice-display-field-name>" +
                                 "<conditions></conditions>" + "</popular-choice-correlation-table>" + "</paragraph>";

            Assert.AreEqual(expectedXml, paragraph.ToXml());
        }

        [Test]
        public void RecordCountFunctionFieldRtfProducesRecordCountFunctionXml()
        {
            IFunction function = createRecordCountFunction();

            string encryptedFunctionData = RtfUtility.EncodeHexString(@"FF$" + function.InstanceId, false);

            string rtfString = rtfPrefixString + @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                               @"\itap0\plain\f0\fs24" + @"{\*\txfieldstart\txfieldtype0\txfieldflags216" + @"\txfielddataval" +
                               function.InstanceId + @"\txfielddata " + encryptedFunctionData + "}" + @"<<FORM RECORD COUNT>>{" +
                               @"\*\txfieldend}\par }";

            var parser = new RtfParser(rtfString);
            parser.Parse();

            IXmlElement element = new XmlElement(parser.ToXml());

            var paragraph = new Paragraph(element);

            string expectedXml = "<paragraph indent=\"0\" align=\"left\">" + "<record-count version=\"3\">" +
                                 "<form-name>Form 1</form-name>" + "<conditions>" + "<form name=\"Form 1\" />" + "</conditions>" +
                                 "</record-count>" + "</paragraph>";

            Assert.AreEqual(expectedXml, paragraph.ToXml());
        }

        [Test]
        public void RecordCountFunctionFieldXmlProducesRecordCountFunctionXml()
        {
            IFunction function = createRecordCountFunction();

            string functionFieldXml = "<functionField instanceId=\"" + function.InstanceId + "\"/>";

            IXmlElement element = new XmlElement(functionFieldXml);

            DocumentFunctionField functionField = new DocumentIdedFunctionField(element);

            string expectedXml = "<record-count version=\"3\">" + "<form-name>Form 1</form-name>" + "<conditions>" +
                                 "<form name=\"Form 1\" />" + "</conditions>" + "</record-count>";

            Assert.AreEqual(expectedXml, functionField.ToXml());
        }

        [Test]
        public void RecordCountFunctionProducesExpectedDisplayString()
        {
            IFunction function = createRecordCountFunction();

            string expectedString = "<<FORM RECORD COUNT(Form 1)>>";

            Assert.AreEqual(expectedString, function.ToDisplayString());
        }

        [Test]
        public void RecordCountFunctionProducesRecordCountXml()
        {
            IFunction function = createRecordCountFunction();

            string expectedString = "<record-count version=\"3\">" + "<form-name>Form 1</form-name>" + "<conditions>" +
                                    "<form name=\"Form 1\" />" + "</conditions>" + "</record-count>";

            Assert.AreEqual(expectedString, function.ToXml());
        }

        [Test]
        public void RecordCountFunctionProducesRecordCountXmlWithDefaultValues()
        {
            IFunction function = functions["record-count"].CreateInstance();

            function.SetValue("form-name", form);

            string expectedString = "<record-count version=\"3\">" + "<form-name>Form 1</form-name>" + "<conditions>" + "</conditions>" +
                                    "</record-count>";

            Assert.AreEqual(expectedString, function.ToXml());
        }
    }
}