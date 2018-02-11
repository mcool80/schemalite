using System;
using System.Collections;
using System.Windows.Forms;
using SchemaLite;

namespace SchemaLiteUI
{
	/// <summary>
	/// This class controlls all that is displayed in the schema UI
	/// Revision:
	/// ID			Date	Author				Text
	/// #1.01.001	051203	Markus Svensson		Solved bug SL-5
	/// #1.01.002	051208	Markus Svensson		Solved issue SL-6 and SL-1
	/// #1.01.007	060118	Markus Svensson		Solved issue SL-11
	/// </summary>
	public class SchemaUI : System.Windows.Forms.Panel, Observer
	{
		// #1.02.001 - New variables - start
		private static ResourceBoxUI selected = null;
		// #1.02.001 - New variables - end

		public static int nr_of_calls = 0;

		// Data for graphic design of the scheme

		public static int SCHEME_DAY_START_PIXEL = 0;	
		/// <summary>
		/// Pixels in diagram left to the fiorst day
		/// </summary>
		public static int SCHEME_FIRST_DAY_PIXEL = 30;

		/// <summary>
		/// Pixels that represents 1 hour
		/// </summary>
		public static int SCHEME_HOUR_PIXELS = 26;

		/// <summary>
		/// Pixels that represents 1 day
		/// </summary>
		public static int SCHEME_DAY_PIXELS = 50;

		/// <summary>
		/// The first hour that is displayed
		/// </summary>
		public static int SCHEME_FIRST_HOUR = 0;	

		/// <summary>
		/// Top of the panel scheme
		/// </summary>
		public static int SCHEME_PANEL_TOP = 60;

		/// <summary>
		/// Days that will be shown in the schema
		/// </summary>
		public static int DAYS_SHOWN = 14;

		/// <summary>
		/// Number of resourceboxes in the SchemaUI
		/// </summary>
 		private int rboxcount_ = 0;

		/// <summary>
		/// The currently edited resourceboxUI
		/// </summary>
		// #1.02.001 - line removed
//		public static ResourceBoxUI currentrbui = null;

		/// <summary>
		/// List with all cached resourceboxeUIs in system
		/// </summary>
		private static SortedList resursboxuis_ = new SortedList();

		/// <summary>
		/// Size of cache of resourceboxuis
		/// </summary>
		private int cacheSize = 50;

		/// <summary>
		/// Information label
		/// </summary>
		private static Observer information_;

		/// <summary>
		/// List with all labels used in grid.
		/// </summary>

//		public static Label marker_ = new Label(); #1.02.001 - row removed
// 		private SchemaLiteUI.Sheet sheetLines1 = new SchemaLiteUI.Sheet(); #1.02.001 - row changed
		private SchemaLiteUI.Sheet sheetLines1 = Sheet.Instance; 

		public SchemaUI()
		{
			this.BackColor = System.Drawing.Color.White;
//			this.MouseMove +=new System.Windows.Forms.MouseEventHandler(SchemaUI_MouseMove); #1.02.001 - function removed 
			this.DragDrop +=new DragEventHandler(SchemaUI_DragDrop);
			this.DragEnter += new DragEventHandler(SchemaUI_DragEnter);
			this.Resize +=new EventHandler(SchemaUI_Resize);

			this.sheetLines1.BackColor = System.Drawing.Color.WhiteSmoke;
			this.sheetLines1.ForeColor = System.Drawing.Color.Black;
			this.sheetLines1.Location = new System.Drawing.Point(0, 0);
			this.sheetLines1.Name = "sheetLines1";
			this.sheetLines1.Size = new System.Drawing.Size(536, 496);
			this.sheetLines1.TabIndex = 23;						
			this.Controls.Add(this.sheetLines1); 
			createSchemeUI();
			this.AllowDrop = true;
		}
/* #1.01.007 - remove function
		internal void restoreGrid()
		{
		}
*/		

		/// <summary>
		/// Reads all resourcesboxes from system and creates ResourceBoxUIs
		/// </summary>
		public void initResources()
		{
			this.SuspendLayout();
		}

		/// <summary>
		/// Add Information Observer
		/// </summary>
		/// <param name="information">Information observer</param>
		public void AddObserver(Observer information)
		{
			information_ = information;
		}

