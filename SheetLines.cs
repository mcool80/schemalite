using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Data;
using System.Windows.Forms;

namespace SchemaLiteUI
{
	/// <summary>
	/// Summary description for SheetLines, and singleton class.
	/// Revision:
	/// ID			Date	Author				Text
	/// #1.01.002	051209	Markus Svensson		Solved issue SL-6 and SL-1
	/// </summary>
//	public class SheetLines : System.Windows.Forms.UserControl #1.01.002 - Changed line
	public class Sheet : System.Windows.Forms.UserControl, Observer
	{

		// #1.02.001 - added variables - start
		int org_x;
		int org_y;
		Rectangle org_rect;
		Rectangle prev_rect = new Rectangle(0,0,0,0);
		Rectangle curr_rect = new Rectangle(0,0,0,0);
		Graphics savedgrp = null;
		public bool redraw_allowed = true;
//		bool changesize;
		bool cs_up;
		bool cs_down;
		bool move;
		ResourceBoxUI currentrbui = null;
		GraphicsState gstate = null;
		// #1.02.001 - added variables - end
		/// <summary>
		/// Singleton instance
		/// </summary>
		private static Sheet instance_ = null;
		
		/// <summary>
		/// Get the instance of singleton class Scheme
		/// </summary>
		public static Sheet Instance
		{
			get
			{
				if ( instance_ == null )
					instance_ = new Sheet();
				return instance_;
			}
		}
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public Sheet()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
			
