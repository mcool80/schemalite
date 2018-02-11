using System;
using System.Collections;
using System.Drawing;
using System.Windows.Forms;
using SchemaLite;
namespace SchemaLiteUI
{
	/// <summary>
	/// Summary description for ActivityUI.
	/// Revision:
	/// ID			Date	Author				Text
	///	1.01.003	051208	Markus Svensson		Solves issue SL-9
	/// </summary>
	public class ActivityUI : System.Windows.Forms.TextBox, Observer
	{
		Activity activity_;
		bool exist_;
		bool edited_ = false;

		/// <summary>
		/// Creates the UI for a corresponding Activity
		/// </summary>
		/// <param name="activity">The activity to represent</param>
		/// <param name="exist">True of the activity exists in system, else this will be written to system on change</param>
		public ActivityUI(Activity activity, bool exist)
		{
			this.Font = new Font("Arial", 7);	// #1.01.003 - changed font
			this.BackColor = System.Drawing.Color.White;
			// this.BorderStyle = System.Windows.Forms.BorderStyle.None; - #1.01.003 removed line
			this.Click +=new EventHandler(ActivityUI_Click);
			this.Leave +=new EventHandler(ActivityUI_Leave);
			this.KeyPress +=new KeyPressEventHandler(ActivityUI_KeyPress);
			this.Enter += new EventHandler(ActivityUI_Enter);

			activity_ = activity;
			/* # 1.01.003 - moved lines
			Text = activity_.Text;
			if ( Text == "" )
				Text = "<Fyll i aktivitet>";
			*/
			activity_.AddObserver(this);
			exist_ = exist;
			this.AutoSize = false;
			/*
			this.Width = SchemaUI.SCHEME_DAY_PIXELS*90/100;
			this.Height = 38;
			SelectedDay day = new SelectedDay(activity.Day);
			this.Left = SchemaUI.SCHEME_DAY_PIXELS*day.DayInWeek;
			this.Top = 52;
			*/
			this.Multiline = true;
			this.WordWrap = true;
			this.BringToFront();
			
			// # 1.01.003 new lines
			this.AutoSize = false;		
			activity_.AddObserver(this);
//			this.changeActivity(activity, exist);	#1.01.003 - removed line
		}

		/// <summary>
		/// Change an activity in the current UI
		/// </summary>
		/// <param name="activity">New Activity to display</param>
		/// <param name="exist">True if the activity exist in db</param>
		public void changeActivity(Activity activity, bool exist)
		{
			try
			{
				activity_.RemoveObserver((Observer)activity_);
			}
			catch
			{
			}
			activity_ = activity;
			exist_ = exist;
			this.Update(this);
		}

		#region Observer Members

		public void Update(object subject)
		{
			this.SuspendLayout();
			SelectedDay day = new SelectedDay(activity_.Day);
			this.Top = 52;
			this.Left = SchemaUI.SCHEME_DAY_PIXELS*day.DayInWeek;
			this.Width = SchemaUI.SCHEME_DAY_PIXELS*90/100;
			this.Height = 38;
			
			// #1.01.003 - changed code
			if ( activity_.Text == "" )
				Text = "<Fyll i aktivitet>";
			else
				Text = activity_.Text;

			this.BringToFront();
			if ( edited_ == true )
				this.BorderStyle = BorderStyle.Fixed3D;
			else
				this.BorderStyle = BorderStyle.None;
			this.ResumeLayout();
		}

		#endregion

		/// <summary>
		/// Save data to db
		/// </summary>
		private void savedata()
		{
			activity_.Text = this.Text;
			Scheme.Instance.setActivity(activity_, !exist_);
			exist_ = true;
			if ( Text == "" )
				Text = "<Fyll i aktivitet>";
		}

		/// <summary>
		/// Set the activityfield as editable
		/// </summary>
		public void startModify()
		{
			edited_ = true;
			Update(this);
		}

		/// <summary>
		/// Set the field as not editable and save data
		/// </summary>
		public void stopModify()
		{
			if ( edited_ == true )
			{
				savedata();
				this.BorderStyle = BorderStyle.None;
				edited_ = false;
			}
			Update(this);
		}

		/// <summary>
		/// The user has clicked the activityfield, start edititing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ActivityUI_Click(object sender, EventArgs e)
		{
			startModify();
			Text = activity_.Text;
		}

		/// <summary>
		///  The user leaves activityfield, save data given i field
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ActivityUI_Leave(object sender, EventArgs e)
		{
			if ( this.ContainsFocus == false )
				stopModify();
		}

		/// <summary>
		/// A key is pressed, if it is [enter] save data and leave field
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ActivityUI_KeyPress(object sender, KeyPressEventArgs e)
		{
			if ( e.KeyChar == 13 )
			{
				stopModify();
				Parent.Focus();
			}
		}

		/// <summary>
		/// If user enters activityfield start editing
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void ActivityUI_Enter(object sender, EventArgs e)
		{
			startModify();
		}

		// #1.01.003 - start
		public Activity Activity
		{
			get 
			{
				return activity_;
			}
		}
		// #1.01.003 - slut
	}
}
