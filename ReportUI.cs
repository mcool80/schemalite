using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Drawing.Printing;
using System.IO;
using SchemaLite;

namespace SchemaLiteUI
{
	/// <summary>
	/// Summary description for ReportUI.
	/// </summary>
	public class ReportUI : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textBox1;
		private System.Windows.Forms.TextBox textBox2;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.RichTextBox richTextBox1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button button3;
		private System.Windows.Forms.Button button4;
		private System.Windows.Forms.PrintDialog printDialog1;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public int linesPrinted = 0;
		public string[] lines;
		public ReportUI(DateTime starttime, DateTime endtime)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			textBox1.Text = starttime.Year+"-"+((starttime.Month<10)?"0":"")+starttime.Month+"-"+((starttime.Day<10)?"0":"")+starttime.Day;
			textBox2.Text = endtime.Year+"-"+((endtime.Month<10)?"0":"")+endtime.Month+"-"+((endtime.Day<10)?"0":"")+endtime.Day;
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
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(ReportUI));
			this.panel1 = new System.Windows.Forms.Panel();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.textBox2 = new System.Windows.Forms.TextBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.richTextBox1 = new System.Windows.Forms.RichTextBox();
			this.button2 = new System.Windows.Forms.Button();
			this.button3 = new System.Windows.Forms.Button();
			this.button4 = new System.Windows.Forms.Button();
			this.printDialog1 = new System.Windows.Forms.PrintDialog();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.button1);
			this.panel1.Controls.Add(this.textBox2);
			this.panel1.Controls.Add(this.textBox1);
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(512, 64);
			this.panel1.TabIndex = 0;
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(360, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(55, 16);
			this.label2.TabIndex = 4;
			this.label2.Text = "Slutdatum";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(288, 8);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(60, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "Startdatum";
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(432, 24);
			this.button1.Name = "button1";
			this.button1.TabIndex = 2;
			this.button1.Text = "Kontrollera";
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// textBox2
			// 
			this.textBox2.Location = new System.Drawing.Point(360, 24);
			this.textBox2.MaxLength = 10;
			this.textBox2.Name = "textBox2";
			this.textBox2.Size = new System.Drawing.Size(64, 20);
			this.textBox2.TabIndex = 1;
			this.textBox2.Text = "";
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(288, 24);
			this.textBox1.MaxLength = 10;
			this.textBox1.Name = "textBox1";
			this.textBox1.Size = new System.Drawing.Size(64, 20);
			this.textBox1.TabIndex = 0;
			this.textBox1.Text = "";
			// 
			// richTextBox1
			// 
			this.richTextBox1.Font = new System.Drawing.Font("Courier New", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.richTextBox1.Location = new System.Drawing.Point(0, 64);
			this.richTextBox1.Name = "richTextBox1";
			this.richTextBox1.ReadOnly = true;
			this.richTextBox1.Size = new System.Drawing.Size(512, 368);
			this.richTextBox1.TabIndex = 1;
			this.richTextBox1.Text = "";
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(272, 440);
			this.button2.Name = "button2";
			this.button2.TabIndex = 2;
			this.button2.Text = "Spara";
			this.button2.Visible = false;
			// 
			// button3
			// 
			this.button3.Location = new System.Drawing.Point(352, 440);
			this.button3.Name = "button3";
			this.button3.TabIndex = 3;
			this.button3.Text = "Skriv ut";
			this.button3.Click += new System.EventHandler(this.button3_Click);
			// 
			// button4
			// 
			this.button4.Location = new System.Drawing.Point(432, 440);
			this.button4.Name = "button4";
			this.button4.TabIndex = 4;
			this.button4.Text = "Stäng";
			this.button4.Click += new System.EventHandler(this.button4_Click);
			// 
			// ReportUI
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(512, 469);
			this.Controls.Add(this.button4);
			this.Controls.Add(this.button3);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.richTextBox1);
			this.Controls.Add(this.panel1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "ReportUI";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Rapport";
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// Run the report
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, System.EventArgs e)
		{
			DateTime starttime;
			DateTime endtime;
			// Check that the dates are correct
			try
			{
				starttime = DateTime.Parse(textBox1.Text);
			}
			catch
			{
				ErrorHandler.ShowInformation(ErrorHandler.ERR_DATE_WRONG);
				this.textBox1.Focus();
				this.textBox1.SelectAll();
				return;
			}
			try
			{
				endtime = DateTime.Parse(textBox2.Text).AddDays(1);
			}
			catch
			{
				ErrorHandler.ShowInformation(ErrorHandler.ERR_DATE_WRONG);
				this.textBox2.Focus();
				this.textBox2.SelectAll();
				return;
			}

			// Run report
			Report rep;
			if ( ErrorHandler.ShowError(Scheme.Instance.runReport(starttime, endtime, out rep)) )
				return;
			SortedList replist = rep.getReportLines();
			richTextBox1.Text = "Rapport\nStartdatum: "+this.textBox1.Text+"\nSlutdatum: "+this.textBox2.Text+"\n\n";
			richTextBox1.Text += "Person/Resurs                 Timmar     Avvikelse\n\n";
			// Write all reportlines on to the textbox
			for ( int i=0; i < replist.Count; i++ )
			{
				ReportLine repline = (ReportLine)replist[replist.GetKey(i)];
				richTextBox1.Text += repline.Name;
				for(int o=0; o<30-repline.Name.Length;o++)
					richTextBox1.Text += " ";
				richTextBox1.Text += repline.Hours;
				for(int o=0; o<11-repline.Hours.ToString().Length;o++)
					richTextBox1.Text += " ";
				richTextBox1.Text += repline.Deviation+"\n";
			}
			if ( replist.Count == 0 )
			{
				richTextBox1.Text += "Inga timmar är registrerade";
			}

		}

		/// <summary>
		/// Print the report
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button3_Click(object sender, System.EventArgs e)
		{
			string currdir = Directory.GetCurrentDirectory();
			PrintDocument pd = new PrintDocument();
			printDialog1.Document = pd;
			DialogResult dr = printDialog1.ShowDialog();
			// Print if user choose to
			if ( dr == System.Windows.Forms.DialogResult.OK )
			{
				pd.PrintPage +=new PrintPageEventHandler(pd_PrintPage);
				pd.DocumentName = "Tidrapport";
				linesPrinted = 0;
				OnBeginPrint();
				pd.Print();
			}
			Directory.SetCurrentDirectory(currdir);
		}

		/// <summary>
		/// Prepare to print
		/// </summary>
		private void OnBeginPrint()
		{
			// Add all text into a variable with lines
			char[] param = {'\n'};

			if (printDialog1.PrinterSettings.PrintRange == PrintRange.Selection)
			{
				lines = richTextBox1.SelectedText.Split(param);
			}
			else
			{
				lines = richTextBox1.Text.Split(param);
			}
            
			int i = 0;
			char[] trimParam = {'\r'};
			foreach (string s in lines)
			{
				lines[i++] = s.TrimEnd(trimParam);
			}
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void pd_PrintPage(object sender, PrintPageEventArgs e)
		{
			int x = e.MarginBounds.Left;
			int y = e.MarginBounds.Top;
			Brush brush = new SolidBrush(richTextBox1.ForeColor);
            
			// Add all text to the graphics to be printed
			while (linesPrinted < lines.Length)
			{
				e.Graphics.DrawString (lines[linesPrinted++],
					richTextBox1.Font, brush, x, y);
				y += 15;
				if (y >= e.MarginBounds.Bottom)
				{
					e.HasMorePages = true;
					return;
				}
				else
				{
					e.HasMorePages = false;
				}
			}
		}

		/// <summary>
		/// Close the report form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button4_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