			// #1.02.001 - start
			this.MouseMove += new MouseEventHandler(Sheet_MouseMove);
			this.MouseDown += new MouseEventHandler(Sheet_MouseDown);
			// #1.02.001 - end
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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// SheetLines
			// 
			this.BackColor = System.Drawing.Color.WhiteSmoke;
			this.ForeColor = System.Drawing.Color.Black;
			this.Name = "SheetLines";
			this.Size = new System.Drawing.Size(536, 496);
//			this.SizeChanged += new System.EventHandler(this.SheetLines_SizeChanged); #1.01.002 - removed line
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.SheetLines_Paint);

		}
		#endregion

		/// <summary>
		/// Draw text in a graphic
		/// </summary>
		/// <param name="g">Graphic to draw in</param>
		/// <param name="x">X-position</param>
		/// <param name="y">Y-position</param>
		/// <param name="w">width of string, not used</param>
		/// <param name="h">height of string, not used</param>
		/// <param name="col">Color of string</param>
		/// <param name="text">Text to draw</param>
		public static void createText(Graphics g, int x, int y, int w, int h, System.Drawing.Color col, String text)
		{
			Font f = new Font("Arial", 7);
			Brush b = new SolidBrush(Color.Black);
			g.DrawString(text, f,b, new Point(x,y));
		}

		/// <summary>
		/// Creates a line in a graphic
		/// </summary>
		/// <param name="g">Graphic to draw in</param>
		/// <param name="x">X-position</param>
		/// <param name="y">Y-position</param>
		/// <param name="w">width of line</param>
		/// <param name="h">height of line</param>
		/// <param name="col">color of line</param>
		public static void createLine(Graphics g, int x, int y, int w, int h, System.Drawing.Color col)
		{
			int linewidth = ( w > h )? h : w;
			Point startp;
			Point endp;
			Pen p = new Pen(col, linewidth);
			if ( linewidth == h )
			{
				startp = new Point(x,y);
				endp = new Point(x+w,y);
			}
			else
			{
				startp = new Point(x,y);
				endp = new Point(x,y+h);
			}
			g.DrawLine(p, startp, endp);

		}
		private Size oldsize_ = new Size(0,0);

		/// <summary>
		/// Draws a sheme skeleton
		/// </summary>
		/// <param name="c">Control on which to draw on/over</param>
		/// <param name="g">Graphics element to draw</param>
		/// <param name="x">X-position in graphic points</param>
		/// <param name="y">Y-position in graphic points</param>
		/// <param name="scalex">The scale to use, 1 is as the control c indicates</param>
		/// <param name="scaley">The scale to use, 1 is as the control c indicates</param>
		/// <param name="days">No of days to show</param>
		public static void drawSkeleton(Control c, Graphics g, int x, int y, double scalex, double scaley, int days)
		{
			// Create lines for hours and half hours
			for ( int i = 0; i < 25-SchemaUI.SCHEME_FIRST_HOUR; i ++)
			{
				createLine( g, x, y+(int)(i * SchemaUI.SCHEME_HOUR_PIXELS*scaley), (int)(c.Width*scalex), 1, System.Drawing.Color.Black);
				createLine( g, x+(int)((SchemaUI.SCHEME_FIRST_DAY_PIXEL*scalex)), y+(int)((i * SchemaUI.SCHEME_HOUR_PIXELS+SchemaUI.SCHEME_HOUR_PIXELS/2)*scaley), (int)((c.Width-SchemaUI.SCHEME_FIRST_DAY_PIXEL)*scalex), 1, System.Drawing.Color.Gray);
				if ( i+SchemaUI.SCHEME_FIRST_HOUR < 24 )
				{
					createText(g, x, y+(int)((i * SchemaUI.SCHEME_HOUR_PIXELS)*scaley)+1, (int)((SchemaUI.SCHEME_FIRST_DAY_PIXEL-1)*scalex), (int)((SchemaUI.SCHEME_HOUR_PIXELS-2)*scaley), System.Drawing.Color.White, (i+SchemaUI.SCHEME_FIRST_HOUR)+":00");
				}

			}

			// Create day lines 
			for ( int i = 0; i < days; i ++)
			{
				createLine( g, x+(int)((SchemaUI.SCHEME_FIRST_DAY_PIXEL+(i+1)*SchemaUI.SCHEME_DAY_PIXELS)*scalex),y,1,(int)((25-SchemaUI.SCHEME_FIRST_HOUR)*SchemaUI.SCHEME_HOUR_PIXELS*scaley),System.Drawing.Color.Black);
			}	
		}

		private void redraw(Rectangle rect)
		{
			SuspendLayout();
			// #1.02.001 - new line
			if ( !redraw_allowed )
				return;

			Graphics g = this.CreateGraphics();
			GraphicsState gs = g.Save();
			
			// #1.02.001 new lines
			Region reg = null;
			if ( move || cs_up || cs_down )
			{
				drawTempRect(g);
			}
			
// #1.01.002 - new functionality
			if ( rect.Height != 0 )
			{
				reg = new Region(rect);
//				reg.Exclude(Sheet.getResourceBoxesRegion(g,0,0,1,1,14));
//				reg.Exclude(curr_rect);
//				g.SetClip(reg, CombineMode.Replace);
			}
			else
			{
				reg = g.Clip;
//				g.SetClip(reg, CombineMode.Replace);
//				Region reg = g.Clip;
//				reg.Exclude(Sheet.getResourceBoxesRegion(g,0,0,1,1,14));
//				reg.Exclude(curr_rect);
//				g.SetClip(reg, CombineMode.Replace);
			}
			
			// #1.02.001 - row removed
//			g.FillRectangle(new SolidBrush(Color.WhiteSmoke),0,0,this.Width,this.Height);				

			// Check if size changed, then the whole control should be cleared
// #1.01.002 - moved lines
/*			if ( oldsize_ != this.Size )
			{
				g.Clear(Color.WhiteSmoke);
			}
*/

			Region skel_reg = new Region(new Rectangle(0,0,0,0));
			skel_reg.Union(reg);
			skel_reg.Exclude(Sheet.getResourceBoxesRegion(g,0,0,1,1,14));
			if ( move || cs_up || cs_down )
				skel_reg.Exclude(new Rectangle(curr_rect.X, curr_rect.Y, curr_rect.Width, curr_rect.Height));
			g.SetClip(skel_reg, CombineMode.Replace);
			Sheet.drawSkeleton(this, g, 0,0,1,1,13);
//			g.SetClip(reg, CombineMode.Replace);
			
			// HACK: Better way? Do this, else it dont work
// #1.01.002 - removed lines
/*			if ( oldsize_ != this.Size )
			{
				oldsize_ = this.Size;
				g.Clear(Color.WhiteSmoke);	
			}	 */
// #1.02.001 - rows added - start
			g.Clip = reg;
			Sheet.drawResourceBoxes(g, 0, 0, 1, 1, SchemaUI.DAYS_SHOWN);			
			if ( move || cs_up || cs_down )
			{
				drawTempRect(g);
			}
			g.Flush();
			g.Restore(gs);		
// #1.02.001 - rows added - end
			g.Dispose();
			this.ResumeLayout();
		}

		private void SheetLines_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
//			redraw(); #1.01.002 - Changed line
			redraw(e.ClipRectangle);
		}

/* #1.01.002 - removed function
		private void SheetLines_SizeChanged(object sender, EventArgs e)
		{
			redraw();
		}
*/		

// #1.01.002 - Added function
		#region Observer Members

		public void Update(object subject)
		{
//			redraw(); #1.01.002 - Changed line
			Rectangle rect = new Rectangle(0,0,0,0);
			if ( subject.GetType() == Type.GetType("Rectangle") )
			{
				rect = (Rectangle)subject;
			}
			redraw(rect);
		}

		#endregion

