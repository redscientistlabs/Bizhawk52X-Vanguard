﻿namespace BizHawk.Client.EmuHawk
{
	partial class ControllerConfig
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
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.NormalControlsTab = new System.Windows.Forms.TabPage();
			this.AutofireControlsTab = new System.Windows.Forms.TabPage();
			this.AnalogControlsTab = new System.Windows.Forms.TabPage();
			this.FeedbacksTab = new System.Windows.Forms.TabPage();
			this.checkBoxAutoTab = new System.Windows.Forms.CheckBox();
			this.buttonOK = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.testToolStripMenuItem = new BizHawk.WinForms.Controls.ToolStripMenuItemEx();
			this.loadDefaultsToolStripMenuItem = new BizHawk.WinForms.Controls.ToolStripMenuItemEx();
			this.clearToolStripMenuItem = new BizHawk.WinForms.Controls.ToolStripMenuItemEx();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.label3 = new BizHawk.WinForms.Controls.LocLabelEx();
			this.label2 = new BizHawk.WinForms.Controls.LocLabelEx();
			this.label38 = new BizHawk.WinForms.Controls.LocLabelEx();
			this.btnMisc = new BizHawk.Client.EmuHawk.MenuButton();
			this.flpUDLR = new BizHawk.WinForms.Controls.LocSingleRowFLP();
			this.lblUDLR = new BizHawk.WinForms.Controls.LabelEx();
			this.rbUDLRForbid = new BizHawk.WinForms.Controls.RadioButtonEx();
			this.rbUDLRPriority = new BizHawk.WinForms.Controls.RadioButtonEx();
			this.rbUDLRAllow = new BizHawk.WinForms.Controls.RadioButtonEx();
			this.tabControl1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			this.contextMenuStrip1.SuspendLayout();
			this.flpUDLR.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.NormalControlsTab);
			this.tabControl1.Controls.Add(this.AutofireControlsTab);
			this.tabControl1.Controls.Add(this.AnalogControlsTab);
			this.tabControl1.Controls.Add(this.FeedbacksTab);
			this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tabControl1.Location = new System.Drawing.Point(3, 3);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(562, 521);
			this.tabControl1.TabIndex = 1;
			// 
			// NormalControlsTab
			// 
			this.NormalControlsTab.Location = new System.Drawing.Point(4, 22);
			this.NormalControlsTab.Name = "NormalControlsTab";
			this.NormalControlsTab.Padding = new System.Windows.Forms.Padding(3);
			this.NormalControlsTab.Size = new System.Drawing.Size(554, 495);
			this.NormalControlsTab.TabIndex = 0;
			this.NormalControlsTab.Text = "Normal Controls";
			this.NormalControlsTab.UseVisualStyleBackColor = true;
			// 
			// AutofireControlsTab
			// 
			this.AutofireControlsTab.Location = new System.Drawing.Point(4, 22);
			this.AutofireControlsTab.Name = "AutofireControlsTab";
			this.AutofireControlsTab.Padding = new System.Windows.Forms.Padding(3);
			this.AutofireControlsTab.Size = new System.Drawing.Size(554, 495);
			this.AutofireControlsTab.TabIndex = 1;
			this.AutofireControlsTab.Text = "Autofire Controls";
			this.AutofireControlsTab.UseVisualStyleBackColor = true;
			// 
			// AnalogControlsTab
			// 
			this.AnalogControlsTab.Location = new System.Drawing.Point(4, 22);
			this.AnalogControlsTab.Name = "AnalogControlsTab";
			this.AnalogControlsTab.Size = new System.Drawing.Size(554, 495);
			this.AnalogControlsTab.TabIndex = 2;
			this.AnalogControlsTab.Text = "Analog Controls";
			this.AnalogControlsTab.UseVisualStyleBackColor = true;
			// 
			// FeedbacksTab
			// 
			this.FeedbacksTab.Location = new System.Drawing.Point(4, 22);
			this.FeedbacksTab.Name = "FeedbacksTab";
			this.FeedbacksTab.Size = new System.Drawing.Size(554, 495);
			this.FeedbacksTab.TabIndex = 3;
			this.FeedbacksTab.Text = "Feedbacks";
			this.FeedbacksTab.UseVisualStyleBackColor = true;
			// 
			// checkBoxAutoTab
			// 
			this.checkBoxAutoTab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.checkBoxAutoTab.AutoSize = true;
			this.checkBoxAutoTab.Location = new System.Drawing.Point(371, 548);
			this.checkBoxAutoTab.Name = "checkBoxAutoTab";
			this.checkBoxAutoTab.Size = new System.Drawing.Size(70, 17);
			this.checkBoxAutoTab.TabIndex = 3;
			this.checkBoxAutoTab.Text = "Auto Tab";
			this.checkBoxAutoTab.UseVisualStyleBackColor = true;
			this.checkBoxAutoTab.CheckedChanged += new System.EventHandler(this.CheckBoxAutoTab_CheckedChanged);
			// 
			// buttonOK
			// 
			this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOK.Location = new System.Drawing.Point(764, 542);
			this.buttonOK.Name = "buttonOK";
			this.buttonOK.Size = new System.Drawing.Size(75, 23);
			this.buttonOK.TabIndex = 5;
			this.buttonOK.Text = "&Save";
			this.buttonOK.UseVisualStyleBackColor = true;
			this.buttonOK.Click += new System.EventHandler(this.ButtonOk_Click);
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(845, 542);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 6;
			this.buttonCancel.Text = "&Cancel";
			this.buttonCancel.UseVisualStyleBackColor = true;
			this.buttonCancel.Click += new System.EventHandler(this.ButtonCancel_Click);
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 340F));
			this.tableLayoutPanel1.Controls.Add(this.tabControl1, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.pictureBox1, 1, 0);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 1;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(908, 527);
			this.tableLayoutPanel1.TabIndex = 7;
			// 
			// pictureBox1
			// 
			this.pictureBox1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.pictureBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.pictureBox1.Location = new System.Drawing.Point(571, 23);
			this.pictureBox1.Margin = new System.Windows.Forms.Padding(3, 23, 3, 3);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(334, 501);
			this.pictureBox1.TabIndex = 2;
			this.pictureBox1.TabStop = false;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.testToolStripMenuItem,
            this.loadDefaultsToolStripMenuItem,
            this.clearToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(147, 70);
			// 
			// testToolStripMenuItem
			// 
			this.testToolStripMenuItem.Text = "Save Defaults";
			this.testToolStripMenuItem.Click += new System.EventHandler(this.ButtonSaveDefaults_Click);
			// 
			// loadDefaultsToolStripMenuItem
			// 
			this.loadDefaultsToolStripMenuItem.Text = "Load Defaults";
			this.loadDefaultsToolStripMenuItem.Click += new System.EventHandler(this.ButtonLoadDefaults_Click);
			// 
			// clearToolStripMenuItem
			// 
			this.clearToolStripMenuItem.Text = "Clear";
			this.clearToolStripMenuItem.Click += new System.EventHandler(this.ClearBtn_Click);
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(11, 550);
			this.label3.Name = "label3";
			this.label3.Text = "Tips:";
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.Location = new System.Drawing.Point(197, 550);
			this.label2.Name = "label2";
			this.label2.Text = "* Disable Auto Tab to multiply bind";
			// 
			// label38
			// 
			this.label38.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label38.Location = new System.Drawing.Point(41, 550);
			this.label38.Name = "label38";
			this.label38.Text = "* Escape clears a key mapping";
			// 
			// btnMisc
			// 
			this.btnMisc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnMisc.Location = new System.Drawing.Point(683, 542);
			this.btnMisc.Menu = this.contextMenuStrip1;
			this.btnMisc.Name = "btnMisc";
			this.btnMisc.Size = new System.Drawing.Size(75, 23);
			this.btnMisc.TabIndex = 11;
			this.btnMisc.Text = "Misc...";
			this.btnMisc.UseVisualStyleBackColor = true;
			// 
			// flpUDLR
			// 
			this.flpUDLR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.flpUDLR.Controls.Add(this.lblUDLR);
			this.flpUDLR.Controls.Add(this.rbUDLRForbid);
			this.flpUDLR.Controls.Add(this.rbUDLRPriority);
			this.flpUDLR.Controls.Add(this.rbUDLRAllow);
			this.flpUDLR.Location = new System.Drawing.Point(474, 468);
			this.flpUDLR.Name = "flpUDLR";
			// 
			// lblUDLR
			// 
			this.lblUDLR.Name = "lblUDLR";
			this.lblUDLR.Text = "U+D/L+R:";
			// 
			// rbUDLRForbid
			// 
			this.rbUDLRForbid.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.rbUDLRForbid.Name = "rbUDLRForbid";
			this.rbUDLRForbid.Text = "Forbid";
			// 
			// rbUDLRPriority
			// 
			this.rbUDLRPriority.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.rbUDLRPriority.Name = "rbUDLRPriority";
			this.rbUDLRPriority.Text = "Priority";
			// 
			// rbUDLRAllow
			// 
			this.rbUDLRAllow.Margin = new System.Windows.Forms.Padding(0, 3, 0, 3);
			this.rbUDLRAllow.Name = "rbUDLRAllow";
			this.rbUDLRAllow.Text = "Allow";
			// 
			// ControllerConfig
			// 
			this.AcceptButton = this.buttonOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(932, 572);
			this.Controls.Add(this.flpUDLR);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label38);
			this.Controls.Add(this.btnMisc);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOK);
			this.Controls.Add(this.checkBoxAutoTab);
			this.MinimumSize = new System.Drawing.Size(948, 611);
			this.Name = "ControllerConfig";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Controller Config";
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.ControllerConfig_FormClosed);
			this.Load += new System.EventHandler(this.ControllerConfig_Load);
			this.tabControl1.ResumeLayout(false);
			this.tableLayoutPanel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			this.contextMenuStrip1.ResumeLayout(false);
			this.flpUDLR.ResumeLayout(false);
			this.flpUDLR.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage NormalControlsTab;
		private System.Windows.Forms.TabPage AutofireControlsTab;
		private System.Windows.Forms.CheckBox checkBoxAutoTab;
		private System.Windows.Forms.Button buttonOK;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.TabPage AnalogControlsTab;
		private System.Windows.Forms.TabPage FeedbacksTab;
		private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
		private System.Windows.Forms.ToolTip toolTip1;
		private MenuButton btnMisc;
				private BizHawk.WinForms.Controls.ToolStripMenuItemEx testToolStripMenuItem;
				private BizHawk.WinForms.Controls.ToolStripMenuItemEx loadDefaultsToolStripMenuItem;
				private BizHawk.WinForms.Controls.ToolStripMenuItemEx clearToolStripMenuItem;
				private BizHawk.WinForms.Controls.LocLabelEx label3;
				private BizHawk.WinForms.Controls.LocLabelEx label2;
				private BizHawk.WinForms.Controls.LocLabelEx label38;
		private WinForms.Controls.LocSingleRowFLP flpUDLR;
		private WinForms.Controls.RadioButtonEx rbUDLRForbid;
		private WinForms.Controls.RadioButtonEx rbUDLRPriority;
		private WinForms.Controls.RadioButtonEx rbUDLRAllow;
		private WinForms.Controls.LabelEx lblUDLR;
	}
}