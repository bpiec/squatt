using System;

using Dabarto.Data.Squatt.Data.Attributes;

namespace Dabarto.Data.Squatt.Examples.Orders
{
    public class Order
    {
        [DbKeyField]
        public long? OrderID
        {
            get;
            set;
        }

        [DbField]
        public DateTime? CreationDate
        {
            get;
            set;
        }

        [DbField]
        public long? ClientID
        {
            get;
            set;
        }
    }
}