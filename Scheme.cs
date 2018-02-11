using System;
using System.IO;
using System.Collections;
using System.Windows.Forms;
using System.Diagnostics;
namespace SchemaLite
{
	/// <summary>
	/// Scheme class, this is a singleton class
	/// Scheme controls the logic in the program
	/// ID			Date	Author				Text
	/// #1.01.003	051209	Markus Svensson		Solved issue SL-6
	/// </summary>
	public class Scheme : Subject
	{
		/// <summary>
		/// Currently selected week
		/// </summary>
		private SelectedWeek selweek_;

		/// <summary>
		/// All persons/resources in system
		/// </summary>
		private SortedList activePersonResources_;

		/// <summary>
		/// All resourceboxes in system
		/// </summary>
		private SortedList resourceBoxes_;

		/// <summary>
		/// All resourceboxes per week(SelectedWeek) (each week has its own sortedlist)
		/// </summary>
		
		private SortedList weeks_ = new SortedList();

		/// <summary>
		/// True if user has logged in
		/// </summary>
		private bool loggedin_;

		/// <summary>
		/// Database controllerclass
		/// </summary>
		private DataBase db_ = new DataBase();

		/// <summary>
		/// The currently selected name in system
		/// </summary>
		private String activeName_ = null;

		/// <summary>
		/// The guilist that contains all person/resources
		/// </summary>
		private Observer checklistUI_;
		
		/// <summary>
		/// All activitys for currently selected week
		/// </summary>
		private SortedList activitys_;

		/// <summary>
		/// Settings for the system
		/// </summary>
		private Settings settings_;

		/// <summary>
		/// True if the logged in user is administrator
		/// </summary>
		private bool admin_;

		/// <summary>
		/// Files that must exist if the program should work
		/// </summary>
		private string[] files_ = { "streck.gif", "lock.gif" };

		/// <summary>
		/// Singleton instance
		/// </summary>
		private static Scheme instance_ = null;
		
		/// <summary>
		/// Get the instance of singleton class Scheme
		/// </summary>
		public static Scheme Instance
		{
			get
			{
				if ( instance_ == null )
					instance_ = new SchemaLite.Scheme();
				return instance_;
			}
		}

		/// <summary>
		/// Creates the scheme object which is the center of this system
		/// </summary>
		private Scheme()
		{
			loggedin_ = false;
			// Select current week
			this.selweek_ = new SelectedWeek(DateTime.Now);
			checkFiles();
		}

		/// <summary>
		/// Check that files nessacary exists in current directory
		/// </summary>
		private void checkFiles()
		{
			// Check for database file
			if ( DataBase.checkDatabase() == ErrorHandler.ERR_DATABASE_NOT_FOUND )
			{
				ErrorHandler.ShowErrorMessage(ErrorHandler.ERR_DATABASE_NOT_FOUND, "Ingen databas fil existerar, installera om programmet!\nProgrammet kommer att avslutas!", ErrorHandler.ErrorType.Error);
				Process.GetCurrentProcess().Kill();
			}
			foreach ( string str in files_ )
			{
				if ( !File.Exists(str) )
				{
					ErrorHandler.ShowErrorMessage(ErrorHandler.ERR_FILE_MISSING, "Filen "+str+" saknas, installera om programmet!\nProgrammet kommer att avslutas!", ErrorHandler.ErrorType.Error);
					Process.GetCurrentProcess().Kill();
				}
			}
			
		}

		/// <summary>
		/// Initiate scheme, get all data from database.
		/// </summary>
		/// <param name="clu">Gui object that shows persons/resources</param>
		public void initScheme(Observer clu)
		{
			SortedList prl;

			// Get and setup all persons and resources 
			ErrorHandler.ShowError(db_.getPersonsResources(out prl));
			setupPersonResourceList(prl, clu);
			clu.Update(this);

			// Get all resourceboxes 
			this.selweek_ = new SelectedWeek(DateTime.Now);
			ErrorHandler.ShowError(db_.getActivitys(new DateTime(0), new DateTime(0), out activitys_));
			ErrorHandler.ShowError(db_.getSettings(out settings_));

			ErrorHandler.ShowError(updateResourceBoxes());
			this.Notify();
		}	

