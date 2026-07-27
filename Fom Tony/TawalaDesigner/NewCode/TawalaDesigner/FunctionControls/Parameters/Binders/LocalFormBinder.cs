// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.ComponentModel;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.Projects.Forms;

namespace Tawala.Functions.Controls
{
    public class LocalFormBinder : ParameterBinder
    {
        private readonly FormBindingList formBindingList;

        protected LocalFormBinder(IFunction instance, IParameterInfo info)
            : base(instance, info)
        {
            formBindingList = new FormBindingList();
        }

        protected override object RangeCheck(object o)
        {
            if (!(o is IForm))
            {
                o = Project.Current.AllForms.Count >= 0 ? Project.Current.AllForms[0] : NullObjects.Form;
            }
            return o;
        }

        protected override void Bind(IParameterControl c)
        {
            if (c is ComboBox)
            {
                var cb = c as ComboBox;
                cb.DataSource = formBindingList;
                cb.DisplayMember = "Name";
                cb.ValueMember = "Item";
                new EditBinding(c, "SelectedItem", this);
            }
        }

        // The binding engine will automatically hook to one of several standard change notification.
        // Unfortunately the FormList doesn't implement any of these so we tell the binding list when 
        // it has changed manually.  We like doing everything manually.

        #region Nested type: FormBindingList

        private class FormBindingList : BindingList<IForm>
        {
            public FormBindingList() : base(Project.Current.FormList)
            {
                Project.Events.ComponentAdded +=
                    delegate(object o, ComponentEventArgs args)
                    {
                        if (args.Component is IForm)
                        {
                            ResetBindings();
                        }
                    };
                Project.Events.ComponentRemoved += delegate(object o, ComponentEventArgs args)
                                                   {
                                                       if (args.Component is IForm)
                                                       {
                                                           ResetBindings();
                                                       }
                                                   };
                Project.Events.ComponentRenamed += delegate(object o, ComponentRenamedEventArgs args)
                                                   {
                                                       if (args.Component is IForm)
                                                       {
                                                           ResetBindings();
                                                       }
                                                   };
            }
        }

        #endregion
    }
}