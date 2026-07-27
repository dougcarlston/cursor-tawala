// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms
{
    public interface IFileUploadItem : IFormItem, IDefaultLabel, IOperatorDataSource, IPaletteField
    {
        IFormItemContents NewContents { get; set; }
        bool Required { get; set; }
        string Rtf { get; set; }
    }
}