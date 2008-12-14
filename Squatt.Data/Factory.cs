using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

using Piec.Info.Squatt.Data.Attributes;
using Piec.Info.Squatt.Data.Exceptions;
using Piec.Info.Squatt.Data.Providers;

namespace Piec.Info.Squatt.Data
{
    /// <summary>
    /// An abstract class providing some basic operations on a database.
    /// </summary>
    public class Factory<T>
    {
        private SquattProvider _provider;

        private SquattProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    // Load the provider
                    if (Configuration.ProviderName == null)
                    {
                        throw new SquattConfigurationException("Provider name wasn't given.");
                    }

                    // Trying in all libraries in a dll's directory
                    var files = Directory.GetFiles(Path.GetDirectoryName(typeof(Factory<T>).Assembly.Location), "*.dll");
                    foreach (var dll in files)
                    {
                        Assembly assembly = Assembly.LoadFile(dll);

                        // First try with the full name
                        Type type = assembly.GetType(Configuration.ProviderName);
                        if (type == null)
                        {
                            // Maybe there is only a class name and it is in a default namespace?
                            string name = string.Concat(typeof(SquattProvider).Namespace, ".", Configuration.ProviderName);
                            type = assembly.GetType(name);
                        }
                        if (type != null)
                        {
                            // We got it! Load and break
                            _provider = (SquattProvider)Activator.CreateInstance(type);
                            break;
                        }
                    }
                }

                if (_provider == null)
                {
                    throw new SquattConfigurationException("Cannot load database provider. Ensure that the name is correct and that a proper library is placed in the application directory.");
                }

                return _provider;
            }
        }

        public virtual T Select(int id)
        {
            string tableName = null;
            string keyField = null;
            List<string> fields = new List<string>();

            Type type = typeof(T);
            object[] attributes = type.GetCustomAttributes(true);
            foreach (var attribute in attributes)
            {
                if (attribute is DbTableAttribute)
                {
                    DbTableAttribute tableAttribute = (DbTableAttribute)attribute;
                    tableName = tableAttribute.TableName;
                    break;
                }
            }

            foreach (var mi in type.GetMembers())
            {
                if (mi.MemberType == MemberTypes.Property)
                {
                    attributes = mi.GetCustomAttributes(true);
                    foreach (var attribute in attributes)
                    {
                        if (attribute is DbKeyFieldAttribute)
                        {
                            keyField = mi.Name;
                            break;
                        }
                        else if (attribute is DbFieldAttribute)
                        {
                            fields.Add(mi.Name);
                        }
                    }
                }   
            }

            if (tableName == null)
            {
                throw new SquattException("Unable to find table name attribute.");
            }

            if (keyField == null)
            {
                throw new SquattException("Unable to find key field name attribute.");
            }

            string query = Provider.GetSelectQuery(tableName, keyField, id, fields);
            DataTable dataTable = Provider.ExecuteQuery(query);

            Type tType = typeof(T);
            foreach (DataRow row in dataTable.Rows)
	        {
                T instance = (T)Activator.CreateInstance(tType);
                for (int i = 0; i < row.ItemArray.Length; i++)
	            {
                    string columnName = dataTable.Columns[i].ColumnName;
                    object value = row.ItemArray[i];
                    var property = type.GetProperty(columnName);
                    if (property.CanWrite)
                    {
                        if (value is DBNull)
                        {
                            if (property.PropertyType.IsValueType)
                            {
                                if (IsNullableType(property.PropertyType))
                                {
                                    value = null;
                                }
                                else
                                {
                                    throw new SquattException("Trying to assign DbNull to a not-nullable value type (column: " + columnName);
                                }
                            }
                            else
                            {
                                value = null;
                            }
                        }

                        property.SetValue(instance, value, null);
                    }
	            }

                return instance;
	        }

            return default(T);
        }

        private bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}