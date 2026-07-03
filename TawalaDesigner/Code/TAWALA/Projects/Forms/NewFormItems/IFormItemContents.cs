// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Forms.FormItemContents;

namespace Tawala.Projects.Forms.NewFormItems
{
    public interface IFormItemContents
    {
        IFormItemContents Contents { get; set; }
        string Text { get; }
        string ToXml();
        string ToXhtml(IFormItem formItem);
        FormItemContentsCollection GetDescendants(Type descendantType);
        void ApplyFontStyle(FontStyle style);
        FontStyle GetInnermostFontStyle();
        void ResolveFieldReferences();
        void ResolveFunctionReferences();
    }
}