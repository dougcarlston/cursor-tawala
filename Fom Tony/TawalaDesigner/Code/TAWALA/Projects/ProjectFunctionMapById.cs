// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using Tawala.Functions.Runtime;

namespace Tawala.Projects
{
    public class ProjectFunctionMapById
    {
        private readonly FunctionMapById map = new FunctionMapById();

        public IFunction this[int id] { get { return map.ContainsKey(id) ? map[id] : null; } }

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
            var textDictionary = new Dictionary<int, string>();

            foreach (var keyValuePair in map)
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

        #region Nested type: FunctionMapById

        private class FunctionMapById : Dictionary<int, IFunction>
        {
        }

        #endregion
    }
}