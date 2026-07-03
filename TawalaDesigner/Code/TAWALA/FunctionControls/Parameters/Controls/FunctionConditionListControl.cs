// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;
using Tawala.Functions.Runtime;
using Tawala.Projects;
using Tawala.ProjectUI;

namespace Tawala.Functions.Controls
{
    public partial class FunctionConditionListControl : UserControl, IParameterControl
    {
        public FunctionConditionListControl()
        {
            InitializeComponent();
        }

        #region IParameterControl Members

        public bool CustomControl
        {
            get
            {
                return true;
            }
        }

        public void CommitPendingChanges()
        {
            Conditions conditions = conditionListControl.Conditions;
            var conditionsWrapper = ControlManager.Function[ControlManager.LookupParameterInfo(this)] as FunctionFilterConditions;
            conditionsWrapper.Forms = ReferencedFormsHelper.Get(ControlManager.Function);
            conditionsWrapper.Conditions = conditions;
        }

        #endregion

        protected override void OnLoad(EventArgs e)
        {
            IParameterInfo parameterInfo = ControlManager.LookupParameterInfo(this);
            conditionListControl.WhereText = parameterInfo.Name + " where";

            var conditionsWrapper = ControlManager.Function[parameterInfo] as FunctionFilterConditions;
            if (conditionsWrapper.Conditions == null)
            {
                conditionsWrapper.Conditions = new Conditions();
            }

            conditionListControl.Conditions = conditionsWrapper.Conditions;

            base.OnLoad(e);
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            if (!DesignMode)
            {
                FieldsPalette.Palette.ConditionsForms = FunctionFormCollection.NULL;
                ParameterUtils.UpdateFieldsPaletteChoices(null);
            }
            base.OnHandleDestroyed(e);
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            base.OnEnabledChanged(e);
            if (Enabled)
            {
                ParameterUtils.UpdateFieldsPaletteChoices(conditionListControl);
            }
        }
    }
}