using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.IO;
using SchemaLite;

namespace SchemaLiteUI
{
	/// <summary>
	/// Head form for this program, with big scheme, small scheme and userchecklistbox.
	/// 
	/// Revision:
	/// ID			Date	Author				Text
	///	1.01.002	051208	Markus Svensson		Solves issue SL-6 and SL-1
	///	1.01.003	051208	Markus Svensson		Solves issue SL-9
	///	1.01.004	051209	Markus Svensson		Solves issue SL-3
	/// #1.01.007	060118	Markus Svensson		Solved issue SL-11
	/// </summary>
	public class Form1 : System.Windows.Forms.Form, Observer
	{
		/// <summary>
		/// True if this program is registred
		/// </summary>
		public static bool registered = false; 

		/// <summary>
		/// If true then the next week schemaui is shown
		/// </summary>
		private bool showNextWeek_ = false;

		/// <summary>
		/// ActivityUIs for current week
		/// </summary>
		private SortedList activityuis_ = new SortedList();
		
		private System.Windows.Forms.MonthCalendar monthCalendar1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.VScrollBar vScrollBar1;
		private CheckListUI checkedListBox;

		// Internal variables
		private System.ComponentModel.IContainer components;
		private System.Windows.Forms.Label Day1;
		private System.Windows.Forms.Label Day4;
		private System.Windows.Forms.Label Day3;
		private System.Windows.Forms.Label Day2;
		private System.Windows.Forms.Label Day5;
		private System.Windows.Forms.Label Day6;
		private System.Windows.Forms.Label Day7;
		private System.Windows.Forms.Button button1;
		private System.Windows.Forms.Button button2;
		private System.Windows.Forms.PrintDialog printDialog1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.MainMenu mainMenu1;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuItem3;
		private System.Windows.Forms.MenuItem menuItem4;
		private System.Windows.Forms.MenuItem menuItem7;
		private System.Windows.Forms.MenuItem menuItem8;
		private System.Windows.Forms.MenuItem menuItem9;
		private System.Windows.Forms.MenuItem menuItem10;
		private System.Windows.Forms.MenuItem menuItem11;
		private System.Windows.Forms.MenuItem menuItem12;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.MenuItem menuItem13;
		private System.Windows.Forms.Label label3;
		private InformationLabel infoLabel;
		private SchemaUI panel3;
		private System.Windows.Forms.MenuItem menuItem15;
		private System.Windows.Forms.PrintDialog printSchemaDialog;
		private System.Windows.Forms.MenuItem menuItem6;
		private System.Windows.Forms.Label Day13;
		private System.Windows.Forms.Label Day9;
		private System.Windows.Forms.Label Day10;
		private System.Windows.Forms.Label Day11;
		private System.Windows.Forms.Label Day8;
		private System.Windows.Forms.Label Day12;
		private System.Windows.Forms.Label Day14;
		private System.Windows.Forms.Panel panel2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.MenuItem menuItem5;
		private System.Windows.Forms.MenuItem menuItem16;
		private System.Windows.Forms.MenuItem menuItem17;
		private System.Windows.Forms.MenuItem menuItem19;
		private System.Windows.Forms.MenuItem menuItem18;
		private System.Windows.Forms.MenuItem menuItem20;
		private System.Windows.Forms.Timer timer1;
		private System.Windows.Forms.MenuItem menuItem14;

		/// <summary>
		/// Initiates the components, creates the scheme, shows the loginbox
		/// </summary>
		public Form1()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			// Set database if ini-file exists
			try
			{
				using ( System.IO.StreamReader sr = new System.IO.StreamReader("schemalite.ini") )
				{
					String st;
					if ( (st = sr.ReadLine()) != null )
					{
						// file has the line database=filename
						Scheme.Instance.DatabaseFile = st.Substring(9);
					}
				}
			}
			catch 
			{
			}
			this.panel3.AddObserver(infoLabel);
			// Create the scheme 
			Scheme.Instance.AddObserver(checkedListBox);
			Scheme.Instance.AddObserver(this);

			// Show loginbox, if password exist in db
			if ( Scheme.Instance.login("admin", "") != ErrorHandler.NO_ERROR )
			{
				LoginBox lb = new LoginBox();
				while ( !Scheme.Instance.LoggedIn )
				{
					lb.ShowDialog(); 
				}
			}
			
			if ( !Scheme.Instance.Admin )
			{
				// Disable menuitems that only admin can use
				menuItem15.Enabled = false;
				menuItem6.Enabled = false;
			}

			// Fetch regcode from registry
			string code = "";
			try 
			{
				Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Celit\\SchemaLite");
				if ( regkey != null )
				{
				
					code = regkey.GetValue("Serial").ToString();
				}
			}
			catch
			{
			}

			registered = false;
			
			// This is the feature on the demo-version, that the monthcalender cannot be used
			this.monthCalendar1.Enabled = false;
			
			// If the code in correct or not found show a registration form
			if ( !RegisterLicense.isLicensKeyOK(code) )
			{
				Register reg = new Register();
				reg.ShowDialog();
				code = reg.Code;
				if ( RegisterLicense.isLicensKeyOK(code) )
				{
					registered = true;
					this.monthCalendar1.Enabled = true;
					ErrorHandler.ShowError(ErrorHandler.INFO_PROGRAM_REGISTERED);
					try
					{
						Microsoft.Win32.RegistryKey regkey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE", true);
						regkey.CreateSubKey("Celit");
						regkey = regkey.OpenSubKey("Celit", true);
						regkey.CreateSubKey("SchemaLite");
						regkey = regkey.OpenSubKey("SchemaLite", true);
						regkey.DeleteValue("Serial", false);
						regkey.SetValue("Serial", code);
					}
					catch
					{
						// Error: could not save registration
					}
				}
			}
			else
			{
				registered = true;
				this.monthCalendar1.Enabled = true;
			}
			if ( registered == false )
			{
				// TODO: Add meny for registration
			}

