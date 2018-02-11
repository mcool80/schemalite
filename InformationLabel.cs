using System;

namespace SchemaLiteUI
{
	/// <summary>
	/// 
	/// </summary>
	public class InformationLabel : System.Windows.Forms.Label, Observer
	{
		public InformationLabel()
		{
		}
		#region Observer Members

		public void Update(object subject)
		{
			Text = (String)subject;
		}

		#endregion
	}
}
