using Piec.Info.Squatt.Data.Attributes;

namespace Piec.Info.Squatt.Examples.Orders
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