			Loading l = new Loading();
			l.Show();
			l.Focus();
			l.BringToFront();
			l.Update();
			Cursor = Cursors.AppStarting;
			// Init the scheme and resourceboxes
			Scheme.Instance.initScheme(checkedListBox);
			checkedListBox.Update(this);

			// Draw lines in the big scheme 
			ErrorHandler.ShowError(Scheme.Instance.changeWeek(monthCalendar1.SelectionStart));
			updateWeekUI();
			l.Hide();
			Cursor = Cursors.Default;
			// #1.03.001 - Trigger timer that will show Add person dialogbox
			timer1.Enabled = true;
		}

		/// <summary>
		/// ReDraws the activitys in the week,
		/// </summary>
		/// <param name="sender"></param>
		public void Update(object sender) //DateTime starttime)
		{
			SelectedWeek selweek = Scheme.Instance.SelectedWeek;
			DateTime starttime = Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek;

			this.setDayTitles();
			foreach ( ActivityUI a in this.activityuis_.Values )
			{
				a.Update(this);
			}
		}
		
		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
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
			this.components = new System.ComponentModel.Container();
			System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(Form1));
			this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
			this.panel1 = new System.Windows.Forms.Panel();
			this.Day7 = new System.Windows.Forms.Label();
			this.Day6 = new System.Windows.Forms.Label();
			this.Day5 = new System.Windows.Forms.Label();
			this.Day2 = new System.Windows.Forms.Label();
			this.Day3 = new System.Windows.Forms.Label();
			this.Day4 = new System.Windows.Forms.Label();
			this.Day1 = new System.Windows.Forms.Label();
			this.Day8 = new System.Windows.Forms.Label();
			this.Day9 = new System.Windows.Forms.Label();
			this.Day10 = new System.Windows.Forms.Label();
			this.Day11 = new System.Windows.Forms.Label();
			this.Day12 = new System.Windows.Forms.Label();
			this.Day13 = new System.Windows.Forms.Label();
			this.Day14 = new System.Windows.Forms.Label();
			this.vScrollBar1 = new System.Windows.Forms.VScrollBar();
			this.label1 = new System.Windows.Forms.Label();
			this.button1 = new System.Windows.Forms.Button();
			this.button2 = new System.Windows.Forms.Button();
			this.printDialog1 = new System.Windows.Forms.PrintDialog();
			this.mainMenu1 = new System.Windows.Forms.MainMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.menuItem15 = new System.Windows.Forms.MenuItem();
			this.menuItem3 = new System.Windows.Forms.MenuItem();
			this.menuItem4 = new System.Windows.Forms.MenuItem();
			this.menuItem5 = new System.Windows.Forms.MenuItem();
			this.menuItem16 = new System.Windows.Forms.MenuItem();
			this.menuItem17 = new System.Windows.Forms.MenuItem();
			this.menuItem18 = new System.Windows.Forms.MenuItem();
			this.menuItem20 = new System.Windows.Forms.MenuItem();
			this.menuItem13 = new System.Windows.Forms.MenuItem();
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.menuItem6 = new System.Windows.Forms.MenuItem();
			this.menuItem19 = new System.Windows.Forms.MenuItem();
			this.menuItem7 = new System.Windows.Forms.MenuItem();
			this.menuItem8 = new System.Windows.Forms.MenuItem();
			this.menuItem9 = new System.Windows.Forms.MenuItem();
			this.menuItem10 = new System.Windows.Forms.MenuItem();
			this.menuItem11 = new System.Windows.Forms.MenuItem();
			this.menuItem12 = new System.Windows.Forms.MenuItem();
			this.label2 = new System.Windows.Forms.Label();
			this.checkedListBox = new SchemaLiteUI.CheckListUI();
			this.label3 = new System.Windows.Forms.Label();
			this.infoLabel = new SchemaLiteUI.InformationLabel();
			this.panel3 = new SchemaLiteUI.SchemaUI();
			this.printSchemaDialog = new System.Windows.Forms.PrintDialog();
			this.panel2 = new System.Windows.Forms.Panel();
			this.label4 = new System.Windows.Forms.Label();
			this.timer1 = new System.Windows.Forms.Timer(this.components);
			this.panel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.SuspendLayout();
			// 
			// monthCalendar1
			// 
			this.monthCalendar1.Location = new System.Drawing.Point(768, 336);
			this.monthCalendar1.Name = "monthCalendar1";
			this.monthCalendar1.TabIndex = 3;
			this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateChanged);
			// 
			// panel1
			// 
			this.panel1.BackColor = System.Drawing.Color.White;
			this.panel1.Controls.Add(this.Day7);
			this.panel1.Controls.Add(this.Day6);
			this.panel1.Controls.Add(this.Day5);
			this.panel1.Controls.Add(this.Day2);
			this.panel1.Controls.Add(this.Day3);
			this.panel1.Controls.Add(this.Day4);
			this.panel1.Controls.Add(this.Day1);
			this.panel1.Location = new System.Drawing.Point(38, -24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(350, 96);
			this.panel1.TabIndex = 5;
			// 
			// Day7
			// 
			this.Day7.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day7.Location = new System.Drawing.Point(432, 24);
			this.Day7.Name = "Day7";
			this.Day7.Size = new System.Drawing.Size(28, 14);
			this.Day7.TabIndex = 6;
			this.Day7.Text = "label9";
			// 
			// Day6
			// 
			this.Day6.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day6.Location = new System.Drawing.Point(368, 24);
			this.Day6.Name = "Day6";
			this.Day6.Size = new System.Drawing.Size(28, 14);
			this.Day6.TabIndex = 5;
			this.Day6.Text = "label8";
			// 
			// Day5
			// 
			this.Day5.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day5.Location = new System.Drawing.Point(304, 24);
			this.Day5.Name = "Day5";
			this.Day5.Size = new System.Drawing.Size(28, 14);
			this.Day5.TabIndex = 4;
			this.Day5.Text = "label7";
			// 
			// Day2
			// 
			this.Day2.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day2.Location = new System.Drawing.Point(112, 24);
			this.Day2.Name = "Day2";
			this.Day2.Size = new System.Drawing.Size(28, 14);
			this.Day2.TabIndex = 3;
			this.Day2.Text = "label4";
			// 
			// Day3
			// 
			this.Day3.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day3.Location = new System.Drawing.Point(176, 24);
			this.Day3.Name = "Day3";
			this.Day3.Size = new System.Drawing.Size(28, 14);
			this.Day3.TabIndex = 2;
			this.Day3.Text = "label4";
			// 
			// Day4
			// 
			this.Day4.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day4.Location = new System.Drawing.Point(240, 24);
			this.Day4.Name = "Day4";
			this.Day4.Size = new System.Drawing.Size(28, 14);
			this.Day4.TabIndex = 1;
			this.Day4.Text = "label4";
			// 
			// Day1
			// 
			this.Day1.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day1.Location = new System.Drawing.Point(40, 24);
			this.Day1.Name = "Day1";
			this.Day1.Size = new System.Drawing.Size(28, 14);
			this.Day1.TabIndex = 0;
			this.Day1.Text = "label4";
			// 
			// Day8
			// 
			this.Day8.BackColor = System.Drawing.Color.White;
			this.Day8.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day8.Location = new System.Drawing.Point(56, 24);
			this.Day8.Name = "Day8";
			this.Day8.Size = new System.Drawing.Size(28, 14);
			this.Day8.TabIndex = 0;
			this.Day8.Text = "label4";
			// 
			// Day9
			// 
			this.Day9.BackColor = System.Drawing.Color.White;
			this.Day9.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day9.Location = new System.Drawing.Point(-8, 24);
			this.Day9.Name = "Day9";
			this.Day9.Size = new System.Drawing.Size(28, 14);
			this.Day9.TabIndex = 3;
			this.Day9.Text = "label4";
			// 
			// Day10
			// 
			this.Day10.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day10.Location = new System.Drawing.Point(25, 24);
			this.Day10.Name = "Day10";
			this.Day10.Size = new System.Drawing.Size(28, 14);
			this.Day10.TabIndex = 2;
			this.Day10.Text = "label4";
			// 
			// Day11
			// 
			this.Day11.BackColor = System.Drawing.Color.White;
			this.Day11.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day11.Location = new System.Drawing.Point(150, 24);
			this.Day11.Name = "Day11";
			this.Day11.Size = new System.Drawing.Size(28, 14);
			this.Day11.TabIndex = 1;
			this.Day11.Text = "label4";
			// 
			// Day12
			// 
			this.Day12.BackColor = System.Drawing.Color.White;
			this.Day12.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day12.Location = new System.Drawing.Point(104, 24);
			this.Day12.Name = "Day12";
			this.Day12.Size = new System.Drawing.Size(28, 14);
			this.Day12.TabIndex = 4;
			this.Day12.Text = "label7";
			// 
			// Day13
			// 
			this.Day13.BackColor = System.Drawing.Color.White;
			this.Day13.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day13.Location = new System.Drawing.Point(24, 24);
			this.Day13.Name = "Day13";
			this.Day13.Size = new System.Drawing.Size(28, 14);
			this.Day13.TabIndex = 5;
			this.Day13.Text = "label8";
			// 
			// Day14
			// 
			this.Day14.BackColor = System.Drawing.Color.White;
			this.Day14.Font = new System.Drawing.Font("Arial", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.Day14.Location = new System.Drawing.Point(136, 24);
			this.Day14.Name = "Day14";
			this.Day14.Size = new System.Drawing.Size(28, 14);
			this.Day14.TabIndex = 7;
			this.Day14.Text = "label4";
			// 
			// vScrollBar1
			// 
			this.vScrollBar1.LargeChange = 40;
			this.vScrollBar1.Location = new System.Drawing.Point(752, 80);
			this.vScrollBar1.Maximum = 296;
			this.vScrollBar1.Name = "vScrollBar1";
			this.vScrollBar1.Size = new System.Drawing.Size(16, 480);
			this.vScrollBar1.SmallChange = 20;
			this.vScrollBar1.TabIndex = 9;
			this.vScrollBar1.Visible = false;
			// 
			// label1
			// 
			this.label1.BackColor = System.Drawing.Color.Silver;
			this.label1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.label1.Location = new System.Drawing.Point(200, 168);
			this.label1.Name = "label1";
			this.label1.TabIndex = 5;
			this.label1.Text = "Läser in vecka...";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(536, 272);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(160, 24);
			this.button1.TabIndex = 13;
			this.button1.Text = "Lägga till resurs";
			this.button1.Visible = false;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(536, 272);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(88, 24);
			this.button2.TabIndex = 14;
			this.button2.Text = "Kontrollera antal timmar";
			this.button2.Visible = false;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// mainMenu1
			// 
			this.mainMenu1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem1,
																					  this.menuItem5,
																					  this.menuItem13,
																					  this.menuItem7,
																					  this.menuItem9});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem2,
																					  this.menuItem15,
																					  this.menuItem3,
																					  this.menuItem4});
			this.menuItem1.Text = "&Arkiv";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 0;
			this.menuItem2.Text = "&Skriv ut schema";
			this.menuItem2.Click += new System.EventHandler(this.button3_Click);
			// 
			// menuItem15
			// 
			this.menuItem15.Index = 1;
			this.menuItem15.Text = "Inställningar";
			this.menuItem15.Click += new System.EventHandler(this.menuItem15_Click);
			// 
			// menuItem3
			// 
			this.menuItem3.Index = 2;
			this.menuItem3.Text = "-";
			// 
			// menuItem4
			// 
			this.menuItem4.Index = 3;
			this.menuItem4.Text = "&Avsluta";
			this.menuItem4.Click += new System.EventHandler(this.menuItem4_Click);
			// 
			// menuItem5
			// 
			this.menuItem5.Index = 1;
			this.menuItem5.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem16,
																					  this.menuItem17,
																					  this.menuItem18,
																					  this.menuItem20});
			this.menuItem5.Text = "Visa";
			// 
			// menuItem16
			// 
			this.menuItem16.Index = 0;
			this.menuItem16.Text = "1-veckasschema";
			this.menuItem16.Click += new System.EventHandler(this.menuItem16_Click);
			// 
			// menuItem17
			// 
			this.menuItem17.Checked = true;
			this.menuItem17.Index = 1;
			this.menuItem17.Text = "2-veckasschema";
			this.menuItem17.Click += new System.EventHandler(this.menuItem17_Click);
			// 
			// menuItem18
			// 
			this.menuItem18.Index = 2;
			this.menuItem18.Text = "-";
			// 
			// menuItem20
			// 
			this.menuItem20.Index = 3;
			this.menuItem20.Text = "Uppdatera";
			this.menuItem20.Click += new System.EventHandler(this.menuItem20_Click);
			// 
			// menuItem13
			// 
			this.menuItem13.Index = 2;
			this.menuItem13.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					   this.menuItem14,
																					   this.menuItem6,
																					   this.menuItem19});
			this.menuItem13.Text = "Redigera";
			// 
			// menuItem14
			// 
			this.menuItem14.Index = 0;
			this.menuItem14.Text = "Kopiera från föreg. vecka";
			this.menuItem14.Click += new System.EventHandler(this.menuItem14_Click);
			// 
			// menuItem6
			// 
			this.menuItem6.Index = 1;
			this.menuItem6.Text = "Lägg till person";
			this.menuItem6.Click += new System.EventHandler(this.button1_Click);
			// 
			// menuItem19
			// 
			this.menuItem19.Index = 2;
			this.menuItem19.Text = "Lås alla";
			this.menuItem19.Click += new System.EventHandler(this.menuItem19_Click);
			// 
			// menuItem7
			// 
			this.menuItem7.Index = 3;
			this.menuItem7.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem8});
			this.menuItem7.Text = "Rapport";
			// 
			// menuItem8
			// 
			this.menuItem8.Index = 0;
			this.menuItem8.Text = "Skapa tidsrapport";
			this.menuItem8.Click += new System.EventHandler(this.button2_Click);
			// 
			// menuItem9
			// 
			this.menuItem9.Index = 4;
			this.menuItem9.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuItem10,
																					  this.menuItem11,
																					  this.menuItem12});
			this.menuItem9.Text = "Hjälp";
			// 
			// menuItem10
			// 
			this.menuItem10.Index = 0;
			this.menuItem10.Text = "Hjälp för program";
			this.menuItem10.Click += new System.EventHandler(this.button4_Click);
			// 
			// menuItem11
			// 
			this.menuItem11.Index = 1;
			this.menuItem11.Text = "-";
			// 
			// menuItem12
			// 
			this.menuItem12.Index = 2;
			this.menuItem12.Text = "Om programmet";
			this.menuItem12.Click += new System.EventHandler(this.menuItem12_Click);
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label2.Location = new System.Drawing.Point(784, 24);
			this.label2.Name = "label2";
			this.label2.TabIndex = 15;
			this.label2.Text = "Resurslista";
			// 
			// checkedListBox
			// 
			this.checkedListBox.BackColor = System.Drawing.Color.WhiteSmoke;
			this.checkedListBox.Location = new System.Drawing.Point(784, 40);
			this.checkedListBox.Name = "checkedListBox";
			this.checkedListBox.Size = new System.Drawing.Size(136, 139);
			this.checkedListBox.TabIndex = 0;
			this.checkedListBox.ThreeDCheckBoxes = true;
			this.checkedListBox.MouseMove += new System.Windows.Forms.MouseEventHandler(this.checkedListBox_MouseMove);
			this.checkedListBox.SelectedIndexChanged += new System.EventHandler(this.checkedListBox_SelectedIndexChanged);
			this.checkedListBox.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.checkedListBox_ItemCheck);
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.label3.Location = new System.Drawing.Point(776, 200);
			this.label3.Name = "label3";
			this.label3.TabIndex = 18;
			this.label3.Text = "Information";
			// 
			// infoLabel
			// 
			this.infoLabel.Location = new System.Drawing.Point(776, 224);
			this.infoLabel.Name = "infoLabel";
			this.infoLabel.Size = new System.Drawing.Size(128, 104);
			this.infoLabel.TabIndex = 19;
			// 
			// panel3
			// 
			this.panel3.AllowDrop = true;
			this.panel3.BackColor = System.Drawing.Color.Transparent;
			this.panel3.Location = new System.Drawing.Point(8, 72);
			this.panel3.Name = "panel3";
			this.panel3.Size = new System.Drawing.Size(736, 688);
			this.panel3.TabIndex = 20;
			// 
			// panel2
			// 
			this.panel2.BackColor = System.Drawing.Color.White;
			this.panel2.Controls.Add(this.Day13);
			this.panel2.Controls.Add(this.Day12);
			this.panel2.Controls.Add(this.Day11);
			this.panel2.Controls.Add(this.Day10);
			this.panel2.Controls.Add(this.Day9);
			this.panel2.Controls.Add(this.Day8);
			this.panel2.Controls.Add(this.Day14);
			this.panel2.Location = new System.Drawing.Point(388, -24);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(356, 96);
			this.panel2.TabIndex = 21;
			// 
			// label4
			// 
			this.label4.BackColor = System.Drawing.Color.White;
			this.label4.Location = new System.Drawing.Point(8, 0);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(30, 72);
			this.label4.TabIndex = 22;
			// 
			// timer1
			// 
			this.timer1.Interval = 500;
			this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
			// 
			// Form1
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(962, 571);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.vScrollBar1);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.panel3);
			this.Controls.Add(this.infoLabel);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkedListBox);
			this.Controls.Add(this.button2);
			this.Controls.Add(this.button1);
			this.Controls.Add(this.monthCalendar1);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Menu = this.mainMenu1;
			this.Name = "Form1";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SchemaLite Version 1.04";
			this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
			this.MaximumSizeChanged += new System.EventHandler(this.Form1_MaximumSizeChanged);
			this.MinimumSizeChanged += new System.EventHandler(this.Form1_MinimumSizeChanged);
			this.panel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.ResumeLayout(false);

		}
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			if ( ErrorHandler.ShowError(DataBase.checkDatabase()) == true )
			{
				Application.Exit();
			}
			else
			{
				Application.Run(new Form1());
			}
		}

		/// <summary>
		/// Sets selecteditem as active name
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Scheme.Instance.ActiveName = (String)checkedListBox.SelectedItem;
		}

		private void setDayTitles()
		{
			DateTime day = new DateTime(Scheme.Instance.SelectedWeek.Year,1,1);
			SortedList mnames = Scheme.Instance.SelectedWeek.MonthNames;
			SortedList dnames = Scheme.Instance.SelectedWeek.WeekDays;
			for (int i = 1; i <= SchemaUI.DAYS_SHOWN; i++)
			{
				
				for ( int o = 0; o < this.panel1.Controls.Count; o ++ )
				{
					if ( this.panel1.Controls[o].Name == "Day"+i )
					{
						this.panel1.Controls[o].Text = ((SelectedDay)dnames[i-1]).getNameofDay()+"\n"+((SelectedDay)dnames[i-1]).Day+" "+mnames[i-1];
						this.panel1.Controls[o].Left = SchemaUI.SCHEME_DAY_PIXELS*(i-1);
						this.panel1.Controls[o].Height = 28;
						this.panel1.Controls[o].Width = SchemaUI.SCHEME_DAY_PIXELS;
					}
				}
				for ( int o = 0; o < this.panel2.Controls.Count; o ++ )
				{
					if ( this.panel2.Controls[o].Name == "Day"+i )
					{
						this.panel2.Controls[o].Text = ((SelectedDay)dnames[i-1]).getNameofDay()+"\n"+((SelectedDay)dnames[i-1]).Day+" "+mnames[i-1];
						this.panel2.Controls[o].Left = SchemaUI.SCHEME_DAY_PIXELS*((i-1)%7);
						this.panel2.Controls[o].Height = 28;
						this.panel2.Controls[o].Width = SchemaUI.SCHEME_DAY_PIXELS;
					}
				}
			}
		}
		
		// #1.01.003 - new function
		/// <summary>
		/// Create or get an old activityUI for a given day
		/// </summary>
		/// <param name="panel">Panel where the Activity exist</param>
		/// <param name="day">The day (0-6)</param>
		/// <returns>A ActivityUI object</returns>
		private ActivityUI getActivityUI(Control panel, int day)
		{
			ActivityUI acui;
			foreach ( object obj in panel.Controls )
			{
				if ( obj.GetType() == Type.GetType("SchemaLiteUI.ActivityUI") )
				{
					acui = (ActivityUI)obj;
					SelectedDay selday = new SelectedDay(acui.Activity.Day);
					if ( selday.DayInWeek == day )
					{
						return acui;
					}
				}
			}
			acui = new ActivityUI(new Activity(), false);
			panel.Controls.Add(acui);
			return acui;
		}
		
		/// <summary>
		/// Update the text and layout of the day headers of the week
		/// </summary>
		/// Revision:
		/// #1.04 - New index for week list (week number)
		public void updateWeekUI()
		{
			panel1.SuspendLayout();
			panel2.SuspendLayout();
			
			// Hide marker
//			SchemaUI.marker_.Size = new Size(0,0); - #1.02.001 - row removed
			this.infoLabel.Text = "";
			// #1.02.001 - row added
			SchemaUI.Selected = null;
			
			this.setDayTitles();
			// Create ActivityUIs
			// #1.01.003 - start
/*			for ( int i=0; i < activityuis_.Count; i++)						
			{
				panel1.Controls.Remove((Control)activityuis_[activityuis_.GetKey(i)]);
				((ActivityUI)activityuis_[activityuis_.GetKey(i)]).Dispose();
			}
			*/
			// #1.01.003 - slut
			activityuis_.Clear();
			SelectedWeek selweek = new SelectedWeek(this.monthCalendar1.SelectionStart);
			DateTime firstday = selweek.DateOfFirstDayInWeek;

			// Create activityfield for every day shown
			for ( int i=0; i < SchemaUI.DAYS_SHOWN; i++)
			{
				Activity activity;
				ActivityUI acui;

				if ( i < 7 )
					// panel1.Controls.Add(acui); #1.01.003 - row changed
					acui = getActivityUI(panel1, i%7);
				else
					//	panel2.Controls.Add(acui); #1.01.003 - row changed
					acui = getActivityUI(panel2, i%7);
					

				// If activity found, create a ActivityUI with that data, else create an "empty" ActivityUI
				if (Scheme.Instance.Activitys.ContainsKey(firstday.AddDays(i)) == true )
				{
					activity = (Activity)Scheme.Instance.Activitys[firstday.AddDays(i)];
					// acui = new ActivityUI(activity, true); #1.01.003 - row changed
					acui.changeActivity(activity, true);
				}
				else
				{
					activity = new Activity(firstday.AddDays(i),"");
					// acui = new ActivityUI(activity, false); #1.01.003 - row changed
					acui.changeActivity(activity, false);
				}
				activityuis_[activity.Day] = acui;
			}
			panel1.BringToFront();
			panel2.BringToFront();
			panel2.ResumeLayout();
			panel1.ResumeLayout();
			
			Sheet.Instance.redraw_allowed = false;
			// Add graphics for the resouceboxes in the weeks
			// #1.04 - start 
			if ( Scheme.Instance.ResourceBoxPerWeek.ContainsKey(new SelectedWeek(Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek).Week) )
			{
				foreach ( ResourceBox rb in ((SortedList)Scheme.Instance.ResourceBoxPerWeek[new SelectedWeek(Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek).Week]).Values )
				{
					panel3.addResourcebox(rb);
				}
			}
			if ( Scheme.Instance.ResourceBoxPerWeek.ContainsKey(new SelectedWeek(Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek.AddDays(7)).Week) )
			{
				foreach ( ResourceBox rb in ((SortedList)Scheme.Instance.ResourceBoxPerWeek[new SelectedWeek(Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek.AddDays(7)).Week]).Values )
				{
					panel3.addResourcebox(rb);
				}
			}
			// #1.04 - stop 
			Sheet.Instance.redraw_allowed = true;
			// #1.01.003 - row added
			panel3.Update(this);

			this.checkedListBox.Focus();
		}

		/// <summary>
		/// Changes the selected week
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void monthCalendar1_DateChanged(object sender, System.Windows.Forms.DateRangeEventArgs e)
		{
			if ( registered == true )
			{
				// Check if the week has changed 
				SelectedWeek selweek = new SelectedWeek(e.Start);
				if ( Scheme.Instance.SelectedWeek.Week != selweek.Week )
				{
					label1.BringToFront();
					label1.Update();
					ErrorHandler.ShowError(Scheme.Instance.changeWeek(e.Start));
					updateWeekUI();
					label1.SendToBack();
				}
			}
			// #1.01.004 - lines removed
//			else
//				ErrorHandler.ShowError(ErrorHandler.ERR_NOT_REGISTERED);

		}
	
		/// <summary>
		/// Sets or unsets a person or resoruces as selected
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkListBox_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			if ( ( e.NewValue == System.Windows.Forms.CheckState.Checked ) != ( ((ActivePersonResource)Scheme.Instance.ActivePersonsResources[checkedListBox.Items[e.Index]]).Selected) )
			{
				((ActivePersonResource)Scheme.Instance.ActivePersonsResources[checkedListBox.Items[e.Index]]).Selected = (CheckState.Checked == e.NewValue);
			}
		}

		/// <summary>
		/// Open the dialog where persons and resources can be added.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button1_Click(object sender, System.EventArgs e)
		{
			AddPersonResourceUI aprUI = new AddPersonResourceUI();
			aprUI.setAsAdd();
			aprUI.ShowDialog(this);
			checkedListBox.Update(this);			
		}

		/// <summary>
		/// Opens the report creation dialog.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button2_Click(object sender, System.EventArgs e)
		{
			ReportUI rui = new ReportUI(this.monthCalendar1.SelectionStart, this.monthCalendar1.SelectionEnd);
			rui.ShowDialog(this);
		}

		/// <summary>
		/// Check so the reourcebox will be correctly updated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
