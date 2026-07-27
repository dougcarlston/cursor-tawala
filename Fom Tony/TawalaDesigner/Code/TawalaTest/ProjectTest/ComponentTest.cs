using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Tawala.Projects;
using NUnit.Framework;
using Tawala.Projects.Components;

namespace TawalaTest.ProjectTest
{
    [TestFixture]
    public class ComponentTest
    {
        [Test]
        public void ComponentRaisesSerializingEvent()
        {
            TawalaTest.TestSupport.Util.NewTestProject();
            IProjectComponent c = Project.Current.AddDocument();
            c.Name = "My Document";
            Project.Events.ComponentSerializing += componentSerializing;
            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, c);
                Assert.AreEqual("componentSerializing", c.Name);
            }
        }

        private void componentSerializing(object o, ComponentEventArgs args)
        {
            Assert.AreEqual(1, Project.Current.DocumentList.Count);
            Assert.AreSame(args.Component, Project.Current.DocumentList[0]);
            Project.Current.RenameDocument("My Document", "componentSerializing");
        }
    }
}
