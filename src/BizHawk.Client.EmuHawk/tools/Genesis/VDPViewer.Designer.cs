﻿using BizHawk.WinForms.Controls;

namespace BizHawk.Client.EmuHawk
{
	partial class GenVdpViewer
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
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.bmpViewTiles = new BizHawk.Client.EmuHawk.BmpView();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.bmpViewPal = new BizHawk.Client.EmuHawk.BmpView();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.bmpViewNTW = new BizHawk.Client.EmuHawk.BmpView();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.bmpViewNTA = new BizHawk.Client.EmuHawk.BmpView();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.bmpViewNTB = new BizHawk.Client.EmuHawk.BmpView();
			this.label1 = new BizHawk.WinForms.Controls.LocLabelEx();
			this.menuStrip1 = new MenuStripEx();
			this.fileToolStripMenuItem = new BizHawk.WinForms.Controls.ToolStripMenuItemEx();
			this.saveBGAScreenshotToolStripMenuItem = new BizHawk.WinForms.Controls.ToolStripMenuItemEx();
			this.saveBGBScreenshotToolStripMenuItem = new BizHawk.WinForms.Controls.ToolStripMenuItemEx();
			this.saveTilesScreenshotToolStripMenuItem = new BizHawk.WinForms.Controls.ToolStripMenuItemEx();
			this.saveWindowScreenshotToolStripMenuItem = new BizHawk.WinForms.Controls.ToolStripMenuItemEx();
			this.savePaletteScreenshotToolStripMenuItem = new BizHawk.WinForms.Controls.ToolStripMenuItemEx();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.menuStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.bmpViewTiles);
			this.groupBox1.Location = new System.Drawing.Point(12, 568);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(524, 281);
			this.groupBox1.TabIndex = 5;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Tiles";
			// 
			// bmpViewTiles
			// 
			this.bmpViewTiles.Location = new System.Drawing.Point(6, 19);
			this.bmpViewTiles.Name = "bmpViewTiles";
			this.bmpViewTiles.Size = new System.Drawing.Size(512, 256);
			this.bmpViewTiles.TabIndex = 0;
			this.bmpViewTiles.Text = "bmpView1";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.bmpViewPal);
			this.groupBox2.Location = new System.Drawing.Point(12, 855);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(268, 89);
			this.groupBox2.TabIndex = 6;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Palettes";
			// 
			// bmpViewPal
			// 
			this.bmpViewPal.Location = new System.Drawing.Point(6, 19);
			this.bmpViewPal.Name = "bmpViewPal";
			this.bmpViewPal.Size = new System.Drawing.Size(256, 64);
			this.bmpViewPal.TabIndex = 4;
			this.bmpViewPal.Text = "bmpView1";
			this.bmpViewPal.MouseClick += new System.Windows.Forms.MouseEventHandler(this.BmpViewPal_MouseClick);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.bmpViewNTW);
			this.groupBox3.Location = new System.Drawing.Point(542, 568);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(524, 281);
			this.groupBox3.TabIndex = 7;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Window";
			// 
			// bmpViewNTW
			// 
			this.bmpViewNTW.Location = new System.Drawing.Point(6, 19);
			this.bmpViewNTW.Name = "bmpViewNTW";
			this.bmpViewNTW.Size = new System.Drawing.Size(75, 23);
			this.bmpViewNTW.TabIndex = 3;
			this.bmpViewNTW.Text = "bmpView1";
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.bmpViewNTA);
			this.groupBox4.Location = new System.Drawing.Point(12, 25);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(524, 537);
			this.groupBox4.TabIndex = 8;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "BG A";
			// 
			// bmpViewNTA
			// 
			this.bmpViewNTA.Location = new System.Drawing.Point(6, 19);
			this.bmpViewNTA.Name = "bmpViewNTA";
			this.bmpViewNTA.Size = new System.Drawing.Size(75, 23);
			this.bmpViewNTA.TabIndex = 1;
			this.bmpViewNTA.Text = "bmpView1";
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.bmpViewNTB);
			this.groupBox5.Location = new System.Drawing.Point(542, 25);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(524, 537);
			this.groupBox5.TabIndex = 9;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "BG B";
			// 
			// bmpViewNTB
			// 
			this.bmpViewNTB.Location = new System.Drawing.Point(6, 19);
			this.bmpViewNTB.Name = "bmpViewNTB";
			this.bmpViewNTB.Size = new System.Drawing.Size(75, 23);
			this.bmpViewNTB.TabIndex = 2;
			this.bmpViewNTB.Text = "bmpView1";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(743, 855);
			this.label1.Name = "label1";
			this.label1.Text = "CTRL+C copies the pane under the mouse pointer to the clipboard.";
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem});
			this.menuStrip1.TabIndex = 11;
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveBGAScreenshotToolStripMenuItem,
            this.saveBGBScreenshotToolStripMenuItem,
            this.saveTilesScreenshotToolStripMenuItem,
            this.saveWindowScreenshotToolStripMenuItem,
            this.savePaletteScreenshotToolStripMenuItem});
			this.fileToolStripMenuItem.Text = "&File";
			// 
			// saveBGAScreenshotToolStripMenuItem
			// 
			this.saveBGAScreenshotToolStripMenuItem.Text = "Save BG A Screenshot...";
			this.saveBGAScreenshotToolStripMenuItem.Click += new System.EventHandler(this.SaveBGAScreenshotToolStripMenuItem_Click);
			// 
			// saveBGBScreenshotToolStripMenuItem
			// 
			this.saveBGBScreenshotToolStripMenuItem.Text = "Save BG B Screenshot...";
			this.saveBGBScreenshotToolStripMenuItem.Click += new System.EventHandler(this.SaveBGBScreenshotToolStripMenuItem_Click);
			// 
			// saveTilesScreenshotToolStripMenuItem
			// 
			this.saveTilesScreenshotToolStripMenuItem.Text = "Save Tiles Screenshot...";
			this.saveTilesScreenshotToolStripMenuItem.Click += new System.EventHandler(this.SaveTilesScreenshotToolStripMenuItem_Click);
			// 
			// saveWindowScreenshotToolStripMenuItem
			// 
			this.saveWindowScreenshotToolStripMenuItem.Text = "Save Window Screenshot...";
			this.saveWindowScreenshotToolStripMenuItem.Click += new System.EventHandler(this.SaveWindowScreenshotToolStripMenuItem_Click);
			// 
			// savePaletteScreenshotToolStripMenuItem
			// 
			this.savePaletteScreenshotToolStripMenuItem.Text = "Save Palette Screenshot...";
			this.savePaletteScreenshotToolStripMenuItem.Click += new System.EventHandler(this.SavePaletteScreenshotToolStripMenuItem_Click);
			// 
			// GenVDPViewer
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1078, 954);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.menuStrip1);
			this.KeyPreview = true;
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "GenVdpViewer";
			this.ShowIcon = false;
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.VDPViewer_KeyDown);
			this.groupBox1.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private BmpView bmpViewTiles;
		private BmpView bmpViewNTA;
		private BmpView bmpViewNTB;
		private BmpView bmpViewNTW;
		private BmpView bmpViewPal;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
		private BizHawk.WinForms.Controls.LocLabelEx label1;
		private MenuStripEx menuStrip1;
		private BizHawk.WinForms.Controls.ToolStripMenuItemEx fileToolStripMenuItem;
		private BizHawk.WinForms.Controls.ToolStripMenuItemEx saveBGAScreenshotToolStripMenuItem;
		private BizHawk.WinForms.Controls.ToolStripMenuItemEx saveBGBScreenshotToolStripMenuItem;
		private BizHawk.WinForms.Controls.ToolStripMenuItemEx saveTilesScreenshotToolStripMenuItem;
		private BizHawk.WinForms.Controls.ToolStripMenuItemEx saveWindowScreenshotToolStripMenuItem;
		private BizHawk.WinForms.Controls.ToolStripMenuItemEx savePaletteScreenshotToolStripMenuItem;
	}
}