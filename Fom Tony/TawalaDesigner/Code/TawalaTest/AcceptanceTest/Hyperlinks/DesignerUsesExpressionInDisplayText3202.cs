// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using Tawala.Projects.Links;
using Tawala.XmlSupport;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Hyperlinks
{
    [TestFixture]
    public class DesignerUsesExpressionInDisplayText3202
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();
        }

        #endregion

        [Test]
        public void OldEmptyHyperLinkXmlProducesExpectedXml()
        {
            string oldXml =
                @"<link>" + Environment.NewLine +
                @"<description>" + Environment.NewLine +
                @"<string value=""""/>" + Environment.NewLine +
                @"</description>" + Environment.NewLine +
                @"<url>" + Environment.NewLine +
                @"<string value=""http://www.tawala.com""/>" + Environment.NewLine +
                @"</url>" + Environment.NewLine +
                @"</link>" + Environment.NewLine;

            string expectedXml =
                @"<link>" + Environment.NewLine +
                @"<description>" + Environment.NewLine +
                @"</description>" + Environment.NewLine +
                @"<url>" + Environment.NewLine +
                @"<string value=""http://www.tawala.com""/>" + Environment.NewLine +
                @"</url>" + Environment.NewLine +
                @"</link>" + Environment.NewLine;

            var link = new Hyperlink(new XmlElement(oldXml));

            Assert.AreEqual(expectedXml, link.ToXml());
        }

        [Test]
        public void HyperLinkDescriptionExpressionRoundTripsInXml()
        {
            string expectedXml =
                @"<link>" + Environment.NewLine +
                @"<description>" + Environment.NewLine +
                @"<field name=""Form 1:Q1:a""/>" + Environment.NewLine +
                @"</description>" + Environment.NewLine +
                @"<url>" + Environment.NewLine +
                @"<string value=""http://www.tawala.com""/>" + Environment.NewLine +
                @"</url>" + Environment.NewLine +
                @"</link>" + Environment.NewLine;

            var link = new Hyperlink(new XmlElement(expectedXml));

            var form = Project.Current.AddForm();
            var fib = new FibItem();
            form.ItemList.Add(fib);
            
            Assert.AreEqual(expectedXml, link.ToXml());
        }
    }
}