// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects;

namespace Tawala.DesignerUI
{
    internal class ConnectedProcessNode : ProcessNode
    {
        public ConnectedProcessNode(IForm form, int imageIndex)
            : base(form.ConnectedPostProcess, imageIndex)
        {
            Tag = form;
        }
    }
}