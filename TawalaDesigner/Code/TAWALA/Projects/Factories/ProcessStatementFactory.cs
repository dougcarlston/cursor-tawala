// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects.Processes;
using Tawala.XmlSupport;

namespace Tawala.Projects.Factories
{
    /// <summary>
    /// Produces Tawala IProcessStatement objects from XML elements.
    /// </summary>
    public static class ProcessStatementFactory
    {
        private static Factory<IProcessStatement> processStatementFactory = new Factory<IProcessStatement>();

        static ProcessStatementFactory()
        {
            processStatementFactory.Register("addTo", typeof(AddStatement));
            processStatementFactory.Register("append", typeof(AppendStatement));
            processStatementFactory.Register("comment", typeof(CommentStatement));
            processStatementFactory.Register("delete", typeof(DeleteStatement));
            processStatementFactory.Register("divideBy", typeof(DivideStatement));
            processStatementFactory.Register("foreach", typeof(ForEachRecordStatement));
            processStatementFactory.Register("get", typeof(GetStatement));
            processStatementFactory.Register("if", typeof(IfStatement));
            processStatementFactory.Register("multiplyBy", typeof(MultiplyStatement));
            processStatementFactory.Register("send", typeof(SendStatement));
            processStatementFactory.Register("set", typeof(SetStatement));
            processStatementFactory.Register("edit", typeof(ShowRecordStatement));
            processStatementFactory.Register("show", "document", typeof(ShowDocumentStatement));
            processStatementFactory.Register("show", "form", typeof(ShowFormStatement));
            processStatementFactory.Register("show", typeof(ShowUrlStatement));
            processStatementFactory.Register("skip", typeof(SkipToStatement));
            processStatementFactory.Register("subtractFrom", typeof(SubtractStatement));
        }

        public static IProcessStatement MakeObject(IXmlElement element, Process process)
        {
            var processStatement = processStatementFactory.MakeObject(element, process);

            if (processStatement == null)
            {
                return ProcessStatement.NULL;
            }

            return processStatement;
        }

        public static ProcessStatementList MakeChildren(IXmlElement element, Process process)
        {
            var children = new ProcessStatementList();

            foreach (IXmlElement childElement in element.GetChildren())
            {
                children.Add(processStatementFactory.MakeObject(childElement, process));
            }

            return children;
        }
    }
}