using System;
using System.IO;
using System.Data;
using System.Collections;
using System.Data.OleDb;
namespace SchemaLite
{
	/// <summary>
	/// Database communication class
	/// Reads and writes data to/from a access database
	/// ID			Date	Author				Text
	/// #1.01.003	051209	Markus Svensson		Solved issue SL-6
	/// </summary>
	public class DataBase
	{

		/// <summary>
		/// Database filename
		/// </summary>
		public static string DBFILE = "sldb.mdb";
		
		/// <summary>
		/// An open database connection, if none this variable is null
		/// </summary>
		private OleDbConnection conn = null;

		public DataBase()
		{
		}
		
		/// <summary>
		/// Checks the database for the user and compares the given password with the one in db
		/// </summary>
		/// <param name="name">Name of the user</param>
		/// <param name="password">Password for the user</param>
		/// <returns>If 0 all is ok if error login in ERR_WRONG_LOGIN_STR</returns>
		internal int login(String name, String password, out bool admin)
		{
			OleDbDataReader reader;
			String sqlcmd = "SELECT name, pass, administrator FROM Person WHERE Name='"+name+"' AND pass='"+password+"'";
			reader = executeSql(sqlcmd);
			admin = false;
			// If a row is found, the password and name os correct 
			if ( reader.HasRows )
			{
				reader.Read();
				admin = reader.GetBoolean(2);
				reader.Close();
				return ErrorHandler.NO_ERROR;
			}
			reader.Close();
			return ErrorHandler.ERR_WRONG_LOGIN;
		}

		/// <summary>
		/// Fetches all persons and resources from database
		/// </summary>
		/// <returns>Returns list of all person and resources, If return is null something has gone wrong</returns>
		internal int getPersonsResources(out SortedList retlist)
		{
			// Get all persons i database
			String sqlcmd = "SELECT name, color, pass, administrator FROM Person WHERE name<>'admin'";
			OleDbDataReader reader = executeSql(sqlcmd);
			retlist = new SortedList();
			if ( reader == null )
			{
				return ErrorHandler.ERR_FETCH_PERSON;
			}

			// Add all persons to the sortedlist 
			int i = 0;
			while ( reader.Read() )
			{
				Person person = new Person(reader.GetString(0), reader.GetString(1), reader.GetString(2), reader.GetBoolean(3));
				retlist.Add(i, person);
				i++;
			}
			reader.Close();

			// Get all resources from database 
			sqlcmd = "SELECT name, color FROM Resource";
			reader = executeSql(sqlcmd);

			if ( reader == null )
			{
				return ErrorHandler.ERR_FETCH_RESOURCE;
			}

			// Add all resources to the sortedlist 
			while ( reader.Read() )
			{
				Resource resurs = new Resource(reader.GetString(0), reader.GetString(1));
				retlist.Add(i, resurs);
				i++;
			}
			reader.Close();

			return ErrorHandler.NO_ERROR;
		}

		/// <summary>
		/// Adds or updates a person in the database
		/// </summary>
		/// <param name="person">Person to add or update</param>
		/// <param name="add">If add is true the a new person is created</param>
		/// <returns>0 - If all is ok, ERR_PERSON_EXISTS if the person already exists, else database error</returns>
		internal int setPerson(Person person, bool add)
		{
			String sqlcmd;
			// Create the SQL for add or update the person 
			if ( add == true )
			{
				// Check if the name already exists in database 
				if ( entityExists(person) == true )
				{
					return ErrorHandler.ERR_PERSON_EXISTS;
				}
				sqlcmd = "INSERT INTO Person (name, color, pass ) VALUES ('"+person.Name+"','"+person.Color.ToString()+"','"+person.Password+"' )";
			}
			else
			{
				sqlcmd = "UPDATE Person Set color = '"+person.Color.ToString()+"', pass='"+person.Password+"' WHERE name='"+person.Name+"'";
			}
			// Run the SQL-code 
			int fel = this.executeSqlNonQry(sqlcmd);
			if ( fel != 0 )
			{
				return fel;
			}
			return 0;
		}

