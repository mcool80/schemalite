using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace SchemaLiteUI
{
	/// <summary>
	/// Summary description for About.
	/// </summary>
	public class About : System.Windows.Forms.Form
	{
		private System.Windows.Forms.PictureBox pictureBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Create the About box
		/// </summary>
		/// <param name="registered">True if the program is registred</param>
		public About(bool registered)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			label3.Text = "";
			if ( !registered )
				label3.Text = "Programmet är inte registrerat!";
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(About));
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// pictureBox1
			// 
			this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
			this.pictureBox1.Location = new System.Drawing.Point(8, 16);
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.Size = new System.Drawing.Size(150, 122);
			this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
			this.pictureBox1.TabIndex = 0;
			this.pictureBox1.TabStop = false;
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label1.Location = new System.Drawing.Point(176, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(160, 32);
			this.label1.TabIndex = 1;
			this.label1.Text = "SchemaLite Version 1.03";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Verdana", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(176, 75);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(148, 17);
			this.label2.TabIndex = 2;
			this.label2.Text = "Copyright CelIT,  2006";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(216, 111);
			this.button1.Name = "button1";
			this.button1.TabIndex = 4;
			this.button1.Text = "Ok";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(16, 144);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(328, 23);
			this.label3.TabIndex = 5;
			this.label3.Text = "label3";
			// 
			// About
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(354, 167);
			this.ControlBox = false;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.pictureBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "About";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Om";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Close the About box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

	}
}
