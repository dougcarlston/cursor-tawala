// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedTextItems
{
    /// <summary>
    /// Acceptance tests for story 2403 as applied to Text Items (Designer uses 1 default tab stop in Form Items).
    /// </summary>
    [TestFixture]
    public class DefaultTabStopsInTextItems2403
    {
        [Test]
        public void TextItemRtfHasOneDefaultTabStop()
        {
            var textItem = new TextItem();

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                RtfConstants.DefaultFontNameRtf + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20" +
                @"{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"\red0\green0\blue1;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf + // this is what we're looking for
                @"\pard \tx2880{\f1\fs21\cf3 [Replace this with text of your own.]}\par }";

            Console.WriteLine(textItem.ToRtf());

            Assert.AreEqual(expectedRtf, textItem.ToRtf());
        }
    }
}