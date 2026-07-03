// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;

namespace Tawala.Projects.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class AssignableField : Field, IAssignableField
    {
        public AssignableField()
        {
        }

        public AssignableField(string name) : base(name)
        {
        }

        public AssignableField(int id) : base(id)
        {
        }

        #region IAssignableField Members

        public string AssignmentName { get { return QualifiedFieldName; } }

        #endregion
    }
}