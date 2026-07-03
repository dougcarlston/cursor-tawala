// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Tawala.Interfaces;
using Tawala.MainApplication;
using Tawala.FormDesigner.FormItemOptions;
using Tawala.ComponentDesigner;

namespace Tawala.FormDesigner
{
	public class TawalaFormDesigner : ProjectComponentDesigner, ITawalaFormDesigner
	{
		private IFormView currentFormView;

		public TawalaFormDesigner()
		{
			this.ItemsPalette = new FormItemsPalette();
		}

		#region IFormDesigner Members

		public IFormView CurrentFormView
		{
			get
			{
				return currentFormView;
			}
			set
			{
				currentFormView = value;
				activateFormView();
			}
		}

		#endregion

		private void activateFormView()
		{
			currentFormView.Show();
			currentFormView.Activate();

			setDesignerPalette();
		}

		private static FormItemOptionsDialog formItemOptionsDialog = new FormItemOptionsDialog();

		public static FormItemOptionsDialog FormItemOptionsDialog
		{
			get { return formItemOptionsDialog; }
		}
	}
}
