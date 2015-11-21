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
        public static void AddLine(this DataGridView dataGridView, bool extension, bool fullPath, string oldLine, string newLine)
        {
            dataGridView.Rows.Add(new DataGridViewRow());

            int index = dataGridView.Rows.Count - 1;
            string input = oldLine;
            string output = newLine;
            DirectoryInfo inputDI = new DirectoryInfo(oldLine);
            DirectoryInfo outputDI = new DirectoryInfo(newLine);

            if (!fullPath)
            {
                input = inputDI.Name;
                output = outputDI.Name;
            }

            if (!extension && File.Exists(oldLine) && inputDI.Extension != "")
            {
                int ext_length = inputDI.Extension.Length;

                int input_ext_index = input.LastIndexOf(inputDI.Extension);
                input = input.Remove(input_ext_index, ext_length);

                int output_ext_index = output.LastIndexOf(outputDI.Extension);
                output = output.Remove(output_ext_index, ext_length);
            }

            dataGridView.Rows[index].Cells[0].Value = input;
            dataGridView.Rows[index].Cells[0].Tag = oldLine;

            dataGridView.Rows[index].Cells[1].Value = output;
            dataGridView.Rows[index].Cells[1].Tag = inputDI.Parent.FullName;
        }

        /// <summary>
        /// Toggle extension show/hide
        /// </summary>
        public static void EditExtension(this DataGridView dataGridView, bool extension)
        {
            dataGridView.Enabled = false;

            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                dataGridView.EditExtension(extension, i);
            }

            dataGridView.Enabled = true;
        }

        /// <summary>
        /// Toggle extension show/hide
        /// </summary>
        private static void EditExtension(this DataGridView dataGridView, bool extension, int index)
        {
            string oldFileName = (string)dataGridView.Rows[index].Cells[0].Tag;

            FileInfo fsi = new FileInfo(oldFileName);

            if (!fsi.Exists || string.IsNullOrEmpty(fsi.Extension))
                return;

            string input  = (string)dataGridView.Rows[index].Cells[0].Value;
            string output = (string)dataGridView.Rows[index].Cells[1].Value;

            string fileExtension = fsi.Extension;

            if (extension)
            {
                input += fileExtension;
                output += fileExtension;
            }
            else
            {
                input = input.Replace(fileExtension, "");
                output = output.Replace(fileExtension, "");
            }

            dataGridView.Rows[index].Cells[0].Value = input;
            dataGridView.Rows[index].Cells[1].Value = output;
        }

        /// <summary>
        /// Toggle extension show/hide
        /// </summary>
        public static void EditFullPath(this DataGridView dataGridView, bool fullPath)
        {
            dataGridView.Enabled = false;

            for (int i = 0; i < dataGridView.RowCount; i++)
            {
                dataGridView.EditFullPath(fullPath, i);
            }

            dataGridView.Enabled = true;
        }

        /// <summary>
        /// Toggle full path show/hide
        /// </summary>
        private static void EditFullPath(this DataGridView dataGridView, bool fullPath, int index)
        {
            string directory = (string)dataGridView.Rows[index].Cells[1].Tag + "\\";

            string input =  (string)dataGridView.Rows[index].Cells[0].Value;
            string output = (string)dataGridView.Rows[index].Cells[1].Value;

            if (fullPath)
            {
                input = directory + input;
                output = directory + output;
            }
            else
            {
                input = input.Replace(directory, "");
                output = output.Replace(directory, "");
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
    }
}
