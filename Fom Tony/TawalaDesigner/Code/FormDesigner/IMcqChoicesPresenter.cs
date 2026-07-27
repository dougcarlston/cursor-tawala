using System;
using System.Collections.Generic;
using System.Text;
using Tawala.Proj;

namespace Tawala.FormDesigner
{
	public interface IMcqChoicesPresenter
	{
		void ChoicesAccepted();
		void FieldsPaletteDoubleClicked(IField field);
		void FieldDropped(IField field);
		void ConfigurationRequested();
		void ChoiceSourceChanged(int choiceSourceIndex);
	}
}
