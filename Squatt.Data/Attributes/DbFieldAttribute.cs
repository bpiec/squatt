using System;

namespace Piec.Info.Squatt.Data.Attributes
{
    /// <summary>
    /// An attribute that can be placed on a field to specify that it is a database table column.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class DbFieldAttribute : Attribute
    {
    }
}