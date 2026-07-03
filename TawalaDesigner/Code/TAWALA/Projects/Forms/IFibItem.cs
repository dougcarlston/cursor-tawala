// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;

namespace Tawala.Projects.Forms
{
    public interface IFibItem : IFormItem, IDefaultLabel
    {
        Collection<IBlank> BlankList { get; }
        void InsertBlanksIntoFieldMapByName();
    }
}