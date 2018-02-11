using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using SchemaLite;

// #1.02.001 - new row
using System.Drawing.Drawing2D;

namespace SchemaLiteUI
{
	/// <summary>
	/// Graphical representation of a resourcebox
	/// 
	/// Revision:
	/// ID			Date	Author				Text
	///	1.01.005	051222	Markus Svensson		Solves issue SL-4
	///	1.01.006	051230	Markus Svensson		Solves issue SL-10
	/// </summary>
	
	// #1.01.005 - row changed
//	public class ResourceBoxUI : System.Windows.Forms.Label,  Observer
//	public class ResourceBoxUI : System.Windows.Forms.UserControl, Observer
	public class ResourceBoxUI : Observer, IDisposable
	{
		// #1.02.001 - new variables - start
		bool shown = false;
		//		Size Size;
		//		Color BackColor;
		//		Point Location;
		//		String Text;
		public Control Parent;
		Font Font;
		bool issel_ = false;
		Rectangle curr_rect = new Rectangle(0,0,0,0);
		// #1.02.001 - new variables - end
		/// <summary>
		/// The position of resourcebox before change
		/// </summary>
		private int origo_x, origo_y, origo_height;
		/// <summary>
		/// 
		/// </summary>
		private int left_, top_;

		/// <summary>
		/// True if the boxes is draged
		/// </summary>
		private bool drag=false;
		private int position_ = 0;

		/// <summary>
		/// True if the box is changed size on
		/// </summary>
		private bool changesize = false;
		private bool inMouseMove = false;

		/// <summary>
		/// True if the box is pulled up
		/// </summary>
		private bool pull_upper = false;
		/// <summary>
		/// True if the box is pulled down
		/// </summary>
		private bool pull_lower = false;
		private ResourceBox resursbox_;

		/// <summary>
		/// Differens in pixels between top or bottom of this and mousepointer 
		/// </summary>
		private int diff_y;	
		Observer information_;
		/// <summary>
		/// 
		/// </summary>
		public ResourceBoxUI()
		{
		}

		/// <summary>
		/// Creates a resourceboxUI given a resourcebox
		/// </summary>
		/// <param name="resursbox">Resourcebox to represent</param>
		public ResourceBoxUI(ResourceBox resursbox, Observer information)
		{
			resursbox_ = resursbox;
			//			this.AllowDrop = true; #1.02.001 - row removed
			// Setup events
			// #1.02.001 - rows removed - start 
			//			this.MouseDown += new System.Windows.Forms.MouseEventHandler(ResourceBoxUI_MouseDown);
			//			this.MouseMove +=new System.Windows.Forms.MouseEventHandler(ResourceBoxUI_MouseMove);
			//			this.MouseUp += new System.Windows.Forms.MouseEventHandler(ResourceBoxUI_MouseMove);
			//			this.Leave +=new EventHandler(ResourceBoxUI_Leave);
			// #1.02.001 - rows removed - end
			// Set apperance properties 
			
			// #1.01.005 removed property
			//			this.BorderStyle = BorderStyle.Fixed3D;
			this.Font = new Font("Arial", 7);
			//			this.Paint +=new PaintEventHandler(ResourceBoxUI_Paint); #1.02.001 - row removed
			information_ = information;
		}

		/// <summary>
		/// Gets a location (x,y) given a start and endtime
		/// </summary>
		/// <param name="starttime">Starttime</param>
		/// <param name="endtime">Endtime</param>
		/// <returns>A location</returns>
		public System.Drawing.Point getLocation(DateTime starttime, DateTime endtime)
		{
			int y = SchemaUI.SCHEME_DAY_START_PIXEL+(starttime.Hour-SchemaUI.SCHEME_FIRST_HOUR)*SchemaUI.SCHEME_HOUR_PIXELS+starttime.Minute*SchemaUI.SCHEME_HOUR_PIXELS/60-position_;
			int adddays = 0;
			SelectedDay day = new SelectedDay(starttime);
			if ( Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek.AddDays(7) <= starttime )
				adddays = 7;
			int x = SchemaUI.SCHEME_FIRST_DAY_PIXEL+(day.DayInWeek+adddays)*SchemaUI.SCHEME_DAY_PIXELS;
			return new System.Drawing.Point(x,y);
		}

