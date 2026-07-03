// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;
using Tawala.Projects;

namespace Tawala.Functions.Controls
{
    public class LocalFormBinder : ParameterBinder
    {
        private readonly FormBindingList formBindingList;

        protected LocalFormBinder(IParameterInfo info) : base(info)
        {
            formBindingList = new FormBindingList(Project.Current.FormList);
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
            if (c is ComboBoxControl)
            {
                var cb = c as ComboBoxControl;
                cb.DataSource = formBindingList;
                cb.DisplayMember = "Name";
                cb.ValueMember = "Item";
                new EditBinding(c, "SelectedItem", this);
            }
        }
    }
}