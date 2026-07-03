// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;

namespace Tawala.Projects.Exceptions
{
    public class ProjectMissingDataSourcesException : Exception
    {
        private readonly List<string> missingDataSourceNames = new List<string>();

        public List<string> MissingDataSourceNames { get { return missingDataSourceNames; } }
    }
}