// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

namespace Tawala.Functions.Controls
{
    public class DebugInitEventsBehavior : Behavior
    {
        public DebugInitEventsBehavior(Control c)
            : base(c)
        {
#if false
			c.HandleCreated += c_HandleCreated;
			c.ParentChanged += new EventHandler(c_ParentChanged);
			c.DataBindings.CollectionChanged += new System.ComponentModel.CollectionChangeEventHandler(dataBindings_CollectionChanged);

			if (c is Form)
				((Form)c).Load += new EventHandler(Form_Load);

			if (c is UserControl)
				((UserControl)c).Load += new EventHandler(UserControl_Load);
#endif
        }

        private void dataBindings_CollectionChanged(object sender, CollectionChangeEventArgs e)
        {
            message("ControlBindingsCollection.CollectionChanged");
        }

        private void UserControl_Load(object sender, EventArgs e)
        {
            message("Load");
        }

        private void Form_Load(object sender, EventArgs e)
        {
            message("Load");
        }

        private void c_ParentChanged(object sender, EventArgs e)
        {
            Control p = AssociatedControl.Parent;
            message("ParentChanged [Parent = {0}]", p != null ? p.GetType().FullName : "(null)");
        }

        private void c_HandleCreated(object sender, EventArgs e)
        {
            message("HandleCreated");
        }

        public override void Detach(Control c)
        {
            c.DataBindings.CollectionChanged -= dataBindings_CollectionChanged;
        }

        private void message(string format, params object[] o)
        {
            string msg = string.Format("{0} - {1}", AssociatedControl.GetType().FullName, string.Format(format, o));
            Debug.WriteLine(msg);
        }
    }
}