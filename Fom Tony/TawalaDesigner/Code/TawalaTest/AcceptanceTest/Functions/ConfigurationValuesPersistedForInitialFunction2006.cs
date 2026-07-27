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
    public class ConfigurationValuesPersistedForInitialFunction2006 : FunctionTest
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

        private IFunction createPopularChoiceTableFunction()
        {
            IFunction function = functions["popular-choice-correlation-table"].CreateInstance();

            function.SetValue("rank", 2);
            function.SetValue("choice-available-field-name", mcItem1);
            function.SetValue("choice-preferred-field-name", mcItem2);
            function.SetValue("popular-choice-display-field-name", fibItem.BlankList[0]);

            Project.FunctionMapById.AddUnique(function);

            return function;
        }

        [Test]
        public void PopularChoiceTableFunctionFieldRtfProducesPopularChoiceTableFunctionFieldRtf()
        {
            IFunction function = createPopularChoiceTableFunction();

            string encryptedFunctionData = RtfUtility.EncodeHexString(@"FF$" + function.InstanceId, false);

            string rtfString = rtfPrefixString + @"\deftab1134\paperw12240\paperh15840\margl720\margt720\margr720\margb720\notabind\pard" +
                               @"\itap0\plain\f0\fs24" + @"{\*\txfieldstart\txfieldtype0\txfieldflags216" + @"\txfielddataval" +
                               function.InstanceId + @"\txfielddata " + encryptedFunctionData + "}" +
                               @"<<RANKED MULTIQUESTION RESPONSE LIST>>{" + @"\*\txfieldend}\par }";

            var parser = new RtfParser(rtfString);
            parser.Parse();

            IXmlElement element = new XmlElement(parser.ToXml());

            var paragraph = new Paragraph(element);

            string expectedRtf = @"\pard " + @"{\*\txfieldstart\txfieldtype0\txfieldflags" + RtfUtility.NonEditableFieldFlags +
                                 @"\txfielddataval" + function.InstanceId + @"\txfielddata " +
                                 RtfUtility.EncodeHexString(@"FF$" + function.InstanceId) + "}" + function.ToDisplayString() + "{" +
                                 @"\*\txfieldend}\par ";

            Assert.AreEqual(expectedRtf, paragraph.ToRtf());
        }

        [Test]
        public void PopularChoiceTableFunctionFieldXmlProducesPopularChoiceTableFunctionFieldRtf()
        {
            IFunction function = createPopularChoiceTableFunction();

            string functionFieldXml = "<functionField instanceId=\"" + function.InstanceId + "\"/>";

            IXmlElement element = new XmlElement(functionFieldXml);

            DocumentFunctionField functionField = new DocumentIdedFunctionField(element);

            string expectedRtf = @"{\*\txfieldstart\txfieldtype0\txfieldflags" + RtfUtility.NonEditableFieldFlags + @"\txfielddataval" +
                                 function.InstanceId + @"\txfielddata " + RtfUtility.EncodeHexString(@"FF$" + function.InstanceId) + "}" +
                                 function.ToDisplayString() + "{" + @"\*\txfieldend}";

            Assert.AreEqual(expectedRtf, functionField.ToRtf());
        }

        [Test]
        public void PopularChoiceTableFunctionPersistenceXmlProducesPopularChoiceTableFunctionPersistenceXml()
        {
            IFunction function = createPopularChoiceTableFunction();

            string xmlString = "<popular-choice-correlation-table version=\"1\">" + "<rank>2</rank>" +
                               "<choice-available-field-name>Form 1:Q2</choice-available-field-name>" +
                               "<choice-preferred-field-name>Form 1:Q3</choice-preferred-field-name>" +
                               "<popular-choice-display-field-name>Form 1:Q1:a</popular-choice-display-field-name>" +
                               "<conditions></conditions>" + "</popular-choice-correlation-table>";

            IXmlElement element = new XmlElement(xmlString);

            DocumentFunctionField functionField = new DocumentPersistedFunctionField(element);

            Assert.AreEqual(xmlString, functionField.ToXml());
        }

        [Test]
        public void RecordCountFunctionPersistenceXmlProducesRecordCountFunctionPersistenceXml()
        {
            IFunction function = createPopularChoiceTableFunction();

            string xmlString = "<record-count version=\"3\">" + "<form-name>Form 1</form-name>" + "<conditions>" +
                               "<form name=\"Form 1\" />" + "</conditions>" + "</record-count>";

            IXmlElement element = new XmlElement(xmlString);

            DocumentFunctionField functionField = new DocumentPersistedFunctionField(element);

            Assert.AreEqual(xmlString, functionField.ToXml());
        }
    }
}