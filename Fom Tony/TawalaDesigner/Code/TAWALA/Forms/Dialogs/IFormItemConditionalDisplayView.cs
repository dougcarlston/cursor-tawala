// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects;

namespace Tawala.Forms.Dialogs
{
    public interface IFormItemConditionalDisplayView
    {
        void SetDisplayConditions(Conditions conditions);
        Conditions GetDisplayConditions();
    }
}