		/// <summary>
		/// Get all resourceboxes from the database
		/// </summary>
		/// <returns>0 - If all is Ok, else a database error</returns>
		/// #1.04 - New index for week list (week number)
		private int updateResourceBoxes()
		{
			// Get all resourceboxes from database
			int fel = db_.getResourceBoxes(new DateTime(0), this.selweek_.DateOfFirstDayInWeek.AddDays(7), out this.resourceBoxes_);
			if ( fel != 0 )
			{
				return fel;
			}
			// Set correct id-list in each resourcebox

//			updateIdsNextTo(this.resourceBoxes_); #1.01.003 - removed line, this is done in db_.getResourceBoxes
			
			// Sort each resourcebox into the week-list
			weeks_ = new SortedList();
			// #1.04 - start
			foreach ( ResourceBox rb in resourceBoxes_.Values)
			{
				SelectedWeek selweek = new SelectedWeek(rb.StartTime);
				if ( !weeks_.ContainsKey(selweek.Week) )
				{
					weeks_[selweek.Week] = new SortedList();
				}
				((SortedList)weeks_[selweek.Week]).Add(rb.Id, rb);
				((ActivePersonResource)this.activePersonResources_[rb.Name]).AddObserver(rb);
			}
			// #1.04 - stop
			// #1.01.003 - moved line
			this.Notify();

			return 0;
		}

		/// <summary>
		/// Creates the persons and resources list.
		/// </summary>
		/// <param name="prl">SortedList with all Persons and Resources objects in system</param>
		/// <param name="clu">Graphic object with all persons and resources</param>
		private void setupPersonResourceList(SortedList prl, Observer clu)
		{
			activePersonResources_ = new SortedList();
			for(int i=0; i < prl.Count; i++)
			{
				ActivePersonResource apr = new ActivePersonResource((Entity)prl[i],false,true);
				activePersonResources_.Add(((Entity)prl[i]).Name, apr);
				apr.AddObserver(clu);
			}
			checklistUI_ = clu;
		}

		/// <summary>
		/// Check if the person has given the correct password
		/// </summary>
		/// <param name="name">Name of user/person</param>
		/// <param name="password">Password</param>
		/// <returns>Returns 0 if the user has logged in, if not it returns ERR_WRONG_LOGIN</returns>
		public int login(String name, String password)
		{
			int fel = db_.login(name, password, out admin_);
			if ( fel == 0 )
				loggedin_ = true;
			return fel;
		}

		/// <summary>
		/// Updates all resourcesboxes id-lists that has the given id.
		/// </summary>
		/// <param name="listids">List of ids, which are to have their id-list update</param>
		/// <returns>0 - if all Ok, else a database error</returns>
		private int updateIdsNextTo(SortedList listids)
		{
			for ( int i = 0;i < listids.Count; i ++)
			{
				if ( (ResourceBox)this.resourceBoxes_[listids.GetKey(i)] != null )
				{
					// Update one resourcebox with correct id-list
					int fel = ((ResourceBox)this.resourceBoxes_[listids.GetKey(i)]).updateIdsNextTo(this.db_);
					if ( fel != 0 )
						return fel;
				}																   
			}
			return 0;
		}
		
		/// <summary>
		/// Updates or adds resourcebox to system
		/// </summary>
		/// <param name="resourcebox">Resourcebox to be updated or added</param>
		/// <param name="add">True if the resourcebox is to be added</param>
		/// <returns>0 - If all Ok, else a database error</returns>
		/// #1.04 - New index for week list (week number)
		public int setResourceBox(ResourceBox resourcebox, bool add)
		{
			try
			{
				// Save the current id-list
				SortedList oldids = new SortedList(resourcebox.IdsNextTo);
				int fel = db_.setResourceBox(resourcebox, add);
				if ( fel != 0 )
					return fel;
				if ( add == true )
				{
					// Add the resourcebox to the list
					this.resourceBoxes_.Add(resourcebox.Id, resourcebox);
					SelectedWeek selweek = new SelectedWeek(resourcebox.StartTime);
					// #1.04 - start
					if ( !weeks_.ContainsKey(selweek.Week) )
					{
						weeks_[selweek.Week] = new SortedList();
					}
					((SortedList)weeks_[selweek.Week]).Add(resourcebox.Id, resourcebox);
					// #1.04 - stop
					((ActivePersonResource)this.activePersonResources_[resourcebox.Name]).AddObserver(resourcebox);
				} 
				// Update alla resourceboxes with the old id-list
				fel = this.updateIdsNextTo(oldids);
				
				// Update id-list of the resourcebox itself
				resourcebox.updateIdsNextTo(db_);
				
				// Update alla resourceboxes with the new id-list
				fel += this.updateIdsNextTo(resourcebox.IdsNextTo);
				resourcebox.Notify();
				if ( fel != 0 )
					return fel;
				return 0;
			}
			catch 
			{
				// If there was en error update alla resourceboxes
				this.updateResourceBoxes();
			}
			return ErrorHandler.ERR_UNEXPECTED_RESOURCEBOX_ADD;
		}

