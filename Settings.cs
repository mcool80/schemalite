using System;

namespace SchemaLite
{
	/// <summary>
	/// 
	/// </summary>
	public class Settings
	{
		/// <summary>
		/// Copany name
		/// </summary>
		public String companyname_;
		
		/// <summary>
		/// No of days before a resourcebox can be changed
		/// </summary>
		public int editstopdays_;
		public Settings(String companyname, int editstopdays)
		{
			companyname_ = companyname;
			editstopdays_ = editstopdays;
		}

		/// <summary>
		/// Check if edit is allowed on a specific date
		/// </summary>
		/// <param name="day">Date to check</param>
		/// <returns>True if edit is allowed on date</returns>
		public bool editAllowed(DateTime day)
		{
			if ( editstopdays_ == 0 )
				return true;
			if ( DateTime.Now.AddDays(editstopdays_).Ticks > day.Ticks )
				return false;
			return true;
		}

		/// <summary>
		/// Get and set the company name
		/// </summary>
		public String CompanyName
		{
			get
			{
				return companyname_;
			}
			set
			{
				companyname_ = value;
			}
		}

		/// <summary>
		/// Get and set no of days before a resourcebox should be changed
		/// </summary>
		public int EditStopDays
		{
			get
			{
				return editstopdays_;
			}
			set
			{
				editstopdays_ = value;
			}
		}
	}
}
