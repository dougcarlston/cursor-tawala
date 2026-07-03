// Copyright © 2005-2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Runtime.Serialization;

namespace Tawala.Projects
{
	[Serializable]
	public abstract class Component : IComponent
	{
		private string name;

		protected Component()
		{
		}

		protected Component(string name)
		{
			this.name = name.Trim();
		}

		/// <summary>
		/// Component name property.
		/// </summary>
		public virtual string Name
		{
			get
			{
				return name;
			}
			set
			{
				// we don't want leading or trailing whitespace
				name = value.Trim();
			}
		}

        public abstract string UserVisibleComponentTypeName
        {
            get;
        }

		public abstract string ToXml();

		/// <summary>
		/// When a ComponentList is used as a data source overriding this method cause the data source to return Component names,
		/// otherwise, the default implementation (from object) would return the fully qualified name of the Component class for each 
		/// Component in the ComponentList.
		/// </summary>
		public override string ToString()
		{
			return Name;
		}

        [OnSerializing]
        private void onSerializing(StreamingContext context)
        {
            Project.Events.RaiseComponentSerializingEvent(new ComponentEventArgs(this));
        }

	}
}
