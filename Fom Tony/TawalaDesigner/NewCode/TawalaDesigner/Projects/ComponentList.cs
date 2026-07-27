// $Workfile: ComponentList.cs $
// $Revision: 16 $	$Date: 8/23/07 8:49a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Tawala.Projects
{
    /// <summary>
    /// Summary description for ComponentList.
    /// </summary>
    [Serializable]
    public abstract class ComponentList<T> : Collection<T> where T : IComponent
    {
        private bool enableEvents = false;
		private Dictionary<IComponent, object> map = new Dictionary<IComponent, object>();

		// These must be set by derived class in a static constructor

        protected static string xmlStartTag;
		protected static string xmlEndTag;

        public bool EnableEvents
        {
            get
            {
                return enableEvents;
            }
            set
            {
                enableEvents = value;
            }
        }

		/// <summary>
		/// For cases where we don't want the strictness of the generics specific version of Contain
		/// and just want to see if any component is in a particular list regardless of whether its
		/// a FormList, etc.
		/// </summary>
		public bool ContainsComponent(IComponent comp)
		{
			return comp == null? false : map.ContainsKey(comp);
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

		[NonSerialized]
		private ReadOnlyCollection<T> readOnlyWrapper = null;

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
            StringBuilder xmlString = new StringBuilder();

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
				IComponent existingComponent = component;
				IComponent pastedComponent = paste;

				if (existingComponent == pastedComponent)
				{
					System.Diagnostics.Debug.Assert(false, "setPasteName", "Attempting to paste existing component object into project!");
					return false;
                }
            }

            // if the name already exists
            if (IndexOf(paste.Name) >= 0)
            {
                // "Copy of $NAME"
                StringBuilder pasteName = new StringBuilder(Properties.Resources.PasteName1);
                pasteName.Replace("$NAME", paste.Name);

                int num = 2;

                // as long as we keep finding matches, increment the counter
                while (IndexOf(pasteName.ToString()) >= 0)
                {
                    // "Copy ($NUM) of $NAME"
					pasteName = new StringBuilder(Properties.Resources.PasteName2);
                    pasteName.Replace("$NAME", paste.Name);
                    pasteName.Replace("$NUM", num.ToString());
                    num++;
                }

                paste.Name = pasteName.ToString();
            }

            this.Add(paste);

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
            IComponent item = this[index];
            if (enableEvents)
            {
                ComponentCancelEventArgs queryCancel = new ComponentCancelEventArgs(item);
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
			map = new Dictionary<IComponent, object>();
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
