// $Workfile: TypeCollectionTest.cs $
// $Revision: 1 $	$Date: 2/26/08 2:29p $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using NUnit.Framework;
using TawalaTest.TestSupport;

namespace TawalaTest.TestSupportTest
{
    [TestFixture]
    public class TypeCollectionTest
    {
        [Test]
        public void ConstructFromAssembly()
        {
            TypeCollection types = TypeCollection.Create(typeof(Tawala.Projects.Project).Assembly, TypeCollection.NullFilter);
            Assert.Contains(typeof(Tawala.Projects.Project), types);
            Assert.IsTrue(types.Count > 1);
        }
    }
}