		/// <summary>
		/// Remove a person from the database */
		/// </summary>
		/// <param name="person">Person to remove</param>
		/// <returns>0 - If all is ok, else the database error</returns>
		internal int removePerson(Person person)
		{
			String sqlcmd;
			// Remove all resourceboxes connected to the person 
			sqlcmd = "DELETE FROM ResourceBox WHERE name='"+person.Name+"'";
			int fel = this.executeSqlNonQry(sqlcmd);
			if ( fel == 0 )
			{
				// Remove the person 
				sqlcmd = "DELETE FROM Person WHERE name='"+person.Name+"'";
				fel = this.executeSqlNonQry(sqlcmd);
				if ( fel != 0 )
				{	
					return fel;
				}
				return ErrorHandler.NO_ERROR;
			}
			return fel;
		}

		/// <summary>
		/// Add or update a resource in the database
		/// </summary>
		/// <param name="resource">Resource to add or update</param>
		/// <param name="add">If add is true the a new person is created</param>
		/// <returns>0 - If all is ok, ERR_RESOURCE_EXISTS if the resource already exists, else database error</returns>
		internal int setResource(Resource resource, bool add)
		{
			String sqlcmd;
			// Create the SQL to add or update the database 
			if ( add == true )
			{
				// Check if the name of the Resource exists 
				if ( entityExists(resource) == true )
				{
					return ErrorHandler.ERR_RESOURCE_EXISTS;
				}
				sqlcmd = "INSERT INTO Resource (name, color ) VALUES ('"+resource.Name+"','"+resource.Color.ToString()+"')";
			}
			else
			{
				sqlcmd = "UPDATE Resource SET color='"+resource.Color.ToString()+"' WHERE name='"+resource.Name+"'";
			}

			// Update the database 
			int fel = this.executeSqlNonQry(sqlcmd);
			if ( fel != 0 )
			{
				return fel;
			}
			return ErrorHandler.NO_ERROR;		
		}

		/// <summary>
		/// Remove a resource and all resourceboxes connected to it.
		/// </summary>
		/// <param name="resource">Resource to remove</param>
		/// <returns>0 - If all is ok, else database error</returns>
		internal int removeResource(Resource resource)
		{
			String sqlcmd;
			// Remove all resoruceboxes connected to the resource 
			sqlcmd = "DELETE FROM ResourceBox WHERE name='"+resource.Name+"'";
			int fel = this.executeSqlNonQry(sqlcmd);
			if ( fel == 0 )
			{
				// Remove the resource 
				sqlcmd = "DELETE FROM Resource WHERE name='"+resource.Name+"'";
				fel = this.executeSqlNonQry(sqlcmd);
				if ( fel != 0 )
				{
					return fel;
				}
				return 0;
			}
			return fel;
		}

