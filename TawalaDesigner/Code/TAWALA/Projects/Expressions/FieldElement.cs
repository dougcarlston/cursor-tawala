using System;
using System.Runtime.Serialization;
using Tawala.Projects.Fields;

namespace Tawala.Projects.Expressions
{
    /// <summary>
    /// Implements a field element of an expression
    /// </summary>
    [Serializable]
    public class FieldElement : ExpressionElement
    {
        protected const string xmlFieldNameTag = "<field name=\"{0}\"/>";
        private IDeserializedField field;

        public FieldElement(IField field)
        {
            this.field = (IDeserializedField)field;
        }

        public IDeserializedField Field
        {
            get
            {
                if (field is UnresolvedField)
                {
                    field = FieldUtil.ResolveFormField(field) as IDeserializedField;
                }

                return field;
            }
            set { field = value; }
        }

        public override string Text { get { return Field.QualifiedFieldName; } }

        public override string ToString()
        {
            return Field.FieldString;
        }

        public override string ToXml()
        {
            return String.Format(xmlFieldNameTag, Text);
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            field = Field.DeserializedFieldReference;
        }
    }
}