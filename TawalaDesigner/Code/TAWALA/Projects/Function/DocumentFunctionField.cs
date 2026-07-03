// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Functions.Runtime;
using Tawala.Projects.Documents;
using Tawala.RtfSupport;
using Tawala.XmlSupport;

namespace Tawala.Projects.Function
{
    [Serializable]
    public abstract class DocumentFunctionField : ParagraphComponent, IDocumentConversions
    {
        private const string encodedFieldPrefix = "460046002400"; // "FF$"
        protected int functionInstanceId = -1;

        public int FunctionInstanceId { get { return functionInstanceId; } }

        #region IDocumentConversions Members

        public override string ToXml()
        {
            if (!Project.FunctionMapById.ContainsKey(functionInstanceId))
            {
                return string.Empty;
            }
            return Project.FunctionMapById[functionInstanceId].ToXml();
        }

        public override string ToHtml()
        {
            return "";
        }

        public override string ToRtf()
        {
            string displayText = "<<Unknown Function>>";

            IFunction function = Project.FunctionMapById[functionInstanceId];
            if (function != null)
            {
                displayText = function.ToDisplayString();
            }

            string rtfString = @"{{\*\txfieldstart\txfieldtype0\txfieldflags" + RtfUtility.NonEditableFieldFlags + @"\txfielddataval{0}" +
                               @"\txfielddata " + encodedFieldPrefix + "{1}}}" + @"{2}{{" + @"\*\txfieldend}}";

            return String.Format(rtfString, functionInstanceId, RtfUtility.EncodeHexString(functionInstanceId.ToString()),
                                 RtfUtility.EscapeSpecialCharacters(displayText));
        }

        public override string ToRtf(RtfDocument document)
        {
            return ToRtf();
        }

        #endregion

        // REVISIT: JDF TO DO
        //[OnDeserialized]
        //private void onDeserialized(StreamingContext context)
        //{
        //    // probably need to clone the function here and add it to the map
        //}
    }

    [Serializable]
    public class DocumentIdedFunctionField : DocumentFunctionField
    {
        public DocumentIdedFunctionField(IXmlElement element)
        {
            functionInstanceId = Convert.ToInt32(element.GetAttribute("instanceId"));
        }
    }

    [Serializable]
    public class DocumentPersistedFunctionField : DocumentFunctionField
    {
        public DocumentPersistedFunctionField(IXmlElement element)
        {
            var convert = new XmlToFunctionConverter();
            IFunction function = convert.ConvertFrom(element);
            Project.FunctionMapById.AddUnique(function);

            functionInstanceId = function.InstanceId;
        }
    }
}