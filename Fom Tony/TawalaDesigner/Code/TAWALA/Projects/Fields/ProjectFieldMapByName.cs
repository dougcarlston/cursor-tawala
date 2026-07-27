// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using Tawala.Projects.Forms;

namespace Tawala.Projects
{
    // delegation rather than inheritance helps keep surface area of class low
    public class ProjectFieldMapByName
    {
        private readonly FieldMapByName map = new FieldMapByName();

        public IField this[string name] { get { return map.ContainsKey(name) ? map[name] : null; } }

        public int Count { get { return map.Count; } }

        public void AddUnique(IPaletteField field)
        {
            if (!map.ContainsKey(field.QualifiedFieldName))
            {
                map.Add(field.QualifiedFieldName, field);
            }
        }

        public void AddUnique(FormItem formItem)
        {
            add(formItem, formItem.QualifiedFieldName);
        }

        public void AddUnique(McqItem mcItem)
        {
            add(mcItem, mcItem.QualifiedFieldName);
        }

        private void add(IPaletteField field, string name)
        {
            if (!string.IsNullOrEmpty(name))
            {
                if (map.ContainsKey(name))
                {
                    if (map[name] != field)
                    {
                        map.Remove(name);
                        map.Add(name, field);
                    }
                }
                else
                {
                    map.Add(name, field);
                }
            }
        }

        public void AddUnique(Blank blank)
        {
            add(blank, blank.QualifiedFieldName);
        }

        public void AddUnique(IBlank blank)
        {
            add(blank, blank.QualifiedFieldName);
        }

        public void Clear()
        {
            map.Clear();
        }

        public bool ContainsKey(string name)
        {
            return map.ContainsKey(name);
        }

        public void Remove(IPaletteField field)
        {
            map.Remove(field.FieldName);
            map.Remove(field.QualifiedFieldName);
        }

        #region Nested type: FieldMapByName

        private class FieldMapByName : Dictionary<string, IField>
        {
        }

        #endregion
    }
}