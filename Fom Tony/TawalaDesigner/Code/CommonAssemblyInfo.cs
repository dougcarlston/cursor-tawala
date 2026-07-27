// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Reflection;
using System.Runtime.InteropServices;

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//
//      Minor Version 	1
//
//      Build Number	* = number of days since January 1, 2000 local time
//
//      Revision		number of seconds since midnight local time, divided by two
////

[assembly: AssemblyVersion("0.1.*")]

#if DEBUG

[assembly: AssemblyConfiguration("Debug Build")]
#endif

//
// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//

[assembly: AssemblyCompany("Tawala Systems, Inc")]
[assembly: AssemblyProduct("Tawala")]
[assembly: AssemblyCopyright("© 2005-2009 Tawala Systems, Inc. All rights reserved.")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]
[assembly: ComVisible(false)]

// If the AssemblyFileVersionAttribute is not supplied, the AssemblyVersionAttribute is used for the Win32 file version.
//[assembly: AssemblyFileVersion("1.0.0.0")]

[assembly: AssemblyInformationalVersion("1.0")] // prevents version info from getting tacked onto Application.LocalUserAppDataPath.