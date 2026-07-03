namespace Tawala.FormDesigner.Dialogs.SkipInstructionsDialog
{
	partial class SkipInstructionsStatementSelector
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
			this.tableLayoutPanelButtons = new System.Windows.Forms.TableLayoutPanel();
			this.gradientLabelStatements = new Tawala.Common.GradientLabel();
			this.SuspendLayout();
			// 
			// tableLayoutPanelButtons
			// 
			this.tableLayoutPanelButtons.ColumnCount = 1;
			this.tableLayoutPanelButtons.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelButtons.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanelButtons.GrowStyle = System.Windows.Forms.TableLayoutPanelGrowStyle.FixedSize;
			this.tableLayoutPanelButtons.Location = new System.Drawing.Point(0, 20);
			this.tableLayoutPanelButtons.Name = "tableLayoutPanelButtons";
			this.tableLayoutPanelButtons.Padding = new System.Windows.Forms.Padding(0, 8, 0, 0);
			this.tableLayoutPanelButtons.RowCount = 1;
			this.tableLayoutPanelButtons.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanelButtons.Size = new System.Drawing.Size(148, 276);
			this.tableLayoutPanelButtons.TabIndex = 0;
			// 
			// gradientLabelStatements
			// 
			this.gradientLabelStatements.Dock = System.Windows.Forms.DockStyle.Top;
			this.gradientLabelStatements.Font = new System.Drawing.Font("Arial", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.gradientLabelStatements.ForeColor = System.Drawing.Color.White;
			this.gradientLabelStatements.Location = new System.Drawing.Point(0, 0);
			this.gradientLabelStatements.Margin = new System.Windows.Forms.Padding(0);
			this.gradientLabelStatements.Name = "gradientLabelStatements";
			this.gradientLabelStatements.Size = new System.Drawing.Size(148, 20);
			this.gradientLabelStatements.TabIndex = 1;
			this.gradientLabelStatements.Text = "Statements";
			this.gradientLabelStatements.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// SkipInstructionsStatementSelector
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.Controls.Add(this.tableLayoutPanelButtons);
			this.Controls.Add(this.gradientLabelStatements);
			this.Name = "SkipInstructionsStatementSelector";
			this.Size = new System.Drawing.Size(148, 296);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanelButtons;
		private Tawala.Common.GradientLabel gradientLabelStatements;
	}
}
