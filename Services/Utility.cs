using System.Collections.Generic;
using System.Windows;

namespace AutoRename.Services
{
    public static class Utility
    {
        /// <summary>
        /// Current app info
        /// </summary>
#if WINDOWS_UWP
		public static Windows.ApplicationModel.Package CurrentApplication { get; } = Windows.ApplicationModel.Package.Current;
#else
        public static System.Reflection.AssemblyName CurrentApplication { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName();
#endif

        /// <summary>
        /// Get maximum index or "defaultIndex" if multiple maximums
        /// </summary>
        public static T GetMaxIndex<T>(Dictionary<T, int> dictionary, T defaultIndex)
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
            {
                index = defaultIndex;
            }

            return index;
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
            if (str.StartsWith(key))
            {
                if (bool.TryParse(str.Substring(key.Length + 1), out bool val))
                {
                    value = val;
                    return true;
                }
            }

            value = false;
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
            if (str.StartsWith(key))
            {
                string[] vals = str.Substring(key.Length + 1).Split('x');
                if (vals.Length == 2 && double.TryParse(vals[0], out double x) &&
                    double.TryParse(vals[1], out double y))
                {
                    value = new Point(x, y);
                    return true;
                }
            }

            value = new Point();
            return false;
        }
    }
}