// #1.02.001 - rows removed - start
/*		private void panel2_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( SchemaUI.currentrbui != null )
			{
				SchemaUI.currentrbui.ResourceBoxUI_MouseMove(sender, e);
				SchemaUI.currentrbui.Update(sender);
				SchemaUI.currentrbui = null;
			}
		}
*/
// #1.02.001 - rows removed - end

		/// <summary>
		/// Print scheme to printer
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button3_Click(object sender, System.EventArgs e)
		{
			string currdir = Directory.GetCurrentDirectory();
			try 
			{
				PrintSchema psdoc = new PrintSchema(panel3, panel1.Controls, panel3.Controls, Color.White, 750, 500, "Schema för vecka "+Scheme.Instance.SelectedWeek.Week);
				printSchemaDialog.Document = psdoc;
				DialogResult dr = printSchemaDialog.ShowDialog(this);
				if ( dr == DialogResult.OK )
				{
					psdoc.Print();
				}
			}
			catch
			{
				ErrorHandler.ShowError(ErrorHandler.ERR_PRINTING);
			}
			Directory.SetCurrentDirectory(currdir);
		}

		/// <summary>
		/// Show help
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void button4_Click(object sender, System.EventArgs e)
		{
			Help.ShowHelp(this, "./SchemaLite.chm");
		}

		/// <summary>
		/// Exit the application
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItem4_Click(object sender, System.EventArgs e)
		{
			Application.Exit();
		}

		/// <summary>
		/// Show about form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItem12_Click(object sender, System.EventArgs e)
		{
			About about = new About(registered);
			about.ShowDialog(this);
		}

		/// <summary>
		/// Copy previous weeks resourceboxes to current week
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItem14_Click(object sender, System.EventArgs e)
		{
			DialogResult dr = ErrorHandler.ShowMessageYesNo(ErrorHandler.Q_COPY_WEEK);
			if ( dr == DialogResult.Yes )
			{
				SortedList newrb;
				ErrorHandler.ShowError(Scheme.Instance.copyResourceBoxes(Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek.AddDays(-7),Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek.AddMinutes(-1),0,7, out newrb));
				// Add the resouceboxes for to graphic display
				foreach ( ResourceBox rb in newrb.Values )
				{
					panel3.addResourcebox(rb);
				}
			}
		}

		/// <summary>
		/// Select a name of person/resource
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkedListBox_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			Scheme.Instance.ActiveName = (String)checkedListBox.SelectedItem;
			infoLabel.Text = "Person: "+Scheme.Instance.ActiveName+"\nÅrsavvikelse: "+(Scheme.Instance.getTotalDev(Scheme.Instance.ActiveName, new DateTime(Scheme.Instance.SelectedWeek.Year, 1,1), new DateTime(Scheme.Instance.SelectedWeek.Year, 12,31))).ToString();
