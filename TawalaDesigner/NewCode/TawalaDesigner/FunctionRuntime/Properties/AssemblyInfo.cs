// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Reflection;

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//

[assembly: AssemblyTitle("Tawala Function Runtime")]
[assembly: AssemblyDescription("Runtime support for dynamic functions.")]
[assembly: AssemblyInformationalVersion("1.0")]
// So that function loader can figure out local app data path without Application class

[assembly: CLSCompliant(false)]