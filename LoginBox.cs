using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SchemaLite;
using System.Diagnostics;

namespace SchemaLiteUI
{
	/// <summary>
	/// Summary description for LoginBox.
	/// </summary>
	public class LoginBox : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public LoginBox()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(LoginBox));
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(88, 32);
			this.textBox1.Name = "textBox1";
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(88, 64);
			this.textBox2.Name = "textBox2";
			this.textBox2.PasswordChar = '*';
			this.textBox2.TabIndex = 1;
			this.textBox2.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 23);
			this.label1.TabIndex = 2;
			this.label1.Text = "Namn:";
			// 
			// label2
			// 
			this.label2.BackColor = System.Drawing.SystemColors.Control;
			this.label2.Location = new System.Drawing.Point(16, 64);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 23);
			this.label2.TabIndex = 3;
			this.label2.Text = "Lösenord:";
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(120, 104);
			this.button1.Name = "button1";
			this.button1.TabIndex = 4;
			this.button1.Text = "Logga in";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(208, 104);
			this.button2.Name = "button2";
			this.button2.TabIndex = 5;
			this.button2.Text = "Avbryt";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// LoginBox
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button2;
			this.ClientSize = new System.Drawing.Size(288, 141);
			this.ControlBox = false;
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "LoginBox";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Logga in";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Check the login data
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, System.EventArgs e)
		{
			if ( !ErrorHandler.ShowError(Scheme.Instance.login(textBox1.Text, textBox2.Text)) )
				this.Close();
			else
			{
				textBox1.Text = "";
				textBox2.Text = "";
				textBox1.Focus();
			}
		}

		/// <summary>
		/// Exit the program
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button2_Click(object sender, System.EventArgs e)
		{
			Process.GetCurrentProcess().Kill();
		}

	}
}
