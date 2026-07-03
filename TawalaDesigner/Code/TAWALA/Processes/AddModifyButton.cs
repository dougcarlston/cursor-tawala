// $Workfile: AddModifyButton.cs $
// $Revision: 4 $	$Date: 5/21/07 3:46p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace Tawala.Processes
{
	/// <summary>
	/// Summary description for AddModifyButton.
	/// </summary>
	[Designer(typeof(AddModifyButtonDesigner))]
	internal class AddModifyButton : System.Windows.Forms.Button
	{
		private bool bModify;

		public AddModifyButton()
		{
			FlatStyle = System.Windows.Forms.FlatStyle.Standard;
			Image = Properties.Resources.DownArrow;
			ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
			Location = new System.Drawing.Point(0, 0);
			Name = "AddModifyButton";
			Size = new System.Drawing.Size(80, 24);
			TabIndex = 0;
			Text = Properties.Resources.DetailsAdd;
			TextAlign = ContentAlignment.MiddleCenter;
		}

		[ReadOnly(true)] // at design time
		public bool Modify
		{
			get
			{
				return bModify;
			}
			set
			{
				if (bModify != value)
				{
					bModify = value;
					base.Text = bModify ? Properties.Resources.DetailsModify : Properties.Resources.DetailsAdd;
				}
			}
		}
	}

	internal class AddModifyButtonDesigner : ControlDesigner
	{
		private string[] removeProps = { "AllowDrop", "AutoEllipsis", "ContextMenuStrip", "Dock", "FlatAppearance", "ImageList", "Modify", "MaximumSize", "MinimumSize", "Padding" };

		protected override void PreFilterProperties(IDictionary properties)
		{
			base.PreFilterProperties(properties);

			foreach (string name in removeProps)
			{
				if (properties.Contains(name))
				{
					properties.Remove(name);
				}
			}
		}

		protected override void PostFilterProperties(IDictionary properties)
		{
			base.PostFilterProperties(properties);

			ArrayList keys = new ArrayList(properties.Keys);

			foreach (Object key in keys)
			{
				PropertyDescriptor pd = (PropertyDescriptor)properties[key];
				if (pd.Category == "Appearance")
				{
					properties[key] = TypeDescriptor.CreateProperty(typeof(AddModifyButton),
						pd, new Attribute[] { new ReadOnlyAttribute(true) });
				}
			}
		}
	}
}
