using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using SchemaLite;

namespace SchemaLiteUI
{
	/// <summary>
	/// Summary description for AddPersonResourceUI.
	/// </summary>
	public class AddPersonResourceUI : System.Windows.Forms.Form
	{
		private bool add = true;

		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.CheckBox checkBox1;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ColorDialog colorDialog1;
		private System.Windows.Forms.Button button3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public AddPersonResourceUI()
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(AddPersonResourceUI));
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.checkBox1 = new System.Windows.Forms.CheckBox();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.colorDialog1 = new System.Windows.Forms.ColorDialog();
			this.button3 = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(72, 8);
			this.textBox1.Name = "textBox1";
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(72, 40);
			this.textBox2.Name = "textBox2";
			this.textBox2.PasswordChar = '*';
			this.textBox2.TabIndex = 1;
			this.textBox2.Text = "";
			// 
			// checkBox1
			// 
			this.checkBox1.Location = new System.Drawing.Point(184, 8);
			this.checkBox1.Name = "checkBox1";
			this.checkBox1.Size = new System.Drawing.Size(160, 24);
			this.checkBox1.TabIndex = 3;
			this.checkBox1.Text = "Ej knuten till en person";
			this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(248, 112);
			this.button1.Name = "button1";
			this.button1.TabIndex = 4;
			this.button1.Text = "Stäng";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(168, 112);
			this.button2.Name = "button2";
			this.button2.TabIndex = 5;
			this.button2.Text = "button2";
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(8, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(64, 23);
			this.label1.TabIndex = 6;
			this.label1.Text = "Namn:";
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(8, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 23);
			this.label2.TabIndex = 7;
			this.label2.Text = "Färg:";
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(8, 40);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 23);
			this.label3.TabIndex = 8;
			this.label3.Text = "Lösenord:";
			// 
			// button3
			// 
			this.button3.BackColor = System.Drawing.Color.Blue;
			this.button3.ForeColor = System.Drawing.SystemColors.ControlText;
			this.button3.Location = new System.Drawing.Point(72, 72);
			this.button3.Name = "button3";
			this.button3.Size = new System.Drawing.Size(104, 23);
			this.button3.TabIndex = 9;
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// AddPersonResourceUI
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(328, 141);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.checkBox1);
			this.Controls.Add(this.textBox2);
			this.Controls.Add(this.textBox1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "AddPersonResourceUI";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "AddPersonResourceUI";
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Show colordialog
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button3_Click(object sender, System.EventArgs e)
		{
			DialogResult dr;
			dr = colorDialog1.ShowDialog(this);
			if ( dr == DialogResult.OK )
				button3.BackColor = colorDialog1.Color;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		/// <summary>
		/// Set the dialogbox to be use to add person or resource
		/// </summary>
		public void setAsAdd()
		{
			this.Text = "Lägg till resurs";
			this.button2.Text = "Lägg till";
			checkBox1.Enabled = true;
			textBox1.Enabled = true;
			add = true;
		}

		/// <summary>
		/// Set the dialogbox to be used to change person or resource
		/// </summary>
		/// <param name="name"></param>
		/// <param name="resource"></param>
		/// <param name="pass"></param>
		/// <param name="color"></param>
		public void setAsChange(String name, bool resource, String pass, String color)
		{
			textBox1.Text = name;
			textBox1.Enabled = false;
			checkBox1.Checked = resource;
			checkBox1.Enabled = false;
			textBox2.Text = pass;
			button3.BackColor = Color.FromArgb((Int32.Parse(color)));
			this.Text = "Uppdatera resurs";
			this.button2.Text = "Uppdatera";
			add = false;
		}

		/// <summary>
		/// Click add/update
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button2_Click(object sender, System.EventArgs e)
		{
			bool fel = false;
			// Resource 
			if ( checkBox1.Checked == true )
			{
				Resource resurs = new Resource(textBox1.Text, (button3.BackColor.ToArgb()).ToString());
				fel = ErrorHandler.ShowError(Scheme.Instance.setEntity(resurs,add));
			}
			else
			// Person 
			{
				Person person = new Person(textBox1.Text, (button3.BackColor.ToArgb()).ToString(), textBox2.Text, false);
				fel = ErrorHandler.ShowError(Scheme.Instance.setEntity(person, add));
			}
			if ( !fel )
				this.Close();
		}
		
		/// <summary>
		/// If the resource or not box is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
			if ( checkBox1.Checked == true )
			{
				textBox2.Enabled = false;
			}
			else
			{
				textBox2.Enabled = true;
			}
		}
	}
}
