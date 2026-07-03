// $Workfile: TextSelection.cs $
// $Revision: 16 $	$Date: 10/26/07 7:14p $
// Copyright © 2005-2006 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Tawala.FontSupport;

namespace Tawala.TextEditor
{
	public class TextSelection : ITextSelection
	{
		#region ITextSelection

		public Tristate Bold
		{
			get
			{
                if (txSel != null)
                {
                    if (!txSel.IsCommonValueSelected(TXTextControl.Selection.Attribute.Bold))
                    {
                        return Tristate.Undefined;
                    }
                    else
                    {
                        return txSel.Bold ? Tristate.True : Tristate.False;
                    }
                }

                return Tristate.Undefined;
			}
			set
			{
				if (value != Tristate.Undefined)
				{
					txSel.Bold = value == Tristate.True ? true : false;
				}
			}
		}

		public Tristate Italic
		{
			get
			{
                if (txSel != null)
                {
                    if (!txSel.IsCommonValueSelected(TXTextControl.Selection.Attribute.Italic))
                    {
                        return Tristate.Undefined;
                    }
                    else
                    {
                        return txSel.Italic ? Tristate.True : Tristate.False;
                    }
                }

                return Tristate.Undefined;
            }
			set
			{
				if (value != Tristate.Undefined)
				{
					txSel.Italic = value == Tristate.True ? true : false;
				}
			}
		}

		public Tristate Underline
		{
			get
			{
                if (txSel != null)
                {
                    if (!txSel.IsCommonValueSelected(TXTextControl.Selection.Attribute.Underline))
                    {
                        return Tristate.Undefined;
                    }
                    else
                    {
                        return txSel.Underline == TXTextControl.FontUnderlineStyle.Single ? Tristate.True : Tristate.False;
                    }
                }

                return Tristate.Undefined;
            }
			set
			{
				if (value != Tristate.Undefined)
				{
					txSel.Underline = value == Tristate.True ? TXTextControl.FontUnderlineStyle.Single : TXTextControl.FontUnderlineStyle.None;
				}
			}
		}

		public bool GetFontColor(out Color c)
		{
            if (txSel != null)
            {
                if (txSel.IsCommonValueSelected(TXTextControl.Selection.Attribute.ForeColor))
                {
                    c = txSel.ForeColor;
                    return true;
                }
                else
                {
                    c = Color.Black;
                    return false;
                }
            }

            c = Color.Black;
            return false;
        }

		public void SetFontColor(Color c)
		{
			txSel.ForeColor = c;
		}

		public string FontName
		{
			get
			{
                if (txSel != null)
                {
                    if (txSel.IsCommonValueSelected(TXTextControl.Selection.Attribute.FontName))
                    {
                        return txSel.FontName;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }

                return string.Empty;
            }
			set
			{
				txSel.FontName = value;
			}
		}

		public double FontPointSize
		{
			get
			{
                if (txSel != null)
                {
                    if (txSel.IsCommonValueSelected(TXTextControl.Selection.Attribute.FontSize))
                    {
                        return ((double)txSel.FontSize) / 20.0;  // twips to Points
                    }
                    else
                    {
                        return 0.0;
                    }
                }
            
                return 0.0;
            }
			set
			{
//				txSel.FontSize = Convert.ToInt32(value) * 20; // Points to twips
				txSel.FontSize = (int)(value * 20); // Points to twips
			}
		}

		public void ResetFormatting()
		{
			SetFontColor(Fonts.DefaultFontColor);
			FontName = Fonts.DefaultFontName;
			FontPointSize = Fonts.DefaultFontSize;

			Bold = Tristate.False;
			Italic = Tristate.False;
			Underline = Tristate.False;
		}

		public HorizontalAlignment ParagraphHAlignment
		{
			get
			{
				if (txSel.ParagraphFormat == null)
				{
					return HorizontalAlignment.Undefined;
				}
				if (txSel.IsCommonValueSelected(TXTextControl.ParagraphFormat.Attribute.Alignment))
				{
					switch (txSel.ParagraphFormat.Alignment)
					{
						case TXTextControl.HorizontalAlignment.Left:
							{
								return HorizontalAlignment.Left;
							}
						case TXTextControl.HorizontalAlignment.Center:
							{
								return HorizontalAlignment.Center;
							}
						case TXTextControl.HorizontalAlignment.Right:
							{
								return HorizontalAlignment.Right;
							}
						case TXTextControl.HorizontalAlignment.Justify:
							{
								return HorizontalAlignment.Justify;
							}
					}
					return HorizontalAlignment.Undefined;
				}
				else
				{
					return HorizontalAlignment.Undefined;
				}
			}
			set
			{
				if (txSel.ParagraphFormat != null)
				{
					switch (value)
					{
						case HorizontalAlignment.Left:
							{
								txSel.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Left;
								return;
							}
						case HorizontalAlignment.Center:
							{
								txSel.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Center;
								return;
							}
						case HorizontalAlignment.Right:
							{
								txSel.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Right;
								return;
							}
						case HorizontalAlignment.Justify:
							{
								txSel.ParagraphFormat.Alignment = TXTextControl.HorizontalAlignment.Justify;
								return;
							}
					}
				}
			}
		}

		public string Text
		{
			get
			{
				return (txSel == null ? string.Empty : txSel.Text);
			}
			set
			{
				if (txSel != null)
				{
					txSel.Text = value;
				}
			}
		}

		public int Start
		{
			get
			{
				return (txSel == null ? 0 : txSel.Start);
			}
			set
			{
				if (txSel != null)
				{
					txSel.Start = value;
				}
			}
		}

		public int Length
		{
			get
			{
				return (txSel == null ? 0 : txSel.Length);
			}
			set
			{
				if (txSel != null)
				{
					txSel.Length = value;
				}
			}
		}

		#endregion

		public TextSelection()
		{
			txSel = new TXTextControl.Selection();
		}

		#region Implementation Details

		public TextSelection(TXTextControl.Selection sel)
		{
			txSel = sel;
		}

		private TXTextControl.Selection txSel;

		#endregion
	}
}
