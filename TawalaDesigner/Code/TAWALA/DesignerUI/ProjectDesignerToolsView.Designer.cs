namespace Tawala.DesignerUI
{
    partial class ProjectDesignerToolsView
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageProject = new System.Windows.Forms.TabPage();
            this.projectExplorer = new Tawala.DesignerUI.ProjectExplorer();
            this.tabPageTools = new System.Windows.Forms.TabPage();
            this.tabControl.SuspendLayout();
            this.tabPageProject.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageProject);
            this.tabControl.Controls.Add(this.tabPageTools);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(176, 458);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageProject
            // 
            this.tabPageProject.Controls.Add(this.projectExplorer);
            this.tabPageProject.Location = new System.Drawing.Point(4, 22);
            this.tabPageProject.Name = "tabPageProject";
            this.tabPageProject.Size = new System.Drawing.Size(168, 432);
            this.tabPageProject.TabIndex = 0;
            this.tabPageProject.Text = "Project";
            this.tabPageProject.UseVisualStyleBackColor = true;
            // 
            // projectExplorer
            // 
            this.projectExplorer.BackColor = System.Drawing.Color.White;
            this.projectExplorer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.projectExplorer.Location = new System.Drawing.Point(0, 0);
            this.projectExplorer.Margin = new System.Windows.Forms.Padding(0);
            this.projectExplorer.MinimumSize = new System.Drawing.Size(170, 0);
            this.projectExplorer.Name = "projectExplorer";
            this.projectExplorer.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.projectExplorer.Size = new System.Drawing.Size(170, 432);
            this.projectExplorer.TabIndex = 0;
            // 
            // tabPageTools
            // 
            this.tabPageTools.Location = new System.Drawing.Point(4, 22);
            this.tabPageTools.Name = "tabPageTools";
            this.tabPageTools.Size = new System.Drawing.Size(168, 432);
            this.tabPageTools.TabIndex = 1;
            this.tabPageTools.Text = "Tools";
            this.tabPageTools.UseVisualStyleBackColor = true;
            // 
            // ProjectDesignerToolsView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tabControl);
            this.Margin = new System.Windows.Forms.Padding(0);
            this.MinimumSize = new System.Drawing.Size(181, 0);
            this.Name = "ProjectDesignerToolsView";
            this.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.Size = new System.Drawing.Size(181, 458);
            this.tabControl.ResumeLayout(false);
            this.tabPageProject.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageProject;
        private System.Windows.Forms.TabPage tabPageTools;
        private ProjectExplorer projectExplorer;
    }
}
