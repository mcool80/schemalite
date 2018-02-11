using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SchemaLite;

namespace SchemaLiteUI
{
	/// <summary>
	/// Summary description for SettingsUI.
	/// </summary>
	public class SettingsUI : System.Windows.Forms.Form
	{
		private Settings settings_;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.TextBox textBox3;
		private System.Windows.Forms.TextBox textBox4;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.Label label5;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		/// <summary>
		/// Create the SettingsUI
		/// </summary>
		/// <param name="settings">The settings given</param>
		public SettingsUI(Settings settings)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			settings_ = settings;
			this.textBox1.Text = settings_.CompanyName;
			this.textBox2.Text = settings_.EditStopDays.ToString();
			if ( Scheme.Instance.login("admin", "" ) == ErrorHandler.NO_ERROR )
				usepassword(false);
			else
				usepassword(true);
			this.label9.Text = Scheme.Instance.DatabaseFile;
		}

		/// <summary>
		/// Set textbox available or not wheater or not passwords is to be used
		/// </summary>
		/// <param name="usepassword">True if password is to be used</param>
		private void usepassword(bool usepassword)
		{
			if ( !usepassword )
			{
				textBox3.Text = "";
				textBox4.Text = "";
				textBox3.Enabled = false;
				textBox4.Enabled = false;
				checkBox1.Checked = true;
			}
			else
			{
				textBox3.Enabled = true;
				textBox4.Enabled = true;
				checkBox1.Checked = false;
			}
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
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.textBox3 = new System.Windows.Forms.TextBox();
			this.textBox4 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.button3 = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.label5 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(96, 48);
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(152, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 23);
			this.label1.TabIndex = 1;
			this.label1.Text = "Företagsnamn";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 84);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(152, 23);
			this.label2.TabIndex = 2;
			this.label2.Text = "Planeringsstopp";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(96, 80);
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(104, 20);
			this.textBox2.TabIndex = 3;
			this.textBox2.Text = "";
			this.textBox2.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textBox2_KeyUp);
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Arial", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(0, 8);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(296, 23);
			this.label3.TabIndex = 4;
			this.label3.Text = "Schemainställningar";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(224, 288);
			this.button1.Name = "button1";
			this.button1.TabIndex = 5;
			this.button1.Text = "Spara";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.button2.Location = new System.Drawing.Point(304, 288);
			this.button2.Name = "button2";
			this.button2.TabIndex = 6;
			this.button2.Text = "Avbryt";
			// 
			// textBox3
			// 
			this.textBox3.Location = new System.Drawing.Point(128, 144);
			this.textBox3.Name = "textBox3";
			this.textBox3.PasswordChar = '*';
			this.textBox3.Size = new System.Drawing.Size(120, 20);
			this.textBox3.TabIndex = 7;
			this.textBox3.Text = "";
			// 
			// textBox4
			// 
			this.textBox4.Location = new System.Drawing.Point(128, 168);
			this.textBox4.Name = "textBox4";
			this.textBox4.PasswordChar = '*';
			this.textBox4.Size = new System.Drawing.Size(120, 20);
			this.textBox4.TabIndex = 8;
			this.textBox4.Text = "";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label4.Location = new System.Drawing.Point(8, 128);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(168, 23);
			this.label4.TabIndex = 9;
			this.label4.Text = "Byt lösenord på admin";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(32, 172);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(104, 23);
			this.label6.TabIndex = 11;
			this.label6.Text = "Nytt lösenord";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(200, 84);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(152, 23);
			this.label7.TabIndex = 12;
			this.label7.Text = "dagar";
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(128, 192);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(136, 24);
			this.checkBox1.TabIndex = 13;
			this.checkBox1.Text = "Använd inte lösenord";
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label8.Location = new System.Drawing.Point(8, 208);
			this.label8.Name = "label8";
			this.label8.TabIndex = 14;
			this.label8.Text = "Databasfil:";
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 7F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label9.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.label9.Location = new System.Drawing.Point(16, 232);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(280, 48);
			this.label9.TabIndex = 15;
			this.label9.Text = "label9";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(304, 240);
			this.button3.Name = "button3";
			this.button3.TabIndex = 16;
			this.button3.Text = "Välj fil...";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.Filter = "Databasfil|*.mdb";
			this.openFileDialog1.ReadOnlyChecked = true;
			this.openFileDialog1.Title = "Välj databasfil";
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(32, 148);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(104, 23);
			this.label5.TabIndex = 17;
			this.label5.Text = "Gammalt lösenord";
			// 
			// SettingsUI
			// 
			this.AcceptButton = this.button1;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.button2;
			this.ClientSize = new System.Drawing.Size(386, 319);
			this.ControlBox = false;
			this.Controls.Add(this.textBox4);
			this.Controls.Add(this.textBox3);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Name = "SettingsUI";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Inställningar";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Save button pressed, save data given i form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, System.EventArgs e)
		{
			settings_.CompanyName = this.textBox1.Text;
			if ( this.textBox2.Text == "" )
				this.textBox2.Text = "0";
			settings_.EditStopDays = int.Parse(this.textBox2.Text);
			Scheme.Instance.setSettings(settings_);

			// Change password if the old or the new password is entered 
			if ( textBox3.Text != "" || textBox4.Text != "" )
			{
				// Test that the old password is correct 
				if ( Scheme.Instance.login("admin", textBox3.Text ) != ErrorHandler.NO_ERROR )
				{
					ErrorHandler.ShowError(ErrorHandler.ERR_WRONG_PASS_ON_CHANGE);
				}
				else
				{
					// Change password 
					Person person = new Person("admin", "", textBox4.Text, true);
					Scheme.Instance.setEntity(person, false);
				}
			}
			if ( checkBox1.Checked == true )
			{
				Person person = new Person("admin", "", "", true);
				Scheme.Instance.setEntity(person, false);
			}

			if ( label9.Text != Scheme.Instance.DatabaseFile )
			{
				Scheme.Instance.DatabaseFile = label9.Text;
				using ( System.IO.StreamWriter sw = new System.IO.StreamWriter(Application.StartupPath+"\\schemalite.ini") )
				{
					sw.WriteLine("database="+label9.Text);
				}
			}
			this.Close();
		}

		/// <summary>
		/// Check that only numbers are enterd
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void textBox2_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			String newtext = "";
			for (int i=0; i < textBox2.Text.Length; i++)
			{
				if ( textBox2.Text[i] >= 48 && textBox2.Text[i] <= 57 )
				{
					newtext += textBox2.Text[i];
				}
			}
			textBox2.Text = newtext;
		}

		/// <summary>
		/// If checkbox change, check if the user wants password or not
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
			usepassword(!checkBox1.Checked);
		}

		private void button3_Click(object sender, System.EventArgs e)
		{
			if ( DialogResult.OK == openFileDialog1.ShowDialog() )
			{
				label9.Text = openFileDialog1.FileName;
			}
		}

		/// <summary>
		/// Get settings
		/// </summary>
		public Settings Settings
		{
			get
			{
				return  settings_;
			}
		}
	}
}
