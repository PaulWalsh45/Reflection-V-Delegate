using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;


namespace ReflectionDelegate
{
    public static class Extensions
    {
        public delegate PropertyInfo[] RequestPropertyInfoDelegate(Type type);
        public delegate PropertyInfo RequestPropertyDelegate(Type type, string propertyName);

        public static List<string> Compare<T>(this T x, T y, List<string> ignore = null)
        {
            var diffs = new List<string>();
            var type = typeof(T);

            // Loop to measure performance
            for (int i = 0; i < 1000000; i++)
            {
                foreach (var property in type.GetProperties())
                {
                    var xValue = string.Empty;
                    var yValue = string.Empty;

                    if (type.GetProperty(property.Name)?.GetValue(x, null) != null)
                        xValue = type.GetProperty(property.Name)?.GetValue(x, null)?.ToString();

                    if (type.GetProperty(property.Name)?.GetValue(y, null) != null)
                        yValue = type.GetProperty(property.Name)?.GetValue(y, null)?.ToString();

                    if (ignore == null || !ignore.Contains(property.Name))
                    {
                        if (xValue != yValue)
                        {
                            diffs.Add($"{type.Name} difference - '{property.Name} : {xValue}' != {property.Name} :'{yValue}'");
                        }
                    }

                }
            }
            

            return diffs;
        }
        

        public static List<string> CompareUsingDelegate<T>(this T x, T y, List<string> ignore = null)
        {
            var diffs = new List<string>();
            var type = typeof(T);
            var propertyDelegate = new RequestPropertyDelegate(GetProperty);
            var propertyInfoDelegate = new RequestPropertyInfoDelegate(GetProperties);
            var props = propertyInfoDelegate(type);

            // Loop to measure performance
            for (int i = 0; i < 1000000; i++)
            {
                foreach (var property in props)
                {
                    var xValue = string.Empty;
                    var yValue = string.Empty;

                    var prop = propertyDelegate(typeof(T), property.Name);
                    xValue = prop?.GetValue(x, null)?.ToString();
                    yValue = prop?.GetValue(y, null)?.ToString();

                    if (ignore == null || !ignore.Contains(property.Name))
                    {
                        if (xValue != yValue)
                        {
                            diffs.Add($"{type.Name} difference - '{property.Name} : {xValue}' != {property.Name} :'{yValue}'");
                        }
                    }

                }
            }
            
            
            return diffs;

        }

        public static PropertyInfo[] GetProperties(Type type)
        {
            return type.GetProperties();
        }

        public static PropertyInfo GetProperty(Type type, string name)
        {
            return type.GetProperty(name);
        }
        
    }
}
