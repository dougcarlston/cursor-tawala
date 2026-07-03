using System;
using System.Runtime.Serialization;

namespace Tawala.Projects.Expressions
{
    /// <summary>
    /// Implements a string element of an expression
    /// </summary>
    [Serializable]
    public class StringElement : ExpressionElement
    {
        private readonly string elementString;

        public StringElement(string elementString)
        {
            this.elementString = elementString;
        }

        public override string Text { get { return elementString; } }

        public override string ToString()
        {
            return elementString;
        }

        public override string ToXml()
        {
            return Text;
        }

        [OnDeserialized]
        private void onDeserialized(StreamingContext context)
        {
            // REVISIT: This code was put in place specifically to cause an exception to be thrown when opening
            // projects created by Build 29. It may become unnecessary in future builds.
            if (elementString == null)
            {
                throw new SerializationException();
            }
        }
    }
}