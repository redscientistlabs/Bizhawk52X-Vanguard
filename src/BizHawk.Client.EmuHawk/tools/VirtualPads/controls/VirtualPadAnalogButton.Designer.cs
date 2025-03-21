﻿namespace BizHawk.Client.EmuHawk
{
	partial class VirtualPadAnalogButton
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
            this.AnalogTrackBar = new System.Windows.Forms.TrackBar();
            this.DisplayNameLabel = new BizHawk.WinForms.Controls.LocLabelEx();
            this.ValueLabel = new BizHawk.WinForms.Controls.LocLabelEx();
            ((System.ComponentModel.ISupportInitialize)(this.AnalogTrackBar)).BeginInit();
            this.SuspendLayout();
            // 
            // AnalogTrackBar
            // 
            this.AnalogTrackBar.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AnalogTrackBar.Location = new System.Drawing.Point(3, 3);
            this.AnalogTrackBar.Name = "AnalogTrackBar";
            this.AnalogTrackBar.Size = new System.Drawing.Size(291, 45);
            this.AnalogTrackBar.TabIndex = 0;
            this.AnalogTrackBar.ValueChanged += new System.EventHandler(this.AnalogTrackBar_ValueChanged);
            // 
            // DisplayNameLabel
            // 
            this.DisplayNameLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.DisplayNameLabel.Location = new System.Drawing.Point(6, 51);
            this.DisplayNameLabel.Name = "DisplayNameLabel";
            this.DisplayNameLabel.Text = "Slider";
            // 
            // ValueLabel
            // 
            this.ValueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ValueLabel.Location = new System.Drawing.Point(257, 51);
            this.ValueLabel.Name = "ValueLabel";
            this.ValueLabel.Text = "99999";
            // 
            // VirtualPadAnalogButton
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ValueLabel);
            this.Controls.Add(this.DisplayNameLabel);
            this.Controls.Add(this.AnalogTrackBar);
            this.Name = "VirtualPadAnalogButton";
            this.Size = new System.Drawing.Size(297, 74);
            ((System.ComponentModel.ISupportInitialize)(this.AnalogTrackBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TrackBar AnalogTrackBar;
		private BizHawk.WinForms.Controls.LocLabelEx DisplayNameLabel;
		private BizHawk.WinForms.Controls.LocLabelEx ValueLabel;
	}
}
