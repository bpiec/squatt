using Piec.Info.Squatt.Data.Attributes;

namespace Piec.Info.Squatt.Examples.Orders
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