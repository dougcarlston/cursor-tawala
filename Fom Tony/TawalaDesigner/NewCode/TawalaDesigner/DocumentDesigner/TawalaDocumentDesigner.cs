// Copyright © 2008 Tawala Systems, Inc. All rights reserved.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using Tawala.Interfaces;
using Tawala.MainApplication;
using Tawala.ComponentDesigner;

namespace Tawala.DocumentDesigner
{
	public class TawalaDocumentDesigner : ProjectComponentDesigner, ITawalaDocumentDesigner
	{
		private IDocumentView currentDocumentView;

		public TawalaDocumentDesigner()
		{
			this.ItemsPalette = null;
		}

		#region ITawalaDocumentDesigner Members

		public IDocumentView CurrentDocumentView
		{
			get
			{
				return currentDocumentView;
			}
			set
			{
				currentDocumentView = value;
				activateDocumentView();
			}
		}


		#endregion

		private void activateDocumentView()
		{
			currentDocumentView.Show();
			currentDocumentView.Activate();

			setDesignerPalette();
		}
	}
}
