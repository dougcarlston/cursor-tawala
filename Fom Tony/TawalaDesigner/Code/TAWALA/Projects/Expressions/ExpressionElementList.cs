// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace Tawala.Projects.Expressions
{
    /// <summary>
    /// Implments a list of ExpressionElement objects
    /// </summary>
    [Serializable]
    public class ExpressionElementList : Collection<ExpressionElement>
    {
        public override string ToString()
        {
            var sb = new StringBuilder();

            foreach (ExpressionElement e in Items)
            {
                sb.Append(e.ToString());
            }

            return sb.ToString();
        }
    }
}