		/// <summary>
		/// Calculates the time and date using x,y coordinates in SchemaUI
		/// </summary>
		/// <param name="new_x">The x-coordinate</param>
		/// <param name="new_y">The y-coordinate</param>
		/// <param name="time">The datetime that is calculated</param>
		/// <returns>0 - If all is ok</returns>
		public static int calcDateTime(int new_x, int new_y, out DateTime time)
		{
			int a;
			int day = Math.DivRem(new_x-SchemaUI.SCHEME_FIRST_DAY_PIXEL, SchemaUI.SCHEME_DAY_PIXELS, out a);
			int time_h = Math.DivRem(new_y, SchemaUI.SCHEME_HOUR_PIXELS, out a)+SchemaUI.SCHEME_FIRST_HOUR;
			double b = (new_y%SchemaUI.SCHEME_HOUR_PIXELS)*(6000/SchemaUI.SCHEME_HOUR_PIXELS);
			int time_min = (int)Math.Round(b/1500.0, 0)*15; 
			if ( time_min == 60 ) 
			{
				time_h++;
				time_min = 0;
			}
			DateTime startday = Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek.AddDays(day);
			// Because 24 is 0.00 in the next day, the represenation of 24.00 will be 23.59
//			if ( time_h == 24 ) -- #1.02.001 - row changed
			if ( time_h >= 24 )
			{
				time_h = 23;
				time_min = 59;
			}
			// #1.02.001 - added lines
			if ( time_min < 0 )
			{
				time_min = 0;
			}
			if ( time_h < 0 )
			{
				time_h = 0;
			}

			time = new DateTime(startday.Year, startday.Month, startday.Day, time_h, time_min,0,0);
			return 0;
		}

		/// <summary>
		/// Draws all lines and writes hour numbers for a scheme
		/// </summary>
		private void createSchemeUI()
		{
			// #1.02.001 - rows removed
/*			marker_.Size = new System.Drawing.Size(0,0);
			marker_.Left = 0;
			marker_.Top = 0;
			marker_.Text = "";
			marker_.BackColor = System.Drawing.Color.Black;
			marker_.ForeColor = System.Drawing.Color.Black;
			marker_.Visible = true;
			//			marker_.BackColor = System.Drawing.Color.Transparent;
			this.Controls.Add(marker_);
			*/
		}

		/// <summary>
		/// Find a old RbUI in cahce or create a new one.
		/// Revision:
		/// #1.01.001 Added param rb to solve bug SL-5
		/// </summary>
		/// <returns>Free resoucebox UI</returns>
		private ResourceBoxUI findFreeResourceBoxUI(ResourceBox rb)
		{
//			foreach ( ResourceBoxUI rbui in this.resursboxuis_.Values ) #1.02.001 - row changed
			foreach ( ResourceBoxUI rbui in resursboxuis_.Values )
			{
				try
				{
					// Use only resourceboxes that are not displayed
					if ( rbui.ResourceBox.StartTime < Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek ||
						rbui.ResourceBox.StartTime > Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek.AddDays(SchemaUI.DAYS_SHOWN) )
					{
						((ActivePersonResource)Scheme.Instance.ActivePersonsResources[rbui.ResourceBox.Name]).RemoveObserver(rbui);
						rbui.ResourceBox.RemoveObserver(rbui);
						return rbui;
					}
				}
				catch
				{
					// #1.01.007 - cause of SL-11?
//					return rbui; -- line removed

				}
			}
			// No cahed object found, create a new
			// # 1.01.001 Row changed
//			ResourceBoxUI newrbui = new ResourceBoxUI();
			ResourceBoxUI newrbui = new ResourceBoxUI(rb, information_);

//			this.Controls.Add(newrbui); # 1.02.001 - row removed

			return newrbui;
		}

		/// <summary>
		/// Add resourceboxUI to a schemaUI
		/// </summary>
		/// <param name="rb">ResourceBox</param>
		public void addResourcebox(ResourceBox rb)
		{
			// Check if the resoucebox already exists
//			if ( this.resursboxuis_.ContainsKey(rb.Id) ) #1.02.001 - row changed
			if ( resursboxuis_.ContainsKey(rb.Id) )
			{
//				((ResourceBoxUI)this.resursboxuis_[rb.Id]).Update(this); #1.02.001 - row changed
				((ResourceBoxUI)resursboxuis_[rb.Id]).Update(this);
				return;
			}

			// Create graphics object to represent the resourcebox 
			ResourceBoxUI rbui;
//			if ( this.resursboxuis_.Count < this.cacheSize ) #1.02.001 - row changed
			if ( resursboxuis_.Count < this.cacheSize )
			{
				rbui = new ResourceBoxUI(rb, information_);
//				Controls.Add(rbui); #1.02.001 - row removed
//				this.resursboxuis_.Add(rb.Id, rbui); #1.02.001 - row changed
				resursboxuis_.Add(rb.Id, rbui);
			}
			else
			{
				// #1.01.001 Added param rb to solve bug SL-5
				rbui = findFreeResourceBoxUI(rb);
				
				// HACK: Better code please!
				try
				{
					resursboxuis_.Remove(rbui.ResourceBox.Id);
				}
				catch
				{
				}

				rbui.ResourceBox = rb;
				
//				this.resursboxuis_.Add(rb.Id, rbui); #1.02.001 - row changed
				resursboxuis_.Add(rb.Id, rbui);
			}
			// #1.02.001 - row added
			rbui.Parent = this;
			rb.AddObserver(rbui);
			((ActivePersonResource)Scheme.Instance.ActivePersonsResources[rb.Name]).AddObserver(rbui);			
			rbui.Update(this);
		}

