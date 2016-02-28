using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AutoRename
{
    public static class Utility
    {
		/// <summary>
		/// Current Assembly
		/// </summary>
		public static readonly AssemblyName CurrentApplication = Assembly.GetExecutingAssembly().GetName();

		/// <summary>
		/// Get maximum index or "defaultIndex" if multiple maximums
		/// </summary>
		public static T GetMaxIndex<T>(Dictionary<T,int> dictionary, T defaultIndex)
        {
            int max = 0;
			T index = default(T);
            bool equals = false;

            foreach (var kvp in dictionary)
            {
                if (kvp.Value == max)
                {
                    equals = true;
                }
				else if (kvp.Value > max)
                {
                    index = kvp.Key;
					max = kvp.Value;
                    equals = false;
                }
            }

	        if (equals)
		        index = defaultIndex;

            return index;
        }

        /// <summary>
        /// Check if array contains string
        /// </summary>
        public static bool IsStringInArray(this string[] strArray, string key, bool caseSensitive)
        {
            StringComparison comparison = (caseSensitive) ? StringComparison.CurrentCulture : StringComparison.CurrentCultureIgnoreCase;

            foreach (string str in strArray)
            {
                if (string.Equals(str, key, comparison))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Replace char in string at index
        /// </summary>
        public static string ReplaceChar(this string source, int index, char replacement)
        {
            char[] temp = source.ToCharArray();
            temp[index] = replacement;
            return new string(temp);
        }

        /// <summary>
        /// Remove string from array
        /// </summary>
        public static string[] RemoveString(string[] strArray, string str)
        {
            List<string> list = new List<string>();

            foreach (string s in strArray)
            {
                if (s != str)
                    list.Add(s);
            }

            return list.ToArray();
        }

        /// <summary>
        /// Determines whether the specified file or directory exists
        /// </summary>
        /// <param name="path">The path to check</param>
        public static bool ItemExists(string path)
        {
            return File.Exists(path) || Directory.Exists(path);
        }

		/// <summary>
		/// Limits value to interval
		/// </summary>
		/// <param name="value">input value</param>
		/// <param name="min">min value</param>
		/// <param name="max">max value</param>
		/// <returns></returns>
		public static double Clamp(double value, double min, double max)
		{
			return (value < min) ? min : (value > max) ? max : value;
		}
    }
}
