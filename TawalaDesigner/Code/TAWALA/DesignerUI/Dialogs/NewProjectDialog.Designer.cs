namespace Tawala.DesignerUI
{
	partial class NewProjectDialog
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NewProjectDialog));
			System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("All");
			this.labelTemplateDescription = new System.Windows.Forms.Label();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOK = new System.Windows.Forms.Button();
			this.listViewTemplates = new System.Windows.Forms.ListView();
			this.imageListTemplate = new System.Windows.Forms.ImageList(this.components);
			this.labelProjectTypes = new System.Windows.Forms.Label();
			this.labelTemplates = new System.Windows.Forms.Label();
			this.treeViewCategories = new System.Windows.Forms.TreeView();
			this.SuspendLayout();
			// 
			// labelTemplateDescription
			// 
			this.labelTemplateDescription.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.labelTemplateDescription.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.labelTemplateDescription.Location = new System.Drawing.Point(16, 234);
			this.labelTemplateDescription.Name = "labelTemplateDescription";
			this.labelTemplateDescription.Size = new System.Drawing.Size(610, 23);
			this.labelTemplateDescription.TabIndex = 25;
			this.labelTemplateDescription.Text = "Short description of project here.";
			this.labelTemplateDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(552, 270);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 24;
			this.buttonCancel.Text = "Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.buttonCancel_Click);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOK.Location = new System.Drawing.Point(456, 270);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 23;
			this.buttonOK.Text = "OK";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
			// 
			// listViewTemplates
			// 
			this.listViewTemplates.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.listViewTemplates.HideSelection = false;
			this.listViewTemplates.LargeImageList = this.imageListTemplate;
			this.listViewTemplates.Location = new System.Drawing.Point(186, 29);
			this.listViewTemplates.MultiSelect = false;
			this.listViewTemplates.Name = "listViewTemplates";
			this.listViewTemplates.Size = new System.Drawing.Size(440, 200);
			this.listViewTemplates.Sorting = System.Windows.Forms.SortOrder.Ascending;
			this.listViewTemplates.TabIndex = 15;
			this.listViewTemplates.UseCompatibleStateImageBehavior = false;
			this.listViewTemplates.DoubleClick += new System.EventHandler(this.listViewTemplates_DoubleClick);
			this.listViewTemplates.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listViewTemplates_ItemSelectionChanged);
			// 
			// imageListTemplate
			// 
			this.imageListTemplate.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListTemplate.ImageStream")));
			this.imageListTemplate.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListTemplate.Images.SetKeyName(0, "document.png");
			// 
			// labelProjectTypes
			// 
			this.labelProjectTypes.AutoSize = true;
			this.labelProjectTypes.Location = new System.Drawing.Point(16, 12);
			this.labelProjectTypes.Name = "labelProjectTypes";
			this.labelProjectTypes.Size = new System.Drawing.Size(66, 13);
			this.labelProjectTypes.TabIndex = 14;
			this.labelProjectTypes.Text = "Project type:";
			// 
			// labelTemplates
			// 
			this.labelTemplates.AutoSize = true;
			this.labelTemplates.Location = new System.Drawing.Point(186, 12);
			this.labelTemplates.Name = "labelTemplates";
			this.labelTemplates.Size = new System.Drawing.Size(59, 13);
			this.labelTemplates.TabIndex = 16;
			this.labelTemplates.Text = "Templates:";
			// 
			// treeViewCategories
			// 
			this.treeViewCategories.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)));
			this.treeViewCategories.HideSelection = false;
			this.treeViewCategories.Location = new System.Drawing.Point(16, 29);
			this.treeViewCategories.Name = "treeViewCategories";
			treeNode1.Name = "All";
			treeNode1.Text = "All";
			this.treeViewCategories.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1});
			this.treeViewCategories.Size = new System.Drawing.Size(152, 200);
			this.treeViewCategories.TabIndex = 13;
			this.treeViewCategories.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeViewCategories_AfterSelect);
			// 
			// NewProjectDialog
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(644, 304);
			this.Controls.Add(this.labelTemplateDescription);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.listViewTemplates);
			this.Controls.Add(this.labelProjectTypes);
			this.Controls.Add(this.labelTemplates);
			this.Controls.Add(this.treeViewCategories);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.MinimumSize = new System.Drawing.Size(660, 340);
			this.Name = "NewProjectDialog";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "New Project";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelTemplateDescription;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.ListView listViewTemplates;
		private System.Windows.Forms.Label labelProjectTypes;
		private System.Windows.Forms.Label labelTemplates;
		private System.Windows.Forms.TreeView treeViewCategories;
		private System.Windows.Forms.ImageList imageListTemplate;
	}
}