		/// <summary>
		/// Adds a new resourcebox to system
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void SchemaUI_Click(object sender, System.EventArgs e)
		{
			if ( Scheme.Instance.ActiveName != null )
			{
				// Create a new resourcebox 1h where the mouse pointer is 
				int new_x = PointToClient(Cursor.Position).X;
				int	new_y = PointToClient(Cursor.Position).Y;

				DateTime st;
				calcDateTime(new_x, new_y, out st);

				// Create the new resourcebox with calculated time and duration of 1h endtime=starttime+1h 
				ResourceBox rb = new ResourceBox(st, st.AddHours(1), Scheme.Instance.ActiveName, false, false, new DateTime(0), new DateTime(0));

				Scheme.Instance.setResourceBox(rb, true);
				this.addResourcebox(rb);

				// #1.02.001 - row added
				this.Invalidate(true);
			}
			else ErrorHandler.ShowInformation(ErrorHandler.ERR_NO_SELECTED);
		}

		/// <summary>
		/// Control that the resourcebox will be updated
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
// #1.02.001 - rows removed - start
/*		private void SchemaUI_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
		{
			if ( currentrbui != null )
			{
				currentrbui.ResourceBoxUI_MouseMove(sender, e);
				currentrbui.Update(sender);
				currentrbui = null;
			}
		}
*/
// #1.02.001 - rows removed - end		
		#region Observer Members

		/// <summary>
		/// Update all data only if needed
		/// </summary>
		/// <param name="subject"></param>
		public void Update(object subject)
		{
			this.SuspendLayout();
			// If the objects in SchemaUi and Scheme differs, something is wrong
			// a total update of all SchemaUI is made
			if ( Scheme.Instance.ResourceBoxes.Count != rboxcount_ )
			{
				this.initResources();
			}
//			marker_.Size = new System.Drawing.Size(0,0); - #1.02.001 - row removed
// #1.01.002 - code moved 
// #1.02.001 - code removed
//			if ( SchemaUI.currentrbui != null )
//			{
//				SchemaUI.currentrbui.Update(this);
//			}
// #1.01.002 - code moved from Form1.cs
/*			this.Height = this.Parent.Height-this.Top-5;
			int sideX = this.Parent.Width-200;
			this.Width = sideX-20-this.Left;
			SchemaUI.SCHEME_DAY_PIXELS = (Width-SchemaUI.SCHEME_FIRST_DAY_PIXEL)/SchemaUI.DAYS_SHOWN;
			SchemaUI.SCHEME_HOUR_PIXELS = Height/(25-SchemaUI.SCHEME_FIRST_HOUR);
// #1.01.002 - code moved 
//			sheetLines1.Location = new System.Drawing.Point(0,0);
//			sheetLines1.Size = this.Size;
//			sheetLines1.Update(this);

*/			
			// #1.02.001 - new line
			this.sheetLines1.Invalidate();
			this.ResumeLayout();
		}

		#endregion

		private void SchemaUI_DragDrop(object sender, DragEventArgs e)
		{
			this.SchemaUI_Click(sender,e);
		}

		private void SchemaUI_DragEnter(object sender, DragEventArgs e)
		{
			e.Effect = DragDropEffects.Copy;
		}

		private void SchemaUI_Resize(object sender, EventArgs e)
		{
			// #1.01.002 - start
/*
			sheetLines1.Location = new System.Drawing.Point(0,0);
			sheetLines1.Size = this.Size;
			// Update the marker on active object
			marker_.Size = new System.Drawing.Size(0,0);
			if ( SchemaUI.currentrbui != null )
			{
				SchemaUI.currentrbui.Update(this);
			}
*/
//			marker_.Size = new System.Drawing.Size(0,0); - #1.02.001 - row removed
			SchemaUI.SCHEME_DAY_PIXELS = (Width-SchemaUI.SCHEME_FIRST_DAY_PIXEL)/SchemaUI.DAYS_SHOWN;
			SchemaUI.SCHEME_HOUR_PIXELS = Height/(25-SchemaUI.SCHEME_FIRST_HOUR);
			sheetLines1.Size = this.Size;
			sheetLines1.Invalidate();

//			foreach ( ResourceBoxUI rbui in this.resursboxuis_.Values ) #1.02.001 - row changed
			foreach ( ResourceBoxUI rbui in resursboxuis_.Values )
			{
//				rbui.SuspendLayout(); #1.02.001 - row removed
				rbui.Update(this);
//				rbui.ResumeLayout();  #1.02.001 - row removed
			}
			// #1.01.002 - end
		}

		// #1.01.007 -start
		public void resetSchemeUI()
		{
// #1.02.001 - line removed
//			currentrbui = null;
			resursboxuis_ = new SortedList();
			Controls.Clear();
			this.Controls.Add(this.sheetLines1);
			createSchemeUI();
		}
		// #1.01.007 - end
		
		// #1.02.001 - start
		public static ResourceBoxUI Selected 
		{
			get
			{
				return selected;
			}
			set
			{
				if ( selected != null )
					selected.unselect();
				selected = value;
				if ( value != null )
					value.select();
			}
		}

		public static SortedList ResouceBoxes 
		{
			get
			{
				return resursboxuis_;
			}
		}
		// #1.02.001 - end
	}
}