		/// <summary>
		/// Updates the graphics
		/// </summary>
		/// <param name="subject"></param>
		public void Update(object subject)
		{
			//			this.SuspendLayout(); // #1.02.001 - row removed
			if ( ((ActivePersonResource)Scheme.Instance.ActivePersonsResources[this.resursbox_.Name]).Selected == true && 
				Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek <= resursbox_.StartTime && 
				Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek.AddDays(SchemaUI.DAYS_SHOWN) > resursbox_.StartTime )
				this.Show();
			else
			{
				Sheet.Instance.Invalidate(curr_rect);
				curr_rect = new Rectangle(0,0,0,0);
				this.Hide();
				return;
			}
			// #1.01.006 - do not show removed locked resourcebox
			if ( ResourceBox.StartTime == ResourceBox.EndTime )
			{
				//				this.Size = new Size(0,0);
				return;
			}
			DateTime oldstarttime = ResourceBox.StartTime;
			DateTime oldendtime = ResourceBox.EndTime;
			String oldname = ResourceBox.Name;
			bool oldfree = ResourceBox.Free;
			bool oldlocked = ResourceBox.Locked;

			//			this.BringToFront();  #1.02.001 - row removed
			
			// #1.05.001 - images can not be added to UserControl
			/*
			if ( resursbox_.Free == true )
			{
				this.Image = new Bitmap("streck.gif");
				this.ImageAlign = ContentAlignment.TopRight;
			}
			else if ( resursbox_.Locked == true )
			{
				this.Image = new System.Drawing.Bitmap("lock.gif");
				this.ImageAlign = ContentAlignment.BottomRight;
			}
			else
				this.Image = null;
			*/
			//			this.Location = location;
		
			// # 1.01.005 - changed (width, height) to size
			//			this.Width = width;
			//			this.Height = height;
			//			this.Size = new Size(width, height);
			// #1.01.005 - on change the whole sheet will be invalidated
			if ( oldstarttime != ResourceBox.StartTime ||
				oldendtime != ResourceBox.EndTime ||
				oldname != ResourceBox.Name ||
				oldfree != ResourceBox.Free ||
				oldlocked != ResourceBox.Locked )
			{
//				Sheet.Instance.Update(this);
			}
			// #1.02.001 - rows changed - start
			//			this.Invalidate();
			//			this.BringToFront();
			//			this.ResumeLayout();
			//			Sheet.Instance.Update(new Rectangle(Left-5, Top-5, Width+10, Height+10));
			if ( new Rectangle(Left, Top, Width, Height) != curr_rect )
			{
				Sheet.Instance.Invalidate(curr_rect);
			}
			curr_rect = new Rectangle(Left, Top, Width, Height);
			Sheet.Instance.Invalidate(curr_rect);
			// #1.02.001 - end
		}

		/// <summary>
		/// Remove the resourcebox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void remove_click(object sender, EventArgs e)
		{
			bool locked = resursbox_.Locked;

			if ( locked )
			{
				String errorstr = "När du tar bort denna resursbox kommer avvikelsen finnas kvar i systemet\nmen arbetspasset kommer inte att visas.\nDenna operation går inte att ångra.\nVill du ändå genomföra borttagningen?";
				if ( MessageBox.Show(null, errorstr, "Information",MessageBoxButtons.YesNo,MessageBoxIcon.Question) != DialogResult.Yes ) 
					return;
			}

			ErrorHandler.ShowError(Scheme.Instance.removeResourceBox(resursbox_));
			if ( !locked )
				this.Dispose();
				
			this.endDisplayInformation();
		}

		/// <summary>
		/// Change name for the resourcebox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		public void changename_click(object sender, EventArgs e)
		{
			MenuItem mi = (MenuItem)sender;
			resursbox_.Name = mi.Text;
			ErrorHandler.ShowError(Scheme.Instance.setResourceBox(resursbox_, false));
		}

