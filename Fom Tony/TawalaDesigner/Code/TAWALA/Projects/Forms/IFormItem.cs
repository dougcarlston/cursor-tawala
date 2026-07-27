// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Forms
{
    public interface IFormItem : IField
    {
        IForm Form { get; set; }
        bool IsTextItem { get; }
        bool IsQuestionItem { get; }
        string AlternateLabel { get; set; }
        bool Selected { get; set; }
        string Style { get; set; }
        Conditions DisplayConditions { get; set; }
        bool HasDisplayConditions { get; }
        void ClearId();
        void Eliminate();
        string ToXml();
        void ResolveFieldReferences();
        void ResolveFunctionReferences();
    }
}