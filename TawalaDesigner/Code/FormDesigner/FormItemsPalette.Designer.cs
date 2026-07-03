namespace Tawala.FormDesigner
{
	partial class FormItemsPalette
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormItemsPalette));
			System.Windows.Forms.ListViewItem listViewItem1 = new System.Windows.Forms.ListViewItem("Heading", 0);
			System.Windows.Forms.ListViewItem listViewItem2 = new System.Windows.Forms.ListViewItem("Text", 1);
			System.Windows.Forms.ListViewItem listViewItem3 = new System.Windows.Forms.ListViewItem("Fill in the blank", 2);
			System.Windows.Forms.ListViewItem listViewItem4 = new System.Windows.Forms.ListViewItem("Multiple Choice", 4);
			System.Windows.Forms.ListViewItem listViewItem5 = new System.Windows.Forms.ListViewItem("Hidden Field", 5);
			System.Windows.Forms.ListViewItem listViewItem6 = new System.Windows.Forms.ListViewItem("Page Break", 6);
			System.Windows.Forms.ListViewItem listViewItem7 = new System.Windows.Forms.ListViewItem("Skip Instructions", 7);
			this.imageList = new System.Windows.Forms.ImageList(this.components);
			this.gradientLabelFormItems = new Tawala.Common.GradientLabel();
			this.listViewFormItems = new System.Windows.Forms.ListView();
			this.SuspendLayout();
			// 
			// imageList
			// 
			this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
			this.imageList.TransparentColor = System.Drawing.Color.Transparent;
			this.imageList.Images.SetKeyName(0, "Form_ItemHeading.png");
			this.imageList.Images.SetKeyName(1, "Form_ItemText.png");
			this.imageList.Images.SetKeyName(2, "Form_ItemFib.png");
			this.imageList.Images.SetKeyName(3, "Form_ItemTextarea.png");
			this.imageList.Images.SetKeyName(4, "Form_ItemMcq.png");
			this.imageList.Images.SetKeyName(5, "Form_ItemHiddenField.png");
			this.imageList.Images.SetKeyName(6, "Form_ItemBreak.png");
			this.imageList.Images.SetKeyName(7, "Form_ItemSkip.png");
			// 
			// gradientLabelFormItems
			// 
			this.gradientLabelFormItems.Dock = System.Windows.Forms.DockStyle.Top;
			this.gradientLabelFormItems.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gradientLabelFormItems.ForeColor = System.Drawing.Color.White;
			this.gradientLabelFormItems.Location = new System.Drawing.Point(0, 0);
			this.gradientLabelFormItems.Margin = new System.Windows.Forms.Padding(0);
			this.gradientLabelFormItems.Name = "gradientLabelFormItems";
			this.gradientLabelFormItems.Size = new System.Drawing.Size(112, 20);
			this.gradientLabelFormItems.TabIndex = 1;
			this.gradientLabelFormItems.Text = "Form Items";
			this.gradientLabelFormItems.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// listViewFormItems
			// 
			this.listViewFormItems.AutoArrange = false;
			this.listViewFormItems.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.listViewFormItems.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.listViewFormItems.Dock = System.Windows.Forms.DockStyle.Top;
			this.listViewFormItems.FullRowSelect = true;
			listViewItem1.Tag = "HeadingItem";
			listViewItem2.Tag = "TextItem";
			listViewItem3.Tag = "FibItem";
			listViewItem4.Tag = "McqItem";
			listViewItem5.Tag = "FieldItem";
			listViewItem6.Tag = "BreakItem";
			listViewItem7.Tag = "SkipItem";
			this.listViewFormItems.Items.AddRange(new System.Windows.Forms.ListViewItem[] {
            listViewItem1,
            listViewItem2,
            listViewItem3,
            listViewItem4,
            listViewItem5,
            listViewItem6,
            listViewItem7});
			this.listViewFormItems.Location = new System.Drawing.Point(0, 20);
			this.listViewFormItems.Margin = new System.Windows.Forms.Padding(6);
			this.listViewFormItems.MultiSelect = false;
			this.listViewFormItems.Name = "listViewFormItems";
			this.listViewFormItems.Size = new System.Drawing.Size(112, 174);
			this.listViewFormItems.SmallImageList = this.imageList;
			this.listViewFormItems.TabIndex = 2;
			this.listViewFormItems.UseCompatibleStateImageBehavior = false;
			this.listViewFormItems.View = System.Windows.Forms.View.List;
			this.listViewFormItems.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listViewFormItems_MouseDoubleClick);
			this.listViewFormItems.ItemDrag += new System.Windows.Forms.ItemDragEventHandler(this.listViewFormItems_ItemDrag);
			// 
			// FormItemsPalette
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.listViewFormItems);
			this.Controls.Add(this.gradientLabelFormItems);
			this.Name = "FormItemsPalette";
			this.Size = new System.Drawing.Size(112, 184);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.ImageList imageList;
		private Tawala.Common.GradientLabel gradientLabelFormItems;
		private System.Windows.Forms.ListView listViewFormItems;
	}
}
