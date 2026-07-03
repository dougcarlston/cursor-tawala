// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using Tawala.Projects.Expressions;
using Tawala.Projects.Fields;

namespace Tawala.Projects
{
    [Serializable]
    public class MCItemProxy : Field, IOperatorDataSource
    {
        public MCItemProxy() : base("(selection)")
        {
        }

        #region IOperatorDataSource Members

        public override IList OperatorDataSource { get { return MCOneOperator.List.DataSource; } }

        #endregion
    }
}