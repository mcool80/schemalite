using System;

namespace SchemaLite
{
	/// <summary>
	/// This code acts as a day, to recive information about that day
	/// </summary>
	public class SelectedDay : SchemaLite.SelectedWeek
	{
		private String[] daynames_ = { "mån", "tis", "ons", "tor", "fre", "lör", "sön" };
		
		/// <summary>
		/// Creats a specified day
		/// </summary>
		/// <param name="date">Date of the day to create</param>
		public SelectedDay(DateTime date)
		{
			date_ = date;
		}

		/// <summary>
		/// Gets number of day
		/// </summary>
		public int Day
		{
			get
			{
				return date_.Day;
			}
		}

		/// <summary>
		/// Gets name of day.
		/// </summary>
		/// <returns></returns>
		public String getNameofDay()
		{
			return daynames_[this.DayInWeek%7];
		}

		/// <summary>
		/// Gets the date of the day.
		/// </summary>
		/// <returns></returns>
		public DateTime getDateTime()
		{
			return this.date_;
		}
	}
}
