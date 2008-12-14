using System;
using System.Collections.Generic;
using System.Text;

namespace Piec.Info.Squatt.Data.Attributes
{
    /// <summary>
    /// An attribute that can be placed on a class to specify a database table name.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class DbTableAttribute : Attribute
    {
        private string _tableName;

        /// <summary>
        /// Gets or sets a database table name.
        /// </summary>
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
            }
        }

        /// <summary>
        /// Creates a new table attribute.
        /// </summary>
        /// <param name="tableName">A database table name.</param>
        public DbTableAttribute(string tableName)
        {
            _tableName = tableName;
        }
    }
}