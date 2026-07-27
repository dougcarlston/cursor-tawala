// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Tawala.Functions.Runtime.Private
{
    public class ParameterInfoCollection : Collection<IParameterInfo>, IParameterInfoCollection
    {
        private readonly Dictionary<string, IParameterInfo> idToInfo = new Dictionary<string, IParameterInfo>();

        internal ParameterInfoCollection(IFunctionInfo functionInfo, IParameterInfo parameterInfo, Type enclosingType)
        {
            PropertyInfo[] properties = enclosingType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            for (int i = 0; i < properties.Length; ++i)
            {
                PropertyInfo property = properties[i];

                if (property.IsDefined(typeof(ParameterPropertyAttribute), false))
                {
                    object[] attributes = property.GetCustomAttributes(typeof(ParameterPropertyAttribute), false);
                    var ppa = attributes[0] as ParameterPropertyAttribute;
                    var pi = new ParameterInfo(functionInfo, parameterInfo, property, ppa);
                    Add(pi);
                    idToInfo.Add(pi.Id, pi);
                }
            }
        }

        #region IParameterInfoCollection Members

        public IParameterInfo this[string id] { get { return idToInfo[id]; } }

        public bool Contains(string id)
        {
            return idToInfo.ContainsKey(id);
        }

        public ICollection<string> Ids { get { return idToInfo.Keys; } }

        public string ToDisplayString(IFunction function)
        {
            var displayString = new StringBuilder();

            displayString.Append(firstParameterString(function));
            displayString.Append(secondParameterString(function));
            displayString.Append(remainingParameterString(function));

            return displayString.ToString();
        }

        #endregion

        private string firstParameterString(IFunction function)
        {
            return getParameterDisplayString(function, 0);
        }

        private string secondParameterString(IFunction function)
        {
            return (", " + getParameterDisplayString(function, 1));
        }

        private string getParameterDisplayString(IFunction function, int parameterIndex)
        {
            string displayString = "";

            if (Count > parameterIndex && this[parameterIndex].GetValue(function) != null)
            {
                displayString = this[parameterIndex].GetValue(function).ToString();

                // handles string returned by FunctionContentsfield.ToString
                displayString = extractFieldNameFromFieldXml(displayString);
            }

            return displayString;
        }

        /// <summary>
        /// Extracts the name attribute from the specified display string, if the display string consists of
        /// field element XML. Otherwise, returns display string as is.
        /// </summary>
        private static string extractFieldNameFromFieldXml(string displayString)
        {
            Match match = Regex.Match(displayString, "(<field name=\"[^\"]+\" />)");
            if (match.Success)
            {
                string xmlString = match.Groups[1].ToString();
                return Regex.Replace(xmlString, "<field name=\"(.+)\" />", "$1");
            }

            return displayString;
        }

        private string remainingParameterString(IFunction function)
        {
            return (Count > 2 ? ", ..." : "");
        }
    }
}