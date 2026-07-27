// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Windows.Forms;

namespace Tawala.DesignerUI
{
    internal abstract class ProjectNode : TreeNode
    {
        protected ProjectNode(string name)
            : base(name)
        {
        }

        protected ProjectNode(string name, int imageIndex)
            : base(name, imageIndex, imageIndex)
        {
        }
    }
}