		/// <summary>
		/// Display information
		/// </summary>
		//		private void displayInformation() - #1.02.001 - row changed
		public void displayInformation()
		{
			SchemaUI.Selected = this;
			String info;
			info = "Person: "+resursbox_.Name+"\n";
			info += "Start: "+resursbox_.StartTime.Hour+":";
			if ( resursbox_.StartTime.Minute < 10 )
				info+= "0";
			info += resursbox_.StartTime.Minute+"\n";
			info += "Slut: "+resursbox_.EndTime.Hour+":";
			if ( resursbox_.EndTime.Minute < 10 )
				info += "0";
			info += resursbox_.EndTime.Minute+"\n";
			if ( resursbox_.Locked == true )
				info += "Avvikelse: "+resursbox_.Deviation+" h";
			if ( resursbox_.Locked )
				info +="\nTiden är låst";
			if ( resursbox_.Free )
				info +="\nPersonen är ledig";
			DateTime starttime = new DateTime(ResourceBox.StartTime.Year, 1, 1);
			DateTime endtime = new DateTime(ResourceBox.StartTime.AddMonths(1).Year, ResourceBox.StartTime.AddMonths(1).Month, 1);
			double dev = Scheme.Instance.getTotalDev(ResourceBox.Name, starttime, endtime);
			info += "\nMånadssaldo: "+dev+" h";
			// #1.02.001 - rows removed
			/*			SchemaUI.marker_.SuspendLayout();
						SchemaUI.marker_.Size = new Size(this.Width+4, this.Height+4);
						SchemaUI.marker_.Location = new Point(this.Left-2, this.Top-2);
						SchemaUI.marker_.BringToFront();
						SchemaUI.marker_.ResumeLayout(); */
			//			this.BringToFront(); #1.02.001 - row removed
			information_.Update(info);
		}

