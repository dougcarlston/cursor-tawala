using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms.Design;

namespace Tawala.Controls.Design
{
    public class ConditionEditControlDesigner : ControlDesigner
    {
        protected override void PostFilterProperties(System.Collections.IDictionary properties)
        {
//            properties.Remove("Margin");
//            properties.Remove("Padding");
            base.PostFilterProperties(properties);
        }
    }
}
