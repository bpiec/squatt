using Dabarto.Data.Squatt.Data.Attributes;

namespace Dabarto.Data.Squatt.Examples.Orders
{
    public class Client
    {
        [DbKeyField]
        public long? ClientID
        {
            get;
            set;
        }

        [DbField]
        public string FirstName
        {
            get;
            set;
        }

        [DbField]
        public string LastName
        {
            get;
            set;
        }

        [DbField]
        public string Address
        {
            get;
            set;
        }
    }
}