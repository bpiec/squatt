using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Piec.Info.Squatt.Data.Providers
{
    /// <summary>
    /// An abstract class for a database provider.
    /// </summary>
    public abstract class SquattProvider
    {
        protected string _mainTableAlias = "m";

        #region Identifier enclosing characters

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

        #endregion

        #region Database methods

        public abstract void ExecuteNonQuery(string query);

        public abstract DataTable ExecuteQuery(string query);

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