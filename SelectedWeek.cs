using System;
using System.Collections;
using System.Globalization;

namespace SchemaLite 
{
	/// <summary>
	/// A speific week, used to recive more information about week
	/// </summary>
	public class SelectedWeek : IComparable
	{
		/// <summary>
		/// Name of all month in a year
		/// </summary>
		private String[] monthnames_ = { "jan", "feb", "mar", "apr", "maj", "jun", "jul", "aug", "sep", "okt", "nov", "dec" };		

		/// <summary>
		/// Calender
		/// </summary>
		protected Calendar cal_;

		/// <summary>
		/// A given datetime for the week
		/// </summary>
		protected DateTime date_;

		/// <summary>
		/// Cultureinformation
		/// </summary>
		protected CultureInfo myCI_;

		/// <summary>
		/// Creates a week from a given date
		/// </summary>
		/// <param name="date">A date from the week to be created</param>
		public SelectedWeek(DateTime date)
		{
			// Gets the Calendar instance associated with a CultureInfo. 
			myCI_ = new CultureInfo("sv-SE");
			cal_ = myCI_.Calendar;
			date_ = date;
		}

		/// <summary>
		/// Gets first date in the week at time 00:00:00
		/// </summary>
		public DateTime DateOfFirstDayInWeek
		{
			get
			{
				return this.date_.AddDays(-this.DayInWeek).AddHours(-date_.Hour).AddMinutes(-date_.Minute).AddSeconds(-date_.Second);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public SelectedWeek()
		{
		}

		/// <summary>
		/// Get and set year of the week
		/// </summary>
		public int Year
		{
			get
			{
				return date_.Year;
			}
			set
			{
				cal_.AddYears(date_, cal_.GetYear(date_)-value);
			}
		}

		/// <summary>
		/// Get and set month of the week
		/// </summary>
		public int Month
		{
			get
			{
				return date_.Month;
			}
			set
			{
				cal_.AddMonths(date_, cal_.GetMonth(date_)-value);
			}
		}

		/// <summary>
		/// Get and set week number of the week.
		/// </summary>
		/// Revision:
		/// #1.04 - Changed week number to whatever the first day in the week has (eg. the first week of the year has the same week as the monday in that week 1 or 53/52)
		public int Week
		{
			get
			{
				// #1.04
				return cal_.GetWeekOfYear(date_.AddDays(-this.DayInWeek),myCI_.DateTimeFormat.CalendarWeekRule,myCI_.DateTimeFormat.FirstDayOfWeek);
			}
			set
			{
				// TODO: Check if it is a new year or not
				cal_.AddWeeks(date_, cal_.GetWeekOfYear(date_,myCI_.DateTimeFormat.CalendarWeekRule,myCI_.DateTimeFormat.FirstDayOfWeek)-value);
			}
		}

		/// <summary>
		/// Get day number of the current given date, monday=0 sunday=6
		/// </summary>
		public int DayInWeek
		{
			get
			{
				int dayofweek = 0;
				switch (date_.DayOfWeek)
				{
					case DayOfWeek.Monday:
						dayofweek = 0;
						break;
					case DayOfWeek.Tuesday:
						dayofweek = 1;
						break;
					case DayOfWeek.Wednesday:
						dayofweek = 2;
						break;
					case DayOfWeek.Thursday:
						dayofweek = 3;
						break;
					case DayOfWeek.Friday:
						dayofweek = 4;
						break;
					case DayOfWeek.Saturday:
						dayofweek = 5;
						break;
					case DayOfWeek.Sunday:
						dayofweek = 6;
						break;
					default:
						break;
				}
				return dayofweek;
			}
		}

		/// <summary>
		/// SortedList with strings with monthname for each day of a week
		/// </summary>
		public SortedList MonthNames
		{
			get
			{
				SortedList sl = new SortedList();

				DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
				dtfi.Calendar = cal_;
				
				DateTime date = new DateTime(date_.Ticks);
				date = date.AddDays(-DayInWeek);
				for ( int i = 0; i < 14; i++)
				{
					sl.Add(i, monthnames_[date.Month-1]);
					date = date.AddDays(1);
				}
				return sl;
			}
		}

		/// <summary>
		/// Sortedlist with SelectedDay classes
		/// </summary>
		public SortedList WeekDays
		{
			get
			{
				SortedList sl = new SortedList();

				DateTimeFormatInfo dtfi = new DateTimeFormatInfo();
				dtfi.Calendar = cal_;
				DateTime date = new DateTime(date_.Ticks);
				date = date.AddDays(-this.DayInWeek);
				for ( int i = 0; i < 14; i++)
				{
					SelectedDay selday = new SelectedDay(date);
					sl.Add(i, selday);
					date = date.AddDays(1);
				}
				return sl;
			}
		}

		/// <summary>
		/// Sets a new date for this week
		/// </summary>
		/// <param name="date">Date in a week to set</param>
		public void setDate(DateTime date)
		{
			date_ = date;
		}

		/// <summary>
		/// Compares selectedweek to an other selectedweek
		/// </summary>
		/// <param name="obj1">An other selectedweek</param>
		/// <returns>0, if they are the same week, 1 if this week is after, -1 if this week is before</returns>
		public int CompareTo(object obj1)
		{
			SelectedWeek selweek = (SelectedWeek) obj1;
			SelectedWeek selweek2 = (SelectedWeek) this;
			if ( selweek2.Week == selweek.Week && selweek2.Year == selweek.Year )
				return 0;
			if ( selweek2.Year == selweek.Year )
				if ( selweek2.Week > selweek.Week  )
					return 1;
				else
					return -1;
			if ( selweek2.Year > selweek.Year )
				return 1;
			else
				return -1;
			return 0;
		}

	}
}
 