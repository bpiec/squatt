using System.Data;
using MySql.Data.MySqlClient;

namespace Dabarto.Data.Squatt.Data.Providers
{
	public class MySQLSquattProvider : SquattProvider
	{
		public override string IdentifierEnclosingStartChar
		{
			get
			{
				return "`";
			}
		}

		public override string IdentifierEnclosingEndChar
		{
			get
			{
				return "`";
			}
		}

		public MySQLSquattProvider()
		{
		}

		public override int ExecuteNonQuery(string query)
		{
			using (var connection = new MySqlConnection(Configuration.ConnectionString))
			{
				connection.Open();
				var command = connection.CreateCommand();
				command.CommandType = CommandType.Text;
				command.CommandText = query;
				return command.ExecuteNonQuery();
			}
		}

		public override DataTable ExecuteQuery(string query)
		{
			var dataTable = new DataTable();

			using (var connection = new MySqlConnection(Configuration.ConnectionString))
			{
				var adapter = new MySqlDataAdapter(query, connection);
				adapter.Fill(dataTable);
			}

			return dataTable;
		}

		public override int PerformInsert(string query)
		{
			using (var connection = new MySqlConnection(Configuration.ConnectionString))
			{
				connection.Open();
				var transaction = connection.BeginTransaction();

				try
				{
					var command = connection.CreateCommand();
					command.CommandType = CommandType.Text;
					command.CommandText = query;
					command.ExecuteNonQuery();

					command.CommandText = "SELECT LAST_INSERT_ID();";
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

		public override string EscapeString(string str)
		{
			return MySqlHelper.EscapeString(str);
		}
	}
}