// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// List of process elements.
    /// </summary>
    [Serializable]
    public sealed class ProcessElementList : Collection<IProcessElement>, IProcessElement, IRecursiveEnumerable
    {
        public ProcessElementList()
        {
        }

        /// <summary>
        /// Constructor. Create process element list from any process statement
        /// </summary>
        public ProcessElementList(ProcessStatement statement)
        {
            Add(statement.AsProcessElement());
        }

        /// <summary>
        /// Indexer - get field by name
        /// </summary>
        public IProcessElement this[Index index]
        {
            get
            {
                int i = 0;

                foreach (IProcessElement element in RecursiveEnumerator)
                {
                    if (i++ == index.value)
                    {
                        return element;
                    }
                }

                // index out of range
                return null;
            }
        }

        #region Nested type: Index

        public struct Index
        {
            public int value;
        }

        #endregion

        #region IProcessElement Interface

        /// <summary>
        /// Boolean indicating whether this list is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                bool isValid = true;

                foreach (IProcessElement element in this)
                {
                    if (!element.IsValid)
                    {
                        isValid = false;
                        break;
                    }
                }

                return isValid;
            }
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator
        {
            get
            {
                foreach (IProcessElement item in this)
                {
                    foreach (IProcessElement element in item.RecursiveEnumerator)
                    {
                        yield return element;
                    }
                }
            }
        }

        #endregion
    }
}