using System;
using System.Collections;

namespace SchemaLite
{

	/// <summary>
	/// ResourceBox
	/// </summary>
	public class ResourceBox : Subject, Observer
	{
		/// <summary>
		/// Starttime of the resource slot
		/// </summary>
		private DateTime starttime_;
		
		/// <summary>
		/// Endtime of the resource slot
		/// </summary>
		private DateTime endtime_;

		/// <summary>
		/// Name connected to the resourcebox
		/// </summary>
		private String name_;

		/// <summary>
		/// A unique íd for this resourcebox
		/// </summary>
		private int id_;

		/// <summary>
		/// Is this a free/non working slot
		/// </summary>
		private bool free_;

		/// <summary>
		/// Is this locked for a given slot
		/// </summary>
		private bool locked_;
		
		/// <summary>
		/// Locked starttime, only used when lock is true
		/// </summary>
		private DateTime lockedstarttime_;

		/// <summary>
		/// Locked endtime, only used when lock is true
		/// </summary>
		private DateTime lockedendtime_;

		/// <summary>
		/// All id of resourceboxes that overlap this resourcebox in time
		/// </summary>
		private SortedList idsnextto_ = new SortedList();

		/// <summary>
		/// 
		/// </summary>
		public ResourceBox()
		{
		}

		/// <summary>
		/// Id-list of all resoruceboxes that overlaps this one
		/// </summary>
		public SortedList IdsNextTo
		{
			get 
			{
				return idsnextto_;
			}
		}
		
		/// <summary>
		/// Fetches all ids for overlapping resourceboxes
		/// </summary>
		/// <param name="db">Database to collect information from</param>
		/// <returns>0 - If all Ok, else a error code</returns>
		public int updateIdsNextTo(DataBase db)
		{
			int fel = db.getResouceBoxesNextTo(this,out idsnextto_);
			if ( fel != 0 )
			{
				return fel;
			}
			this.Notify();
			// #1.02.001 - row added
			return 0;
		}

		/// <summary>
		/// Creats the resourcebox, puts all data in resourcebox.
		/// </summary>
		/// <param name="starttime">Starttime</param>
		/// <param name="endtime">Endtime</param>
		/// <param name="name">Name of person or resource</param>
		public ResourceBox(DateTime starttime, DateTime endtime, String name, bool free, bool locked, DateTime lockedstarttime, DateTime lockedendtime)
		{
			starttime_ = starttime;
			endtime_ = endtime;
			name_ = name;
			free_ = free;
			locked_ = locked;
			lockedstarttime_ = lockedstarttime;
			lockedendtime_ = lockedendtime;
		}

		/// <summary>
		/// Get and set starttime
		/// </summary>
		public DateTime StartTime
		{
			get
			{
				return starttime_;
			}
			set
			{
				starttime_ = value;
				// #1.02.001 - row added
				this.Notify();
			}
		}
		
		/// <summary>
		/// Get and set endtime
		/// </summary>
		public DateTime EndTime
		{
			get
			{
				return endtime_;
			}
			set
			{
				endtime_ = value;
				// #1.02.001 - row added
				this.Notify();
			}
		}
		
		/// <summary>
		/// Get and set name of the person or resource
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
				// #1.02.001 - row added
				this.Notify();
			}
		}

		/// <summary>
		/// Get and set id for this resourcebox
		/// </summary>
		public int Id
		{
			get
			{
				return id_;
			}
			set
			{
				id_ = value;
			}
		}

		/// <summary>
		/// Get if this time is marked as free/non working
		/// </summary>
		public bool Free
		{
			get
			{
				return free_;
			}
			set
			{
				free_ = value;
				// #1.02.001 - row added
				this.Notify();
			}
		}

		/// <summary>
		/// Get if this is a locked resourcebox
		/// </summary>
		public bool Locked
		{
			get
			{
				return locked_;
			}
		}

		/// <summary>
		/// Get and set the lockedstarttime
		/// </summary>
		public DateTime LockedStartTime
		{
			get
			{
				return lockedstarttime_;
			}
			set
			{
				lockedstarttime_ = value;
			}
		}

		/// <summary>
		/// Get and set the lockedendtime
		/// </summary>
		public DateTime LockedEndTime
		{
			get
			{
				return lockedendtime_;
			}
			set
			{
				lockedendtime_ = value;
			}
		}

		/// <summary>
		/// Lock this resourcebox
		/// </summary>
		public void lockTime()
		{
			if ( locked_ == false )
			{
				lockedstarttime_ = starttime_;
				lockedendtime_ = endtime_;
				locked_ = true;
			}
			else
				locked_ = false;
			// #1.02.001 - row added
			this.Notify();
		}

		/// <summary>
		/// Get the deviation between the locked time and current time in hours
		/// </summary>
		public double Deviation
		{
			get
			{
				if ( locked_ )
				{
					double curr_time = this.Duration;
					int tot_h = lockedendtime_.Hour-lockedstarttime_.Hour;
					int tot_min = lockedendtime_.Minute-lockedstarttime_.Minute;
					if ( tot_min == 59 )
						tot_min = 60;
					return curr_time-(tot_h+tot_min/60.0);
				}
				return 0;
			}
		}

		/// <summary>
		/// Get the duration in hours
		/// </summary>
		public double Duration
		{
			get
			{
				int tot_h = endtime_.Hour-starttime_.Hour;
                int tot_min = endtime_.Minute-starttime_.Minute;
				if ( endtime_.Minute == 59 || starttime_.Minute == 59 )
					tot_min++;
				return tot_h+tot_min/60.0;
			}
		}
		#region Observer Members

		public void Update(object subject)
		{
			// TODO:  Add ResourceBox.Update implementation
			this.Notify();
		}

		#endregion

		/// <summary>
		/// Lock and set the locktime to zero by have the start and endtime as the same time
		/// </summary>
		public void zeroAndLock()
		{
			this.lockTime();
			this.LockedEndTime = this.LockedStartTime;
//			this.Notify(); #1.02.001 - row removed
		}
	}
}
