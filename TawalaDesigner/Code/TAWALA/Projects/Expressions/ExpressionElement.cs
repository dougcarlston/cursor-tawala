// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Expressions
{
    /// <summary>
    /// Implements an element of an expression
    /// </summary>
    [Serializable]
    public class ExpressionElement
    {
        public ExpressionElement()
        {
        }

        public ExpressionElement(string elementString)
        {
        }

        public virtual string Text { get { return String.Empty; } }

        public virtual string ToXml()
        {
            return "";
        }
    }
}