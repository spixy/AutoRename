using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;

namespace AutoRename
{
	public class FileNameProcessor : Singleton<FileNameProcessor>
    {
		private readonly Tuple<string, string>[] BracketsList =
	    {
		    new Tuple<string, string>("(", ")"),
		    new Tuple<string, string>("[", "]"),
		    new Tuple<string, string>("{", "}")
	    };

        public string[] UpperCaseExceptions = { "HD", "HQ", "SD" };

		public bool ShowFullPath { get; set; }

		public bool ShowExtension { get; set; }

		public bool StartWithUpperCase { get; set; }

		public bool RemoveBrackets { get; set; }

		public bool ForceOverwrite { get; set; }

        /// <summary>
        /// Make each word to start with upper case
        /// </summary>
        private string ChangeToUpperCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

	        char[] chars = text.ToCharArray();

			StringBuilder sb = new StringBuilder(chars.Length + 1);

			sb.Append(char.ToUpper(chars[0]));

	        for (int i = 1; i < chars.Length; i++)
	        {
		        if (chars[i-1] == ' ')
			        sb.Append(char.ToUpper(chars[i]));
		        else
					sb.Append(chars[i]);
	        }

	        return sb.ToString();
        }

        /// <summary>
        /// Apply visual rules to input file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string ApplyVisualRules(string file)
        {
            if (ShowFullPath && ShowExtension)
            {
	            return file;
            }
            else if (ShowFullPath)
            {
                return Path.GetDirectoryName(file) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file);
            }
            else if (ShowExtension)
            {
                return Path.GetFileName(file);
            }
            else
            {
               return Path.GetFileNameWithoutExtension(file);
            }
        }

		/// <summary>
		/// Get new file name
		/// </summary>
		/// <param name="file">file path</param>
		public string QRename(string file)
		{
			return QRename(file, StartWithUpperCase, RemoveBrackets);
		}

		/// <summary>
		/// Get new file name
		/// </summary>
		/// <param name="file">file path</param>
		/// <param name="startWithUpperCase">custom settings</param>
		/// <param name="removeBrackets">custom settings</param>
		/// <returns></returns>
		public string QRename(string file, bool startWithUpperCase, bool removeBrackets)
        {
            string newStr;
            bool isDirectory = Directory.Exists(file);

            FileInfo fi = new FileInfo(file);
            string directory = fi.DirectoryName;

            file = Path.GetFileNameWithoutExtension(file);

            char separator = FindSeparator(file);

            switch (separator)
            {
                case '-':
                    newStr = ProcessHyphen(file);
                    break;

                case '.':
                    int bodkaIndex = file.LastIndexOf('.');
                    newStr = file.Replace('.', ' ');
                    if (!isDirectory && bodkaIndex >= 0)
                    {
                        newStr = newStr.ReplaceChar(bodkaIndex, '.');
                    }
                    break;

                case ' ':
                    newStr = file;
                    break;

                case '%':
                    newStr = file.Replace("%20", " ");
                    break;

                default:
                    newStr = ProcessDefault(file, separator);
                    break;
            }

			if (removeBrackets)
	        {
		        //TODO: implement via automaton maybe
		        foreach (var brackets in BracketsList)
					newStr = RemoveStringInBrackets(newStr, brackets);
	        }

			if (startWithUpperCase)
            {
                newStr = ChangeToUpperCase(newStr);
            }

			newStr = RemoveMultipleSpaces(newStr).Trim();

			FileInfo fileInfo = new FileInfo(directory + "\\" + newStr + fi.Extension);

            return fileInfo.FullName;
        }

	    private string RemoveStringInBrackets(string newStr, Tuple<string, string> brackets)
	    {
		    int start = 0;

		    do
		    {
				int startIndex = newStr.IndexOf(brackets.Item1, start, StringComparison.Ordinal);

			    if (startIndex == -1)
				    break;

				int endIndex = newStr.IndexOf(brackets.Item2, startIndex, StringComparison.Ordinal);

			    if (endIndex == -1)
				    break;

				newStr = newStr.Remove(startIndex, endIndex - startIndex + 1);
			    start = startIndex;

		    } while (true);

		    return newStr;
	    }

	    /// <summary>
		/// Try to rename file
		/// </summary>
		/// <returns></returns>
		public bool Rename(string from, string to)
		{
			if (from == to)
				return true;

			if (!ForceOverwrite && File.Exists(to))
			{
				if (MessageBox.Show("Do you want to overwrite " + to + "?", "File already exists", MessageBoxButton.YesNo) == MessageBoxResult.No)
					return false;
			}

			if (Utility.ItemExists(from))
			{
				try
				{
					Directory.Move(from, to);
					return true;
				}
				catch
				{
					// ignored
				}
			}	
			
			return false;
		}

        private string ProcessHyphen(string file)
        {
            string newStr = string.Empty;

            for (int j = 0; j < file.Length; j++)
            {
                if (file[j] == '-')
                {
                    if ((j > 0) && (file[j - 1] != newStr[newStr.Length - 1]))
                        newStr += '-';
                    else
                        newStr += ' ';
                }
                else
                {
                    newStr += file[j];
                }
            }
            return newStr;
        }

        private string ProcessDefault(string file, char separator)
        {
            string newStr = file.Replace(separator, ' ');

            if (newStr.Contains("-"))
            {
                for (int j = 1; j < newStr.Length - 1; j++)
                {
                    if ((newStr[j] == '-') && (newStr[j - 1] != ' ') && (newStr[j + 1] != ' '))
                    {
                        newStr = newStr.Insert(j, " ").Insert(j + 2, " ");
                        j++;
                    }
                }
            }
            return newStr;
        }

        private char FindSeparator(string str)
        {
            if (str.Contains(" "))
                return ' ';

	        Dictionary<char, int> values = new Dictionary<char, int>();

	        values['_'] = 0;
	        values['.'] = 0;
	        values['-'] = 0;
	        values['%'] = 0;
	        
	        for (int i = 0; i < str.Length; ++i)
            {
	            char c = str[i];

				switch (c)
                {
					case '_':
					case '.':
					case '-':
		                values[c]++;
		                break;

					case '0': // %20
                        if ((i >= 2) && (str[i - 1] == '2') && (str[i - 2] == '%'))
                           values['%']++;
                        break;
                }
			}

			return Utility.GetMaxIndex(values, ' ');
        }

        private string RemoveMultipleSpaces(string file)
        {
            string newFile = string.Empty;

            bool lastSpace = false;

            foreach (char s in file)
            {
                if (s == ' ')
                {
                    if (lastSpace) // next
                        continue;

                    lastSpace = true; // first
                }
                else
                {
                    lastSpace = false; // none
                }

                newFile += s;
            }

            return newFile;
        }
    }
}
