// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Forms.NewFormItems;
using Tawala.Projects.Processes;

namespace Tawala.Projects.Forms
{
    public interface ISkipInstructionsItem : IFormItem
    {
        IFormItemContents NewContents { get; set; }
        Process Instructions { get; set; }
        string GetSummary();
    }
}