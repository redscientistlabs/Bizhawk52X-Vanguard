﻿namespace BizHawk.Client.EmuHawk
{
	partial class AnalogBindControl
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.labelButtonName = new BizHawk.WinForms.Controls.LocLabelEx();
			this.trackBarSensitivity = new System.Windows.Forms.TrackBar();
			this.labelSensitivity = new BizHawk.WinForms.Controls.LocLabelEx();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.buttonBind = new System.Windows.Forms.Button();
			this.trackBarDeadzone = new System.Windows.Forms.TrackBar();
			this.labelDeadzone = new BizHawk.WinForms.Controls.LocLabelEx();
			this.buttonFlip = new System.Windows.Forms.Button();
			this.buttonUnbind = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.trackBarSensitivity)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarDeadzone)).BeginInit();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(3, 3);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(100, 20);
			this.textBox1.TabIndex = 0;
			// 
			// labelButtonName
			// 
			this.labelButtonName.Location = new System.Drawing.Point(109, 6);
			this.labelButtonName.Name = "labelButtonName";
			this.labelButtonName.Text = "Bindname";
			// 
			// trackBarSensitivity
			// 
			this.trackBarSensitivity.LargeChange = 4;
			this.trackBarSensitivity.Location = new System.Drawing.Point(267, 21);
			this.trackBarSensitivity.Maximum = 40;
			this.trackBarSensitivity.Minimum = -40;
			this.trackBarSensitivity.Name = "trackBarSensitivity";
			this.trackBarSensitivity.Size = new System.Drawing.Size(160, 45);
			this.trackBarSensitivity.SmallChange = 1;
			this.trackBarSensitivity.TabIndex = 2;
			this.trackBarSensitivity.TickFrequency = 10;
			this.trackBarSensitivity.ValueChanged += new System.EventHandler(this.TrackBarSensitivity_ValueChanged);
			// 
			// labelSensitivity
			// 
			this.labelSensitivity.Location = new System.Drawing.Point(166, 25);
			this.labelSensitivity.Name = "labelSensitivity";
			this.labelSensitivity.Text = "Sensitivity: 5 billion";
			// 
			// timer1
			// 
			this.timer1.Tick += new System.EventHandler(this.Timer1_Tick);
			// 
			// buttonBind
			// 
			this.buttonBind.Location = new System.Drawing.Point(3, 29);
			this.buttonBind.Name = "buttonBind";
			this.buttonBind.Size = new System.Drawing.Size(75, 23);
			this.buttonBind.TabIndex = 4;
			this.buttonBind.Text = "Bind!";
			this.buttonBind.UseVisualStyleBackColor = true;
			this.buttonBind.Click += new System.EventHandler(this.ButtonBind_Click);
			// 
			// trackBarDeadzone
			// 
			this.trackBarDeadzone.LargeChange = 5;
			this.trackBarDeadzone.Location = new System.Drawing.Point(267, 51);
			this.trackBarDeadzone.Maximum = 25;
			this.trackBarDeadzone.Minimum = 0;
			this.trackBarDeadzone.Name = "trackBarDeadzone";
			this.trackBarDeadzone.Size = new System.Drawing.Size(160, 45);
			this.trackBarDeadzone.SmallChange = 1;
			this.trackBarDeadzone.TabIndex = 5;
			this.trackBarDeadzone.TickFrequency = 5;
			this.trackBarDeadzone.ValueChanged += new System.EventHandler(this.TrackBarDeadzone_ValueChanged);
			// 
			// labelDeadzone
			// 
			this.labelDeadzone.Location = new System.Drawing.Point(166, 60);
			this.labelDeadzone.Name = "labelDeadzone";
			this.labelDeadzone.Text = "Deadzone: 5 billion";
			// 
			// buttonFlip
			// 
			this.buttonFlip.Location = new System.Drawing.Point(88, 29);
			this.buttonFlip.Name = "buttonFlip";
			this.buttonFlip.Size = new System.Drawing.Size(75, 23);
			this.buttonFlip.TabIndex = 7;
			this.buttonFlip.Text = "Flip Axis";
			this.buttonFlip.UseVisualStyleBackColor = true;
			this.buttonFlip.Click += new System.EventHandler(this.ButtonFlip_Click);
			// 
			// buttonUnbind
			// 
			this.buttonUnbind.Location = new System.Drawing.Point(3, 58);
			this.buttonUnbind.Name = "buttonUnbind";
			this.buttonUnbind.Size = new System.Drawing.Size(75, 23);
			this.buttonUnbind.TabIndex = 8;
			this.buttonUnbind.Text = "Unbind!";
			this.buttonUnbind.UseVisualStyleBackColor = true;
			this.buttonUnbind.Click += new System.EventHandler(this.Unbind_Click);
			// 
			// AnalogBindControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.buttonUnbind);
			this.Controls.Add(this.buttonFlip);
			this.Controls.Add(this.labelDeadzone);
			this.Controls.Add(this.trackBarDeadzone);
			this.Controls.Add(this.buttonBind);
			this.Controls.Add(this.labelSensitivity);
			this.Controls.Add(this.trackBarSensitivity);
			this.Controls.Add(this.labelButtonName);
			this.Controls.Add(this.textBox1);
			this.Name = "AnalogBindControl";
			this.Size = new System.Drawing.Size(434, 99);
			((System.ComponentModel.ISupportInitialize)(this.trackBarSensitivity)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.trackBarDeadzone)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox1;
		private BizHawk.WinForms.Controls.LocLabelEx labelButtonName;
		private System.Windows.Forms.TrackBar trackBarSensitivity;
		private BizHawk.WinForms.Controls.LocLabelEx labelSensitivity;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.Button buttonBind;
		private System.Windows.Forms.TrackBar trackBarDeadzone;
		private BizHawk.WinForms.Controls.LocLabelEx labelDeadzone;
		private System.Windows.Forms.Button buttonFlip;
		private System.Windows.Forms.Button buttonUnbind;
	}
}
