using System.Collections.Generic;
using System.Data;
using System.Text;
using System;
using System.Globalization;

namespace Dabarto.Data.Squatt.Data.Providers
{
    /// <summary>
    /// An abstract class for a database provider.
    /// </summary>
    public abstract class SquattProvider
    {
        protected string _mainTableAlias = "m";
		protected string _nullString = "NULL";

        #region Identifier and string enclosing characters

        /// <summary>
        /// Gets a start quoting symbol for names that should be treated as identifiers.
        /// </summary>
        public abstract string IdentifierEnclosingStartChar
        {
            get;
        }

        /// <summary>
        /// Gets an end quoting symbol for names that should be treated as identifiers.
        /// </summary>
        public abstract string IdentifierEnclosingEndChar
        {
            get;
        }

		/// <summary>
		/// Gets a start quoting symbol for strings.
		/// </summary>
		public virtual string StringEnclosingStartChar
		{
			get
			{
				return "'";
			}
		}

		/// <summary>
		/// Gets an end quoting symbol for strings.
		/// </summary>
		public virtual string StringEnclosingEndChar
		{
			get
			{
				return "'";
			}
		}

        #endregion

        #region Queries

        /// <summary>
        /// Returns a query to select single row.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="keyFieldName">Field name that is a key in a database.</param>
        /// <param name="id">Identifier to select by.</param>
        /// <param name="fieldNames">Field names to retrieve from database.</param>
        /// <returns>Query to database.</returns>
        public virtual string GetSelectQuery(string tableName, string keyFieldName, long id, List<string> fieldNames)
        {
            return string.Format("{0} WHERE {1} = {2}", GetGeneralSelectQuery(tableName, keyFieldName, fieldNames), GetQuotedIdentifierName(keyFieldName), id);
        }

        public virtual string GetSelectAllQuery(string tableName, string keyFieldName, List<string> fieldNames)
        {
            return GetGeneralSelectQuery(tableName, keyFieldName, fieldNames);
        }

        public virtual string GetSelectConditionalQuery(string tableName, string keyFieldName, List<string> fieldNames, string condition)
        {
            return string.Format("{0} WHERE {1}", GetGeneralSelectQuery(tableName, keyFieldName, fieldNames), condition);
        }

		public virtual string GetInsertQuery(string tableName, string keyFieldName, Dictionary<string, object> values)
		{
			StringBuilder fields = new StringBuilder();
			foreach (var fieldName in values.Keys)
			{
				if (fields.ToString() != string.Empty)
				{
					fields.Append(", ");
				}

				fields.Append(GetQuotedIdentifierName(fieldName));
			}

			StringBuilder vals = new StringBuilder();
			foreach (var val in values.Values)
			{
				if (vals.ToString() != string.Empty)
				{
					vals.Append(", ");
				}

				vals.Append(GetValue(val));
			}

			return string.Format("INSERT INTO {0} ({1}) VALUES ({2})", GetQuotedIdentifierName(tableName), fields, vals);
		}

		public virtual string GetUpdateQuery(string tableName, string keyName, Dictionary<string, object> values)
		{
			StringBuilder fields = new StringBuilder();
			foreach (var fieldName in values.Keys)
			{
				if (fieldName == keyName)
				{
					// do not update key value
					continue;
				}

				if (fields.ToString() != string.Empty)
				{
					fields.Append(", ");
				}

				fields.Append(string.Format("{0} = {1}", GetQuotedIdentifierName(fieldName), GetValue(values[fieldName])));
			}

			return string.Format("UPDATE {0} SET {1} WHERE {2} = {3}", GetQuotedIdentifierName(tableName), fields, GetQuotedIdentifierName(keyName), GetValue(values[keyName]));
		}

        #endregion

        #region Database methods

        public abstract int ExecuteNonQuery(string query);

        public abstract DataTable ExecuteQuery(string query);

		public abstract int PerformInsert(string query);

		public abstract string EscapeString(string str);

        #endregion

        #region Helper methods

        /// <summary>
        /// Returns and identifier name with start and end quoting symbol concatenated.
        /// </summary>
        /// <param name="tableName">Identifier name.</param>
        /// <returns>Concatenated identifier name.</returns>
        private string GetQuotedIdentifierName(string identifierName)
        {
            return string.Concat(IdentifierEnclosingStartChar, identifierName, IdentifierEnclosingEndChar);
        }

		private string GetValue(object val)
		{
			if (val == null)
				return _nullString;

			switch (Type.GetTypeCode(val.GetType()))
			{
				case TypeCode.DBNull:
				case TypeCode.Empty:
					return _nullString;
				case TypeCode.Boolean:
					return (bool)val ? 1.ToString() : 0.ToString();
				case TypeCode.Byte:
				case TypeCode.Int16:
				case TypeCode.Int32:
				case TypeCode.Int64:
				case TypeCode.Object:
				case TypeCode.SByte:
				case TypeCode.UInt16:
				case TypeCode.UInt32:
				case TypeCode.UInt64:
					return val.ToString();
				case TypeCode.Decimal:
					return ((decimal)val).ToString(CultureInfo.InvariantCulture.NumberFormat);
				case TypeCode.Double:
					return ((double)val).ToString(CultureInfo.InvariantCulture.NumberFormat);
				case TypeCode.Single:
					return ((float)val).ToString(CultureInfo.InvariantCulture.NumberFormat);
				case TypeCode.DateTime:
					return string.Concat(StringEnclosingStartChar, ((DateTime)val).ToShortDateString(), " ", ((DateTime)val).ToLongTimeString(), StringEnclosingEndChar);
				case TypeCode.Char:
				case TypeCode.String:
				default:
					return string.Concat(StringEnclosingStartChar, EscapeString(val.ToString()), StringEnclosingEndChar);
			}
		}

        /// <summary>
        /// Returns a general query to be used by other methods.
        /// </summary>
        /// <param name="tableName">Table name.</param>
        /// <param name="keyFieldName">Field name that is a key in a database.</param>
        /// <param name="fieldNames">Field names to retrieve from database.</param>
        /// <returns>Query to database.</returns>
        private string GetGeneralSelectQuery(string tableName, string keyFieldName, List<string> fieldNames)
        {
            StringBuilder fields = new StringBuilder();
            fields.Append(_mainTableAlias);
            fields.Append(".");
            fields.Append(GetQuotedIdentifierName(keyFieldName));
            fields.Append(" AS '");
            fields.Append(keyFieldName);
            fields.Append("'");

            foreach (var fieldName in fieldNames)
            {
                fields.Append(", ");

                fields.Append(_mainTableAlias);
                fields.Append(".");
                fields.Append(GetQuotedIdentifierName(fieldName));
                fields.Append(" AS '");
                fields.Append(fieldName);
                fields.Append("'");
            }

            return string.Format("SELECT {0} FROM {1} {2}", fields, GetQuotedIdentifierName(tableName), _mainTableAlias);
        }

        #endregion
    }
}