// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.

using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//
//      Minor Version 	2
//
//      Build Number	* = number of days since January 1, 2000 local time
//
//      Revision		number of seconds since midnight local time, divided by two
////

[assembly: AssemblyVersion("0.2.*")]

#if DEBUG
[assembly: AssemblyConfiguration("Debug Build")]
#endif

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//

[assembly: AssemblyCompany("Tawala Systems, Inc.")]
[assembly: AssemblyProduct("Tawala Designer")]
[assembly: AssemblyCopyright("© 2005-2008 Tawala Systems, Inc. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

[assembly: ComVisible(false)]

// If the AssemblyFileVersionAttribute is not supplied, the AssemblyVersionAttribute is used for the Win32 file version.
//[assembly: AssemblyFileVersion("1.0.0.0")]