		void endDisplayInformation()
		{
			information_.Update("");
			//			SchemaUI.marker_.Size = new Size(0, 0); - #1.02.001 - row removed
		}
		#region Removed code #1.02.001
		/// <summary>
		/// A user presses mouse down, setup events to come when moving cursor
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		// #1.02.001 - rows removed - start
		/*		private void ResourceBoxUI_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
				{
					if ( e.Button == MouseButtons.Left )
					{
							SchemaUI.currentrbui = this;
							Point ptCursor = Cursor.Position; 
							ptCursor = PointToClient(ptCursor); 

							origo_x = Cursor.Position.X; 
							origo_y = Cursor.Position.Y; 
							origo_height = this.Height;
							left_ = this.Left;
							top_ = this.Top;
							if ( this.Height-ptCursor.Y < PULL_AREA || ptCursor.Y < PULL_AREA )
								changesize = true;
							else
								drag = true;
							if ( ptCursor.Y > this.Height-PULL_AREA )
							{
								pull_lower = true;
								diff_y = this.Height-ptCursor.Y;
							}
							if ( ptCursor.Y < PULL_AREA )
							{
								pull_upper = true;
								diff_y = ptCursor.Y;
							}
						displayInformation(); 
					}
				}

				/// <summary>
				/// If the mouse moves, the resourcebox may be moved or resized, all this is checked
				/// </summary>
				/// <param name="sender"></param>
				/// <param name="e"></param>
				public void ResourceBoxUI_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
				{
					if ( inMouseMove ) 
						return;
					inMouseMove = true;
					Point ptCursor = Cursor.Position; 
					try 
					{
						ptCursor = PointToClient(ptCursor);
					}
					catch
					{
						return;
					}

					if ( this.Height < SchemaUI.SCHEME_HOUR_PIXELS/2 )
						this.Height = SchemaUI.SCHEME_HOUR_PIXELS/2;

					if ( e.Button == MouseButtons.Left && drag == true )
					{
						int new_x = Cursor.Position.X; 
						int new_y = Cursor.Position.Y; 
				
						this.SuspendLayout();
						// Calculate a new position on screen and move the resoucebox there
						if ( left_+new_x - origo_x > SchemaUI.SCHEME_DAY_START_PIXEL+SchemaUI.DAYS_SHOWN*SchemaUI.SCHEME_DAY_PIXELS )
							new_x = SchemaUI.SCHEME_DAY_START_PIXEL+SchemaUI.DAYS_SHOWN*SchemaUI.SCHEME_DAY_PIXELS+origo_x-left_;
						if ( left_+new_x - origo_x < SchemaUI.SCHEME_DAY_START_PIXEL )
							new_x= SchemaUI.SCHEME_DAY_START_PIXEL+origo_x-left_;

						this.Location = new System.Drawing.Point(left_+new_x - origo_x, top_+new_y - origo_y);
						this.BringToFront();
						this.ResumeLayout();
					}

					else if ( drag == true )
					{
						if ( Scheme.Instance.editAllowed(resursbox_.StartTime) )
						{
							if ( this.Location.Y+this.Height >= ((24-SchemaUI.SCHEME_FIRST_HOUR)*SchemaUI.SCHEME_HOUR_PIXELS) )
								this.Location = new Point(this.Location.X, ((24-SchemaUI.SCHEME_FIRST_HOUR)*SchemaUI.SCHEME_HOUR_PIXELS)-this.Height);
							if ( this.Location.Y < 0 )
								this.Location = new Point(this.Location.X, 0);

							int new_x = this.Location.X+this.Size.Width/2;
							int new_y = this.Location.Y;

							DateTime starttime;
							DateTime endtime;
							SchemaUI.calcDateTime(new_x,new_y, out starttime);
							SchemaUI.calcDateTime(new_x,new_y+this.Height, out endtime);

							// In case there is a new day, the resourcebox should end at 23:59 
							if ( endtime.Hour == 0 )
								endtime = endtime.AddMinutes(59-endtime.Minute).AddHours(23);
							resursbox_.StartTime = starttime;
							resursbox_.EndTime = endtime;

							ErrorHandler.ShowError(Scheme.Instance.setResourceBox(resursbox_,false));
						}
						else
							Update(this);
						displayInformation(); 

						drag = false;
					} 
					else if ( e.Button == MouseButtons.Left && changesize == true ) 
					{
						Point ptSchema = this.Parent.PointToClient(Cursor.Position);
						this.SuspendLayout();
						int new_y = Cursor.Position.Y;
						if ( pull_upper )
						{
							int bottom = this.Top+this.Height;
							this.Top = ptSchema.Y-diff_y;
							this.Height = bottom-this.Top;
						}
						else if ( pull_lower ) 
						{
							this.Height = ptSchema.Y+diff_y-this.Top;
							if ( this.Height < SchemaUI.SCHEME_HOUR_PIXELS/2 )
							{
								this.Height = SchemaUI.SCHEME_HOUR_PIXELS/2;
							}
						}
						if ( this.Top < 0 )
							this.Top = 0;
						if ( this.Top+this.Height > SchemaUI.SCHEME_HOUR_PIXELS*(24-SchemaUI.SCHEME_FIRST_HOUR) )
							this.Height = SchemaUI.SCHEME_HOUR_PIXELS*(24-SchemaUI.SCHEME_FIRST_HOUR)-this.Top;
						// #1.01.005 - force control to redraw
						this.Invalidate();
						this.BringToFront();
						this.ResumeLayout();
					}
					else if ( changesize == true )
					{
						if ( Scheme.Instance.editAllowed(resursbox_.StartTime) )
						{
							int new_y = 0;
							if ( ptCursor.Y < PULL_AREA )
								new_y = this.Location.Y;
							else
								new_y = top_;
							DateTime starttime;
							DateTime endtime;
							SchemaUI.calcDateTime(this.Left+this.Width/2,new_y, out starttime);
							SchemaUI.calcDateTime(this.Left+this.Width/2,new_y+this.Height, out endtime);
							resursbox_.StartTime = starttime;
							resursbox_.EndTime = endtime;
							// Change endtime only if this is pulled or the minimum duration is found 
							if ( endtime.Hour == starttime.Hour && endtime.Minute < starttime.Minute-30 ) 
							{
								resursbox_.EndTime = resursbox_.StartTime.AddHours(0).AddMinutes(30);
							}
							ErrorHandler.ShowError(Scheme.Instance.setResourceBox(resursbox_,false));
						}
						else
							Update(this);
						displayInformation(); 

						changesize = false;
						pull_upper = false;
						pull_lower = false;
					}
					else if ( ( ptCursor.Y > 1 && ptCursor.Y < this.Height )&& this.Height-ptCursor.Y < PULL_AREA || ptCursor.Y < PULL_AREA ) //Control.MousePosition.X Cursor.Position.Y-this.Location.Y-this.Parent.Location.Y-this.Parent.Parent.Location.Y > this.Height-10 )
						this.Cursor = System.Windows.Forms.Cursors.SizeNS;
					else 
						this.Cursor = System.Windows.Forms.Cursors.Arrow;
			
					inMouseMove = false;
				}
		*/
		// #1.02.001 - rows removed - end
		#endregion
		/// <summary>
		/// Get the largest with a resourcebox can have, in pixels
		/// </summary>
		public static int MAX_WIDTH 
		{
			get 
			{
				return SchemaUI.SCHEME_DAY_PIXELS*9/10;
			}
		}

		/// <summary>
		/// Get height of area on resourcebox that the user can resize the box, in pixel
		/// </summary>
		public static int PULL_AREA
		{
			get
			{
				return SchemaUI.SCHEME_HOUR_PIXELS/3;
			}
		}

