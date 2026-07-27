// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Tawala.Projects.Forms;

namespace Tawala.Forms
{
    /// <summary>
    /// A Form Break
    /// </summary>
    public partial class BreakItemView : ItemViewBase
    {
        private static readonly Pen pen;
        private static Brush brushBackground;
        private static Font labelFont;

        /// <summary>
        /// class (not instance) constructor
        /// </summary>
        static BreakItemView()
        {
            pen = (Pen)Pens.Blue.Clone();
            pen.DashStyle = DashStyle.Dash;
            pen.Width = 2;
        }

        public BreakItemView()
        {
            // This call is required by the Windows Form Designer.
            InitializeComponent();
        }

        public new BreakItem FormItem { get { return base.FormItem as BreakItem; } }

        public override string DefaultLabel { get { return "BREAK"; } set { } }

        public override Brush LabelTextBrush { get { return Brushes.DarkBlue; } }

        public override Font LabelTextFont
        {
            get
            {
                if (labelFont == null)
                {
                    labelFont = new Font(Font, FontStyle.Italic);
                }
                return labelFont;
            }
        }

        public override bool AlternateLabelEditable { get { return false; } }

        /// <summary>
        /// Called when laying out items.  
        /// Items should always use the proposized width but they
        /// are free to choose there height (and must specify it).  
        /// </summary>
        /// <remarks>
        /// proposedSize.Height is always 0 to indicate it is unconstrained
        /// </remarks>
        public override Size GetPreferredSize(Size proposedSize)
        {
            return new Size(proposedSize.Width, 18);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (brushBackground == null)
            {
                brushBackground = new HatchBrush(HatchStyle.Percent50, SystemColors.ControlLight, SystemColors.ControlDark);
            }
        }

        /// <summary>
        /// Paint the dashed line in the break
        /// </summary>
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.FillRectangle(brushBackground, LabelWidth, 0, Width - LabelWidth, Height - 1);
        }
    }
}