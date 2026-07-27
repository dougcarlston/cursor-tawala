// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using Tawala.Projects.Fields;
using Tawala.Projects.Processes;

namespace Tawala.Projects
{
    [Serializable]
    public class RecordSet : Variable
    {
        [NonSerialized]
        protected FormList forms;

        private SerializationInfo serializationInfo;

        public RecordSet(string name, FormList forms) : base(name)
        {
            this.forms = forms;
        }

        public RecordSet(string name, FormList forms, bool addToFieldMap) : base(name, addToFieldMap)
        {
            this.forms = forms;
        }

        public FormList Forms { get { return forms; } set { forms = value; } }

        public override string QualifiedFieldName { get { return FieldName; } }

        public static RecordSet ShallowCopy(RecordSet sourceRecordSet)
        {
            var forms = new FormList();

            if (sourceRecordSet != null)
            {
                foreach (IForm form in sourceRecordSet.Forms)
                {
                    forms.Add(form);
                }

                return (new RecordSet(sourceRecordSet.FieldName, forms));
            }

            return null;
        }

        public void MapFormFields(Process process)
        {
            foreach (IForm form in Forms)
            {
                process.MapFormFields(form);
            }
        }

        [OnSerializing]
        private void onSerializing(StreamingContext context)
        {
            serializationInfo = new SerializationInfo(this);
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            serializationInfo.Deserialized(this);
            serializationInfo = null;
        }

        #region Nested type: SerializationInfo

        [Serializable]
        private class SerializationInfo
        {
            private readonly Collection<string> formNames = new Collection<string>();

            public SerializationInfo(RecordSet rs)
            {
                foreach (IForm f in rs.Forms)
                {
                    formNames.Add(f.Name);
                }
            }

            public void Deserialized(RecordSet rs)
            {
                rs.Forms = new FormList();

                foreach (string name in formNames)
                {
                    foreach (IForm f in Project.Current.FormList)
                    {
                        if (f.Name.CompareTo(name) == 0)
                        {
                            rs.Forms.Add(f);
                            break;
                        }
                    }
                }
            }
        }

        #endregion
    }
}