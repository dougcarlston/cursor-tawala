// Copyright © 2008 Tawala Systems, Inc. All rights reserved.


namespace Tawala.Interfaces
{
	public interface IMcqOptionsView
	{
		string QuestionLabel { get; set; }
		bool ResponseRequired { get; set; }
		bool SelectMoreThanOne { get; set; }
		IFormItemsPalette GetFormItemsPalette();
		string LabelStatusText { set; }
	}
}
