using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Reflection;

using Dabarto.Data.Squatt.Data.Attributes;
using Dabarto.Data.Squatt.Data.Exceptions;
using Dabarto.Data.Squatt.Data.Providers;
using Dabarto.Data.Squatt.Data.Utils;
using System.Linq.Expressions;

namespace Dabarto.Data.Squatt.Data
{
    /// <summary>
    /// A class providing some basic operations on a database.
    /// </summary>
    /// <typeparam name="T">The type to work on.</typeparam>
    public class Factory<T>
    {
        #region Fields

        private SquattProvider _provider;
        private Type _type;

        private string _tableName;
		private string _viewName;
        private string _keyField;
        private List<string> _fields; // fields to choose from database
		private List<string> _viewFields; // fields to choose from view
		private Dictionary<string, object> _values; // fields to choose from database

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new factory.
        /// </summary>
        public Factory()
        {
            _provider = null;
            _type = typeof(T);

            _tableName = null;
			_viewName = null;
            _keyField = null;
            _fields = null;
			_values = null;
			_viewFields = null;
        }

        #endregion

        #region Select Methods

        /// <summary>
        /// Selects single row from a database.
        /// </summary>
        /// <param name="id">Key field value.</param>
        /// <returns>Single row mapped to object of a given type.</returns>
        public virtual T Select(int id)
        {
            RefactorObject(default(T));            

            // Getting query and executing it
            string query = Provider.GetSelectQuery(_tableName, _keyField, id, _fields);
            DataTable dataTable = Provider.ExecuteQuery(query);

            // Making instances...
            List<T> instances = MakeInstances(dataTable);
            if (instances != null && instances.Count > 0)
            {
                // ...and returning the first one
                return instances.GetFirst();
            }
            else
                return default(T);
        }

        /// <summary>
        /// Selects all rows from a database.
        /// </summary>
        /// <returns>All rows mapped to objects of a given type.</returns>
        public virtual List<T> SelectAll(bool fromView = false)
        {
			RefactorObject(default(T));

            // Getting query and executing it
			var query = string.Empty;
			if (fromView && !string.IsNullOrEmpty(_viewName) && _viewFields != null)
			{
				var tableName = _viewName;
				var fields = _fields;
				fields.AddRange(_viewFields);
				query = Provider.GetSelectAllQuery(tableName, _keyField, fields);
			}
			else
			{
				query = Provider.GetSelectAllQuery(_tableName, _keyField, _fields);
			}
            var dataTable = Provider.ExecuteQuery(query);

            // Making instances...
            return MakeInstances(dataTable);
        }

        /// <summary>
        /// Selects rows by a given condition.
        /// </summary>
        /// <param name="condition">Condition to select rows by.</param>
        /// <returns>Matched rows mapped to objects of a given type.</returns>
        public virtual List<T> SelectConditional(string condition, bool fromView = false)
        {
			// change to SelectAll if empty condition was specified
			if (string.IsNullOrWhiteSpace(condition))
			{
				return SelectAll(fromView);
			}

			RefactorObject(default(T));

            // Getting query and executing it
			var query = string.Empty;
			if (fromView && !string.IsNullOrEmpty(_viewName) && _viewFields != null)
			{
				var tableName = _viewName;
				var fields = _fields;
				fields.AddRange(_viewFields);
				query = Provider.GetSelectConditionalQuery(tableName, _keyField, fields, condition);
			}
			else
			{
				query = Provider.GetSelectConditionalQuery(_tableName, _keyField, _fields, condition);
			}
            var dataTable = Provider.ExecuteQuery(query);

            // Making instances...
            return MakeInstances(dataTable);
        }

        #endregion

		#region Insert Method

		public virtual int Insert(T obj)
		{
			RefactorObject(obj);

			// Getting query and executing it
			string query = Provider.GetInsertQuery(_tableName, _keyField, _values);
			return Provider.PerformInsert(query);
		}

		#endregion

		#region Update Methods

		public void Update(T obj, params Expression<Func<T, object>>[] properties)
		{
			RefactorObject(obj);

			var values = default(Dictionary<string, object>);
			if (properties != null && properties.Length > 0)
			{
				values = new Dictionary<string, object>();
				foreach (var property in properties)
				{
					var propertyName = string.Empty;

					if (property.Body is MemberExpression)
					{
						propertyName = ((MemberExpression)property.Body).Member.Name;
					}
					else if (property.Body is UnaryExpression)
					{
						propertyName = ((MemberExpression)((UnaryExpression)property.Body).Operand).Member.Name;
					}

					var propertyValue = _values[propertyName];

					values.Add(propertyName, propertyValue);
				}
				if (!values.ContainsKey(_keyField))
				{
					// always add key name-value
					values.Add(_keyField, _values[_keyField]);
				}
			}
			else
			{
				values = _values;
			}

			// Getting query and executing it
			string query = Provider.GetUpdateQuery(_tableName, _keyField, values);
			Provider.ExecuteNonQuery(query);
		}

		#endregion

		public string EscapeString(string str)
		{
			return Provider.EscapeString(str);
		}

