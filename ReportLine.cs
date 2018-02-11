using System;

namespace SchemaLite
{
	/// <summary>
	/// A line, part of a report
	/// </summary>
	public class ReportLine
	{
		/// <summary>
		/// Name
		/// </summary>
		private String name_;
		
		/// <summary>
		/// Hours worked
		/// </summary>
		private double hours_;

		/// <summary>
		/// Devation from locked time
		/// </summary>
		private double deviation_;

		/// <summary>
		/// Creates a reportline
		/// </summary>
		/// <param name="name">Name of person or resource</param>
		/// <param name="hours">Hours the person or resource is allocted for</param>
		public ReportLine(String name, double hours, double deviation)
		{
			name_ = name;
			hours_ = hours;
			deviation_ = deviation;
		}

		/// <summary>
		/// Get and set name of person or resource
		/// </summary>
		public String Name
		{
			get
			{
				return name_;
			}
			set
			{
				name_ = value;
			}
		}

		/// <summary>
		/// Get and set hours allocated to person or resource
		/// </summary>
		public double Hours
		{
			get
			{
				return hours_;
			}
			set
			{
				hours_ = value;
			}
		}
		
		/// <summary>
		/// Get and set deviation from locked time
		/// </summary>
		public double Deviation
		{
			get
			{
				return deviation_;
			}
			set
			{
				deviation_ = value;
			}
		}
	}
}
