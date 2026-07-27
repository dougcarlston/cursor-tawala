// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Forms
{
    public interface IChoiceList : IField
    {
        int Count { get; }
        IChoice this[int index] { get; set; }
        void Add(IChoice item);
        void Clear();

        string GetLabel(int choiceIndex);
        string ToXhtml(IFormItem formItem);
    }
}