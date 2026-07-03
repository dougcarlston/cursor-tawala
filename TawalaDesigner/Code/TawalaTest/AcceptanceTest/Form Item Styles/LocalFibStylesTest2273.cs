// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItemStyles
{
    [TestFixture]
    public class LocalFibStylesTest2273
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
        private FibItem fibItem1;
        private FibItem fibItem2;
        private FibItem fibItem3;

        private void setupForms()
        {
            form1 = Project.Current.AddForm();
            form2 = Project.Current.AddForm();

            fibItem1 = new FibItem();
            fibItem1.Text = "Fib Item 1 __________";
            form1.ItemList.Add(fibItem1);

            fibItem2 = new FibItem();
            fibItem2.Text = "Fib Item 2 __________";
            form2.ItemList.Add(fibItem2);

            fibItem3 = new FibItem();
            fibItem3.Text = "Fib Item 3 __________";
            form2.ItemList.Add(fibItem3);
        }
    }
}