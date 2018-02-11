using System;

namespace SchemaLite
{
	/// <summary>
	/// Summary description for Activity.
	/// </summary>
	public class Activity : Subject
	{
		/// <summary>
		/// Day for activity
		/// </summary>
		private DateTime day_;
		
		/// <summary>
		/// Activity text
		/// </summary>
		private String text_;
		public Activity()
		{
		}
		/// <summary>
		/// Creates an activity
		/// </summary>
		/// <param name="day">Day of the activity</param>
		/// <param name="text">Text about activity(s)</param>
		public Activity(DateTime day, String text)
		{
			day_ = day;
			text_ = text;
		}

		/// <summary>
		/// Get day of activity
		/// </summary>
		public DateTime Day
		{
			get
			{
				return day_;
			}
			set
			{
				day_ = value;
			}
		}

		/// <summary>
		/// Get and set the activity text
		/// </summary>
		public String Text
		{
			get
			{
				return text_;
			}
			set
			{
				text_ = value;
			}
		}
	}
}
