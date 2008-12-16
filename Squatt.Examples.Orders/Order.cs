using System;

using Piec.Info.Squatt.Data.Attributes;

namespace Piec.Info.Squatt.Examples.Orders
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