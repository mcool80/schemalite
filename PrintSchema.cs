using System;
using System.Windows.Forms;
using System.Drawing;
using System.Drawing.Drawing2D;
using SchemaLite;
namespace SchemaLiteUI
{
	/// <summary>
	/// Summary description for PrintSchema.
	/// Revision:
	/// ID			Date	Author				Text
	///	1.01.002	051208	Markus Svensson		Solves issue SL-6 and SL-1
	///	1.01.005	051222	Markus Svensson		Solves issue SL-4
	/// </summary>
	public class PrintSchema : System.Drawing.Printing.PrintDocument
	{
		Control.ControlCollection header_;
		Control.ControlCollection data_;
		Color backcolor_;
		int width_;
		int height_;
		String headertext_;
		Control schemaui_;
		public PrintSchema(Control schemaUI, Control.ControlCollection header, Control.ControlCollection data, Color backcolor, int width, int height, String headertext)
		{
			this.PrintPage += new System.Drawing.Printing.PrintPageEventHandler(PrintSchema_PrintPage);
			header_ = header;
			data_ = data;
			backcolor_ = backcolor;
			width_ = width;
			height_ = height;
			headertext_ = headertext;
			schemaui_ = schemaUI;
		}

		/// <summary>
		/// Wordwrap the text
		/// </summary>
		/// <param name="intext">Text to wordwrap</param>
		/// <param name="size">The longest line</param>
		/// <returns>The wordwraped text</returns>
		private String wordwrap(String intext, int size)
		{
			String text = "";
			String[] texter = intext.Split(' ');
			int i = 0;
			foreach ( String a in texter )
			{
				i += (a.Length+1);
				if ( i > size )
				{
					text += "\n";
					i = 0;
				}
				else
					text += " ";
				text += a;
			}
			return text;
		}

