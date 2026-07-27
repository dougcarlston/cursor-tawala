using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Tawala.Projects;
using NUnit.Framework;
using Tawala.Projects.Components;

namespace TawalaTest.BugTest
{
    public abstract class ClipboardTester<T> where T : class, IProjectComponent
    {
        public T CopyPaste()
        {
            return clipboardCopyPaste();
        }

        public string ErrorMessage
        {
            get { return string.Format("Failed to create copy of {0} from contents of clipboard", typeof(T).Name); }
        }

        protected abstract T GetComponent();

        protected Project project;

        protected void SetUpTest()
        {
            TestSupport.Util.NewTestProject();
            project = Project.Current;
        }

        private T clipboardCopyPaste()
        {
			T component = default(T), copiedComponent = default(T);

            try
            {
				component = GetComponent();
				Assert.IsNotNull(component, "TEST FAULT: Current Project missing " + typeof(T).Name);

				System.Windows.Forms.DataObject dataObject = new System.Windows.Forms.DataObject();
				dataObject.SetData(typeof(T), component);
				System.Windows.Forms.Clipboard.SetDataObject(dataObject);

				copiedComponent = System.Windows.Forms.Clipboard.GetDataObject().GetData(typeof(T)) as T;
				Assert.AreNotSame(component, copiedComponent, "TEST FAULT: original component and copied component are same reference??");
			}
            catch (Exception e)
            {
                Assert.Fail(string.Format("TEST FAULT: Unexpected exception: {0}", e.ToString()));
            }
            return copiedComponent;
        }
    }
}
