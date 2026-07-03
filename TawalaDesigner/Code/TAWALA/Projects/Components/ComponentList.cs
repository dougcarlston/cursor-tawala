// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using Tawala.Projects.Properties;

namespace Tawala.Projects.Components
{
    /// <summary>
    /// Summary description for ComponentList.
    /// </summary>
    [Serializable]
    public abstract class ComponentList<T> : Collection<T> where T : IProjectComponent
    {
        protected static string xmlEndTag;
        protected static string xmlStartTag;
        private bool enableEvents;
        private Dictionary<IProjectComponent, object> map = new Dictionary<IProjectComponent, object>();

        [NonSerialized]
        private ReadOnlyCollection<T> readOnlyWrapper;

        // These must be set by derived class in a static constructor

        public bool EnableEvents { get { return enableEvents; } set { enableEvents = value; } }

        /// <summary>
        /// Indexer - get or set form by name
        /// </summary>
        public T this[string name]
        {
            get
            {
                // get list index of form
                int index = IndexOf(name);

                // if form is in list
                if (index != -1)
                {
                    // return form
                    return this[index];
                }

                // form name not found
                return default(T);
            }
        }

        /// <summary>
        /// For cases where we don't want the strictness of the generics specific version of Contain
        /// and just want to see if any component is in a particular list regardless of whether its
        /// a FormList, etc.
        /// </summary>
        public bool ContainsComponent(IProjectComponent comp)
        {
            return comp == null ? false : map.ContainsKey(comp);
        }

        /// <summary>
        /// Index from component name
        /// </summary>
        /// <param name="name">component name</param>
        /// <returns>index of component in list</returns>
        public int IndexOf(string name)
        {
            // for each component in list...
            for (int index = 0; index < Count; index++)
            {
                // if form name found in list
                if (this[index].Name == name)
                {
                    // return list index
                    return index;
                }
            }

            // component name not found
            return -1;
        }

        /// <summary>
        /// Rename a component in the list, not allowing duplicate names
        /// </summary>
        public bool Rename(string oldName, string newName)
        {
            // check for invalid names
            if (!Project.Current.ValidComponentName(newName))
            {
                return false;
            }

            T rename = default(T);

            // we trim leading and trailing whitespace from names
            string trimmedNewName = newName.Trim();

            foreach (T c in this)
            {
                if (c.Name == trimmedNewName)
                {
                    return false;
                }
                if (c.Name == oldName)
                {
                    rename = c;
                }
            }

            if (rename == null)
            {
                return false;
            }

            rename.Name = trimmedNewName;

            if (enableEvents)
            {
                Project.Events.RaiseComponentRenamedEvent(new ComponentRenamedEventArgs(rename, oldName));
            }

            return true;
        }

        public ReadOnlyCollection<T> AsReadOnly()
        {
            if (readOnlyWrapper == null)
            {
                readOnlyWrapper = new ReadOnlyCollection<T>(this);
            }
            return readOnlyWrapper;
        }

        public string ToXml()
        {
            // start with empty xml string
            var xmlString = new StringBuilder();

            // if any documents in list...
            if (Count > 0)
            {
                // append component's start tag
                xmlString.Append(xmlStartTag);

                // for each component in list...
                for (int i = 0; i < Count; i++)
                {
                    // append document component xml
                    xmlString.Append(this[i].ToXml());
                }

                // append components end tag
                xmlString.Append(xmlEndTag);
            }

            return xmlString.ToString();
        }

        /// <summary>
        /// Paste into the list generating a unique name as necesssary.
        /// </summary>
        public bool Paste(T paste)
        {
            foreach (T component in this)
            {
                IProjectComponent existingComponent = component;
                IProjectComponent pastedComponent = paste;

                if (existingComponent == pastedComponent)
                {
                    Debug.Assert(false, "setPasteName", "Attempting to paste existing component object into project!");
                    return false;
                }
            }

            // if the name already exists
            if (IndexOf(paste.Name) >= 0)
            {
                // "Copy of $NAME"
                var pasteName = new StringBuilder(Resources.PasteName1);
                pasteName.Replace("$NAME", paste.Name);

                int num = 2;

                // as long as we keep finding matches, increment the counter
                while (IndexOf(pasteName.ToString()) >= 0)
                {
                    // "Copy ($NUM) of $NAME"
                    pasteName = new StringBuilder(Resources.PasteName2);
                    pasteName.Replace("$NAME", paste.Name);
                    pasteName.Replace("$NUM", num.ToString());
                    num++;
                }

                paste.Name = pasteName.ToString();
            }

            Add(paste);

            return true;
        }

        protected override void InsertItem(int index, T item)
        {
            map.Add(item, null);
            base.InsertItem(index, item);
            if (enableEvents)
            {
                Project.Events.RaiseComponentAddedEvent(new ComponentEventArgs(item));
            }
        }

        protected override void RemoveItem(int index)
        {
            IProjectComponent item = this[index];
            if (enableEvents)
            {
                var queryCancel = new ComponentCancelEventArgs(item);
                Project.Events.RaiseComponentRemovingEvent(queryCancel);
                if (queryCancel.Canceled)
                {
                    return;
                }
            }

            map.Remove(item);
            base.RemoveItem(index);

            if (enableEvents)
            {
                Project.Events.RaiseComponentRemovedEvent(new ComponentEventArgs(item));
            }
        }

        protected override void ClearItems()
        {
            map = new Dictionary<IProjectComponent, object>();
            base.ClearItems();
        }

        protected override void SetItem(int index, T item)
        {
            map.Remove(this[index]);
            map.Add(item, null);
            base.SetItem(index, item);
        }

        public bool ContainsComponentNamed(string name)
        {
            return IndexOf(name) != -1;
        }
    }
}