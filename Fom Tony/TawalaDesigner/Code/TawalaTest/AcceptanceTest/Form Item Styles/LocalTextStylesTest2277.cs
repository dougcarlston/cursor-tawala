// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItemStyles
{
    [TestFixture]
    public class LocalTextStylesTest2277
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            Util.NewTestProject();

            setupForms();
        }

        #endregion

        private IForm form1;
        private IForm form2;
        private TextItem textItem1;
        private TextItem textItem2;
        private TextItem textItem3;

        private void setupForms()
        {
            form1 = Project.Current.AddForm();
            form2 = Project.Current.AddForm();

            textItem1 = new TextItem();
            textItem1.Text = "Text Item 1";
            form1.ItemList.Add(textItem1);

            textItem2 = new TextItem();
            textItem2.Text = "Text Item 2";
            form2.ItemList.Add(textItem2);

            textItem3 = new TextItem();
            textItem3.Text = "Text Item 3";
            form2.ItemList.Add(textItem3);
        }
    }
}