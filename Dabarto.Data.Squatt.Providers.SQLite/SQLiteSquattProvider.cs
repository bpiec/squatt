using System.Data;
using System.Data.SQLite;

namespace Dabarto.Data.Squatt.Data.Providers
{
	public class SQLiteSquattProvider : SquattProvider
	{
		public override string IdentifierEnclosingStartChar
		{
			get
			{
				return "[";
			}
		}

		public override string IdentifierEnclosingEndChar
		{
			get
			{
				return "]";
			}
		}

		public SQLiteSquattProvider()
		{
		}

		public override int ExecuteNonQuery(string query)
		{
			using (var connection = new SQLiteConnection(Configuration.ConnectionString))
			{
				connection.Open();
				var command = connection.CreateCommand();
				command.CommandType = CommandType.Text;
				command.CommandText = query;
				var res = command.ExecuteNonQuery();
				connection.Close();
				return res;
			}
		}

		public override DataTable ExecuteQuery(string query)
		{
			DataTable dataTable = new DataTable();

			using (var connection = new SQLiteConnection(Configuration.ConnectionString))
			{
				var adapter = new SQLiteDataAdapter(query, connection);
				adapter.Fill(dataTable);
			}

			return dataTable;
		}

		public override int PerformInsert(string query)
		{
			using (var connection = new SQLiteConnection(Configuration.ConnectionString))
			{
				connection.Open();
				var transaction = connection.BeginTransaction();

				try
				{
					var command = connection.CreateCommand();
					command.CommandType = CommandType.Text;
					command.CommandText = query;
					command.ExecuteNonQuery();

					command.CommandText = "SELECT LAST_INSERT_ROWID();";
					var result = command.ExecuteScalar();

					transaction.Commit();

					return int.Parse(result.ToString());
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
			}
		}

		public override void TruncateTable(string tableName)
		{
			ExecuteNonQuery(string.Format("DELETE FROM {0}{1}{2}", IdentifierEnclosingStartChar, EscapeString(tableName), IdentifierEnclosingEndChar));
			ExecuteNonQuery(string.Format("DELETE FROM SQLITE_SEQUENCE WHERE name='{0}'", EscapeString(tableName)));
		}

		public override string EscapeString(string str)
		{
			return System.Security.SecurityElement.Escape(str);
		}
	}
}