		/// <summary>
		/// If the mousepointer leaves the box
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		// #1.02.001 - function removed - start
		/*		private void ResourceBoxUI_Leave(object sender, EventArgs e)
				{
					ResourceBoxUI_MouseMove(sender, new System.Windows.Forms.MouseEventArgs(0,0,Cursor.Position.X,Cursor.Position.Y,0));
				}
		*/		
		// #1.02.001 - end
		
		/// <summary>
		/// Lock the resourcebox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ResourceBoxUI_Click(object sender, EventArgs e)
		{
			if ( Scheme.Instance.editAllowed(resursbox_.StartTime) )
			{
				resursbox_.lockTime();
				Scheme.Instance.setResourceBox(resursbox_, false);
				displayInformation();
			}
		}
		
		/// <summary>
		/// Set the resourcebox as fre/unfree
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ResourceBoxUI3_Click(object sender, EventArgs e)
		{
			if ( Scheme.Instance.editAllowed(resursbox_.StartTime) )
			{
				if ( resursbox_.Free == true )
					resursbox_.Free = false;
				else
					resursbox_.Free = true;
				Scheme.Instance.setResourceBox(resursbox_, false);
				displayInformation();
			}
		}

		/// <summary>
		/// Kopiera resursbox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ResourceBoxUI_Click2(object sender, EventArgs e)
		{
			ResourceBox rb = new ResourceBox(resursbox_.StartTime, resursbox_.EndTime, resursbox_.Name, resursbox_.Free,
				resursbox_.Locked, resursbox_.LockedStartTime, resursbox_.LockedEndTime);
			// #1.01
			//			SchemaUI.addResourcebox(this.Parent, rb);
			Scheme.Instance.setResourceBox(rb, true);
			((SchemaUI)this.Parent).addResourcebox(rb);
			displayInformation();
		}

		/// <summary>
		/// Lock and zero the resourcebox
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ResourceBoxUI4_Click(object sender, EventArgs e)
		{
			resursbox_.zeroAndLock();
			Scheme.Instance.setResourceBox(resursbox_, false);
			displayInformation();
		}

		/// <summary>
		/// Get and set the resourcebox, if there is no resoucebox it is null
		/// </summary>
		public ResourceBox ResourceBox
		{
			get
			{
				try
				{
					return resursbox_;
				}
				catch
				{
					return null;
				}
			}
			set
			{
				resursbox_ = value;
			}
		}

		// #1.01.005 - start
		//		private void redraw(Rectangle rect)		#1.02.001 - row changed
		public void redraw(Graphics g, int x, int y, double scalex, double scaley, int days )
		{
			//			Graphics g = this.CreateGraphics(); #1.02.001 - row removed
			//			g.SetClip(rect); #1.02.001 - row removed
			DateTime dt = Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek;
			
			if ( shown && ResourceBox.StartTime < dt.AddDays(days) )
			{
				Region oldreg = g.Clip;
				int draw_x1, draw_y1, draw_x2, draw_y2;
				draw_x1 = x+(int)((Left)*scalex);
				draw_x2 = x+(int)((Left)*scalex)+(int)((Width)*scalex);
				draw_y1 = y+(int)((Top)*scaley);
				draw_y2 = y+(int)((Top)*scaley)+(int)((Height)*scaley);
				g.SetClip(new Rectangle(draw_x1, draw_y1, (draw_x2-draw_x1), (draw_y2-draw_y1)));
				g.FillRectangle(new SolidBrush(BackColor),draw_x1, draw_y1, (draw_x2-draw_x1), (draw_y2-draw_y1));
				if ( issel_ )
				{
					g.FillRectangle(new HatchBrush(HatchStyle.Percent50, BackColor, Color.White)/* SolidBrush(BackColor)*/,x+(int)(Left*scalex),y+(int)(Top*scaley),(int)(this.Width*scalex),(int)(this.Height*scaley));					
				}
				//				g.DrawRectangle(new Pen(new SolidBrush(Color.Black), 1),x+(int)(Left*scalex),y+(int)(Top*scaley),(int)((this.Width-1)*scalex),(int)((this.Height-1)*scaley)); #1.02.001 - rectangle change to 2 lines
				g.DrawLine(new Pen(new SolidBrush(Color.Black),1),new Point(draw_x1, draw_y1), new Point(draw_x1, draw_y2));
				g.DrawLine(new Pen(new SolidBrush(Color.Black),1),new Point(draw_x1, draw_y1), new Point(draw_x2, draw_y1));

				g.DrawLine(new Pen(new SolidBrush(Color.WhiteSmoke),1),new Point(draw_x1, draw_y2), new Point(draw_x2, draw_y2));
				g.DrawLine(new Pen(new SolidBrush(Color.WhiteSmoke),1),new Point(draw_x2, draw_y1), new Point(draw_x2, draw_y2));
				g.DrawString(this.Text, Font, new SolidBrush(Color.Black), draw_x1+1, draw_y1+1, new StringFormat());
				

				if ( ResourceBox.Free )
				{
					g.FillRectangle(new TextureBrush(new Bitmap(Application.StartupPath+"\\"+"streck.gif")), draw_x1, draw_y1, draw_x2-draw_x1, draw_y2-draw_y1);
				}
				if ( ResourceBox.Locked )
				{
					g.DrawImage(new Bitmap(Application.StartupPath+"\\"+"lock.gif"),new Point(draw_x2-(int)(12*scalex), draw_y2-(int)(12*scaley)));
				}
				g.SetClip(oldreg, CombineMode.Replace);
			}
			//			g.Dispose(); #1.02.001 - row removed
		}
		// #1.02.001 - function removed		
		/*		private void ResourceBoxUI_Paint(object sender, PaintEventArgs e)
				{
					redraw(e.ClipRectangle);
				}
		*/		
		// #1.02.001 - end 

