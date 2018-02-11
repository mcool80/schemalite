using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections;
using SchemaLite;

namespace SchemaLiteUI
{
	/// <summary>
	/// Summary description for CheckListUI.
	/// </summary>
	public class CheckListUI : System.Windows.Forms.CheckedListBox, Observer
	{
		public CheckListUI()
		{
			this.MouseDown +=new MouseEventHandler(CheckListUI_MouseDown);
		}

		/// <summary>
		/// Update the graphics
		/// </summary>
		/// <param name="subject"></param>
		public void Update(object subject)
		{
			
			this.Items.Clear();
			for ( int i = 0; i < Scheme.Instance.ActivePersonsResources.Count; i++)
			{
				String key = (String)((SortedList)Scheme.Instance.ActivePersonsResources).GetKey(i);
				this.Items.Add(key,((ActivePersonResource)Scheme.Instance.ActivePersonsResources[Scheme.Instance.ActivePersonsResources.GetKey(i)]).Selected);
	
				if ( key == Scheme.Instance.ActiveName )
				{
					this.SelectedIndex = i;
				} 
			}
		}

		/// <summary>
		/// Create a context menu if the right mousebutton is clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void CheckListUI_MouseDown(object sender, MouseEventArgs e)
		{
			if ( e.Button == MouseButtons.Right && this.SelectedIndex != -1 )
			{
				this.ContextMenu = new ContextMenu();
				this.ContextMenu.MenuItems.Add("Lås tider"); 
				this.ContextMenu.MenuItems.Add("Ändra"); 
				this.ContextMenu.MenuItems.Add("Ta bort"); 
				this.ContextMenu.MenuItems[0].Click += new EventHandler(CheckListUI_Click);
				this.ContextMenu.MenuItems[1].Click += new EventHandler(change_Click);
				this.ContextMenu.MenuItems[2].Click += new EventHandler(remove_Click);
				
			} 
		}
		
		/// <summary>
		/// If the change item is clicked in the context menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void change_Click(object sender, EventArgs e)
		{
			if ( this.SelectedIndex != -1 )
			{
				Entity ent = (Entity)((ActivePersonResource)Scheme.Instance.ActivePersonsResources[this.Items[this.SelectedIndex].ToString()]).Entity;
				AddPersonResourceUI aprui = new AddPersonResourceUI();
				String pass = "";
				if ( ent.GetType().Name == "Person" )
				{
					pass = ((Person)ent).Password;
				}
				aprui.setAsChange(this.Items[this.SelectedIndex].ToString(), ( ent.GetType().Name == "Resource"), pass, ent.Color);
				aprui.ShowDialog(this.Parent);
	
				this.ContextMenu.Dispose();
			}
		}

		/// <summary>
		/// If the remove item is clicked in the context menu
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void remove_Click(object sender, EventArgs e)
		{
			DialogResult dr = MessageBox.Show(this.Parent, "Vill du verkligen ta bort "+this.Items[this.SelectedIndex].ToString()+"?", "Ta bort",MessageBoxButtons.YesNo,MessageBoxIcon.Warning); 
			if ( dr == DialogResult.Yes )
			{
				Entity ent = (Entity)((ActivePersonResource)Scheme.Instance.ActivePersonsResources[this.Items[this.SelectedIndex].ToString()]).Entity;
				ErrorHandler.ShowError(Scheme.Instance.removeEntity(ent));
			}
			this.ContextMenu.Dispose();
		}

		private void CheckListUI_Click(object sender, EventArgs e)
		{
			Scheme.Instance.lockResourceBoxes(this.Items[this.SelectedIndex].ToString());
		}
	}
}
