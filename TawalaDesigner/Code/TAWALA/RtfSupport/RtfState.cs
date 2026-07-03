// $Workfile: RtfState.cs $
// $Revision: 16 $	$Date: 7/13/06 3:54p $
// Copyright © 2005 Tawala Systems, Inc. All rights reserved.
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using Tawala.XmlSupport;

namespace Tawala.RtfSupport
{
	public class RtfState
	{
		private bool inFontTable = false;
		private bool inColorTable = false;
		private bool inStyleSheet = false;
		private bool inOptionalGroup = false;
		private bool inTable = false;
		private bool inFieldData = false;
		private bool inImage = false;

		private bool bold = false;
		private bool italic = false;
		private bool underline = false;

		public enum Alignment
		{
			Left,
			Center,
			Right,
			Justify
		};

		private Alignment paragraphAlignment = Alignment.Left;

		private int paragraphIndent = 0;

		/// <summary>
		/// default font size in half points
		/// </summary>
		private static int defaultFontSize = 24;

		public static int DefaultFontSize
		{
			get { return defaultFontSize; }
		} 


		public RtfState()
		{
		}

		public RtfState Copy()
		{
			return (RtfState)this.MemberwiseClone();
		}

		public bool InFontTable
		{
			get { return inFontTable; }
			set { inFontTable = value; }
		}

		public bool InColorTable
		{
			get { return inColorTable; }
			set { inColorTable = value; }
		}

		public bool InStyleSheet
		{
			get { return inStyleSheet; }
			set { inStyleSheet = value; }
		}

		public bool InOptionalGroup
		{
			get { return inOptionalGroup; }
			set { inOptionalGroup = value; }
		}

		public bool InTable
		{
			get { return inTable; }
			set { inTable = value; }
		}

		public bool InImage
		{
			get { return inImage; }
			set { inImage = value; }
		}

		public bool InFieldData
		{
			get { return inFieldData; }
			set { inFieldData = value; }
		}

		public bool InContent
		{
			get
			{
				return (!inFontTable && !inColorTable && !inStyleSheet && !inOptionalGroup);
			}
		}

		/// <summary>
		/// font size in half-points
		/// </summary>
		private int fontSize = defaultFontSize;

		public int FontSize
		{
			get { return fontSize; }
			set { fontSize = value; }
		}

		/// <summary>
		/// Index into font table
		/// </summary>
		private int fontIndex = 0;

		public int FontIndex
		{
			get { return fontIndex; }
			set { fontIndex = value; }
		}


		/// <summary>
		/// Index into color table
		/// </summary>
		private int colorIndex = 0;

		public int ColorIndex
		{
			get { return colorIndex; }
			set { colorIndex = value; }
		}


		public bool Bold
		{
			get { return bold; }
			set	{bold = value; }
		}

		public bool Italic
		{
			get { return italic; }
			set { italic = value; }
		}

		public bool Underline
		{
			get { return underline; }
			set	{ underline = value; }
		}

		public Alignment ParagraphAlignment
		{
			get { return paragraphAlignment; }
			set { paragraphAlignment = value; }
		}

		public int ParagraphIndent
		{
			get { return paragraphIndent; }
			set { paragraphIndent = value; }
		}

		/// <summary>
		/// Current paragraph nesting level
		///  0 = main document
		///  1 = table cell (default)
		///  2 = nested table cell
		///  3 = nested table cell, etc.
		/// </summary>
		private int paragraphNestingLevel = 1;

		public int ParagraphNestingLevel
		{
			get { return paragraphNestingLevel; }
			set { paragraphNestingLevel = value; }
		}

	}
}