		// #1.01.005 - slut

		// #1.02.001 - start 
		public bool isChangeSizeDown(int x, int y)
		{
			if ( x >= Left && y >= (Top+Height-PULL_AREA) && x <= (Left+Width) && y <= (Top+Height) )
			{
				return true;
			}
			return false;			
		}

		public bool isChangeSizeUp(int x, int y)
		{
			if ( x >= Left && y >= Top && x <= (Left+Width) && y <= (Top+PULL_AREA) )
			{
				return true;
			}
			return false;			
		}

		public bool isMove(int x, int y)
		{
			if ( x >= Left && y >= Top && x <= (Left+Width) && y <= (Top+Height) )
			{
				return true;
			}
			return false;
		}

		public void setNewCoords(Rectangle rect)
		{
			if ( Scheme.Instance.editAllowed(resursbox_.StartTime) )
			{
				/*				if ( this.Location.Y+this.Height >= ((24-SchemaUI.SCHEME_FIRST_HOUR)*SchemaUI.SCHEME_HOUR_PIXELS) )
									this.Location = new Point(this.Location.X, ((24-SchemaUI.SCHEME_FIRST_HOUR)*SchemaUI.SCHEME_HOUR_PIXELS)-this.Height);
								if ( this.Location.Y < 0 )
									this.Location = new Point(this.Location.X, 0); */

				int new_x = rect.X;//this.Location.X+this.Size.Width/2;
				int new_y = rect.Y;//this.Location.Y;
				int new_height = rect.Height;
				int new_width = rect.Width;

				DateTime starttime;
				DateTime endtime;
				SchemaUI.calcDateTime(new_x+new_width/2,new_y, out starttime);
				SchemaUI.calcDateTime(new_x+new_width/2,new_y+new_height, out endtime);

				// In case there is a new day, the resourcebox should end at 23:59 
				if ( endtime.Hour == 0 )
					endtime = endtime.AddMinutes(59-endtime.Minute).AddHours(23);
				resursbox_.StartTime = starttime;
				resursbox_.EndTime = endtime;

				ErrorHandler.ShowError(Scheme.Instance.setResourceBox(resursbox_,false));
			}
			else
				Update(this);
			//			displayInformation();
		}

		public void Hide()
		{
			shown = false;
		}

		public void Show()
		{
			shown = true;
		}

		public bool Visible()
		{
			return shown;
		}

		public bool isSelected
		{
			get 
			{
				return issel_;
			}
			set 
			{
				issel_ = value;
			}
		}

