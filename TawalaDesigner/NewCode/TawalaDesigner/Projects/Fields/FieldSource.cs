// $Workfile: FieldSource.cs $
// $Revision: 15 $	$Date: 7/02/07 8:21a $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace Tawala.Projects
{
	public delegate IField FieldListGenerator();
	public delegate void FieldListPostProcessor();

	/// <summary>
	/// Implements a Field List datasource that automatically updates any list control it is bound to.
	/// </summary>
	[Serializable]
	public class FieldSource : BindingList<IField>, INotifyPropertyChanged
	{
		private FieldListGenerator generateFieldList;
		private FieldListPostProcessor postProcess;
		private string fieldSourceName = "Unnamed Fieldsource";

		public FieldSource()
		{
			this.generateFieldList = defaultFieldListGenerator;
		}

		private IField defaultFieldListGenerator()
		{
			return new FieldList();
		}

		public FieldSource(FieldListGenerator fieldListGenerator)
		{
			this.generateFieldList = fieldListGenerator;

			Project.Events.FormChanged += Events_FormChanged;
			Project.Events.FormItemChanged += Events_FormItemChanged;
			Project.Events.VariableChanged += Events_VariableChanged;
			Project.Events.ProcessChanged += Events_ProcessChanged;

//			Project.Events.CurrentComponentSet += new CurrentComponentSetEventHandler(Events_CurrentComponentSet);
			Project.Events.ProcessConnectedToForm += Events_ProcessConnectedToForm;
			Project.Events.ProcessDisconnectedFromForm += Events_ProcessDisconnectedFromForm;

			this.fieldSourceName = fieldListGenerator.Method.DeclaringType.FullName + "." + fieldListGenerator.Method.Name;
		}

		public FieldSource(FieldListGenerator fieldListGenerator, string fieldSourceName) : this(fieldListGenerator)
		{
			this.fieldSourceName = fieldSourceName;
		}

		public FieldSource(FieldListGenerator fieldListGenerator, FieldListPostProcessor postProcessor)
			: this(fieldListGenerator)
		{
			this.postProcess = postProcessor;
		}

		void Events_FormChanged(object sender, ComponentEventArgs e)
		{
//			Console.WriteLine("FieldSource.Events_FormChanged:");
			notify("FormChanged");
		}

		void Events_FormItemChanged(object sender, FormItemEventArgs e)
		{
//			Console.WriteLine("FieldSource.Events_FormItemChanged:");
			notify("FormItemChanged");
		}

		void Events_VariableChanged(object sender, VariableEventArgs e)
		{
//			Console.WriteLine("FieldSource.Events_VariableChanged:");
			notify("VariableChanged");
		}

		void Events_ProcessChanged(object sender, ComponentEventArgs e)
		{
			//StackTrace st = new StackTrace(true);
			//Console.WriteLine("Events_ProcessChanged: st.ToString() = {0}", st.ToString());
//			Console.WriteLine("FieldSource.Events_ProcessChanged:");
			notify("ProcessChanged");
		}

		void Events_CurrentComponentSet(object sender, ComponentEventArgs e)
		{
//			Console.WriteLine("FieldSource.Events_CurrentComponentSet:");
			notify("CurrentComponentSet");
		}

		void Events_ProcessConnectedToForm(object sender, ProcessConnectionArgs e)
		{
//			Console.WriteLine("FieldSource.Events_ProcessConnectedToForm:");
			notify("ProcessConnectedToForm");
		}

		void Events_ProcessDisconnectedFromForm(object sender, ProcessConnectionArgs e)
		{
//			Console.WriteLine("FieldSource.Events_ProcessDisconnectedFromForm:");
			notify("ProcessDisconnectedFromForm");
		}

		public void Update()
		{
//			Console.WriteLine("FieldSource[{0}].Update:", fieldSourceName);

			IField fieldList = generateFieldList();

			if (fieldList != null)
			{
				disableListChangedEvents();

				Clear();

				foreach (IField field in fieldList.RecursiveEnumerator)
				{
					Add(field);
				}

				enableListChangedEvents();
			}

			if (postProcess != null)
			{
				postProcess();
			}
		}

		private void disableListChangedEvents()
		{
			this.RaiseListChangedEvents = false;
		}

		private void enableListChangedEvents()
		{
			this.RaiseListChangedEvents = true;
			this.OnListChanged(new ListChangedEventArgs(ListChangedType.Reset, 0));
		}

		private void notify(string info)
		{
//			Console.WriteLine("FieldSource[{0}].notify: {1}", fieldSourceName, info);

			Update();

			if (PropertyChanged != null)
			{
				PropertyChanged(this, new PropertyChangedEventArgs(info));
			}
		}

		#region INotifyPropertyChanged Interface

		public event PropertyChangedEventHandler PropertyChanged;

		#endregion
	}
}