		/// <summary>
		/// Gets all ids of ResourceBoxes that overlaps given resourcebox
		/// </summary>
		/// <param name="resourcebox">ResourceBox for wich all overlapping ids is to be calculated</param>
		/// <param name="sl">SortedList with all ids as keys</param>
		/// <returns>0 - If all is ok, </returns>
		internal int getResouceBoxesNextTo(ResourceBox resourcebox, out SortedList sl)
		{
			String sqlcmd;
			// Get all ids from resourceboxes that are overlapping this resourcebox 
			sqlcmd = "SELECT Id FROM ResourceBox rb WHERE Id<>"+resourcebox.Id+" AND ( starttime BETWEEN #"+resourcebox.StartTime.ToShortDateString()+" "+resourcebox.StartTime.ToLongTimeString()+"# AND #"+resourcebox.EndTime.ToShortDateString()+" "+resourcebox.EndTime.ToLongTimeString()+"# OR "+
				     " endtime BETWEEN #"+resourcebox.StartTime.ToShortDateString()+" "+resourcebox.StartTime.ToLongTimeString()+"# AND #"+resourcebox.EndTime.ToShortDateString()+" "+resourcebox.EndTime.ToLongTimeString()+"# ) AND starttime NOT IN (SELECT endtime FROM ResourceBox Where id=rb.Id)";
			sl = new SortedList();
			OleDbDataReader reader = executeSql(sqlcmd);
			if ( reader == null )
			{
				return ErrorHandler.ERR_UNEXPECTED_READ_ERROR_OVERLAPPING;
			}

			// Add all ids to the sortedlist 
			while ( reader.Read() )
			{
				if ( !sl.ContainsKey(reader.GetInt32(0)) )
					sl.Add(reader.GetInt32(0), reader.GetInt32(0));
			}
			reader.Close();

			// Get all ids from resourceboxes that overlaps this resourcebox 
			sqlcmd = "SELECT r2.Id FROM ResourceBox r1, ResourceBox r2 WHERE r2.Id<>"+resourcebox.Id+" AND r1.Id="+resourcebox.Id+" AND ( r1.starttime BETWEEN r2.starttime AND r2.endtime OR "+
				" r1.endtime BETWEEN r2.starttime AND r2.endtime ) AND r1.starttime NOT IN (SELECT endtime FROM ResourceBox WHERE id=r1.id) AND r2.starttime NOT IN (SELECT endtime FROM ResourceBox WHERE id=r2.id)";
			reader = executeSql(sqlcmd);
			if ( reader == null )
			{
				return ErrorHandler.ERR_UNEXPECTED_READ_ERROR_OVERLAPPING;
			}

			// Add all ids to the sortedlist 
			while ( reader.Read() )
			{
				if ( !sl.ContainsKey(reader.GetInt32(0)) )
					sl.Add(reader.GetInt32(0), reader.GetInt32(0));
			}
			reader.Close();
			return ErrorHandler.NO_ERROR;
		}

		/// <summary>
		/// Adds or updates a resourcebox in database
		/// </summary>
		/// <param name="resourcebox"></param>
		/// <param name="add"></param>
		/// <returns>0 - If all is ok, ERR_UNEXCPECTED_RESOURCEBOX_ADD if the resourcebox already exists, else database error</returns>
		internal int setResourceBox(ResourceBox resourcebox, bool add)
		{
			String sqlcmd;
			// Create SQL for add or update database 
			if ( add == true )
			{
				sqlcmd = "INSERT INTO ResourceBox (starttime,endtime,name,free,locked,lockedstarttime,lockedendtime ) VALUES ('"+resourcebox.StartTime.ToShortDateString()+" "+resourcebox.StartTime.ToLongTimeString()+"','"+resourcebox.EndTime.ToShortDateString()+" "+resourcebox.EndTime.ToLongTimeString()+"', '"+resourcebox.Name+"', "+resourcebox.Free+", "+resourcebox.Locked+", '"+resourcebox.LockedStartTime.ToShortDateString()+" "+resourcebox.LockedStartTime.ToLongTimeString()+"','"+resourcebox.LockedEndTime.ToShortDateString()+" "+resourcebox.LockedEndTime.ToLongTimeString()+"')";
			}
			else
			{
				sqlcmd = "UPDATE ResourceBox SET starttime='"+resourcebox.StartTime.ToShortDateString()+" "+resourcebox.StartTime.ToLongTimeString()+"', endtime='"+resourcebox.EndTime.ToShortDateString()+" "+resourcebox.EndTime.ToLongTimeString()+"', name='"+resourcebox.Name+"', free="+resourcebox.Free+", locked="+resourcebox.Locked+", lockedstarttime='"+resourcebox.LockedStartTime.ToShortDateString()+" "+resourcebox.LockedStartTime.ToLongTimeString()+"', lockedendtime='"+resourcebox.LockedEndTime.ToShortDateString()+" "+resourcebox.LockedEndTime.ToLongTimeString()+"' WHERE Id="+resourcebox.Id;
			}

			// Run SQL 
			int fel = this.executeSqlNonQry(sqlcmd);
			if ( fel != 0 )
			{
				return ErrorHandler.ERR_UNEXPECTED_RESOURCEBOX_ADD;
			}
			if ( add == true )
			{
				sqlcmd = "SELECT MAX(id) FROM ResourceBox";
				OleDbDataReader reader = executeSql(sqlcmd);
				if ( reader == null )
				{
					return ErrorHandler.ERR_UNEXPECTED_RESOURCEBOX_ADD;
				}

				// Get id, because of the access counter it will always be max
				if ( reader.HasRows )
				{
					reader.Read();
					resourcebox.Id = reader.GetInt32(0);
					reader.Close();
				}
				else
				{
					reader.Close();
					return ErrorHandler.ERR_UNEXPECTED_RESOURCEBOX_ADD;
				}
			}
			return 0;
		}
		