		/// <summary>
		/// Draw graphics on a Graphics object
		/// </summary>
		/// <param name="g">Graphics object</param>
		/// <param name="controls">The controls</param>
		/// <param name="scale">How to scale the controls</param>
		/// <param name="posX">Position in x</param>
		/// <param name="posY">Position in y</param>
		/// <param name="typestr">Type of control to add</param>
		void DrawForm(Graphics g, Control.ControlCollection controls, double scalex, double scaley, int posX, int posY, string typestr)
		{
			//  Cycle through each control on the form and paint it to the printe
			foreach (Control c in controls)
			{

				//  Get the time of the next control so we can unbox it
				string strType = c.GetType().ToString().Substring(c.GetType().ToString().LastIndexOf(".") + 1);
				switch (strType)
				{
					// Print activityUI
					case "ActivityUI":
					{
						TextBox t = (TextBox)c;

						// Print string
						String text = this.wordwrap(t.Text, 13);
						if ( text.Substring(1,1) != "<" )
							g.DrawString(text, t.Font, new SolidBrush(t.ForeColor), posX+(int)(t.Left*scalex) + 2, 
								posY+(int)(t.Top*scaley) + 2, new StringFormat(StringFormatFlags.FitBlackBox));
					}
						break;
					
// #1.02.001 - rows removed - start
					// Print ResourceboxUI
/*					case "ResourceBoxUI":
					{
//						Label t = (Label)c;	- # 1.01.005 - replaced line
						UserControl t = (UserControl)c;
						if ( t.Visible )
						{
							if ( ((ResourceBoxUI)c).ResourceBox.StartTime >= Scheme.Instance.SelectedWeek.DateOfFirstDayInWeek.AddDays(7) )
								break;

									

							//  Draw a text box by drawing a pushed in button and filling the rectangle with the background color and the text
							//  of the TextBox control

							// First the sunken border
							Rectangle rect = new Rectangle(posX+(int)(t.Left*scalex), posY+(int)(t.Top*scaley), (int)(t.Width*scalex)+1, (int)(t.Height*scaley)+1);
							ControlPaint.DrawBorder(g, rect, t.BackColor,ButtonBorderStyle.Solid);
							//  Then fill it with the background of the textbox
							Brush brush = new SolidBrush(t.BackColor);
							if ( typestr == "ResourceBoxUI" )
							{
								if ( ((ResourceBoxUI)c).ResourceBox.Free )
									brush = new HatchBrush(HatchStyle.DashedUpwardDiagonal, Color.Black, t.BackColor);
							}
							g.FillRectangle(brush, posX+(int)(t.Left*scalex)+1, posY+(int)(t.Top*scaley) + 1, (int)(t.Width*scalex)-2, (int)(t.Height*scaley)-2);
								
							// Finally draw the string inside
//							String text = this.wordwrap(t.Text, 12);	#1.01.005 - removed line
*/
/*							#1.01.005 - changed line
  							g.DrawString( t.Text, t.Font, new SolidBrush(t.ForeColor), posX+(int)(t.Left*scalex)+2, 
								posY+(int)(t.Top*scaley), new StringFormat()); 
 */								
/*							g.DrawString( t.Text, t.Font, new SolidBrush(t.ForeColor), new Rectangle(posX+(int)(t.Left*scalex)+2, 
								posY+(int)(t.Top*scaley),(int)(t.Width*scalex)+1, (int)(t.Height*scaley)+1), new StringFormat()); 
						}
					}
						break;
*/
						// #1.02.001 - rows removed - end
						
					// Print date or Schema skeleton
					case "Label":
					{
						Label t = (Label)c;
						if ( typestr == "Header" )
						{
							// Draw rectangle
							double tmp_scalex = scalex;
							double tmp_scaley = scaley;
							
							Rectangle rect = new Rectangle(posX+(int)(t.Left*scalex), posY+(int)(t.Top*scaley), (int)(t.Width*tmp_scalex)+1, (int)(t.Height*tmp_scaley)+1);
							ControlPaint.DrawBorder(g, rect, t.BackColor,ButtonBorderStyle.Solid);
							//  Then fill it with the background of the textbox
							Brush brush = new SolidBrush(t.BackColor);
							g.FillRectangle(brush, posX+(int)(t.Left*scalex)+1, posY+(int)(t.Top*scaley) + 1, (int)(t.Width*tmp_scalex)-2, (int)(t.Height*tmp_scaley)-2);
								
							// Finally draw the string inside
							String text = this.wordwrap(t.Text, 12);
							
							g.DrawString(text, t.Font, new SolidBrush(t.ForeColor), posX+(int)(t.Left*scalex)+2, 
								posY+(int)(t.Top*scaley), new StringFormat()); 
							
						}
					}
						break;
				}
			}

		}

		private void PrintSchema_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
		{
			// Default height per hour and witdth per day
			double def_width = 90.0; // 100.0;	#1.01.002 - changed line
			double def_height = 26.0;
			
			e.Graphics.SetClip(new Rectangle(0,0,735,1500));

			double scalex = def_width/SchemaUI.SCHEME_DAY_PIXELS;
			double scaley = def_height/SchemaUI.SCHEME_HOUR_PIXELS;
			// Add skeleton, directly to graphics
//			SheetLines.drawSkeleton(schemaui_, e.Graphics, 50, 190, scalex, scaley, 6); #1.01 - changed name of class
			Sheet.drawSkeleton(schemaui_, e.Graphics, 50, 190, scalex, scaley, 6);
			// Add dates and activitys
			DrawForm(e.Graphics, header_, scalex, scaley, 100, 80, "Header");
			// Add resourceboxes
//			DrawForm(e.Graphics, data_, scalex, scaley, 50, 190, "ResourceBoxUI");
			Sheet.drawResourceBoxes(e.Graphics, 50, 190, scalex, scaley, 7);

			int x,y;
			Font font = new Font("Arial", 16, FontStyle.Bold);
			y = 50;
			// #1.01.002 - changed x-position
//			x = (int)(((int)e.Graphics.ClipBounds.Width)/e.Graphics.DpiX/2) - ((int)e.Graphics.MeasureString(headertext_, font).Width/4);
			x = 150;
			// Add header
			e.Graphics.DrawString(headertext_, font, new SolidBrush(Color.Black), x, y, new StringFormat()); 
		}
	}
}
