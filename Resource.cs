using System;

namespace SchemaLite
{
	/// <summary>
	/// 
	/// </summary>
	public class Resource : SchemaLite.Entity
	{
		public Resource()
		{
		}

		/// <summary>
		/// Creates a resource
		/// </summary>
		/// <param name="name">Name on resource</param>
		/// <param name="color">Color coded as argb</param>
		public Resource(String name, String color)
		{
			name_ = name;
			color_ = color;
		}
	}
}
