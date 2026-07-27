// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItems
{
    /// <summary>
    /// Acceptance tests for story 1193 (Text Entry Box).
    /// </summary>
    [TestFixture]
    public class TextEntryBoxTest1193
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            form = Project.Current.AddForm();
        }

        #endregion

        private IForm form;

        [Test]
        public void BlankXmlWithHeightProducesBlankXmlWithHeight()
        {
            string blankXml = "<blank label=\"a\" length=\"50\" height=\"10\" required=\"false\" alternateLabel=\"Alternate\"></blank>";

            IXmlElement element = new XmlElement(blankXml);
            var blank = new Blank(element, new FibItem());

            Assert.AreEqual(blankXml, blank.ToXml());
        }
    }
}