// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// Implements single line of Process for display in Process window.
    /// </summary>
    [Serializable]
    public abstract class ProcessLine : IProcessElement
    {
        /// <summary>
        /// Process line text.
        /// </summary>
        protected string text;

        protected ProcessLine()
        {
        }

        protected ProcessLine(ProcessStatement statement, string text)
        {
            Statement = statement;
            this.text = text;
        }

        public ProcessStatement Statement { get; set; }

        public ProcessStatement Group { get; set; }

        public bool SelectsGroup { get; set; }

        public bool IsDeletable { get; set; }

        public bool IsSelectable { get; set; }

        public bool CanInsertBefore { get; set; }

        public int IndentLevel { get; set; }

        /// <summary>
        /// Make a shallow copy of this line, including a shallow copy of the line's statement, if present
        /// </summary>
        /// <returns></returns>
        public ProcessLine Copy()
        {
            var copiedLine = (ProcessLine)MemberwiseClone();

            if (Statement != null)
            {
                copiedLine.Statement = Statement.Copy();
            }

            return copiedLine;
        }

        public override string ToString()
        {
            if (Statement != null)
            {
                return IsValid ? Statement.ToString() : Statement.ToString();
            }
            return text;
        }

        public virtual string ToXml()
        {
            return Statement.ToXml();
        }

        #region IProcessElement Interface

        private bool isValid = true;
        private string lastValidLineString = "";

        /// <summary>
        /// Boolean indicating whether this line is valid.
        /// </summary>
        public bool IsValid
        {
            get
            {
                return isValid;
#if false
				bool isValid = false;

				Process parentProcess = Project.Current.GetProcessOrSkipInstructions(statement);

				if (parentProcess != null)
				{
					int lineIndex = parentProcess.Lines.GetIndex(statement);

					FieldList fieldResolver = getValidNativeFields(parentProcess, lineIndex);

					isValid = IsValidLine(fieldResolver);
				}

				if (isValid)
				{
					preserveLineString();
				}

				return isValid;
#endif
            }
        }

        /// <summary>
        /// Boolean indicating whether this line is valid.
        /// </summary>
        public virtual bool IsValidLine(FieldList fieldResolver)
        {
            // override this in derived classes
            return false;
        }

        public void Validate()
        {
            isValid = false;

            Process parentProcess = Project.Current.GetProcessOrSkipInstructions(Statement);

            if (parentProcess != null)
            {
                int lineIndex = parentProcess.Lines.GetIndex(Statement);

                FieldList fieldResolver = getValidNativeFields(parentProcess, lineIndex);

                isValid = IsValidLine(fieldResolver);
            }

            if (isValid)
            {
                preserveLineString();
            }
        }

        private void preserveLineString()
        {
            lastValidLineString = Statement.ToString();
        }

        /// <summary>
        /// Returns a list of valid fields from the Form to which the Process containing this line is attached.
        /// </summary>
        private static FieldList getValidNativeFields(Process parentProcess, int lineIndex)
        {
            return parentProcess.GetValidFields(lineIndex);
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator
        {
            get { yield return this; }
        }

        #endregion
    }
}