//			panel3.DoDragDrop((String)checkedListBox.SelectedItem, DragDropEffects.Copy);
		}

		/// <summary>
		/// Show or hide all resourceboxes of a person/resource
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkedListBox_ItemCheck(object sender, System.Windows.Forms.ItemCheckEventArgs e)
		{
			if ( ( e.NewValue == System.Windows.Forms.CheckState.Checked ) != ( ((ActivePersonResource)Scheme.Instance.ActivePersonsResources[checkedListBox.Items[e.Index]]).Selected) )
			{
				((ActivePersonResource)Scheme.Instance.ActivePersonsResources[checkedListBox.Items[e.Index]]).Selected = (CheckState.Checked == e.NewValue);
				// #1.02.001 - line added
				((ActivePersonResource)Scheme.Instance.ActivePersonsResources[checkedListBox.Items[e.Index]]).Notify();
			}
		}

		/// <summary>
		/// Show settings form
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItem15_Click(object sender, System.EventArgs e)
		{
			String dbfile = Scheme.Instance.DatabaseFile;
			SettingsUI settui = new SettingsUI(Scheme.Instance.Settings);
			settui.ShowDialog(this);
			if ( Scheme.Instance.DatabaseFile != dbfile )
			{
				Scheme.Instance.initScheme(this.checkedListBox);
			}
		}

		/// <summary>
		/// Set the personlistbox as a drag and drop
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void checkedListBox_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			try
			{
				if ( e.Button == MouseButtons.Left && (String)checkedListBox.SelectedItem != null )
				{
					checkedListBox.Update();
					Scheme.Instance.ActiveName = (String)checkedListBox.Items[checkedListBox.SelectedIndex];
				
					panel3.DoDragDrop((String)checkedListBox.SelectedItem, DragDropEffects.Copy);
				}
			}
			catch 
			{
			}
		}

		// #1.01.002 - function removed