		/// <summary>
		/// Check if it is allowed or user allowes a resourcebox to be changed
		/// </summary>
		/// <param name="date">The date to check</param>
		/// <returns>true if edit is allowed</returns>
		public bool editAllowed(DateTime date)
		{
			// Check if edit is allowed
			if ( settings_.editAllowed(date) == false )
			{
				// Show yes/no message, if answer is yes edit is allowed
				System.Windows.Forms.DialogResult ret = ErrorHandler.ShowMessageYesNo(ErrorHandler.WRN_EDIT_NOT_ALLOWED);
				if (ret == System.Windows.Forms.DialogResult.Yes )
					return true;
			}
			else
				return true;
			return false;
		}

		/// <summary>
		/// Removes a resourcebox from system.
		/// </summary>
		/// <param name="resourcebox">Resourcebox to remove</param>
		/// <returns>0 - If all Ok, else a database error</returns>
		/// #1.04 - New index for week list (week number)
		public int removeResourceBox(ResourceBox resourcebox)
		{
			// If the resourcebox just is locked, then just zero it, by setting end and startime to the same thing
			if ( resourcebox.Locked == true )
			{
				resourcebox.EndTime = resourcebox.LockedStartTime;
				resourcebox.StartTime = resourcebox.LockedStartTime;
				this.setResourceBox(resourcebox, false);
				resourcebox.Notify();
				return 0;
			}
			else
			{
				try
				{
					// Get old id-list
					SortedList oldids = new SortedList(resourcebox.IdsNextTo);
					int fel = db_.removeResourceBox(resourcebox);
					if ( fel != 0 )
						return fel;
					// Update with old id-list
					fel = this.updateIdsNextTo(oldids);
					
					// HACK: är denna rad nödvändig?
					fel += this.updateIdsNextTo(resourcebox.IdsNextTo);

					this.resourceBoxes_.Remove(resourcebox.Id);

					// Remove for week-lists (this week and next)
					SelectedWeek selweek = new SelectedWeek(resourcebox.StartTime);
					// #1.04 - start
					if ( weeks_.ContainsKey(selweek.Week) )
					{
						((SortedList)weeks_[selweek.Week]).Remove(resourcebox.Id);
					}
					selweek = new SelectedWeek(selweek.DateOfFirstDayInWeek.AddDays(7));
					if ( weeks_.ContainsKey(selweek.Week) )
					{
						((SortedList)weeks_[selweek.Week]).Remove(resourcebox.Id);
					}
					// #1.04 - stop
					// Hide graphics
					resourcebox.StartTime = new DateTime(0);
					resourcebox.EndTime = new DateTime(0);
					resourcebox.Notify();

					if ( fel != 0 )
						return fel;
					return 0;
				}
				catch
				{
					this.updateResourceBoxes();
				}
				return ErrorHandler.ERR_UNEXPECTED_RESOURCEBOX_REM;
			}
		}
		
		/// <summary>
		/// Updates or adds a person/resource in system.
		/// </summary>
		/// <param name="entity">Person or Resource (class) to update or add</param>
		/// <param name="add">True if the person/resource is to be added to system</param>
		/// <returns>0 - If all Ok, else a database error</returns>
		public int setEntity(Entity entity, bool add)
		{
			int ok = ErrorHandler.ERR_WRONGTYPE_ADD;
			// Check if it is a person or Resource 
			if ( entity.GetType().Name == "Person" )
			{
				if ( entity.Name == "" )
					return ErrorHandler.ERR_NONAME_ON_PERSON;
				ok = db_.setPerson((Person)entity,add);
			}
			else if ( entity.GetType().Name == "Resource" )
			{
				if ( entity.Name == "" )
					return ErrorHandler.ERR_NONAME_ON_RESOURCE;
				ok = db_.setResource((Resource)entity,add);
			}
			if ( ok == 0 )
			{
				// Add person to the activepersonlist
				if ( add == false )
				{
					if ( entity.Name != "admin" )
					{
						ActivePersonResource apr = (ActivePersonResource)activePersonResources_[entity.Name];
						apr.Entity.Color = entity.Color;
						if ( entity.GetType().ToString() == "SchemaLite.Person" )
						{
							((Person)apr.Entity).Password = ((Person)entity).Password;
						}
					}
				}
				else
				{
					ActivePersonResource apr = new ActivePersonResource(entity,false,true);
					activePersonResources_.Add(entity.Name, apr);
					apr.AddObserver(checklistUI_);
				}
				
				// Update the resourcebox, if the name or color is changed  
				if ( entity.Name != "admin" )
				{
					((ActivePersonResource)activePersonResources_[entity.Name]).Notify();
				}
				return 0;
			}
			return ok;
		}