		public int ExecuteNonQuery(string query)
		{
			return Provider.ExecuteNonQuery(query);
		}

		public DataTable ExecuteQuery(string query)
		{
			return Provider.ExecuteQuery(query);
		}

		#region Private helper methods and properties

		/// <summary>
        /// Gets the provider class.
        /// </summary>
        private SquattProvider Provider
        {
            get
            {
                if (_provider == null)
                {
                    // Check if connection string was set
                    if (Configuration.ConnectionString == null)
                    {
                        throw new SquattConfigurationException("Connection string wasn't set.");
                    }

                    // Load the provider
                    if (Configuration.ProviderName == null)
                    {
                        throw new SquattConfigurationException("Provider name wasn't given.");
                    }

                    // Trying in all libraries in a dll's directory
                    var files = Directory.GetFiles(new Uri(System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase)).LocalPath, "*.dll");
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

        /// <summary>
        /// Gets some crucial information from class's attributes.
        /// </summary>
        private void RefactorObject(T obj)
        {
            // Do we need to refactor?
            if (_fields == null || _values == null || _keyField == null || _tableName == null)
            {
                _fields = new List<string>();
				_viewFields = new List<string>();
				_values = new Dictionary<string, object>();

                object[] attributes = _type.GetCustomAttributes(true);
                foreach (var attribute in attributes)
                {
                    // We're trying to get the table name
                    if (_tableName == null && attribute is DbTableAttribute)
                    {
                        DbTableAttribute tableAttribute = (DbTableAttribute)attribute;
                        _tableName = tableAttribute.TableName;
						if (_viewName != null)
						{
							break;
						}
                    }

					// Maybe there is a view name?
					if (_viewName == null && attribute is DbViewAttribute)
					{
						var viewAttribute = (DbViewAttribute)attribute;
						_viewName = viewAttribute.ViewName;
						if (_tableName != null)
						{
							break;
						}
					}
                }

                if (_tableName == null)
                {
                    // If the class doesn't name the attribute, get the class name as a table name
                    _tableName = _type.Name;
                }

                if (_tableName == null)
                {
                    // In any case...
                    throw new SquattException("Unable to find table name attribute.");
                }

                // Getting fields (from properties) and values
                foreach (var mi in _type.GetMembers())
                {
                    if (mi.MemberType == MemberTypes.Property)
                    {
                        attributes = mi.GetCustomAttributes(true);
                        foreach (var attribute in attributes)
                        {
                            if (attribute is DbKeyFieldAttribute)
                            {
                                // Key field
                                _keyField = mi.Name;
								if (obj != null)
								{
									_values.Add(mi.Name, GetValue(obj, mi.Name));
								}
                                break;
                            }
                            else if (attribute is DbFieldAttribute)
                            {
                                // Other field
                                _fields.Add(mi.Name);
								if (obj != null)
								{
									_values.Add(mi.Name, GetValue(obj, mi.Name));
								}
                            }
							else if (attribute is DbViewFieldAttribute)
							{
								// View field
								_viewFields.Add(mi.Name);
							}
                        }
                    }
                }

                if (_keyField == null)
                {
                    // We cannot guess this time...
                    throw new SquattException("Unable to find key field name attribute.");
                }
            }
        }

		private object GetValue(T obj, string propertyName)
		{
			PropertyInfo pi = typeof(T).GetProperty(propertyName, BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
			return pi.GetValue(obj, null);
		}

        /// <summary>
        /// Makes instances from DataTable.
        /// </summary>
        /// <param name="dataTable">DataTable to make instances from.</param>
        /// <returns>The list of instances.</returns>
        private List<T> MakeInstances(DataTable dataTable)
        {
            List<T> result = new List<T>();

            if (dataTable != null && dataTable.Rows != null)
            {
                foreach (DataRow row in dataTable.Rows)
                {
                    // Creating an instance
                    T instance = (T)Activator.CreateInstance(_type);

                    // Setting properties
                    for (int i = 0; i < row.ItemArray.Length; i++)
                    {
                        string columnName = dataTable.Columns[i].ColumnName;
                        object value = row.ItemArray[i];

                        var property = _type.GetProperty(columnName);
                        if (property.CanWrite)
                        {
                            if (value is DBNull)
                            {
                                // DBNull - try to set null
                                if (property.PropertyType.IsValueType)
                                {
                                    if (IsNullableType(property.PropertyType))
                                    {
                                        // It is value type but nullable - assign null
                                        value = null;
                                    }
                                    else
                                    {
                                        // Not nullable - what to do?
                                        throw new SquattException(string.Concat("Trying to assign DBNull to a not nullable value type (column: ", columnName, ")"));
                                    }
                                }
                                else
                                {
                                    // Not a value type - just set it to null
                                    value = null;
                                }
                            }

                            property.SetValue(instance, value, null);
                        }
                    }

                    // Adding to a list
                    result.Add(instance);
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if the type is a nullable one.
        /// </summary>
        /// <param name="type">Type to check.</param>
        /// <returns>True if the type is nullable, False otherwise.</returns>
        private bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }

        #endregion
    }
}