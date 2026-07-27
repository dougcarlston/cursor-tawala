// Copyright © 2005-2009 Tawala Systems, Inc. All rights reserved.
using System;
using System.Drawing;
using System.Windows.Forms;
using Tawala.Forms;
using Tawala.Processes;
using Tawala.Projects;
using Tawala.Projects.Processes;

namespace Tawala.DesignerUI
{
    public partial class ProjectDesignerToolsView : UserControl
    {
        private static readonly FormsToolPaletteView formsToolPaletteView = new FormsToolPaletteView();
        private static readonly ProcessesToolPaletteView processesToolPaletteView = new ProcessesToolPaletteView();

        private bool isDrag;
        private Point lastMousePos;
        private Rectangle theRectangle;

        public ProjectDesignerToolsView()
        {
            InitializeComponent();

            projectExplorer.Padding = new Padding(0);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Project.Events.CurrentComponentSet += project_CurrentComponentSet;
        }

        private void project_CurrentComponentSet(object sender, ComponentEventArgs e)
        {
            Control currentPalette = tabControl.TabPages[1].Controls.Count == 0 ? null : tabControl.TabPages[1].Controls[0];

            if (e.Component is IForm)
            {
                if (currentPalette != formsToolPaletteView)
                {
                    tabControl.TabPages[1].Controls.Clear();
                    formsToolPaletteView.Dock = DockStyle.Fill;
                    tabControl.TabPages[1].Controls.Add(formsToolPaletteView);
                }
            }
            else if (e.Component is Process)
            {
                if (currentPalette != processesToolPaletteView)
                {
                    tabControl.TabPages[1].Controls.Clear();
                    processesToolPaletteView.Dock = DockStyle.Fill;
                    tabControl.TabPages[1].Controls.Add(processesToolPaletteView);
                }
            }
            else
            {
                if (currentPalette != null)
                {
                    tabControl.TabPages[1].Controls.Clear();
                }
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);

            if (Cursor != Cursors.Default)
            {
                Cursor = Cursors.Default;
            }
        }

        private bool isMouseOverResizeBar()
        {
            if (MouseButtons == MouseButtons.None || MouseButtons == MouseButtons.Left)
            {
                Point ptClient = PointToClient(MousePosition);
                return ptClient.X >= Right - Padding.Right && ptClient.X < Right - 1;
            }
            return false;
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            if (isMouseOverResizeBar())
            {
                isDrag = true;
                Cursor = Cursors.SizeWE;
                theRectangle = RectangleToScreen(new Rectangle(Right - Padding.Right, 0, Padding.Right, Height));
                Invalidate(RectangleToClient(theRectangle));
                lastMousePos = MousePosition;
            }
            else
            {
                isDrag = false;
                base.OnMouseDown(e);
            }
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (isMouseOverResizeBar())
            {
                Cursor = Cursors.SizeWE;
            }

            if (isDrag)
            {
                Point mp = MousePosition;
                int changeX = mp.X - lastMousePos.X;

                if (Width + changeX > MinimumSize.Width && Width + changeX < Parent.Width - 40)
                {
                    theRectangle = RectangleToScreen(new Rectangle(Right - Padding.Right, 0, Padding.Right, Height));
                    theRectangle.Offset(changeX, 0);

                    Width += changeX;
                    Invalidate(true);
                    Update();
                }

                lastMousePos = mp;
            }
            else
            {
                base.OnMouseMove(e);
            }
        }

        protected override void OnMouseUp(MouseEventArgs e)
        {
            if (isDrag)
            {
                isDrag = false;
                Cursor = Cursors.Default;
                Invalidate(RectangleToClient(theRectangle));
            }
            else
            {
                base.OnMouseUp(e);
            }
        }
    }
}