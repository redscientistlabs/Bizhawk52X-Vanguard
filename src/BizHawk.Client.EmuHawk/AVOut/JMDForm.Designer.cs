﻿namespace BizHawk.Client.EmuHawk
{
	partial class JmdForm
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
            this.okButton = new System.Windows.Forms.Button();
            this.threadsBar = new System.Windows.Forms.TrackBar();
            this.compressionBar = new System.Windows.Forms.TrackBar();
            this.threadLeft = new BizHawk.WinForms.Controls.LocLabelEx();
            this.threadRight = new BizHawk.WinForms.Controls.LocLabelEx();
            this.compressionLeft = new BizHawk.WinForms.Controls.LocLabelEx();
            this.compressionRight = new BizHawk.WinForms.Controls.LocLabelEx();
            this.threadTop = new BizHawk.WinForms.Controls.LocLabelEx();
            this.compressionTop = new BizHawk.WinForms.Controls.LocLabelEx();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.threadsBar)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.compressionBar)).BeginInit();
            this.SuspendLayout();
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(115, 147);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(70, 23);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // threadsBar
            // 
            this.threadsBar.Location = new System.Drawing.Point(56, 25);
            this.threadsBar.Name = "threadsBar";
            this.threadsBar.Size = new System.Drawing.Size(104, 45);
            this.threadsBar.TabIndex = 5;
            this.threadsBar.Scroll += new System.EventHandler(this.ThreadsBar_Scroll);
            // 
            // compressionBar
            // 
            this.compressionBar.Location = new System.Drawing.Point(56, 86);
            this.compressionBar.Name = "compressionBar";
            this.compressionBar.Size = new System.Drawing.Size(104, 45);
            this.compressionBar.TabIndex = 9;
            this.compressionBar.Scroll += new System.EventHandler(this.CompressionBar_Scroll);
            // 
            // threadLeft
            // 
            this.threadLeft.Location = new System.Drawing.Point(15, 25);
            this.threadLeft.Name = "threadLeft";
            this.threadLeft.Text = "label1";
            // 
            // threadRight
            // 
            this.threadRight.Location = new System.Drawing.Point(166, 25);
            this.threadRight.Name = "threadRight";
            this.threadRight.Text = "label2";
            // 
            // compressionLeft
            // 
            this.compressionLeft.Location = new System.Drawing.Point(15, 96);
            this.compressionLeft.Name = "compressionLeft";
            this.compressionLeft.Text = "label3";
            // 
            // compressionRight
            // 
            this.compressionRight.Location = new System.Drawing.Point(166, 96);
            this.compressionRight.Name = "compressionRight";
            this.compressionRight.Text = "label4";
            // 
            // threadTop
            // 
            this.threadTop.Location = new System.Drawing.Point(62, 9);
            this.threadTop.Name = "threadTop";
            this.threadTop.Text = "Number of Threads";
            this.threadTop.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // compressionTop
            // 
            this.compressionTop.Location = new System.Drawing.Point(64, 70);
            this.compressionTop.Name = "compressionTop";
            this.compressionTop.Text = "Compression Level";
            this.compressionTop.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(191, 147);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(70, 23);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // JMDForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(273, 182);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.compressionTop);
            this.Controls.Add(this.threadTop);
            this.Controls.Add(this.compressionRight);
            this.Controls.Add(this.compressionLeft);
            this.Controls.Add(this.threadRight);
            this.Controls.Add(this.threadLeft);
            this.Controls.Add(this.compressionBar);
            this.Controls.Add(this.threadsBar);
            this.Controls.Add(this.okButton);
            this.Name = "JmdForm";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "JMD Compression Options";
            ((System.ComponentModel.ISupportInitialize)(this.threadsBar)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.compressionBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button okButton;
		private System.Windows.Forms.TrackBar threadsBar;
		private System.Windows.Forms.TrackBar compressionBar;
		private BizHawk.WinForms.Controls.LocLabelEx threadLeft;
		private BizHawk.WinForms.Controls.LocLabelEx threadRight;
		private BizHawk.WinForms.Controls.LocLabelEx compressionLeft;
		private BizHawk.WinForms.Controls.LocLabelEx compressionRight;
		private BizHawk.WinForms.Controls.LocLabelEx threadTop;
		private BizHawk.WinForms.Controls.LocLabelEx compressionTop;
		private System.Windows.Forms.Button cancelButton;
	}
}