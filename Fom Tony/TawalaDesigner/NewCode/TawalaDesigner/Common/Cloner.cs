// $Workfile: Cloner.cs $
// $Revision: 2 $	$Date: 11/25/05 4:35p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

namespace Tawala.Common
{
	public static class Cloner
	{
		/// <summary>
		/// Generic method for cloning a binary serializable object.
		/// The type must be marked as [Serializable] for this
		/// method to succeed.
		/// </summary>
		//  Example:  Process aClone = Cloner.Clone<Process>(Process cloneMe)
		public static T Clone<T>(T serializable)
		{
			T clone = default(T);

			using (MemoryStream ms = new MemoryStream())
			{
				var formatter = new BinaryFormatter();
				formatter.AssemblyFormat = FormatterAssemblyStyle.Simple;
				formatter.Serialize(ms, serializable);
				ms.Seek(0, SeekOrigin.Begin);
				clone = (T)formatter.Deserialize(ms);
			}

			return clone;
		}
	}
}
