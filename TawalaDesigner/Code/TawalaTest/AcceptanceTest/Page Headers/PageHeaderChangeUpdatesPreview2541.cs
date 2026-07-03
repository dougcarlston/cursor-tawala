// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.DesignerUI;
using Tawala.Projects;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.PageHeaders
{
    [TestFixture]
    public class PageHeaderChangeUpdatesPreview2541
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            Project.Events.PageHeaderChanged += events_PageHeaderChanged;
            pageHeaderChangedCount = 0;

            dialog = new PageHeaderDialog();
            dialog.Show();
        }

        [TearDown]
        public void TearDown()
        {
            dialog = null;
        }

        #endregion

        private PageHeaderDialog dialog;
        private int pageHeaderChangedCount;

        private void events_PageHeaderChanged(object sender, EventArgs e)
        {
            pageHeaderChangedCount++;
        }

        [Test]
        public void PageHeaderChangedFiredOnOK()
        {
            Assert.AreEqual(0, pageHeaderChangedCount);
            dialog.AcceptButton.PerformClick();
            Assert.AreEqual(1, pageHeaderChangedCount);
        }

        [Test]
        public void PageHeaderChangedNotFiredOnCancel()
        {
            Assert.AreEqual(0, pageHeaderChangedCount);
            dialog.CancelButton.PerformClick();
            Assert.AreEqual(0, pageHeaderChangedCount);
        }
    }
}