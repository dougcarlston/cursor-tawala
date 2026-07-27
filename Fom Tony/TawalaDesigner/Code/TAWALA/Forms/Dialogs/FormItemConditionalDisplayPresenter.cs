// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Forms;

namespace Tawala.Forms.Dialogs
{
    public class FormItemConditionalDisplayPresenter : IFormItemConditionalDisplayPresenter
    {
        private readonly IFormItem formItem;
        private readonly IFormItemConditionalDisplayView view;

        public FormItemConditionalDisplayPresenter(IFormItemConditionalDisplayView view, IFormItem formItem)
        {
            this.view = view;
            this.formItem = formItem;

            view.SetDisplayConditions(formItem.DisplayConditions);
        }

        #region IFormItemConditionalDisplayPresenter Members

        public void ConditionsDefined()
        {
            formItem.DisplayConditions = view.GetDisplayConditions();
        }

        #endregion
    }
}