// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedMCQs
{
    /// <summary>
    /// Acceptance tests for story 2403 as applied to MCQs (Designer uses 1 default tab stop in Form Items).
    /// </summary>
    [TestFixture]
    public class DefaultTabStopsInMCQs2403
    {
        [Test]
        public void McqRtfHasOneDefaultTabStop()
        {
            var mcqItem = new McqItem();

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
                @"\pard \tx2880{\f0\fs20\cf1 ~@!~[Replace this with your question. Use Enter key to add choices below.]~@!~}\par \pard \tx2880{\f0\fs20\cf1    a) }\par }";

            Console.WriteLine(mcqItem.ToRtf());

            Assert.AreEqual(expectedRtf, mcqItem.ToRtf());
        }
    }
}