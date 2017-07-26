using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Windows;

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

		/// <summary>
		/// Get bool value
		/// </summary>
		/// <param name="str">string to parse in format "%key% %value%"</param>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		/// <returns>If key-value pair exists and is valid</returns>
		public static bool TryGetBoolValue(string str, string key, out bool value)
	    {
		    if (str.IndexOf(key) == 0)
		    {
			    bool val;
			    if (bool.TryParse(str.Substring(key.Length + 1), out val))
			    {
				    value = val;
				    return true;
			    }
		    }

		    value = false;
		    return false;
	    }

		/// <summary>
		/// Get string value
		/// </summary>
		/// <param name="str">string to parse in format "%key% %value%"</param>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		/// <returns>If key-value pair exists and is valid</returns>
		public static bool TryGetStringValue(string str, string key, out string value)
	    {
		    if (str.IndexOf(key) == 0)
		    {
			    value = str.Substring(key.Length + 1);
			    return true;
		    }

		    value = null;
		    return false;
	    }

		/// <summary>
		/// Get Vector2 value
		/// </summary>
		/// <param name="str">string to parse in format "%key% %value%"</param>
		/// <param name="key">key</param>
		/// <param name="value">value</param>
		/// <returns>If key-value pair exists and is valid</returns>
		public static bool TryGetVec2Value(string str, string key, out Point value)
	    {
		    if (str.IndexOf(key) == 0)
		    {
			    string[] vals = str.Substring(key.Length + 1).Split('x');
			    if (vals.Length == 2)
			    {
				    double x, y;

				    if (double.TryParse(vals[0], out x) && double.TryParse(vals[1], out y))
				    {
					    value = new Point(x, y);
					    return true;
				    }
			    }
		    }

		    value = new Point();
		    return false;
	    }
	}
}
