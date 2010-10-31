using Dabarto.Data.Squatt.Data.Attributes;

namespace Dabarto.Data.Squatt.Examples.Orders
{
    public class Article
    {
        [DbKeyField]
        public long? ArticleID
        {
            get;
            set;
        }

        [DbField]
        public string Name
        {
            get;
            set;
        }

        [DbField]
        public float? Price
        {
            get;
            set;
        }
    }
}