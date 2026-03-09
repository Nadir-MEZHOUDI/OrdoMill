using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Windows;

namespace OrdoMill.Services
{
    public static class Helper
    {

        public static List<TSource> NewSelect<TSource, TResult>(this IQueryable<TSource> source, Expression<Func<TSource, TResult>> selector) where TSource : new()
        {
            return source.Select(selector).ToList().ToListOf(new TSource());
        }
        public static TSource NewSelect<TSource>(this TSource source) where TSource : new()
        {
            return source.ToTypeOf<TSource>();
        }



        public static T ToTypeOf<T>(this object obj, T type =default(T))
        {
            //create instance of T type object:
            var tmp = Activator.CreateInstance(typeof(T));

            //loop through the properties of the object you want to covert:          
            foreach (PropertyInfo pi in obj.GetType().GetProperties())
            {
                try
                {
                    //get the value of property and try to assign it to the property of T type object:
                    tmp.GetType().GetProperty(pi.Name).SetValue(tmp, pi.GetValue(obj, null), null);
                }
                catch (Exception)
                {

                    //tmp.GetType().GetProperty(pi.Name).SetValue(tmp, pi.GetValue(obj, null).ToTypeOf(pi), null);

                }
            }
            //return the T type object:         
            return (T)tmp;
        }
        public static List<TDestination> ToListOf<TDestination, TSource>(this List<TSource> list, TDestination type)
        {
            //define system Type representing List of objects of T type:
            var genericType = typeof(List<>).MakeGenericType(typeof(TDestination));

            //create an object instance of defined type:
            var l = Activator.CreateInstance(genericType) as List<TDestination>;
            //var l = new ;
            //get method Add from from the list:
            MethodInfo addMethod = l.GetType().GetMethod("Add");

            //loop through the calling list:
            foreach (var item in list)
            {
                //convert each object of the list into T object 
                //by calling extension ToType<T>()
                //Add this object to newly created list:
                l?.Add(item.ToTypeOf<TDestination>());
                //addMethod.Invoke(l, item.ToTypeOf<T>());
            }

            //return List of T objects:
            return l;
        }

        public static object ToListOf<T>(this List<dynamic> list)
        {

            //define system Type representing List of objects of T type:
            var genericType = typeof(List<>).MakeGenericType(typeof(T));

            //create an object instance of defined type:
            // var l = Activator.CreateInstance(genericType);

            var l = new List<T>();

            //get method Add from from the list:
            MethodInfo addMethod = l.GetType().GetMethod("Add");

            //loop through the calling list:
            foreach (var item in list)
            {
                //convert each object of the list into T object 
                //by calling extension ToType<T>()
                //Add this object to newly created list:
                l.Add(item.ToTypeOf<T>());
                //addMethod.Invoke(l, new object { item.ToTypeOf<T>() });
            }

            //return List of T objects:
            return l;
        }

        //public static IEnumerable<T> ToListOf<T>(this IEnumerable<T> list)
        //{

        //    //define system Type representing List of objects of T type:
        //    var genericType = typeof(List<>).MakeGenericType(typeof(T));

        //    //create an object instance of defined type:
        //    List<T> l = new List<T>();

        //    //get method Add from from the list:
        //    MethodInfo addMethod = l.GetType().GetMethod("Add");

        //    //loop through the calling list:
        //    foreach (T item in list)
        //    {
        //        //convert each object of the list into T object 
        //        //by calling extension ToType<T>()
        //        //Add this object to newly created list:
        //        // addMethod.Invoke(l, new List<T> { item.ToTypeOf<T>() });
        //        l.Add(item);
        //    }

        //    //return List of T objects:
        //    return l;
        //}


        public static string GetAssemblyVersion()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(assembly.Location);
            string version = fvi.FileVersion;
            return $"Version:{version}";
        }

        public static string GetString(this string text)
        {
            try
            {
                return (string)Application.Current?.Resources[text] ?? text;
            }
            catch (Exception)
            {
                return text;
            }
        }

        public static List<string> GetArray(this string text)
        {
            try
            {
                return Application.Current.Resources.Contains(text)
                    ? new List<string>((string[])Application.Current.Resources[text])
                    : new List<string>();
            }
            catch (Exception)
            {
                return new List<string>();
            }
        }
    }
}
