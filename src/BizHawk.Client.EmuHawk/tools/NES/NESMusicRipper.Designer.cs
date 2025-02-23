﻿using BizHawk.WinForms.Controls;

namespace BizHawk.Client.EmuHawk
{
	partial class NESMusicRipper
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NESMusicRipper));
			this.btnControl = new System.Windows.Forms.Button();
			this.txtDivider = new System.Windows.Forms.TextBox();
			this.label1 = new BizHawk.WinForms.Controls.LocLabelEx();
			this.btnExport = new System.Windows.Forms.Button();
			this.lblContents = new BizHawk.WinForms.Controls.LocLabelEx();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.txtPatternLength = new System.Windows.Forms.TextBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label2 = new BizHawk.WinForms.Controls.LocLabelEx();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.menuStrip1 = new MenuStripEx();
			this.FileSubMenu = new BizHawk.WinForms.Controls.ToolStripMenuItemEx();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnControl
			// 
			this.btnControl.Location = new System.Drawing.Point(6, 57);
			this.btnControl.Name = "btnControl";
			this.btnControl.Size = new System.Drawing.Size(75, 23);
			this.btnControl.TabIndex = 0;
			this.btnControl.Text = "Start";
			this.btnControl.UseVisualStyleBackColor = true;
			this.btnControl.Click += new System.EventHandler(this.BtnControl_Click);
			// 
			// txtDivider
			// 
			this.txtDivider.Location = new System.Drawing.Point(9, 32);
			this.txtDivider.Name = "txtDivider";
			this.txtDivider.Size = new System.Drawing.Size(100, 20);
			this.txtDivider.TabIndex = 1;
			this.txtDivider.Text = "29824";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 16);
			this.label1.Name = "label1";
			this.label1.Text = "APU Divider (trace interval)";
			// 
			// btnExport
			// 
			this.btnExport.AutoSize = true;
			this.btnExport.Location = new System.Drawing.Point(6, 118);
			this.btnExport.Name = "btnExport";
			this.btnExport.Size = new System.Drawing.Size(100, 23);
			this.btnExport.TabIndex = 3;
			this.btnExport.Text = "Export XRNS File";
			this.btnExport.UseVisualStyleBackColor = true;
			this.btnExport.Click += new System.EventHandler(this.Export_Click);
			// 
			// lblContents
			// 
			this.lblContents.Location = new System.Drawing.Point(6, 102);
			this.lblContents.Name = "lblContents";
			this.lblContents.Text = "(Contents)";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(12, 211);
			this.textBox1.Multiline = true;
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(390, 80);
			this.textBox1.TabIndex = 6;
			this.textBox1.Text = resources.GetString("textBox1.Text");
			// 
			// txtPatternLength
			// 
			this.txtPatternLength.Location = new System.Drawing.Point(12, 37);
			this.txtPatternLength.Name = "txtPatternLength";
			this.txtPatternLength.Size = new System.Drawing.Size(100, 20);
			this.txtPatternLength.TabIndex = 7;
			this.txtPatternLength.Text = "512";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.txtPatternLength);
			this.groupBox1.Location = new System.Drawing.Point(0, 27);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(200, 156);
			this.groupBox1.TabIndex = 8;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Config";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 21);
			this.label2.Name = "label2";
			this.label2.Text = "Pattern Length (512 max)";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.btnControl);
			this.groupBox2.Controls.Add(this.txtDivider);
			this.groupBox2.Controls.Add(this.btnExport);
			this.groupBox2.Controls.Add(this.lblContents);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(206, 32);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(200, 151);
			this.groupBox2.TabIndex = 9;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Log Control";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.FileSubMenu});
			this.menuStrip1.TabIndex = 5;
			// 
			// FileSubMenu
			// 
			this.FileSubMenu.Text = "&File";
			// 
			// NESMusicRipper
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(437, 305);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.menuStrip1);
			this.Name = "NESMusicRipper";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NESMusicRipper_FormClosed);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button btnControl;
		private System.Windows.Forms.TextBox txtDivider;
		private BizHawk.WinForms.Controls.LocLabelEx label1;
		private System.Windows.Forms.Button btnExport;
		private BizHawk.WinForms.Controls.LocLabelEx lblContents;
		private MenuStripEx menuStrip1;
		private BizHawk.WinForms.Controls.ToolStripMenuItemEx FileSubMenu;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox txtPatternLength;
		private System.Windows.Forms.GroupBox groupBox1;
		private BizHawk.WinForms.Controls.LocLabelEx label2;
		private System.Windows.Forms.GroupBox groupBox2;
	}
}