		/// <summary>
		/// Remove resourcebox from database
		/// </summary>
		/// <param name="resourcebox">Resourcebox to remove</param>
		/// <returns>0 - If all is ok, else a Database errorcode</returns>
		internal int removeResourceBox(ResourceBox resourcebox)
		{
			String sqlcmd;
			// Remove the resourcebox 
			sqlcmd = "DELETE FROM ResourceBox WHERE Id="+resourcebox.Id;
			int fel = this.executeSqlNonQry(sqlcmd);
			if ( fel != 0 )
			{
				return fel;
			}
			return ErrorHandler.NO_ERROR;
		}

		/// <summary>
		/// Get resourceboxes from the database
		/// </summary>
		/// <param name="startdate">Startdate of the resourceboxes to read, if DateTime(0) all resourceboxes are fetched</param>
		/// <param name="enddate">Enddate of the resourceboxes to read</param>
		/// <returns>0 - If all is ok</returns>
		internal int getResourceBoxes(DateTime startdate, DateTime enddate, out SortedList retlist)
		{
			/* Create SQL code for fetching resourceboxes */
			String sqlcmd = "SELECT id, starttime, endtime, name, free, locked, lockedstarttime, lockedendtime FROM ResourceBox";  

			// If startdate and enddate is given, use them in a WHERE part of SQL 
            if ( startdate != new DateTime(0) )
			{
				sqlcmd += " WHERE starttime BETWEEN #"+startdate.ToShortDateString()+" "+startdate.ToLongTimeString()+"# AND #"+enddate.ToShortDateString()+" "+enddate.ToLongTimeString()+"#";
			}

			// #1.01.003 - added line
			sqlcmd +=  " ORDER BY starttime";
			retlist = new SortedList();
			
			// Get data from database 
			OleDbDataReader reader = executeSql(sqlcmd);
			if ( reader == null )
			{
				return ErrorHandler.ERR_UNEXPECTED_RESOURCEBOX_FETCH;
			}
			
			int i = 0;
			
			// #1.01.003 - added lines
			ResourceBox[] rbs;
			rbs = new ResourceBox[20000];
			
			// Add all data in the sortedlist 
			while ( reader.Read() )
			{
				ResourceBox resursbox = new ResourceBox(reader.GetDateTime(1), reader.GetDateTime(2), reader.GetString(3), reader.GetBoolean(4), reader.GetBoolean(5), reader.GetDateTime(6), reader.GetDateTime(7));
				resursbox.Id = reader.GetInt32(0);
				// Get all overlaps of resourcebox 
				retlist.Add(resursbox.Id, resursbox);
				
				rbs[i] = resursbox; // #1.01.003 - added line
				
				i++;
			}

			// #1.01.003 - added lines
			for ( int o = 0; o < i-1; o++)
			{
				for ( int a = o+1; rbs[a].StartTime <= rbs[o].EndTime && o < i; a++)
				{
					if ( rbs[a].StartTime != rbs[a].EndTime &&
						rbs[o].StartTime != rbs[o].EndTime )
					{
						rbs[a].IdsNextTo.Add(rbs[o].Id, rbs[o].Id);
						rbs[o].IdsNextTo.Add(rbs[a].Id, rbs[a].Id);
					}
					// #1.03 - added lines
					if ( a+1 >= i )
						break;
				}
			}
			// #1.01.003 - end

			reader.Close();
			return ErrorHandler.NO_ERROR;
		}
		

