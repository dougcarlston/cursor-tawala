using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using NUnit.Framework;
using NMock2;
using Tawala.Proj;
using Tawala.Common;
using Tawala.XmlSupport;
using Tawala.Functions.Runtime;
using Tawala.Functions.ViewPresenter;
using TawalaTest.TestSupport;

namespace TawalaTest.AcceptanceTest.Functions
{
    [TestFixture]
    class DesignerHasAccessToFunctonsOffline2005 : FunctionTest
    {
        [TestFixtureSetUp]
        public void TestFixtureSetup()
        {
            fixtureSetup();
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            fixtureTearDown();
        }

    }
}
