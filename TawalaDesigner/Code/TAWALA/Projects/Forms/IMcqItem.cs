// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;
using Tawala.Projects.Expressions;
using Tawala.Projects.Forms.FormItemContents;
using Tawala.Projects.Forms.NewFormItems;

namespace Tawala.Projects.Forms
{
    public interface IMcqItem : IFormItem, IOperatorDataSource, IPaletteField, IDefaultLabel
    {
        IFormItemContents NewContents { get; set; }
        bool SelectOnlyOne { get; set; }
        bool RequireAtLeastOne { get; set; }
        int ColumnCount { get; set; }
        bool PaddingBottom { set; get; }
        int ChoiceSourceIndex { get; set; }
        Question Question { get; }
        IFormItemContents QuestionContents { get; }
        IChoiceList Choices { get; }
        IFormItemContents ChoiceContents { set; }
        IFunction DataSourceFunction { get; }
        string ChoicesXhtml { get; }
    }
}