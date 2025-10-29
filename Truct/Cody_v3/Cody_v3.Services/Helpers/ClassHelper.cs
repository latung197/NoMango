using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Cody_v3.Services.Helpers
{
    public static  class ClassHelper
    {

            /// <summary>
            /// Convert List<T> sang một DataTable
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="objectClass"></param>
            /// <param name="table_name"></param>
            /// <returns></returns>
            public static DataTable ToDataTable<T>(this List<T> objectClass, string table_name = "Table")
            {
                DataTable dt = new DataTable();
                try
                {
                    dt.TableName = table_name;
                    PropertyInfo[] listInfo = objectClass[0].GetType().GetProperties();
                    foreach (PropertyInfo property in listInfo)
                    {
                        var propertyType = property.PropertyType;
                        if (propertyType.IsGenericType &&
                            propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            propertyType = propertyType.GetGenericArguments()[0];
                        }
                        dt.Columns.Add(new DataColumn(property.Name, propertyType));
                    }

                    foreach (var vehicle in objectClass)
                    {
                        DataRow newRow = dt.NewRow();
                        foreach (PropertyInfo property in vehicle.GetType().GetProperties())
                        {
                            var type = vehicle.GetType().GetProperty(property.Name);
                            var value = type.GetValue(vehicle, null);

                            if (value != null)
                                newRow[property.Name] = value;
                            else
                                newRow[property.Name] = DBNull.Value;
                        }
                        dt.Rows.Add(newRow);
                    }
                    return dt;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return null;
                }
            }

            /// <summary>
            /// Convert DataTable to List<T>
            /// </summary>
            /// <typeparam name="T"></typeparam>
            /// <param name="dt"></param>
            /// <returns></returns>
            public static IEnumerable<T> AsEnumerable<T>(this DataTable dataTable) where T : new()
            {
                foreach (DataRow dr in dataTable.Rows)
                {
                    yield return dr.ToObject<T>();
                }
            }

            public static List<T> ToList<T>(this DataTable dataTable) where T : new()
            {
                return dataTable.AsEnumerable<T>().ToList();
            }

            public static T ToObject<T>(this DataRow dataRow) where T : new()
            {
                T item = new T();

                foreach (DataColumn column in dataRow.Table.Columns)
                {
                    PropertyInfo property = GetProperty(typeof(T), column.ColumnName);

                    if (property != null && dataRow[column] != DBNull.Value && dataRow[column].ToString() != "NULL")
                    {
                        property.SetValue(item, ChangeType(dataRow[column], property.PropertyType), null);
                    }
                }
                return item;
            }

            private static PropertyInfo GetProperty(Type type, string attributeName)
            {
                PropertyInfo property = type.GetProperty(attributeName);

                if (property != null)
                {
                    return property;
                }

                return type.GetProperties()
                     .Where(p => p.IsDefined(typeof(DisplayAttribute), false) && p.GetCustomAttributes(typeof(DisplayAttribute), false).Cast<DisplayAttribute>().Single().Name == attributeName)
                     .FirstOrDefault();
            }

            public static object ChangeType(object value, Type type)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>)))
                {
                    if (value == null)
                    {
                        return null;
                    }

                    return Convert.ChangeType(value, Nullable.GetUnderlyingType(type));
                }

                return Convert.ChangeType(value, type);
            }

        
    }
}
