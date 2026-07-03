// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Components;

namespace Tawala.DesignerUI
{
    internal abstract class ComponentNode : ProjectNode
    {
        protected ComponentNode(IProjectComponent component, int imageIndex)
            : base(component.Name, imageIndex)
        {
            Tag = component;
        }

        public virtual bool Rename(string newName)
        {
            return false;
        }
    }
}