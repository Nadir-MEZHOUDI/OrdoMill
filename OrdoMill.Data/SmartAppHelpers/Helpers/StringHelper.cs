using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

namespace SmartApp.Helpers.Helpers
{
    public static class StringHelper
    {
        public static DateTime? GetDateFromString(this string jusquea)
        {
            if (IsNullOrEmpty(jusquea))
            {
                return null;
            }

            if (DateTime.TryParse(jusquea, out DateTime date))
            {
                return date;
            }

            return null;
        }

        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        public static void AddRang<T>(this ObservableCollection<T> list, params T[] items)
        {
            if (list == null)
            {
                list = new ObservableCollection<T>();
            }

            foreach (T item in items)
            {
                list.Add(item);
            }
        }

        public static void AddRang<T>(this ObservableCollection<T> list, IEnumerable<T> items)
        {
            if (list == null)
            {
                list = new ObservableCollection<T>();
            }

            foreach (T item in items)
            {
                list.Add(item);
            }
        }

        public static string TrimAll(this string s)
        {
            if (!IsNotNullOrEmpty(s))
            {
                return s;
            }

            string a = s.Trim();
            for (int i = 0; i < s.Length / 2; i++)
            {
                a = a.Replace("  ", " ");
            }

            return a;
        }

        public static string ToValidFileName(this string s)
        {
            string newText = Path.GetInvalidFileNameChars().Aggregate(s, (current, c) => current.Replace(c, ' '));
            return newText;
        }

        public static string ToValidPath(this string s)
        {
            string result = s;
            foreach (char pathChar in Path.GetInvalidPathChars())
            {
                result = result.Replace(pathChar, ' ');
            }

            return result;
        }

        public static bool AllNotNullOrEmpty(params string[] txt) => txt.All(s => !IsNullOrEmpty(s));

        public static bool IsNotNullOrEmpty(this string txt) => !string.IsNullOrEmpty(txt);

        public static bool IsNullOrEmpty(this string txt) => string.IsNullOrEmpty(txt?.Trim());

        public static string UppercaseFirst(this string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }

            char[] a = s.ToLower().ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        public static string UppercaseAllFirsts(this string s, char separator = ' ')
        {
            string[] list = s.Split(separator);
            return list.Aggregate(string.Empty, (current, s1) => current + " " + UppercaseFirst(s1));
        }

        public static bool IsNumeric(this string s)
        {
            return decimal.TryParse(s, out _);
        }

        public static int ToInt(this string txt)
        {
            return IsNotNullOrEmpty(txt) && int.TryParse(txt, out int a) ? a : 0;
        }

        public static DateTime ToDate(this string txt)
        {
            return IsNotNullOrEmpty(txt) && DateTime.TryParse(txt, out DateTime date)
                ? date
                : new DateTime(2015, 1, 1);
        }
    }
}
