using System;
using System.Collections.Generic;
using System.Text;

namespace Dabarto.Data.Squatt.Data.Attributes
{
	/// <summary>
	/// An attribute that can be placed on a class to specify a database view name.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class)]
	public class DbViewAttribute : Attribute
	{
		private string _viewName;

		/// <summary>
		/// Gets or sets a database view name.
		/// </summary>
		public string ViewName
		{
			get
			{
				return _viewName;
			}
			set
			{
				_viewName = value;
			}
		}

		/// <summary>
		/// Creates a new view attribute.
		/// </summary>
		/// <param name="tableName">A database table name.</param>
		public DbViewAttribute(string viewName)
		{
			_viewName = viewName;
		}
	}
}