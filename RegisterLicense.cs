using System;

namespace SchemaLite
{
	/// <summary>
	/// Summary description for RegisterLicense.
	/// </summary>
	public class RegisterLicense
	{
		public RegisterLicense()
		{
		}

		/// <summary>
		/// Private key, väldigt hemlig
		/// </summary>
		static private int key = 851;

		/// <summary>
		/// Inverterar med privat nyckel
		/// </summary>
		/// <param name="lkey"></param>
		/// <returns></returns>
		static private string invertKey(string lkey)
		{
			string newkey = "";
			for ( int i = 0; i < lkey.Length; i +=5)
			{
				try
				{
					int a = (int.Parse(lkey.Substring(i,4))^key);
					newkey += convstr(a);
					if ( i != 15 )
						newkey += "-";
				}
				catch
				{
					newkey += "-";
				}
			}
			return newkey;
		}

		/// <summary>
		/// Controls that LicKey is ok
		/// </summary>
		/// <param name="lkey">LicenceKey to check</param>
		/// <returns>True if it is correct</returns>
		static public bool isLicensKeyOK(string lkey)
		{
			// Check length
			if ( lkey.Length < 19 || lkey.Length > 19 )
				return false;
			// Invert the key
			lkey = RegisterLicense.invertKey(lkey);
			// Get the checksum
			int chksum = int.Parse(lkey.Substring(15, 4));
			// Get the sum bases
			int tal1 = int.Parse(lkey.Substring(0,4));
			int tal2 = int.Parse(lkey.Substring(5,4));
			int tal3 = int.Parse(lkey.Substring(10,4));
			int tot;
			// Do calculation
			Math.DivRem(tal1+tal2+tal3, 10000, out tot);
			// If the checksum matches the calcualtion its ok
			if ( tot == chksum )
				return true;
			return false;
		}

		/// <summary>
		/// Converts a number to a 4 digit string, adding 0s infront
		/// </summary>
		/// <param name="tal">number to convert</param>
		/// <returns>A string of 4 digits</returns>
		static private string convstr(int tal)
		{
			int a;
			Math.DivRem(tal, 10000, out a);
			string astr = a.ToString();
			string retstr = "";
			for ( int i = 4; i > astr.Length; i --)
				retstr +="0";
			retstr+=astr;
			return retstr;
		}

		/// <summary>
		/// Gets a new correct noninverted Licenskey
		/// </summary>
		/// <returns>A correct LicensKey</returns>
		static string getstr()
		{
			string newstr = "";
			Random rnd = new Random();
			int tal1;
			Math.DivRem(rnd.Next(), 10000, out tal1);
			int tal2;
			Math.DivRem(rnd.Next(), 10000, out tal2);
			int tal3;
			Math.DivRem(rnd.Next(), 10000, out tal3);
			int tot;
			Math.DivRem(tal1+tal2+tal3, 10000, out tot);
			newstr += convstr(tal1);
			newstr += "-";
			newstr += convstr(tal2);
			newstr += "-";
			newstr += convstr(tal3);
			newstr += "-";
			newstr += convstr(tot);
			return newstr;
		}

		/// <summary>
		/// Get the inverted LicensKey
		/// </summary>
		/// <returns>A inverted licenskey to be used as a registration key</returns>
		static public string createLicensKey()
		{
			string newstr = getstr();
			return invertKey(newstr);
		}
	}
}
