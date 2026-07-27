// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using NUnit.Framework;
using Tawala.Projects;
using Tawala.Projects.Forms;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.FormItemStyles
{
    [TestFixture]
    public class LocalMCQStylesTest2275
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
        private McqItem mcItem1;
        private McqItem mcItem2;
        private McqItem mcItem3;

        private void setupForms()
        {
            form1 = Project.Current.AddForm();
            form2 = Project.Current.AddForm();

            mcItem1 = new McqItem();
            mcItem1.Text = "MCQ1";
            mcItem1.Choices.Clear();
            mcItem1.Choices.Add(new Choice("Choice A"));
            mcItem1.Choices.Add(new Choice("Choice B"));
            form1.ItemList.Add(mcItem1);

            mcItem2 = new McqItem();
            mcItem2.Text = "MCQ2";
            mcItem2.Choices.Clear();
            mcItem2.Choices.Add(new Choice("Choice A"));
            mcItem2.Choices.Add(new Choice("Choice B"));
            form2.ItemList.Add(mcItem2);

            mcItem3 = new McqItem();
            mcItem3.Text = "MCQ3";
            mcItem3.Choices.Clear();
            mcItem3.Choices.Add(new Choice("Choice A"));
            mcItem3.Choices.Add(new Choice("Choice B"));
            form2.ItemList.Add(mcItem3);
        }
    }
}