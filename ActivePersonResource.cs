using System;

namespace SchemaLite
{
	/// <summary>
	/// 
	/// </summary>
	public class ActivePersonResource : Subject
	{
		/// <summary>
		/// The person is active, the resouceboxes for person is showing
		/// </summary>
		private bool active_ = false;

		/// <summary>
		/// The person selected and can be added to scheme
		/// </summary>
		private bool selected_ = false;

		/// <summary>
		/// The entityclass
		/// </summary>
		private Entity entity_;

		public ActivePersonResource()
		{
		}
		
		/// <summary>
		/// Create a active person or resource instance
		/// </summary>
		/// <param name="entity">Person or resoúrce connected to information</param>
		/// <param name="active">True if the person is actively selected by user</param>
		/// <param name="selected">True if the user whiches to display this person or resource</param>
		public ActivePersonResource(Entity entity, bool active, bool selected)
		{
			entity_ = entity;
			active_ = active;
			selected_ = selected;
		}

		/// <summary>
		/// Get and set the person or resource is selected in GUI
		/// </summary>
		public bool Active
		{
			get
			{
				return active_;
			}
			set
			{
				active_ = value;
				this.Notify();
			}
		}

		/// <summary>
		/// Get and set if the person/resource resourceboxes  is to be displayed
		/// </summary>
		public bool Selected
		{
			get
			{
				return selected_;
			}
			set
			{
				selected_ = value;
				this.Notify();
			}
		}

		/// <summary>
		/// Get the person or resource
		/// </summary>
		public Entity Entity
		{
			get 
			{
				return entity_;
			}
		}
	}
}
