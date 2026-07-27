// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedFIBs
{
    /// <summary>
    /// Acceptance tests for story 2403 as applied to FIBs (Designer uses 1 default tab stop in Form Items).
    /// </summary>
    [TestFixture]
    public class DefaultTabStopsInFibs2403
    {
        [Test]
        public void FibRtfHasOneDefaultTabStop()
        {
            var fibItem = new FibItem();

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"{\f0\fswiss Arial;}" + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20" +
                @"{\colortbl;" + Environment.NewLine +
                @"\red0\green0\blue0;" + Environment.NewLine +
                @"\red255\green255\blue255;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf + // this is what we're looking for
                @"\pard \tx2880{\f0\fs20\cf1 ~@!~[Replace this with your question. Underscores create blanks.]~@!~ }____________________\par }";

            Assert.AreEqual(expectedRtf, fibItem.ToRtf());
        }
    }
}