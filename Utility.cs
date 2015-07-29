using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;

namespace QRename
{
    static class Utility
    {
        /// <summary>
        /// Add grid row
        /// </summary>
        public static void AddLine(this DataGridView dataGridView, bool extension, bool fullpath, string oldline, string newline)
        {
            dataGridView.Rows.Add(new DataGridViewRow());

            int index = dataGridView.Rows.Count - 1;
            string input = oldline;
            string output = newline;
            DirectoryInfo inputDI = new DirectoryInfo(oldline);
            DirectoryInfo outputDI = new DirectoryInfo(newline);

            if (!fullpath)
            {
                input = inputDI.Name;
                output = outputDI.Name;
            }

            if (!extension && File.Exists(oldline) && inputDI.Extension != "")
            {
                int ext_length = inputDI.Extension.Length;

                int ext_index = input.LastIndexOf(inputDI.Extension);
                input = input.Remove(ext_index, ext_length);

                ext_index = output.LastIndexOf(outputDI.Extension);
                output = output.Remove(ext_index, ext_length);
            }

            dataGridView.Rows[index].Cells[0].Value = input;
            dataGridView.Rows[index].Cells[0].Tag = oldline;

            dataGridView.Rows[index].Cells[1].Value = output;
            dataGridView.Rows[index].Cells[1].Tag = inputDI.Parent.FullName;
        }

        /// <summary>
        /// Toggle extension show/hide
        /// </summary>
        public static void EditExtension(this DataGridView dataGridView, bool extension, bool fullpath, string oldline, string newline, int index)
        {
            string input = (string) dataGridView.Rows[index].Cells[0].Value;
            string output = (string)dataGridView.Rows[index].Cells[1].Value;

            DirectoryInfo fsi = new DirectoryInfo(oldline);

            if (!File.Exists(oldline) || fsi.Extension == "")
                return;

            if (extension)
            {
                input += fsi.Extension;
                output += fsi.Extension;
            }
            else
            {
                input = output.Replace(fsi.Extension, "");
                output = output.Replace(fsi.Extension, "");
            }

            dataGridView.Rows[index].Cells[0].Value = input;
            dataGridView.Rows[index].Cells[1].Value = output;
        }

        /// <summary>
        /// Toggle full path show/hide
        /// </summary>
        public static void EditFullPath(this DataGridView dataGridView, bool extension, bool fullpath, string oldline, string newline, int index)
        {
            string input = (string)dataGridView.Rows[index].Cells[0].Value;
            string output = (string)dataGridView.Rows[index].Cells[1].Value;

            string dir = (string) dataGridView.Rows[index].Cells[1].Tag + "\\";

            if (fullpath)
            {
                input = dir + input;
                output = dir + output;
            }
            else
            {
                input = input.Replace(dir, "");
                output = output.Replace(dir, "");
            }

            dataGridView.Rows[index].Cells[0].Value = input;
            dataGridView.Rows[index].Cells[1].Value = output;
        }

        /// <summary>
        /// Get maximum or -1 if multiple maximums
        /// </summary>
        public static int Max(int[] a)
        {
            int max = 0;
            int index = -1;
            bool equals = false;

            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == max)
                {
                    equals = true;
                }
                else if (a[i] > max)
                {
                    index = i;
                    max = a[i];
                    equals = false;
                }
            }

            if (equals)
                index = -1;

            return index;
        }

        /// <summary>
        /// Check if array contains string
        /// </summary>
        public static bool IsStringInArray(string[] strArray, string key, bool caseSensitive)
        {
            if (caseSensitive)
            {
                for (int i = 0; i <= strArray.Length - 1; i++)
                {
                    if (strArray[i] == key)
                        return true;
                }
            }
            else
            {
                for (int i = 0; i <= strArray.Length - 1; i++)
                {
                    if (string.Equals(strArray[i], key, StringComparison.CurrentCultureIgnoreCase))
                        return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Replace char in string at index
        /// </summary>
        public static string ReplaceChar(string source, int index, char replacement)
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
    }
}
