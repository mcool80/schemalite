using System;
using System.Windows.Forms;

namespace SchemaLite
{
	/// <summary>
	/// Summary description for ErrorUI.
	/// </summary>
	public class ErrorHandler
	{
		public const int NO_ERROR = 0;
		public const int ERR_RESOURCE_EXISTS = 10001;
		public const int ERR_RESOURCEBOX_EXISTS = 10002;
		public const int ERR_PERSON_EXISTS = 10003;
		public const int ERR_WRONGTYPE_ADD = 10004;
		public const int ERR_WRONGTYPE_REM = 10005;
		public const int ERR_UNEXPECTED_RESOURCEBOX_ADD = 10006; 
		public const int ERR_UNEXPECTED_RESOURCEBOX_REM = 10007; 
		public const int ERR_WRONG_LOGIN = 10008;
		public const int ERR_FETCH_PERSON = 10009;
		public const int ERR_FETCH_RESOURCE = 10010;
		public const int ERR_UNEXPECTED_READ_ERROR_OVERLAPPING = 10011;
		public const int ERR_UNEXPECTED_RESOURCEBOX_FETCH = 10012;
		public const int ERR_NONAME_ON_PERSON = 10013;
		public const int ERR_NONAME_ON_RESOURCE = 10014;
		public const int ERR_NO_SELECTED = 10015;
		public const int ERR_DATE_WRONG = 10016;
		public const int ERR_DATABASE_NOT_FOUND = 10017;
		public const int ERR_DATABASE_NO_WRITE = 10018;
		public const int ERR_WRONG_REGISTRATION_CODE = 10019;
		public const int ERR_UNEXPECTED_ACTIVITYS_FETCH = 10020;
		public const int ERR_UNEXPECTED_ACTIVITY_FETCH = 10021;
		public const int ERR_UNEXPECTED_SETTINGS_FETCH = 10022;
		public const int WRN_EDIT_NOT_ALLOWED = 10023;
		public const int NO_CHANGES = 10024;
		public const int Q_COPY_WEEK = 10025;
		public const int ERR_COPY_WEEK = 10026;
		public const int ERR_NOT_REGISTERED = 10027;
		public const int ERR_PRINTING = 10028;
		public const int INFO_PROGRAM_REGISTERED = 10029;
		public const int ERR_WRONG_PASS_ON_CHANGE = 10030;
		public const int ERR_FILE_MISSING = 10031;
		
		public enum ErrorType {Error=1, Warning, Information};

		public ErrorHandler()
		{
		}
		/// <summary>
		/// Get the errorstring and icon to display
		/// </summary>
		/// <param name="errorcode">Errorcode</param>
		/// <param name="icon">The messagebox icon to display with the errortext</param>
		/// <returns>Errortext</returns>
		private static String getErrorStr(int errorcode, out ErrorType errortype)
		{
			String errorstr = "";
			int verrortype;
			if (errorcode == NO_ERROR )
			{
				errorstr = "";
				errortype = ErrorType.Information;
			}
			else
			{
				if ( Scheme.Instance.DB.getErrorString(errorcode, out errorstr, out verrortype) != 0 )
				{
					errorstr = "Ett fel har inträffat, hjälp oss att förbättra denna produkt. Skicka information om vad du gjorde innan felet inträffade, ange dessuom denna kod '"+errorcode+"' till oss på info@celit.se med ämnesord 'SchemaLite problem'";
					errortype = ErrorType.Error;
				}
				else
					errortype = (ErrorType)verrortype;
				errorstr = errorstr.Replace("\\n","\n");
			}
			return errorstr;
		}
		/// <summary>
		/// Get icon contstant
		/// </summary>
		/// <param name="errortype">Errortype for this errorhandler</param>
		/// <returns>Icon constant</returns>
		private static MessageBoxIcon getIcon(ErrorType errortype)
		{
			switch (errortype)
			{
				case ErrorType.Error:
					return MessageBoxIcon.Error;
					break;
				case ErrorType.Warning:
					return MessageBoxIcon.Warning;
					break;
				case ErrorType.Information:
					return MessageBoxIcon.Information;
					break;
				default:
					return MessageBoxIcon.Error;
					break;
			}
			return MessageBoxIcon.Error;
		}
		public static DialogResult ShowMessageYesNo(int errorcode)
		{
			ErrorType errortype;
			String errorstr = ErrorHandler.getErrorStr(errorcode, out errortype);
			MessageBoxIcon icon = getIcon(errortype);
			return MessageBox.Show(null, errorstr, "Information",MessageBoxButtons.YesNo,icon); 
		}
		/// <summary>
		/// Displays all errors as information
		/// </summary>
		/// <param name="errorcode">Errorcode</param>
		public static void ShowInformation(int errorcode)
		{
			ErrorType errortype;
			String errorstr = ErrorHandler.getErrorStr(errorcode, out errortype);
			MessageBoxIcon icon = getIcon(errortype);
			MessageBox.Show(null, errorstr, "Information",MessageBoxButtons.OK,icon); 
		}
		/// <summary>
		/// Display error as default
		/// </summary>
		/// <param name="errorcode">Errorcode</param>
		/// <returns>Returns true if a error is shown</returns>
		public static bool ShowError(int errorcode)
		{
			ErrorType errortype;
			String errorstr = ErrorHandler.getErrorStr(errorcode, out errortype);
/*			String capt="";
			switch (errortype)
			{
				case ErrorType.Error:
					capt = "Ett fel har inträffat ("+errorcode+")";
					break;
				case ErrorType.Warning:
					capt = "Varning";
					break;
				case ErrorType.Information:
					capt = "Information";
					break;
			}
			
			MessageBoxIcon icon = getIcon(errortype);
			*/
//			MessageBox.Show(null, errorstr, capt,MessageBoxButtons.OK,icon); 
			if ( errorstr == "" )
				return false;
			ShowErrorMessage(errorcode, errorstr, errortype);
			return true;
		}

		public static void ShowErrorMessage(int errorcode, string errorstr, ErrorType errortype)
		{
			String capt="";
			switch (errortype)
			{
				case ErrorType.Error:
					capt = "Ett fel har inträffat ("+errorcode+")";
					break;
				case ErrorType.Warning:
					capt = "Varning";
					break;
				case ErrorType.Information:
					capt = "Information";
					break;
			}
			MessageBoxIcon icon = getIcon(errortype);
			MessageBox.Show(null, errorstr, capt,MessageBoxButtons.OK,icon); 
		}
	}
}