		/// <summary>
		/// Save activity to database
		/// </summary>
		/// <param name="activity">The activity to save</param>
		/// <param name="add">If true create a new activity</param>
		internal int setActivity(Activity activity, bool add)
		{
			String sqlcmd;
			// Create the SQL to add or update the database 
			if ( add == true )
			{
				sqlcmd = "INSERT INTO Activity (daydate,activity ) VALUES ('"+activity.Day.ToShortDateString()+" 00:00:00','"+activity.Text+"')";
			}
			else
			{
				sqlcmd = "UPDATE Activity SET activity='"+activity.Text+"' WHERE daydate=#"+activity.Day.ToShortDateString()+" 00:00:00#";
			}

			// Update the database 
			int fel = this.executeSqlNonQry(sqlcmd);
			if ( fel != 0 )
			{
				return fel;
			}
			return ErrorHandler.NO_ERROR;					
		}

		/// <summary>
		/// Gets all activitys in datbase given a daterange
		/// </summary>
		/// <param name="startdate">Startdate</param>
		/// <param name="enddate">Enddate</param>
		/// <param name="retlist">List with activitys</param>
		/// <returns>0 - if all is ok, else a database error</returns>
		internal int getActivitys(DateTime startdate, DateTime enddate, out SortedList retlist)
		{
			// Create SQL code for fetching activitys 
			String sqlcmd = "SELECT daydate, activity FROM Activity";

			// If startdate and enddate is given, use them in a WHERE part of SQL 
			if ( startdate != new DateTime(0) )
			{
				sqlcmd += " WHERE daydate BETWEEN #"+startdate.ToShortDateString()+" "+startdate.ToLongTimeString()+"# AND #"+enddate.ToShortDateString()+" "+enddate.ToLongTimeString()+"#";
			}
			retlist = new SortedList();

			// Get data from database 
			OleDbDataReader reader = executeSql(sqlcmd);
			if ( reader == null )
			{
				return ErrorHandler.ERR_UNEXPECTED_ACTIVITYS_FETCH;
			}
			
			// Add all data in the sortedlist 
			while ( reader.Read() )
			{
				
				Activity activity = new Activity(reader.GetDateTime(0), reader.GetString(1));
				retlist.Add(activity.Day, activity);
			}
			reader.Close();
			return ErrorHandler.NO_ERROR;
		}

		/// <summary>
		/// Get a activity for a specific day
		/// </summary>
		/// <param name="day">Date with activity</param>
		/// <param name="activity">The fetch activity, if none found this will be null</param>
		/// <returns>0 - if there was no error, else a database error</returns>
		internal int getActivity(DateTime day, out Activity activity)
		{
			// Create SQL code for fetching activitys 
			String sqlcmd = "SELECT daydate, activity FROM Activity day=#"+day.ToShortDateString()+"#";

			activity = null;
			// Get data from database 
			OleDbDataReader reader = executeSql(sqlcmd);
			if ( reader == null )
			{
				return ErrorHandler.ERR_UNEXPECTED_ACTIVITY_FETCH;
			}
			
			// Get data 
			if ( reader.HasRows )
			{
				activity = new Activity(reader.GetDateTime(0), reader.GetString(1));
			}
			reader.Close();
			return ErrorHandler.NO_ERROR;
		}

