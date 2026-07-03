// $Workfile: ProcessList.cs $
// $Revision: 16 $	$Date: 9/22/06 4:21p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections;
using System.Text;
using Tawala.XmlSupport;

namespace Tawala.Projects
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
			foreach (IXmlElement childElement in element.GetChildren("process"))
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