/*		/// <summary>
		/// Change grid and dates in schema
		/// </summary>
		private void changeGrid()
		{
			panel1.SuspendLayout();
			panel2.SuspendLayout();
			panel3.SuspendLayout();
			SchemaUI.SCHEME_DAY_PIXELS = (panel3.Width-SchemaUI.SCHEME_FIRST_DAY_PIXEL)/SchemaUI.DAYS_SHOWN; #1.01.002 removed line

			this.panel1.Width = SchemaUI.SCHEME_DAY_PIXELS*7;
			if ( SchemaUI.DAYS_SHOWN == 14 )
			{
				this.panel2.Width = SchemaUI.SCHEME_DAY_PIXELS*7;
				this.panel2.Left = panel1.Left+panel1.Width;
			}
			this.panel3.Height = this.Height-panel3.Top-5;
			SchemaUI.SCHEME_HOUR_PIXELS = panel3.Height/(26-SchemaUI.SCHEME_FIRST_HOUR);
			((SchemaUI)this.panel3).restoreGrid();
			Scheme.Instance.changeWeek(this.monthCalendar1.SelectionStart);
			this.Update(this);
			panel3.SuspendLayout();
			panel2.SuspendLayout();
			panel1.SuspendLayout();
		}
*/
		/// <summary>
		/// When the size of mainwindow changes, scheme and all boxes change postion and size
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void Form1_SizeChanged(object sender, System.EventArgs e)
		{
			this.SuspendLayout();

			checkedListBox.SuspendLayout();
			infoLabel.SuspendLayout();
			label2.SuspendLayout();
			label3.SuspendLayout();
			monthCalendar1.SuspendLayout();
			panel3.SuspendLayout();
			//#1.01.002 - 2 rows added
			panel2.SuspendLayout();
			panel1.SuspendLayout();
			
			//#1.01.003 - update activityUIs
			foreach ( ActivityUI a in this.activityuis_.Values )
			{
				a.SuspendLayout();
				
			}
			// Set new sizes
			int sideX = this.Width-200;
			this.checkedListBox.Left = sideX;
			this.infoLabel.Left = sideX;
			this.label2.Left = sideX;
			this.label3.Left = sideX;
			this.monthCalendar1.Left = sideX;
			this.panel3.Size = new Size(sideX-20-panel3.Left, this.Height-panel3.Top-5);

			this.panel1.Width = SchemaUI.SCHEME_DAY_PIXELS*7;
			if ( SchemaUI.DAYS_SHOWN == 14 )
			{
				this.panel2.Width = SchemaUI.SCHEME_DAY_PIXELS*7;
				this.panel2.Left = panel1.Left+panel1.Width;
			}
			// #1.02.001 - line added
			setDayTitles();
			//#1.01.003 - update activityUIs
			foreach ( ActivityUI a in this.activityuis_.Values )
			{
				a.Update(this);
			}
//			this.panel3.Width = sideX-20-panel3.Left;	#1.01.002 - removed line
			// Update the sheet grid
//			changeGrid(); #1.01.002 - removed line
//			this.Update(this); #1.01.002 - removed line
			// #1.01.003 - added lines
			foreach ( ActivityUI a in this.activityuis_.Values )
			{
				a.ResumeLayout();
			}
			
			// #1.01.002 - added lines
			panel1.ResumeLayout();
			panel2.ResumeLayout();
			panel3.ResumeLayout();
			monthCalendar1.ResumeLayout();
			label3.ResumeLayout();
			label2.ResumeLayout();
			infoLabel.ResumeLayout();
			checkedListBox.ResumeLayout();
			ResumeLayout();
		}

		/// <summary>
		/// Show only one week
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItem16_Click(object sender, System.EventArgs e)
		{
			if ( SchemaUI.DAYS_SHOWN > 7 )
			{
				// #1.01.002 - lines changed
				SchemaUI.DAYS_SHOWN = 7;
				menuItem17.Checked = false;
				menuItem16.Checked = true;
				panel2.Visible = false;
				panel3.Size = new Size(panel3.Width-1, panel3.Height);
				this.Form1_SizeChanged(this,new System.EventArgs());
/*				this.SuspendLayout();
				panel2.SuspendLayout();
				SchemaUI.DAYS_SHOWN = 7;
				panel2.Visible = false;
				changeGrid();
				menuItem17.Checked = false;
				menuItem16.Checked = true;
				panel2.ResumeLayout();
				// HACK: Better solution? To update the SheetControl
				panel3.Width++;
				panel3.Width--;
				this.ResumeLayout();
*/
			}
		}

		/// <summary>
		/// Show two week scheme
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void menuItem17_Click(object sender, System.EventArgs e)
		{
			if ( SchemaUI.DAYS_SHOWN != 14 )
			{
				// #1.01.002 - lines changed
				SchemaUI.DAYS_SHOWN = 14;
				menuItem16.Checked = false;
				menuItem17.Checked = true;
				panel2.Visible = true;
				panel3.Size = new Size(panel3.Width+1, panel3.Height);
				this.Form1_SizeChanged(this, new System.EventArgs());
/*
				this.SuspendLayout();
				panel2.SuspendLayout();
				panel2.Visible = true;
				SchemaUI.DAYS_SHOWN = 14;
				changeGrid();
				menuItem16.Checked = false;
				menuItem17.Checked = true;
				panel2.ResumeLayout();
				// HACK: Better solution? To update the SheetControl
				panel3.Width++;
				panel3.Width--;
				this.ResumeLayout();
*/				
			}
		}

		private void menuItem19_Click(object sender, System.EventArgs e)
		{
			foreach ( String name in Scheme.Instance.ActivePersonsResources.Keys )
			{
				
				Scheme.Instance.lockResourceBoxes(name);
			}
		}

		// #1.01.002 - löser SL-1 (2 funktioner)
		private void Form1_MaximumSizeChanged(object sender, System.EventArgs e)
		{
			this.Form1_SizeChanged(sender, e);
		}

		private void Form1_MinimumSizeChanged(object sender, System.EventArgs e)
		{
			this.Form1_SizeChanged(sender, e);
		}
		
		// #1.01.007 - New event that updates from database
		private void menuItem20_Click(object sender, System.EventArgs e)
		{
			panel3.resetSchemeUI();
			Loading l = new Loading();
			l.Show();
			l.Focus();
			l.BringToFront();
			l.Update();
			Cursor = Cursors.AppStarting;
			// Init the scheme and resourceboxes
			Scheme.Instance.initScheme(checkedListBox);
			checkedListBox.Update(this);

			// Draw lines in the big scheme 
			ErrorHandler.ShowError(Scheme.Instance.changeWeek(monthCalendar1.SelectionStart));
			updateWeekUI();
			l.Hide();
			Cursor = Cursors.Default;
		}

		// #1.03.001 - added function
		private void timer1_Tick(object sender, System.EventArgs e)
		{
			this.timer1.Enabled = false;
			if ( Scheme.Instance.ActivePersonsResources.Count < 1 )
				button1_Click(this, null);
		}
	}
}
