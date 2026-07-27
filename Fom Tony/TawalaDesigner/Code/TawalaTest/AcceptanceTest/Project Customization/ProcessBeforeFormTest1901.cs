// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.IO;
using System.Text;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.ProjectCustomization
{
    /// <summary>
    /// Acceptance tests for story 1901 (Process before form).
    /// </summary>
    [TestFixture]
    public class ProcessBeforeFormTest1901
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form1 = Project.Current.AddForm();
            form2 = Project.Current.AddForm();

            process1 = Project.Current.AddProcess();
            process1.Name = "PreProcess 1";

            process2 = Project.Current.AddProcess();
            process2.Name = "PreProcess 2";
        }

        #endregion

        private IForm form1;
        private IForm form2;
        private Process process1;
        private Process process2;

        [Test]
        public void CanConnectPreProcessesToMultipleForms()
        {
            Project.Current.ConnectPreProcessToForm(process1, form1.Name);

            Assert.AreSame(process1, form1.ConnectedPreProcess);

            Project.Current.ConnectPreProcessToForm(process2, form2.Name);

            Assert.AreSame(process2, form2.ConnectedPreProcess);
        }

        [Test]
        public void CanConnectPreProcessToForm()
        {
            Project.Current.ConnectPreProcessToForm(process1, form1.Name);

            Assert.AreSame(process1, form1.ConnectedPreProcess);
        }

        [Test]
        public void CanDisconnectPreProcessFromForm()
        {
            Project.Current.ConnectPreProcessToForm(process1, form1.Name);

            Project.Current.DisconnectPreProcessFromForm(form1.Name);

            Assert.AreSame(null, form1.ConnectedPreProcess);
        }

        [Test]
        public void FormXmlWithPreProcessProducesFormXmlWithPreProcess()
        {
            string xmlString =
                "<form name=\"Main Menu\" startPoint=\"true\" themePath=\"default\" preProcess=\"PreProcess 1\" blockBackButton=\"false\">" +
                "</form>";

            form1 = new Form(new XmlElement(xmlString));

            string expectedXml =
                "<form name=\"Main Menu\" startPoint=\"true\" themePath=\"default\" preProcess=\"PreProcess 1\" blockBackButton=\"false\">\r\n" +
                "</form>\r\n";

            Assert.AreEqual(expectedXml, form1.ToXml());
        }

        [Test]
        public void ProjectXmlWithPreProcessProducesFormXmlWithPreProcess()
        {
            string xmlString =
                "<project name=\"ConnectedProcess\" themePath=\"default\" format=\"1.4\">\r\n" +
                "<forms>\r\n" +
                "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" preProcess=\"PreProcess 1\" blockBackButton=\"false\">\r\n" +
                "</form>\r\n" +
                "</forms>\r\n" +
                "<processes>\r\n" +
                "<process name=\"PreProcess 1\">\r\n" +
                "</process>\r\n" +
                "</processes>\r\n" +
                "</project>";

            using (var xmlStream = new MemoryStream())
            {
                byte[] xmlByteArray = Encoding.UTF8.GetBytes(xmlString);
                xmlStream.Write(xmlByteArray, 0, xmlByteArray.Length);

                var converter = new TawalaProjectConverter(xmlStream);
                converter.ConvertXmlToProject();
            }

            form1 = Project.Current.FormList[0];

            string expectedXml =
                "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" preProcess=\"PreProcess 1\" blockBackButton=\"false\">\r\n" +
                "</form>\r\n";

            Assert.AreEqual(expectedXml, form1.ToXml());
        }
    }
}