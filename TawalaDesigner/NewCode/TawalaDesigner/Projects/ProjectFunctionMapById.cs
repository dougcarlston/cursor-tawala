// $Workfile: ProjectFunctionMapById.cs $
// $Revision: 5 $	$Date: 6/22/07 11:46a $
// Copyright © 2005-2007 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Functions.Runtime;

namespace Tawala.Projects
{
	public class ProjectFunctionMapById 
	{
		public IFunction this[int id]
		{
			get 
			{
				return map.ContainsKey(id) ? map[id] : null;
			}
		}

		public void AddUnique(IFunction function)
		{
			if (!map.ContainsKey(function.InstanceId))
			{
				map.Add(function.InstanceId, function);
			}
		}

		/// <summary>
		/// Returns a dictionary in which each key is a function instance ID and
		/// each value is a function display string.
		/// </summary>
		public Dictionary<int, string> GetDisplayStringDictionary()
		{
			Dictionary<int, string> textDictionary = new Dictionary<int, string>();

			foreach (KeyValuePair<int, IFunction> keyValuePair in map)
			{
				IFunction function = keyValuePair.Value;
				textDictionary.Add(function.InstanceId, function.ToDisplayString());
			}

			return textDictionary;
		}

		public void Clear()
		{
			map.Clear();
		}

		public bool ContainsKey(int id)
		{
			return map.ContainsKey(id);
		}

		public bool Remove(int id)
		{
			return map.Remove(id);
		}

		private FunctionMapById map = new FunctionMapById();

		private class FunctionMapById : Dictionary<int, IFunction>
		{
		}
	}
}
