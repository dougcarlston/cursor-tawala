// $Workfile: ExternalField.cs $
// $Revision: 3 $	$Date: 6/10/07 10:28p $
// Copyright © 2007 Tawala Systems, Inc. All rights reserved.using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Tawala.Projects.Expressions;
using Tawala.XmlSupport;

namespace Tawala.Projects.Fields
{
    public class ExternalField : IPaletteField, IOperatorDataSource
    {
        private bool isBlank;
        private bool isMultipleChoiceQuestion;
        private int choiceCount;
        private bool selectOnlyOne;
        private string name;
        private ExternalFieldProvider provider;
        private IList operatorList;

        internal ExternalField(ExternalFieldProvider owningProvider, IXmlElement element)
        {
            provider = owningProvider;
            string type = element.GetAttribute("type");
            isBlank = type.CompareTo("string") == 0;
            isMultipleChoiceQuestion = type.CompareTo("mcq") == 0;

            name = element.GetAttribute("name");

            string choicesAttr = element.GetAttribute("choices");
            choiceCount = string.IsNullOrEmpty(choicesAttr) ? 0 : Convert.ToInt32(choicesAttr);

            string onlyOneAttr = element.GetAttribute("onlyone");
            selectOnlyOne = string.IsNullOrEmpty(onlyOneAttr) ? true : (onlyOneAttr == "true");

            id = Project.NextUniqueID;

            if (isBlank)
            {
                operatorList = HybridOperator.List.DataSource;
            }
            else if (isMultipleChoiceQuestion)
            {
                operatorList = selectOnlyOne  ? MCOneOperator.List.DataSource : MCManyOperator.List.DataSource;
            }
        }

        internal bool IsBlank
        {
            get { return isBlank; }
        }

        internal bool IsMultipleChoice
        {
            get { return isMultipleChoiceQuestion; }
        }

        internal bool IsExtra
        {
            get { return false; }
        }

        internal int ChoiceCount
        {
            get { return choiceCount; }
        }

        internal bool SelectOnlyOne
        {
            get { return selectOnlyOne; }
        }

        #region IPaletteField

        public string QualifiedFieldName
        {
            get { return string.Format("{0}:{1}", provider.Name, name); }
        }

        public string FieldName
        {
            get { return name; }
        }

        public string FieldString
        {
            get { return string.Format("<<{0}>>", name); }
        }

        public IField this[string name]
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

        public int Id
        {
            get { return 0; } 
        }

        private int id;

        #endregion

        #region IEnumerable Interface

        public IEnumerator GetEnumerator()
        {
            yield return this;
        }

        #endregion

        #region IRecursiveEnumerable Interface

        public IEnumerable RecursiveEnumerator
        {
            get
            {
                yield return this;
            }
        }

        #endregion

        #region IOperatorDataSource

        public IList OperatorDataSource
        {
            get
            {
                return operatorList;
            }
        }

        #endregion
    }
}