		/// <summary>
		/// Get settings from database
		/// </summary>
		/// <param name="settings">The settings collected</param>
		/// <returns>0 - if all is ok, else a database error</returns>
		internal int getSettings(out Settings settings)
		{
			// Create SQL code for fetching activitys 
			String sqlcmd = "SELECT companyname, editstopdays FROM Settings";
			settings = null;

			// Get data from database 
			OleDbDataReader reader = executeSql(sqlcmd);
			if ( reader == null )
			{
				return ErrorHandler.ERR_UNEXPECTED_SETTINGS_FETCH;
			}
			
			
			// Get data 
			if ( reader.Read() )
			{
				settings = new Settings(reader.GetString(0), reader.GetInt32(1));
			}
			else
				settings = new Settings("No name", 0);
			reader.Close();
			return ErrorHandler.NO_ERROR;			
		}

		/// <summary>
		/// Update settings in database
		/// </summary>
		/// <param name="settings">Settings to update</param>
		/// <returns>0 - if all is ok, else a data base error</returns>
		internal int setSettings(Settings settings)
		{
			String sqlcmd;
			// Create the SQL to add or update the database 
			sqlcmd = "DELETE FROM Settings";
			int fel = this.executeSqlNonQry(sqlcmd);
			sqlcmd = "INSERT INTO Settings (companyname,editstopdays ) VALUES ('"+settings.CompanyName+"','"+settings.EditStopDays+"')";

			// Update the database 
			fel = this.executeSqlNonQry(sqlcmd);
			if ( fel != 0 )
			{
				return fel;
			}
			return ErrorHandler.NO_ERROR;					
		}

		/// <summary>
		/// Check if a name already exists in database
		/// </summary>
		/// <param name="entity">Person or resource to compare with</param>
		/// <returns>True if the name already exists</returns>
		private bool entityExists(Entity entity)
		{
			String sqlcmd;
			// Get the name from both person and resource tables 
			sqlcmd = "(SELECT name FROM Person WHERE Name='"+entity.Name+"') UNION (SELECT name FROM Resource WHERE Name='"+entity.Name+"')";
			OleDbDataReader reader = this.executeSql(sqlcmd);
			// If there is some error, take it carefully and act as if the name exists 
			if ( reader == null )
			{
				return true;
			}
			
			// If there is a row in database then the name already exits 
			if ( reader.HasRows == true )
			{
				reader.Close();
				return true;
			}
			reader.Close();
			return false;
		}

		/// <summary>
		/// Runs a SELECT statement in database
		/// </summary>
		/// <param name="sqlcmd">SQL command to execute in database</param>
		/// <returns>On error null is returned else a OleDbDataReader is returned</returns>
		private OleDbDataReader executeSql(String sqlcmd)
		{
			// Create/get database connection 
			OleDbConnection localconn = getConnection();
			OleDbCommand cmd = new OleDbCommand(sqlcmd, localconn);
			OleDbDataReader reader;
			// Try to run the SQL-statement 
			try
			{
				reader = cmd.ExecuteReader();
			}
			catch (OleDbException e) 
			{
				// If there is a error write data in programlog for later debugging 
				string errorMessages = "";
				for (int i=0; i < e.Errors.Count; i++)
				{
					errorMessages += "Index #" + i + "\n" +
						"Message: " + e.Errors[i].Message + "\n" +
						"NativeError: " + e.Errors[i].NativeError + "\n" +
						"Source: " + e.Errors[i].Source + "\n" +
						"SQLState: " + e.Errors[i].SQLState + "\n";
				}
				errorMessages += "SQL: "+sqlcmd;
				System.Diagnostics.EventLog log = new System.Diagnostics.EventLog();
				log.Source = "SchemaLite";
				log.WriteEntry(errorMessages);
				return null;
			}
			return reader;
		}

