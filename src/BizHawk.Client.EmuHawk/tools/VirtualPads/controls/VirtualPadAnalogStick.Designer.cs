﻿namespace BizHawk.Client.EmuHawk
{
	partial class VirtualPadAnalogStick
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
			this.XLabel = new BizHawk.WinForms.Controls.LocLabelEx();
			this.ManualX = new System.Windows.Forms.NumericUpDown();
			this.YLabel = new BizHawk.WinForms.Controls.LocLabelEx();
			this.ManualY = new System.Windows.Forms.NumericUpDown();
			this.MaxLabel = new BizHawk.WinForms.Controls.LocLabelEx();
			this.MaxXNumeric = new System.Windows.Forms.NumericUpDown();
			this.MaxYNumeric = new System.Windows.Forms.NumericUpDown();
			this.rLabel = new BizHawk.WinForms.Controls.LocLabelEx();
			this.manualR = new System.Windows.Forms.NumericUpDown();
			this.manualTheta = new System.Windows.Forms.NumericUpDown();
			this.thetaLabel = new BizHawk.WinForms.Controls.LocLabelEx();
			this.AnalogStick = new BizHawk.Client.EmuHawk.AnalogStickPanel();
			((System.ComponentModel.ISupportInitialize)(this.ManualX)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.ManualY)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MaxXNumeric)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.MaxYNumeric)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.manualR)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.manualTheta)).BeginInit();
			this.SuspendLayout();
			// 
			// XLabel
			// 
			this.XLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.XLabel.Location = new System.Drawing.Point(187, 7);
			this.XLabel.Name = "XLabel";
			this.XLabel.Text = "X";
			// 
			// ManualX
			// 
			this.ManualX.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ManualX.Location = new System.Drawing.Point(205, 3);
			this.ManualX.Name = "ManualX";
			this.ManualX.Size = new System.Drawing.Size(44, 20);
			this.ManualX.TabIndex = 24;
			this.ManualX.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ManualXY_ValueChanged);
			// 
			// YLabel
			// 
			this.YLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.YLabel.Location = new System.Drawing.Point(187, 33);
			this.YLabel.Name = "YLabel";
			this.YLabel.Text = "Y";
			// 
			// ManualY
			// 
			this.ManualY.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.ManualY.Location = new System.Drawing.Point(205, 29);
			this.ManualY.Name = "ManualY";
			this.ManualY.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.ManualY.Size = new System.Drawing.Size(44, 20);
			this.ManualY.TabIndex = 25;
			this.ManualY.KeyUp += new System.Windows.Forms.KeyEventHandler(this.ManualXY_ValueChanged);
			// 
			// MaxLabel
			// 
			this.MaxLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MaxLabel.Location = new System.Drawing.Point(205, 107);
			this.MaxLabel.Name = "MaxLabel";
			this.MaxLabel.Text = "Range%";
			// 
			// MaxXNumeric
			// 
			this.MaxXNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MaxXNumeric.Location = new System.Drawing.Point(205, 124);
			this.MaxXNumeric.Name = "MaxXNumeric";
			this.MaxXNumeric.Size = new System.Drawing.Size(44, 20);
			this.MaxXNumeric.TabIndex = 28;
			this.MaxXNumeric.ValueChanged += new System.EventHandler(this.MaxManualXY_ValueChanged);
			this.MaxXNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MaxManualXY_ValueChanged);
			// 
			// MaxYNumeric
			// 
			this.MaxYNumeric.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.MaxYNumeric.Location = new System.Drawing.Point(205, 147);
			this.MaxYNumeric.Name = "MaxYNumeric";
			this.MaxYNumeric.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.MaxYNumeric.Size = new System.Drawing.Size(44, 20);
			this.MaxYNumeric.TabIndex = 29;
			this.MaxYNumeric.ValueChanged += new System.EventHandler(this.MaxManualXY_ValueChanged);
			this.MaxYNumeric.KeyUp += new System.Windows.Forms.KeyEventHandler(this.MaxManualXY_ValueChanged);
			// 
			// rLabel
			// 
			this.rLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.rLabel.Location = new System.Drawing.Point(167, 60);
			this.rLabel.Name = "rLabel";
			this.rLabel.Text = "Ray";
			// 
			// manualR
			// 
			this.manualR.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.manualR.Location = new System.Drawing.Point(193, 58);
			this.manualR.Maximum = 181.0M;
			this.manualR.Name = "manualR";
			this.manualR.Size = new System.Drawing.Size(56, 20);
			this.manualR.TabIndex = 31;
			// 
			// manualTheta
			// 
			this.manualTheta.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.manualTheta.Location = new System.Drawing.Point(193, 84);
			this.manualTheta.Maximum = 359.0M;
			this.manualTheta.Name = "manualTheta";
			this.manualTheta.Size = new System.Drawing.Size(56, 20);
			this.manualTheta.TabIndex = 33;
			// 
			// thetaLabel
			// 
			this.thetaLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.thetaLabel.Location = new System.Drawing.Point(167, 86);
			this.thetaLabel.Name = "thetaLabel";
			this.thetaLabel.Text = "θ (°)";
			// 
			// AnalogStick
			// 
			this.AnalogStick.BackColor = System.Drawing.Color.Gray;
			this.AnalogStick.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.AnalogStick.Location = new System.Drawing.Point(3, 3);
			this.AnalogStick.Name = "AnalogStick";
			this.AnalogStick.Size = new System.Drawing.Size(164, 164);
			this.AnalogStick.TabIndex = 0;
			this.AnalogStick.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AnalogStick_MouseDown);
			this.AnalogStick.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AnalogStick_MouseMove);
			// 
			// VirtualPadAnalogStick
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.manualTheta);
			this.Controls.Add(this.thetaLabel);
			this.Controls.Add(this.manualR);
			this.Controls.Add(this.rLabel);
			this.Controls.Add(this.MaxYNumeric);
			this.Controls.Add(this.MaxXNumeric);
			this.Controls.Add(this.MaxLabel);
			this.Controls.Add(this.YLabel);
			this.Controls.Add(this.ManualY);
			this.Controls.Add(this.ManualX);
			this.Controls.Add(this.XLabel);
			this.Controls.Add(this.AnalogStick);
			this.Name = "VirtualPadAnalogStick";
			this.Size = new System.Drawing.Size(253, 172);
			((System.ComponentModel.ISupportInitialize)(this.ManualX)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.ManualY)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MaxXNumeric)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.MaxYNumeric)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.manualR)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.manualTheta)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private AnalogStickPanel AnalogStick;
		private BizHawk.WinForms.Controls.LocLabelEx XLabel;
		private System.Windows.Forms.NumericUpDown ManualX;
		private BizHawk.WinForms.Controls.LocLabelEx YLabel;
		private System.Windows.Forms.NumericUpDown ManualY;
		private BizHawk.WinForms.Controls.LocLabelEx MaxLabel;
		private System.Windows.Forms.NumericUpDown MaxXNumeric;
		private System.Windows.Forms.NumericUpDown MaxYNumeric;
		private BizHawk.WinForms.Controls.LocLabelEx rLabel;
		private System.Windows.Forms.NumericUpDown manualR;
		private System.Windows.Forms.NumericUpDown manualTheta;
		private BizHawk.WinForms.Controls.LocLabelEx thetaLabel;
	}
}
