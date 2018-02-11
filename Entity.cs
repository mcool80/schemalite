using System;

namespace SchemaLite
{
	/// <summary>
	/// An entity, only a base class
	/// </summary>
	public class Entity
	{
		/// <summary>
		/// Name of the entity
		/// </summary>
		protected String name_;

		/// <summary>
		/// Color of the entity coded as argb
		/// </summary>
		protected String color_;

		/// <summary>
		/// 
		/// </summary>
		public Entity()
		{
		}

		/// <summary>
		/// Get and set name
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
		/// Get and set color coded as argb-number
		/// </summary>
		public String Color
		{
			get
			{
				return color_;
			}
			set
			{
				color_ = value;
			}
		}
	}
}