		/// <summary>
		/// Executes a insert, delete or update statement.
		/// </summary>
		/// <param name="sqlcmd">SQL-statement to execute</param>
		/// <returns>0 If all is ok, else a errorcode is returned</returns>
		private int executeSqlNonQry(String sqlcmd)
		{
			OleDbConnection localconn = getConnection();
			OleDbTransaction myTrans = null;
			try
			{
				// Start a local transaction 
				myTrans = localconn.BeginTransaction(IsolationLevel.ReadCommitted);
				// Assign transaction object for a pending local transaction 

				OleDbCommand cmd = 
					new OleDbCommand(sqlcmd, localconn);
				cmd.Connection = localconn;
				cmd.Transaction = myTrans;

				// Try to execute the sql-statement 
				cmd.ExecuteNonQuery();
				myTrans.Commit();
 			}
			catch (OleDbException e) 
			{
				if ( myTrans != null )
					myTrans.Rollback();
				conn.Close();
				conn = null;
				// If there is a error write data in programlog for later debugging 
				string errorMessages = "";
				for (int i=0; i < e.Errors.Count; i++)
				{
					errorMessages += "Index #" + i + "\n" +
						"Message: " + e.Errors[i].Message + "\n" +
						"NativeError: " + e.Errors[i].NativeError + "\n" +
						"Source: " + e.Errors[i].Source + "\n" +
						"SQLState: " + e.Errors[i].SQLState + "\n";
				}
				errorMessages += "SQL: "+sqlcmd;
				System.Diagnostics.EventLog log = new System.Diagnostics.EventLog();
				log.Source = "SchemaLite";
				log.WriteEntry(errorMessages);

				// Return the database error 
				return e.Errors[0].NativeError;
			} 
			
			conn.Close();
			conn = null;
			return 0;
		}

		/// <summary>
		/// Close the connection for a reader and the database
		/// </summary>
		/// <param name="reader">Reader to close connection for</param>
		private void endConnection(OleDbDataReader reader)
		{
			if ( reader != null )
				reader.Close();
			conn.Close();
		}

		/// <summary>
		/// Create a new connection
		/// </summary>
		/// <returns>A open connection</returns>
		private OleDbConnection getConnection()
		{
				if ( conn == null )
				{
					conn = new OleDbConnection(
						"Provider=Microsoft.Jet.OLEDB.4.0; " + 
						"Data Source=" + DBFILE); // + ";User ID=Admin;Password=träslövsläge");
				}
				if ( conn.State == ConnectionState.Fetching )
					conn.Close();
				if ( conn.State == ConnectionState.Closed )
					conn.Open();
			return conn;
		}

		/// <summary>
		/// Checks if the databasefile exists and has write-access
		/// </summary>
		/// <returns>0 - If all is ok, else errorcodes</returns>
		internal static int checkDatabase()
		{
			if ( !File.Exists(DBFILE) )
				return ErrorHandler.ERR_DATABASE_NOT_FOUND;
			if ( ( File.GetAttributes(DBFILE) & FileAttributes.ReadOnly ) == FileAttributes.ReadOnly )
				return ErrorHandler.ERR_DATABASE_NO_WRITE;
			return ErrorHandler.NO_ERROR;
		}

		/// <summary>
		/// Gets a errorstring and type for a specified errorcode
		/// </summary>
		/// <param name="errorcode">The errorcode</param>
		/// <param name="errorstring">The errorstring</param>
		/// <param name="errortype">Errortype (see ErrorHandler.ErrorType enum)</param>
		/// <returns>0 - if all is ok, else database error</returns>
		internal int getErrorString(int errorcode, out string errorstring, out int errortype)
		{
			// Create SQL code for fetching activitys 
			String sqlcmd = "SELECT errortext, errortype FROM ErrorMessage WHERE errorcode="+errorcode;
			errorstring = "";
			errortype = 0;

			// Get data from database 
			OleDbDataReader reader = executeSql(sqlcmd);
			if ( reader == null )
			{
				return 1;
			}
			
			// Get data 
			if ( reader.Read() )
			{
				errorstring = reader.GetString(0);
				errortype = reader.GetInt32(1);
			}
			else
				return 1;
			reader.Close();
			return ErrorHandler.NO_ERROR;			
		}
	}
}
