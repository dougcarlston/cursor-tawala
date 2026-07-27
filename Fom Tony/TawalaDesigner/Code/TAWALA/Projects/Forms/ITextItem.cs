// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms
{
    public interface ITextItem : IFormItem, IDefaultLabel
    {
        IFormItemContents NewContents { get; set; }
        bool PaddingBottom { set; get; }
    }
}