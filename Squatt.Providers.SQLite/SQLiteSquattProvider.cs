using System.Data;
using System.Data.SQLite;

namespace Piec.Info.Squatt.Data.Providers
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

        public override void ExecuteNonQuery(string query)
        {
            throw new System.NotImplementedException();
        }

        public override DataTable ExecuteQuery(string query)
        {
            DataTable dataTable = new DataTable();

            using (SQLiteConnection connection = new SQLiteConnection(Configuration.ConnectionString))
            {
                SQLiteDataAdapter adapter = new SQLiteDataAdapter(query, connection);
                adapter.Fill(dataTable);
            }

            return dataTable;
        }
    }
}