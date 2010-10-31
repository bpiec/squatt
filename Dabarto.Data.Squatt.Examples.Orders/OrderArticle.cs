using Dabarto.Data.Squatt.Data.Attributes;

namespace Dabarto.Data.Squatt.Examples.Orders
{
    public class OrderArticle
    {
        [DbKeyField]
        public long? OrderArticleID
        {
            get;
            set;
        }

        [DbField]
        public long? OrderID
        {
            get;
            set;
        }

        [DbField]
        public long? ArticleID
        {
            get;
            set;
        }

        [DbField]
        public long? Quantity
        {
            get;
            set;
        }
    }
}