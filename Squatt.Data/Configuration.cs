namespace Dabarto.Data.Squatt.Data
{
    /// <summary>
    /// Configuration for Squatt.
    /// </summary>
    public static class Configuration
    {
        private static string _connectionString = null;
        private static string _providerName = null;

        /// <summary>
        /// Gets or sets a connection string to a database.
        /// </summary>
        public static string ConnectionString
        {
            get
            {
                return Configuration._connectionString;
            }
            set
            {
                Configuration._connectionString = value;
            }
        }

        /// <summary>
        /// Gets or sets the name of provider respondible for connection with a database.
        /// </summary>
        public static string ProviderName
        {
            get
            {
                return Configuration._providerName;
            }
            set
            {
                Configuration._providerName = value;
            }
        }
    }
}