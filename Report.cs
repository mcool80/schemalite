using System;
using System.Collections;

namespace SchemaLite
{
	/// <summary>
	/// A report
	/// </summary>
	public class Report
	{
		private SortedList reportlines_;
		/// <summary>
		/// 
		/// </summary>
		public Report()
		{
		}

		/// <summary>
		/// Gets all reportslines in this report
		/// </summary>
		/// <returns></returns>
		public SortedList getReportLines()
		{
			return reportlines_;
		}

		/// <summary>
		/// Creates the report
		/// </summary>
		/// <param name="startdate">Starttime</param>
		/// <param name="enddate">Endtime</param>
		/// <param name="db">Database to collect data from</param>
		/// <returns>0 - If all ok, else a database error</returns>
		public int startreport(DateTime startdate, DateTime enddate, DataBase db)
		{
			reportlines_ = new SortedList();
			SortedList lista;
			int fel = db.getResourceBoxes(startdate,enddate, out lista);
			if ( fel != 0 )
			{
				return fel;
			}
			for ( int i=0; i < lista.Count; i++)
			{
				ResourceBox rb = (ResourceBox)lista[lista.GetKey(i)];
				// Calculate time in hours 
				if ( !rb.Free )
				{
					if ( !reportlines_.ContainsKey(rb.Name) )
						reportlines_[rb.Name] = new ReportLine(rb.Name, rb.Duration, rb.Deviation);
					else
					{
						((ReportLine)reportlines_[rb.Name]).Hours += rb.Duration;
						((ReportLine)reportlines_[rb.Name]).Deviation += rb.Deviation;
					}
				}
			}
			return 0;
		}
	}
}
