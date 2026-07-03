// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using Tawala.Projects;
using Tawala.Projects.Documents;

namespace Tawala.DesignerUI
{
    internal class DocumentNode : ComponentNode
    {
        private const int imageDocument = 10;

        public DocumentNode(RtfDocument document)
            : this(document, imageDocument)
        {
        }

        public DocumentNode(RtfDocument document, int imageIndex)
            : base(document, imageIndex)
        {
        }

        public override bool Rename(string newName)
        {
            return !Project.Current.RenameDocument(Text, newName);
        }
    }
}