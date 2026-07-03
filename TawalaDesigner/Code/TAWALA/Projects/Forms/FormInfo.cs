// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;

namespace Tawala.Projects.Forms
{
    public class FormInfo : IForm
    {
        private string name = "";

        public FormInfo(string name)
        {
            this.name = name;
        }

        #region IForm Members

        public string Name { get { return name; } set { name = value; } }

        public override string ToString()
        {
            return name;
        }

        public string ToXml()
        {
            return string.Format("<form name=\"{0}\"/>", name);
        }

        public string UserVisibleComponentTypeName { get { throw new NotImplementedException(); } }

        public FormItemList ItemList { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public FieldList GetAllFields()
        {
            throw new NotImplementedException();
        }

        public FieldList GetMCFields()
        {
            throw new NotImplementedException();
        }

        public IProcess ConnectedPostProcess { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IFormItem GetFormItem(string formItemLabel)
        {
            throw new NotImplementedException();
        }

        public IProcess ConnectedPreProcess { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public IFormItem GetFormItem(int formItemId)
        {
            throw new NotImplementedException();
        }

        public string GetDefaultLabel(IFormItem formItem)
        {
            throw new NotImplementedException();
        }

        public bool IsDataSource { get { throw new NotImplementedException(); } }

        public string DataSourceName { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public bool StartingPoint { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public FieldList GetFormItemFields()
        {
            throw new NotImplementedException();
        }

        public FieldList GetFields()
        {
            throw new NotImplementedException();
        }

        public FieldList GetFormItemFieldsAndRecordVariables()
        {
            throw new NotImplementedException();
        }

        public SkipToDestinationList SkipToDestinations { get { throw new NotImplementedException(); } }

        public bool DataEntryOnly { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public Process GetSkipInstructions(ProcessStatement statement)
        {
            throw new NotImplementedException();
        }

        public SkipInstructionsItem ActiveSkipToItem { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        public void ResolveProcessReferences()
        {
            throw new NotImplementedException();
        }

        public string ToValueXml()
        {
            return ToString();
        }

        public string FormattedString { get { return name; } }

        public bool BlockBackButton { get { throw new NotImplementedException(); } set { throw new NotImplementedException(); } }

        #endregion
    }
}