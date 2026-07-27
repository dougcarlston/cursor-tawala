// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Text.RegularExpressions;
using Tawala.Projects.Expressions;

namespace Tawala.Projects.Fields
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class Field : IComparable, IPaletteField, IOperatorDataSource
    {
        private readonly int id;

        /// <summary>
        /// User assigned name or the default name.
        /// Used for drop-down lists, etc.
        /// </summary>
        protected string name;

        public Field()
        {
            id = Project.NextUniqueID;
        }

        public Field(string name)
        {
            id = Project.NextUniqueID;
            this.name = name;
        }

        public Field(int id)
        {
            this.id = id;
        }

        #region IComparable Members

        /// <summary>
        /// Compare a field to this one.
        /// </summary>
        public virtual int CompareTo(object obj)
        {
            if (obj is Field)
            {
                // return comparison of display names
                return FieldName.CompareTo(((Field)obj).FieldName);
            }

            // say that this field is less than other one
            return -1;
        }

        #endregion

        #region IPaletteField Members

        public virtual string FieldName { get { return name; } }

        public virtual string QualifiedFieldName { get { return FieldName; } }

        /// <summary>
        /// Placed in documents, expressions, etc
        /// </summary>
        public virtual string FieldString { get { return "<<" + name + ">>"; } }

        public virtual IField this[string name]
        {
            get
            {
                if (name == FieldName)
                {
                    return this;
                }

                return null;
            }
        }

        public int Id { get { return id; } }

        #endregion

        public override string ToString()
        {
            return name;
        }

        /// <summary>
        /// Compares this field's name with another
        /// </summary>
        protected int CompareNameTo(string compName)
        {
            string numericPattern = @"^([a-zA-Z])(\d+)(.*)";

            bool thisIsNumeric = Regex.IsMatch(FieldName, numericPattern);
            bool compIsNumeric = Regex.IsMatch(compName, numericPattern);

            // if both fields have a numeric component...
            if (thisIsNumeric && compIsNumeric)
            {
                Match thisMatch = Regex.Match(FieldName, numericPattern);
                Match compMatch = Regex.Match(compName, numericPattern);

                string thisPrefix = thisMatch.Groups[1].Value;
                string compPrefix = compMatch.Groups[1].Value;

                // if fields have identical prefixes...
                if (thisPrefix == compPrefix)
                {
                    int thisNumber = Convert.ToInt32(thisMatch.Groups[2].Value);
                    int compNumber = Convert.ToInt32(compMatch.Groups[2].Value);

                    // if numeric parts differ...
                    if (thisNumber != compNumber)
                    {
                        // return comparison of numeric components
                        return thisNumber.CompareTo(compNumber);
                    }
                }
            }

            // otherwise return a simple comparison
            return FieldName.CompareTo(compName);
        }

        #region IEnumerable Interface

        public IEnumerator GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public virtual IEnumerable RecursiveEnumerator { get { yield return this; } }

        #endregion

        #region IOperatorDataSource

        public virtual IList OperatorDataSource { get { return new ArrayList(); } }

        #endregion
    }
}