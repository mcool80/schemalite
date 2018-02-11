using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SchemaLite;
namespace SchemaLiteUI
{
	/// <summary>
	/// Summary description for Register.
	/// </summary>
	public class Register : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox code;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Register()
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
			this.code = new System.Windows.Forms.TextBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// code
			// 
			this.code.Location = new System.Drawing.Point(96, 24);
			this.code.Name = "code";
			this.code.Size = new System.Drawing.Size(152, 20);
			this.code.TabIndex = 0;
			this.code.Text = "";
			// 
			// button1
			// 
			this.button1.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button1.Location = new System.Drawing.Point(176, 80);
			this.button1.Name = "button1";
			this.button1.TabIndex = 1;
			this.button1.Text = "Registrera";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(264, 80);
			this.button2.Name = "button2";
			this.button2.TabIndex = 2;
			this.button2.Text = "Ångra";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 24);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 23);
			this.label1.TabIndex = 3;
			this.label1.Text = "Serienummer:";
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(24, 48);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(256, 24);
			this.label2.TabIndex = 4;
			this.label2.Text = "Skriv in det serienummer, du fick när du köpte produkten, för att kunna använda f" +
				"ullversionen.";
			// 
			// Register
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button2;
			this.ClientSize = new System.Drawing.Size(352, 109);
			this.ControlBox = false;
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.code);
			this.Name = "Register";
			this.Text = "Registrera SchemaLite";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Check if the code is ok, if not an error is shown
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, System.EventArgs e)
		{
			if ( !RegisterLicense.isLicensKeyOK(code.Text) )
			{
				ErrorHandler.ShowError(ErrorHandler.ERR_WRONG_REGISTRATION_CODE);
			}
			else
			{
				Close();
			} 
		}

		/// <summary>
		/// Close the form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button2_Click(object sender, System.EventArgs e)
		{
			code.Text = "";
			this.Close();
		}
	
		/// <summary>
		/// Get the code given by the user
		/// </summary>
		public String Code
		{
			get
			{
				return this.code.Text;
			}
		}

	}
}
