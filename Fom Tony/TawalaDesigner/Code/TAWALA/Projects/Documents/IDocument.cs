// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Components;
using Tawala.Projects.Fields;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Documents
{
    public interface IDocument : IProjectComponent
    {
        IFormItemContents NewContents { get; set; }

        [Obsolete("Text property should be phased out!!")]
        string Text { get; set; }

        FieldList GetFields();
    }
}