		/// <summary>
		/// Removes a person/resource from system.
		/// </summary>
		/// <param name="entity">Person or Resource (class) to be removed</param>
		/// <returns>0 - If all Ok, else a database error</returns>
		public int removeEntity(Entity entity)
		{
			int ok = 0;
			// Check if it is a person or resource 
			if ( entity.GetType().Name == "Person" )
			{
				ok = db_.removePerson((Person)entity);
			}
			else if ( entity.GetType().Name == "Resource" )
			{
				ok = db_.removeResource((Resource)entity);
			}
			if ( ok == 0 )
			{
				// Update the resourceboxes, all resourceboxes for the removed user/resource is gone 
				// Remove all graphic displays of resouceboxes
				SortedList rb_to_remove = new SortedList();
				foreach ( ResourceBox rb in this.resourceBoxes_.Values )
				{
					if ( rb.Name == entity.Name )
					{
						//this.resourceBoxes_.Remove(rb.Id);
						rb_to_remove.Add(rb.Id, rb.Id);
					}
				}
				// Remove the person resourceboxes
				foreach ( int rbid in rb_to_remove.Values )
				{
					removeResourceBox((ResourceBox)this.resourceBoxes_[rbid]);
				}
				// Remove the person
				activePersonResources_.Remove(entity.Name);

				this.Notify();
				return 0;
			}
			return ok;
		}

		private void NotifyAll(SortedList sl)
		{
			foreach ( Subject sub in sl.Values )
			{
				sub.Notify();
			}
		}
		/// <summary>
		/// Set the system in a new week 
		/// </summary>
		/// <param name="date">A date within the week to be changed to</param>
		/// #1.04 - New index for week list (week number)
		public int changeWeek(DateTime date)
		{
			SelectedWeek oldweek = new SelectedWeek(selweek_.DateOfFirstDayInWeek);
			SelectedWeek oldweek2 = new SelectedWeek(selweek_.DateOfFirstDayInWeek.AddDays(7));
			SelectedWeek nextweek = new SelectedWeek(date.AddDays(7));
			this.selweek_.setDate(date);

			// #1.04 - start
			// Update all resourceboxes of the old week
			if ( weeks_.ContainsKey(oldweek.Week) )
			{
				NotifyAll((SortedList)weeks_[oldweek.Week]);
			}
			if ( weeks_.ContainsKey(oldweek2.Week) )
			{
				NotifyAll((SortedList)weeks_[oldweek2.Week]);
			}

			
			// Update all resourceboxes of the new week
			if ( weeks_.ContainsKey(selweek_.Week) )
			{
				NotifyAll((SortedList)weeks_[selweek_.Week]);
			}
			if ( weeks_.ContainsKey(nextweek.Week) )
			{
				NotifyAll((SortedList)weeks_[nextweek.Week]);
			}
			// #1.04 - stop
			Notify();
			return 0;
		}
		
		/// <summary>
		/// Creates a report
		/// </summary>
		/// <param name="starttime">Starttime for the report</param>
		/// <param name="endtime">Endtime for the report</param>
		/// <param name="rep">The created report</param>
		/// <returns>0 - If all Ok, else a error</returns>
		public int runReport(DateTime starttime, DateTime endtime, out Report rep)
		{
			rep = new Report();
			return rep.startreport(starttime, endtime, db_);
		}

		/// <summary>
		/// Creates/updates an activity in system
		/// </summary>
		/// <param name="activity">The activity</param>
		/// <param name="add">If true the activity is added else it is updated</param>
		public void setActivity(Activity activity, bool add)
		{
			if ( db_.setActivity(activity, add) == ErrorHandler.NO_ERROR )
				this.activitys_[activity.Day] = activity;
		}

		/// <summary>
		/// Updates settings for system
		/// </summary>
		/// <param name="settings">The new settings</param>
		public void setSettings(Settings settings)
		{
			if ( db_.setSettings(settings) == ErrorHandler.NO_ERROR )
				this.settings_ = settings;
		}

