// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Documents;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormattedDocuments
{
    /// <summary>
    /// Acceptance tests for story 2357 (Designer uses 1 default tab stop in Documents).
    /// </summary>
    [TestFixture]
    public class DefaultTabStops2357
    {
        [Test]
        public void DocumentRtfHasOneDefaultTabStop()
        {
            var rtfDocument = new RtfDocument("Document 1");

            string expectedRtf =
                @"{\rtf1\ansi\ansicpg1252\uc1\deff0" + Environment.NewLine +
                @"{\fonttbl" + Environment.NewLine +
                @"}" + Environment.NewLine +
                @"\fs20" +
                @"{\colortbl;" + Environment.NewLine +
                @"}" + Environment.NewLine +
                RtfConstants.DefaultTabsRtf + // this is what we're looking for
                @"}";

            Assert.AreEqual(expectedRtf, rtfDocument.ToRtf());
        }
    }
}