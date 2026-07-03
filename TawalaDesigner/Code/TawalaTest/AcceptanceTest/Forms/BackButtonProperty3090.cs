// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Forms
{
    [TestFixture]
    public class BackButtonProperty3090
    {
        #region Setup/Teardown

        [SetUp]
        public void Setup()
        {
            Util.NewTestProject();
        }

        #endregion

        private const string xmlString =
            "<form name=\"Form 1\" startPoint=\"true\" themePath=\"default\" blockBackButton=\"true\">\r\n</form>";

        [Test]
        public void BlockBackButtonIsFalseByDefaultInXml()
        {
            IForm form = Project.Current.AddForm();
            string xml = form.ToXml();

            Assert.IsTrue(xml.Contains("blockBackButton=\"false\""));
        }

        [Test]
        public void BlockBackButtonPropertyIsFalseByDefault()
        {
            IForm form = Project.Current.AddForm();

            Assert.IsFalse(form.BlockBackButton);
        }

        [Test]
        public void WhenBlockBackButtonAttributeIsTrueFormPropertyIsTrue()
        {
            IForm form = new Form(new XmlElement(xmlString));

            Assert.IsTrue(form.BlockBackButton);
        }

        [Test]
        public void WhenBlockBackButtonIsTrueXmlIsTrue()
        {
            IForm form = Project.Current.AddForm();
            form.BlockBackButton = true;
            string xml = form.ToXml();

            Assert.IsTrue(xml.Contains("blockBackButton=\"true\""));
        }
    }
}