		/// <summary>
		/// Copy all resourceboxes in a given daterange, by adding hours and days
		/// </summary>
		/// <param name="starttime">Start of the copyregion</param>
		/// <param name="endtime">End of the copyregion</param>
		/// <param name="addhours">No of days to add to the copied resourcebox</param>
		/// <param name="adddays">No of hours to add to the copied resourcebox</param>
		/// <param name="newrboxes">A list of new resourceboxes added to system</param>
		/// <returns>0 if all is ok</returns>
		public int copyResourceBoxes(DateTime starttime, DateTime endtime, int addhours, int adddays, out SortedList newrboxes)
		{
			// TODO: Add hour support to this function, now it will probably crack out if oldhour+newhour>24
			SortedList rboxes;
			db_.getResourceBoxes(starttime, endtime, out rboxes);
			newrboxes = new SortedList();
			for ( int i = 0; i < rboxes.Count; i++)
			{
				// Add hours and days on all resoureboxes times 
				ResourceBox rb = (ResourceBox)rboxes[rboxes.GetKey(i)];
				if ( rb.Locked )
				{
					try
					{
						rb.LockedEndTime = rb.LockedEndTime.AddDays(adddays).AddHours(addhours);
						rb.LockedStartTime = rb.LockedStartTime.AddDays(adddays).AddHours(addhours);
					}
					catch 
					{
						return ErrorHandler.ERR_COPY_WEEK;
					}

				}
				rb.StartTime = rb.StartTime.AddDays(adddays).AddHours(addhours);
				rb.EndTime = rb.EndTime.AddDays(adddays).AddHours(addhours);
				rb.Id = 0;
				int fel = setResourceBox(rb, true);
				if ( fel != ErrorHandler.NO_ERROR )
					return fel;
			}
			newrboxes = rboxes;
			return 0;
		}

		/// <summary>
		/// Get the total devitation for all resourceboxes for a person
		/// </summary>
		/// <param name="name">Person to get deviation for</param>
		/// <param name="starttime">Starttime</param>
		/// <param name="endtime">Endtime</param>
		/// <returns></returns>
		public double getTotalDev(String name, DateTime starttime, DateTime endtime)
		{
			double dev = 0;
			
			foreach ( ResourceBox rb in resourceBoxes_.Values)
			{
				if ( rb.StartTime > starttime && rb.EndTime < endtime && rb.Name == name )
				{
					dev += rb.Deviation;
				}
			}
			return dev;
		}

		/// <summary>
		/// Lock all resourceboxes for a given name
		/// </summary>
		/// <param name="name">Name on person to lock boxes</param>
		public void lockResourceBoxes(String name)
		{
			foreach ( ResourceBox rb in resourceBoxes_.Values )
			{
				if ( rb.Name == name )
				{
					if ( !rb.Locked )
					{
						rb.lockTime();
						rb.Notify();
					}
				}
			}
		}

		/// <summary>
		/// Get all activitys for current week
		/// </summary>
		public SortedList Activitys
		{
			get
			{
				return activitys_;
			}
		}
		
		/// <summary>
		/// Get database control class
		/// </summary>
		internal DataBase DB
		{
			get
			{
				return db_;
			}
		}

		/// <summary>
		/// Get settings for system
		/// </summary>
		public Settings Settings
		{
			get
			{
				return settings_;
			}
		}

		/// <summary>
		/// Get if the user logged in is administrator
		/// </summary>
		public bool Admin
		{
			get
			{
				return admin_;
			}
		}

		/// <summary>
		/// Get if a user is loggedin
		/// </summary>
		public bool LoggedIn
		{
			get
			{
				return this.loggedin_;
			}
		}

		/// <summary>
		/// Name of the user/resources that currently is selected.
		/// </summary>
		public String ActiveName
		{
			get 
			{
				return activeName_;
			}
			set
			{
				activeName_ = value;
			}
		}

		/// <summary>
		/// The list of all persons and resources
		/// </summary>
		public SortedList ActivePersonsResources
		{
			get
			{
				return activePersonResources_;
			}
		}
		
		/// <summary>
		/// Get list of all resourcesboxes in system
		/// </summary>
		public SortedList ResourceBoxes
		{
			get
			{
				return resourceBoxes_;
			}
		} 
	
		/// <summary>
		/// Get selected week
		/// </summary>
		public SelectedWeek SelectedWeek
		{
			get
			{
				return this.selweek_;
			}
		}

		/// <summary>
		/// Database file
		/// </summary>
		public String DatabaseFile
		{
			get
			{
				return DataBase.DBFILE;
			}
			set
			{
				DataBase.DBFILE = value;
			}
		}

		/// <summary>
		/// Get a sortedlist with keys of SelectedWeek, 
		/// where a SortedList contains all Resouceboxes for that week
		/// </summary>
		public SortedList ResourceBoxPerWeek
		{
			get
			{
				return weeks_;
			}
		}
	}
}
