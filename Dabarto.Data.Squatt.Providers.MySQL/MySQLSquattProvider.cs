using System.Data;
using MySql.Data.MySqlClient;

namespace Dabarto.Data.Squatt.Data.Providers
{
	public class SQLiteSquattProvider : SquattProvider
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

		public SQLiteSquattProvider()
		{
		}

		public override int ExecuteNonQuery(string query)
		{
			using (var connection = new MySqlConnection(Configuration.ConnectionString))
			{
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
	}
}