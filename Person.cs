using System;

namespace SchemaLite
{
	/// <summary>
	/// A person
	/// </summary>
	public class Person : SchemaLite.Entity
	{
		/// <summary>
		/// Password for the person
		/// </summary>
		private String password_;

		/// <summary>
		/// True if the person is adminstrator
		/// </summary>
		private bool admin_;

		/// <summary>
		/// 
		/// </summary>
		public Person()
		{
		}

		/// <summary>
		/// Creates the person
		/// </summary>
		/// <param name="name">Name</param>
		/// <param name="color">Color coded as argb</param>
		/// <param name="password">Password</param>
		public Person(String name, String color, String password, bool admin)
		{
			password_ = password;
			color_ = color;
			name_ = name;
			admin_ = admin;
		}

		/// <summary>
		/// Get and set password
		/// </summary>
		public String Password
		{
			get
			{
				return password_;
			}
			set
			{
				password_ = value;
			}
		}
		/// <summary>
		/// Get if the person is administrator
		/// </summary>
		public bool Administrator
		{
			get
			{
				return admin_;
			}
		}
	}
}
