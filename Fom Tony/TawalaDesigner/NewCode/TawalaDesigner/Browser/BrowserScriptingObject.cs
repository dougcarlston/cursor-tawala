// Copyright © 2008 Tawala Systems, Inc. All rights reserved.
using System;
using System.Runtime.InteropServices;
using Tawala.Projects;

namespace Tawala.Browser
{
    [ComVisible(true)]
    public abstract class BrowserScriptingObject
    {
        public string CreateFieldXhtml(string id)
        {
            int fieldId = Convert.ToInt32(id);
            string text = Project.FieldMapById[fieldId].QualifiedFieldName;
            return string.Format("<t:field id=\"field_{0}\" fieldID=\"{1}\">{2}</t:field>",
                                 DateTime.Now.Ticks.ToString("x"), id, text);
        }
    }
}