// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects;

namespace Tawala.DesignerUI
{
    internal class ConnectedPreProcessNode : ProcessNode
    {
        public ConnectedPreProcessNode(IForm form, int imageIndex)
            : base(form.ConnectedPreProcess, imageIndex)
        {
            Tag = form;
        }
    }
}