		public int Width 
		{
			get 
			{
				if ( ResourceBox.StartTime == ResourceBox.EndTime )
					return 0;
				int width = ResourceBoxUI.MAX_WIDTH;
				Point location = getLocation(resursbox_.StartTime, resursbox_.EndTime);

				// Make a check if there are any overlapping resourceboxes 
				if ( resursbox_.IdsNextTo.Count > 0 )
				{
					width = ResourceBoxUI.MAX_WIDTH/(resursbox_.IdsNextTo.Count+1);
					SortedList sl = new SortedList(resursbox_.IdsNextTo);
					sl.Add(resursbox_.Id, resursbox_.Id);
					location.X += sl.IndexOfKey(resursbox_.Id)*width;
				}
				return width;
			}
		}

		public int Height
		{
			get
			{
				if ( ResourceBox.StartTime == ResourceBox.EndTime )
					return 0;
				return SchemaUI.SCHEME_DAY_START_PIXEL+((resursbox_.EndTime.Hour-resursbox_.StartTime.Hour)*SchemaUI.SCHEME_HOUR_PIXELS+(resursbox_.EndTime.Minute-resursbox_.StartTime.Minute)*SchemaUI.SCHEME_HOUR_PIXELS/60);;
			}
		}

		public int Left
		{
			get
			{

				int x = getLocation(resursbox_.StartTime, resursbox_.EndTime).X;
				int width;
				// Make a check if there are any overlapping resourceboxes 
				if ( resursbox_.IdsNextTo.Count > 0 )
				{
					width = ResourceBoxUI.MAX_WIDTH/(resursbox_.IdsNextTo.Count+1);
					SortedList sl = new SortedList(resursbox_.IdsNextTo);
					sl.Add(resursbox_.Id, resursbox_.Id);
					x += sl.IndexOfKey(resursbox_.Id)*width;
				}
				return x;
			}
		}
		public int Top
		{
			get 
			{
				return getLocation(resursbox_.StartTime, resursbox_.EndTime).Y;
			}
		}

		public ContextMenu ContextMenu
		{
			get 
			{
				int v_index = 0;
				// Create the contextmenu
				ContextMenu conmen = new ContextMenu();
				conmen.MenuItems.Add("Ta bort");
				conmen.MenuItems[v_index++].Click += new System.EventHandler(remove_click);
			
				if ( resursbox_.Locked == true )
					conmen.MenuItems.Add("Lås upp");
				else
					conmen.MenuItems.Add("Lås");
				conmen.MenuItems[v_index++].Click +=new EventHandler(ResourceBoxUI_Click);

				conmen.MenuItems.Add("Ledig");
				if ( resursbox_.Free == true )
					conmen.MenuItems[v_index].Checked = true;
				else
					conmen.MenuItems[v_index].Checked = false;
				conmen.MenuItems[v_index++].Click +=new EventHandler(ResourceBoxUI3_Click);

				conmen.MenuItems.Add("Kopiera");
				conmen.MenuItems[v_index++].Click +=new EventHandler(ResourceBoxUI_Click2);
			
				conmen.MenuItems.Add("Nollställ");
				conmen.MenuItems[v_index++].Click += new EventHandler(ResourceBoxUI4_Click);
				conmen.MenuItems.Add("-");

				// Add all person and resources in contextmenu 
				for ( int i = 0; i < Scheme.Instance.ActivePersonsResources.Count; i++)
				{
					conmen.MenuItems.Add((String)Scheme.Instance.ActivePersonsResources.GetKey(i));
					conmen.MenuItems[i+v_index+1].Click += new System.EventHandler(changename_click);
					if ( (String)Scheme.Instance.ActivePersonsResources.GetKey(i) == resursbox_.Name )
						conmen.MenuItems[i+v_index+1].Checked = true;
				}
				return conmen;
			}
		}

		public Color BackColor
		{
			get
			{
				return Color.FromArgb(int.Parse(((ActivePersonResource)Scheme.Instance.ActivePersonsResources[resursbox_.Name]).Entity.Color));
			}
		}
		public String Text
		{
			get
			{
				String v_text = resursbox_.Name+"\n"+resursbox_.Duration+"h\n";
				if ( resursbox_.Locked == true )
					v_text += resursbox_.Deviation+"h";
				return v_text;
			}
		}

		public void unselect()
		{
			issel_ = false;
			Update(this);
		}

		public void select()
		{
			issel_ = true;
			Update(this);
		}
		public Rectangle Current_Rect
		{
			get 
			{
				return curr_rect;
			}
		}

		#region IDisposable Members

		public void Dispose()
		{
			// TODO:  Add ResourceBoxUI.Dispose implementation
			GC.SuppressFinalize(this);
		}

		#endregion
		// #1.02.001 - slut 
	}
}
