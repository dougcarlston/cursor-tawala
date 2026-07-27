// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tawala.Projects
{
    // delegation rather than inheritance helps keep surface area of class low
    public sealed class ProjectFieldMapById
    {
        private readonly FieldIdMap map = new FieldIdMap();

        public int Count { get { return map.Count; } }

        public IAnyField this[int id] { get { return map[id]; } }

        public Dictionary<int, IAnyField>.KeyCollection Keys { get { return map.Keys; } }

        public Dictionary<int, IAnyField>.ValueCollection Values { get { return map.Values; } }

        public void AddUnique(IAnyField field)
        {
            map[field.Id] = field;
        }

        public IField FindField(string searchName)
        {
            return map.FindField(searchName);
        }

        public void Remove(int id)
        {
            map.Remove(id);
        }

        public bool ContainsKey(int id)
        {
            return map.ContainsKey(id);
        }

        public void Clear()
        {
            map.Clear();
        }

        public Dictionary<int, string> GetQualifiedFieldDictionary()
        {
            int capacity = Math.Max(16, map.Count/2);

            var qualifiedMap = new Dictionary<int, string>(capacity);

            foreach (int id in map.Keys)
            {
                if (map[id] is IPaletteField)
                {
                    qualifiedMap.Add(id, (map[id]).QualifiedFieldName);
                }
            }

            return qualifiedMap;
        }

        public Dictionary<int, string> GetQualifiedFieldDictionary(Collection<int> idsOfInterest)
        {
            var qualifiedMap = new Dictionary<int, string>(idsOfInterest.Count);

            foreach (int id in idsOfInterest)
            {
                if (map[id] is IPaletteField && !qualifiedMap.ContainsKey(id))
                {
                    qualifiedMap.Add(id, (map[id]).QualifiedFieldName);
                }
            }

            return qualifiedMap;
        }

        #region Nested type: FieldIdMap

        private class FieldIdMap : Dictionary<int, IAnyField>
        {
            public IField FindField(string searchName)
            {
                var fields = new Collection<IAnyField>();

                foreach (IAnyField field in Values)
                {
                    fields.Add(field);
                }

                foreach (IField field in fields)
                {
                    if (field is IPaletteField)
                    {
                        if ((field).QualifiedFieldName.Equals(searchName))
                        {
                            return field;
                        }
                    }

                    if (field.FieldName.Equals(searchName))
                    {
                        return field;
                    }
                }

                return null;
            }
        }

        #endregion
    }
}