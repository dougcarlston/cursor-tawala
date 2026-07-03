// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Documents;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms
{
    public interface IChoice : IFormItemContents, IPaletteField
    {
        new string Text { get; set; }
        string ContentsXhtml(IFormItem formItem);
        string ToXml(string label);
        string ToRtf(string label, RtfDocument document);
    }
}