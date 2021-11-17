using System;
using System.Collections;
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

        
        public static List<string> Compare<T>(this T x, T y, int loop, List<string> ignore = null)
        {
            var diffs = new List<string>();
            var type = typeof(T);

            // Loop to measure performance,
            for (int i = 0; i < loop; i++)
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
        

        public static List<string> CompareUsingDelegate<T>(this T x, T y, int loop, List<string> ignore = null)
        {
            var diffs = new List<string>();
            var type = typeof(T);
            var propertyDelegate = new RequestPropertyDelegate(GetProperty);
            var propertyInfoDelegate = new RequestPropertyInfoDelegate(GetProperties);
            var props = propertyInfoDelegate(type);

            // Loop to measure performance
            for (int i = 0; i < loop; i++)
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

        /// <summary>
        /// This can handle properties of type object, but NOT properties of type object lists
        /// </summary>
        /// <param name="x">first object to compare</param>
        /// <param name="y">second object to compare</param>
        /// <param name="ignore">the properties to ignore</param>
        /// <returns>a list of strings </returns>
        public static List<string> CompareUsingDelegateSH(this object x, object y, List<string> ignore = null)
        {
            var diffs = new List<string>();
            var type = x.GetType();
            var propertyDelegate = new RequestPropertyDelegate(GetProperty);
            var propertyInfoDelegate = new RequestPropertyInfoDelegate(GetProperties);
            var props = propertyInfoDelegate(type);

           
            foreach (var property in props)
            {
                var xValue = string.Empty;
                var yValue = string.Empty;

                var prop = propertyDelegate(type, property.Name);
                xValue = prop?.GetValue(x, null)?.ToString();
                yValue = prop?.GetValue(y, null)?.ToString();

                // this will check if any properties are class (i.e objects)
                if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    var xChildValues = prop?.GetValue(x, null);
                    var yChildValues = prop?.GetValue(y, null);

                    //recursive call
                    diffs.AddRange(xChildValues.CompareUsingDelegateSH(yChildValues));
                }

                if (ignore == null || !ignore.Contains(property.Name))
                {
                    if (xValue != yValue)
                    {
                        diffs.Add($"{type.Name} difference - '{property.Name} : {xValue}' != {property.Name} :'{yValue}'");
                    }
                }

            }
            return diffs;
        }

        /// <summary>
        /// This is an iteration of method CompareUsingDelegateSH, fixing the handling of ignore list
        /// </summary>
        /// <param name="x">first object to compare</param>
        /// <param name="y">second object to compare</param>
        /// <param name="ignore">the properties to ignore</param>
        /// <returns>a list of strings </returns>
        public static List<string> CompareUsingDelegateSH1(this object x, object y, List<string> ignore = null)
        {
            var diffs = new List<string>();
            var type = x.GetType();
            var propertyDelegate = new RequestPropertyDelegate(GetProperty);
            var propertyInfoDelegate = new RequestPropertyInfoDelegate(GetProperties);
            var props = propertyInfoDelegate(type);


            foreach (var property in props)
            {
                var xValue = string.Empty;
                var yValue = string.Empty;

                var prop = propertyDelegate(type, property.Name);
                xValue = prop?.GetValue(x, null)?.ToString();
                yValue = prop?.GetValue(y, null)?.ToString();

                var isEnumerable = property.PropertyType != typeof(string) &&
                                   typeof(IEnumerable).IsAssignableFrom(property.PropertyType);

                if (ignore == null || !ignore.Contains(property.Name))
                {
                    // this will check if any properties are class (i.e objects)
                    if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {
                        var xChildValues = prop?.GetValue(x, null);
                        var yChildValues = prop?.GetValue(y, null);

                        //recursive call
                        diffs.AddRange(xChildValues.CompareUsingDelegateSH1(yChildValues));
                    }

                    if (xValue != yValue)
                    {
                        diffs.Add($"{type.Name} difference - '{property.Name} : {xValue}' != {property.Name} :'{yValue}'");
                    }
                }

            }
            return diffs;
        }
        public static List<string> CompareUsingDelegateSH2(this object x, object y, List<string> ignore = null)
        {
            var diffs = new List<string>();
            var type = x.GetType();
            var propertyDelegate = new RequestPropertyDelegate(GetProperty);
            var propertyInfoDelegate = new RequestPropertyInfoDelegate(GetProperties);
            var props = propertyInfoDelegate(type);


            foreach (var property in props)
            {
                var pwIgnores = property.GetCustomAttributes(typeof(CompareIgnoreAttribute));
                if (pwIgnores.Any())
                {
                    continue;
                }

                var xValue = string.Empty;
                var yValue = string.Empty;

                var prop = propertyDelegate(type, property.Name);
                if (prop.PropertyType.IsIList())
                {
                    var xList = (IList)prop.GetValue(x);
                    var yList = (IList)prop.GetValue(y);
                    if (yList != null && xList != null && xList.Count != yList.Count)
                    {
                        diffs.Add($"{type.Name} difference - '{property.Name} counts : {xList.Count} != {yList.Count}");
                    }
                    else
                    {
                        if (xList != null)
                            for (var index = 0; index < xList.Count; index++)
                            {
                                var xItem = xList[index];
                                if (yList != null)
                                {
                                    var yItem = yList[index];
                                    diffs.AddRange(xItem.CompareUsingDelegateSH2(yItem, ignore));
                                }
                            }
                    }
                }
                else
                {

                    try
                    {
                        xValue = prop?.GetValue(x, null)?.ToString();
                        yValue = prop?.GetValue(y, null)?.ToString();

#if DEBUG
                        var isEnumerable = property.PropertyType != typeof(string) &&
                                           typeof(IEnumerable).IsAssignableFrom(property.PropertyType);
#endif


                        if (ignore == null || !ignore.Contains(property.Name))
                        {
                            // this will check if any properties are class (i.e objects)
                            if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                            {
                                var xChildValues = prop?.GetValue(x, null);
                                var yChildValues = prop?.GetValue(y, null);

                                //recursive call
                                diffs.AddRange(xChildValues.CompareUsingDelegateSH2(yChildValues));
                            }

                            if (xValue != yValue)
                            {
                                diffs.Add(
                                    $"{type.Name} difference - '{property.Name} : {xValue}' != {property.Name} :'{yValue}'");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        diffs.Add(ex.Message);
                        break;
                    }
                }

            }
            return diffs;
        }

        public static List<string> CompareUsingDelegate1<T>(this T x, T y, List<string> ignore = null)
        {
            var diffs = new List<string>();
            var type = typeof(T);
            var propertyDelegate = new RequestPropertyDelegate(GetProperty);
            var propertyInfoDelegate = new RequestPropertyInfoDelegate(GetProperties);
            var props = propertyInfoDelegate(type);

            var dataObjects = new Dictionary<string, Type>();
            dataObjects.Add("TestSubClass",typeof(TestSubClass));

            
                foreach (var property in props)
                {
                    var xValue = string.Empty;
                    var yValue = string.Empty;

                    var prop = propertyDelegate(typeof(T), property.Name);
                    xValue = prop?.GetValue(x, null)?.ToString();
                    yValue = prop?.GetValue(y, null)?.ToString();

                    // this will check if any properties are class (i.e objects)
                    if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {
                        switch (xValue)
                        {
                            case "ReflectionDelegate.TestSubClass":
                                var xTestSubClassValues = (TestSubClass) prop?.GetValue(x, null);
                                var yTestSubClassValues = (TestSubClass) prop?.GetValue(y, null);
                                //recursive call
                                diffs.AddRange(xTestSubClassValues.CompareUsingDelegate1(yTestSubClassValues));
                                break;

                            case "ReflectionDelegate.AnotherSubClass":
                                var xAnotherSubClassValues = (AnotherSubClass) prop?.GetValue(x, null);
                                var yAnotherSubClassValues = (AnotherSubClass) prop?.GetValue(y, null);
                                //recursive call
                                diffs.AddRange(xAnotherSubClassValues.CompareUsingDelegate1(yAnotherSubClassValues));
                                break;

                            default:
                                diffs.Add("No Configuration defined for sub class");
                                break;
                        
                    }
                    
                }

                    if (ignore == null || !ignore.Contains(property.Name))
                    {
                        if (xValue != yValue)
                        {
                            diffs.Add($"{type.Name} difference - '{property.Name} : {xValue}' != {property.Name} :'{yValue}'");
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

        public static bool IsIList(this Type type)
        {
            return type.GetInterfaces().Contains(typeof(IList));
        }

    }
}
