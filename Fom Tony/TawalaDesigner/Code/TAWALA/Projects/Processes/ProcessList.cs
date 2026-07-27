// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Components;
using Tawala.XmlSupport;

namespace Tawala.Projects.Processes
{
    /// <summary>
    /// List of processes.
    /// </summary>
    [Serializable]
    public class ProcessList : ComponentList<Process>
    {
        static ProcessList()
        {
            xmlStartTag = "<processes>\r\n";
            xmlEndTag = "</processes>\r\n";
        }

        public ProcessList()
        {
        }

        public ProcessList(IXmlElement element)
        {
            foreach (var childElement in element.GetChildren("process"))
            {
                Add(new Process(childElement));
            }
        }

        /// <summary>
        /// Indexer - get process by statement
        /// </summary>
        public Process this[ProcessStatement statement]
        {
            get
            {
                // for each process in list...
                foreach (Process p in this)
                {
                    // for each line in process...
                    foreach (ProcessLine l in p.Lines)
                    {
                        if (l.Statement == statement)
                        {
                            return p;
                        }
                    }
                }

                // process name not found
                return null;
            }
        }
    }
}