// #1.02.001 - Added functions - start
		private void Sheet_MouseMove(object sender, MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Left )
			{
				if ( ! ( move || cs_up || cs_down ) )
				{
					ResourceBoxUI rbui = this.findRBUI(e.X, e.Y);
					if ( rbui != null )
					{
						if ( rbui.isChangeSizeUp(e.X, e.Y) )
						{
							org_x = e.X;
							org_y = e.Y;
							this.currentrbui = rbui;
							curr_rect = org_rect = rbui.Current_Rect;
							this.cs_up = true;
						}
						else if ( rbui.isChangeSizeDown(e.X, e.Y) )
						{
							org_x = e.X;
							org_y = e.Y;
							this.currentrbui = rbui;
							curr_rect = org_rect = rbui.Current_Rect;
							this.cs_down = true;
						}
						else if ( rbui.isMove(e.X, e.Y) )
						{
							org_x = e.X;
							org_y = e.Y;
							this.move = true;
							this.currentrbui = rbui;
							curr_rect = org_rect = rbui.Current_Rect;
						}
					}					
				}
			}
			if ( move && e.Button == MouseButtons.Left )
			{
				//				this.Invalidate(new Rectangle(curr_rect.X-5, curr_rect.Y-5, curr_rect.Width+10, curr_rect.Height+10));
				Point p = PointToClient(Cursor.Position);
				int diff_x = org_x - p.X;
				int diff_y = org_y - p.Y;
				Rectangle rect = new Rectangle(org_rect.X-diff_x, org_rect.Y-diff_y, org_rect.Width, org_rect.Height);

				//				Rectangle rect = curr_rect;
				
				prev_rect = curr_rect;
				curr_rect = rect;
				
//				this.Invalidate(new Rectangle(Math.Min(curr_rect.X-1,prev_rect.X-1), Math.Min(curr_rect.Y-1,prev_rect.Y-1),Math.Max(curr_rect.X+curr_rect.Width, prev_rect.X+prev_rect.Width)-Math.Min(curr_rect.X,prev_rect.X)+20,Math.Max(curr_rect.Y+curr_rect.Height, prev_rect.Y+prev_rect.Height)-Math.Min(curr_rect.Y,prev_rect.Y)+20));
				this.Invalidate(prev_rect);
//				this.Update();
			}
			else if ( move ) 
			{
				currentrbui.setNewCoords(curr_rect);
				move = false;
//				this.Invalidate(new Rectangle(Math.Min(curr_rect.X-1,prev_rect.X-1), Math.Min(curr_rect.Y-1,prev_rect.Y-1),Math.Max(curr_rect.X+curr_rect.Width, prev_rect.X+prev_rect.Width)-Math.Min(curr_rect.X,prev_rect.X)+20,Math.Max(curr_rect.Y+curr_rect.Height, prev_rect.Y+prev_rect.Height)-Math.Min(curr_rect.Y,prev_rect.Y)+20));
				this.Invalidate();
				this.Update();
			}
			else if ( ( cs_up || cs_down ) && e.Button == MouseButtons.Left )
			{
				Point p = PointToClient(Cursor.Position);
//				int diff_x = org_x - p.X;
				int diff_y = org_y - p.Y;

				Rectangle rect = new Rectangle(0,0,0,0);
				if ( cs_up )
				{
					if ( org_rect.Y-diff_y > org_rect.Y+org_rect.Height-SchemaUI.SCHEME_HOUR_PIXELS/2 )
						diff_y = -org_rect.Height+SchemaUI.SCHEME_HOUR_PIXELS/2;
					rect = new Rectangle(org_rect.X, org_rect.Y-diff_y, org_rect.Width,Math.Max(SchemaUI.SCHEME_HOUR_PIXELS/2,org_rect.Height)+diff_y);
				}
				else if ( cs_down )
				{
					rect = new Rectangle(org_rect.X, org_rect.Y, org_rect.Width, Math.Max(SchemaUI.SCHEME_HOUR_PIXELS/2,org_rect.Height-diff_y));
				}

				//				Rectangle rect = curr_rect;
								
				prev_rect = curr_rect;
				curr_rect = rect;
//				this.Invalidate(new Rectangle(Math.Min(curr_rect.X-1,prev_rect.X-1), Math.Min(curr_rect.Y-1,prev_rect.Y-1),Math.Max(curr_rect.X+curr_rect.Width, prev_rect.X+prev_rect.Width)-Math.Min(curr_rect.X,prev_rect.X)+20,Math.Max(curr_rect.Y+curr_rect.Height, prev_rect.Y+prev_rect.Height)-Math.Min(curr_rect.Y,prev_rect.Y)+20));
//				this.Invalidate(new Rectangle(Math.Min(curr_rect.X,prev_rect.X), Math.Min(curr_rect.Y,prev_rect.Y),Math.Max(curr_rect.X+curr_rect.Width, prev_rect.X+prev_rect.Width)-Math.Min(curr_rect.X,prev_rect.X)+5,Math.Max(curr_rect.Y+curr_rect.Height, prev_rect.Y+prev_rect.Height)-Math.Min(curr_rect.Y,prev_rect.Y)+20));
				this.Invalidate(prev_rect);
//				this.Update();
			}
			else if ( cs_up || cs_down )
			{
				if ( curr_rect.Height < SchemaUI.SCHEME_HOUR_PIXELS/2 )
					curr_rect.Height = SchemaUI.SCHEME_HOUR_PIXELS/2;
				currentrbui.setNewCoords(curr_rect);
				cs_up = false;
				cs_down = false;
//				this.Invalidate(new Rectangle(Math.Min(curr_rect.X,prev_rect.X), Math.Min(curr_rect.Y,prev_rect.Y),Math.Max(curr_rect.X+curr_rect.Width, prev_rect.X+prev_rect.Width)-Math.Min(curr_rect.X,prev_rect.X)+20,Math.Max(curr_rect.Y+curr_rect.Height, prev_rect.Y+prev_rect.Height)-Math.Min(curr_rect.Y,prev_rect.Y)+20));
				this.Invalidate();
//				this.Update();
			}
			else
			{
				ResourceBoxUI rbui = this.findRBUI(e.X, e.Y);
				if ( rbui != null )
				{
					if ( rbui.isChangeSizeDown(e.X,e.Y) || rbui.isChangeSizeUp(e.X,e.Y) )
					{
						Cursor.Current = Cursors.SizeNS;
					}
					else
						Cursor.Current = Cursors.Default;
				}
			}
		}

		private void Sheet_MouseDown(object sender, MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Left )
			{
				ResourceBoxUI rbui = this.findRBUI(e.X, e.Y);
				if ( rbui != null )
				{
					rbui.displayInformation();
				}	
/*				SchemaUI.currentrbui = this;
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
				displayInformation(); */
			}
			if ( e.Button == MouseButtons.Right )
			{
				ResourceBoxUI rbui = this.findRBUI(e.X, e.Y);
				if ( rbui != null )
				{
					this.ContextMenu = rbui.ContextMenu;
					this.ContextMenu.Show(this, new Point(e.X, e.Y));
				}
				else
					this.ContextMenu = null;
			}
		}

		public static void drawResourceBoxes(Graphics g, int x, int y, double scalex, double scaley, int days)
		{
			foreach ( ResourceBoxUI rbui in SchemaUI.ResouceBoxes.Values )
			{
				if ( g.Clip.IsVisible(rbui.Current_Rect) )
					rbui.redraw(g, x, y, scalex, scaley, days);
			}
		}

		public static Region getResourceBoxesRegion(Graphics g, int x, int y, double scalex, double scaley, int days)
		{
			Region reg = new Region(new Rectangle(0,0,0,0));
			foreach ( ResourceBoxUI rbui in SchemaUI.ResouceBoxes.Values )
			{
				if ( rbui.Visible() )
					reg.Union(new Region(rbui.Current_Rect));
			}
			return reg;
		}

		private ResourceBoxUI findRBUI(int x, int y)
		{
			foreach ( ResourceBoxUI rbui in SchemaUI.ResouceBoxes.Values )
			{
				if ( rbui.isMove(x,y) )
					return rbui;
			}
			return null;
		}

		private void drawTempRect(Graphics g)
		{
//				g.Restore(gstate);
			g.SetClip(curr_rect);
			g.FillRectangle(new SolidBrush(currentrbui.BackColor),curr_rect.X+1,curr_rect.Y+1,curr_rect.Width-2,curr_rect.Height-2);
			g.DrawRectangle(new Pen(new SolidBrush(Color.Black),1), new Rectangle(curr_rect.X,curr_rect.Y,curr_rect.Width,curr_rect.Height));
			g.ResetClip();
//				prev_rect = curr_rect;
		}
/*		private Rectangle curr_rect
		{
			get
			{
				Point p = PointToClient(Cursor.Position);
				int diff_x = org_x - p.X;
				int diff_y = org_y - p.Y;
				Rectangle rect = new Rectangle(org_rect.X-diff_x, org_rect.Y-diff_y, org_rect.Width, org_rect.Height);
				return rect;
			}
		} */
	}
}
