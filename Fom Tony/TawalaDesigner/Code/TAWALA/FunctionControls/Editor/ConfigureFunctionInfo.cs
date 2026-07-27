// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Tawala.Function.Controls.Properties;
using Tawala.Functions.Runtime;

namespace Tawala.FunctionControls.Editor
{
    public partial class ConfigureFunctionInfo : FlowLayoutPanel
    {
        public ConfigureFunctionInfo()
        {
            InitializeComponent();
        }

        public void SetFunction(IFunction function)
        {
            labelFunctionName.Text = function.Info.Name;
            labelFunctionDescription.Text = function.Info.Description;
        }

        public void SetCurrentParameter(IParameterInfo info, Dictionary<string, string> replacements)
        {
            labelParameterName.Text = applyReplacements(info.Name, replacements);
            labelParameterDescription.Text = applyReplacements(info.Description, replacements);
            labelParameterTag.Text = info.MapInfo.TagLine;
            labelParameterRequired.Visible = info.Required;
        }

		public void SetNoParameters()
		{
			labelParameterName.Text = Resources.NoFunctionParametersLabel;
			labelParameterDescription.Text = string.Empty;
			labelParameterTag.Text = Resources.NoFunctionParametersTag;
			labelParameterRequired.Visible = false;
		}

        private static string applyReplacements(string text, Dictionary<string, string> replacements)
        {
            var sb = new StringBuilder(text);

            if (replacements != null)
            {
                foreach (string key in replacements.Keys)
                {
                    sb.Replace(key, replacements[key]);
                }
            }

            return sb.ToString();
        }
    }
}