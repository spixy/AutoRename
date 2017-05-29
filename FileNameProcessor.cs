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

        public string[] UpperCaseExceptions = { "HD", "HQ", "SD", "DJ" };

		public bool ShowFullPath { get; set; }

		public bool ShowExtension { get; set; }

		public bool StartWithUpperCase { get; set; }

		public bool RemoveStartingNumber { get; set; }

		public bool RemoveBrackets { get; set; }

		public bool ForceOverwrite { get; set; }

        /// <summary>
        /// Apply visual rules to input file
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public string ApplyVisualRules(string file)
        {
	        if (file == null)
	        {
		        return null;
	        }
            if (ShowFullPath && ShowExtension)
            {
	            return file;
            }
            if (ShowFullPath)
            {
                return Path.GetDirectoryName(file) + Path.DirectorySeparatorChar + Path.GetFileNameWithoutExtension(file);
            }
            if (ShowExtension)
            {
                return Path.GetFileName(file);
            }
            // else
            {
               return Path.GetFileNameWithoutExtension(file);
            }
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

		/// <summary>
		/// Get new file name
		/// </summary>
		/// <param name="file">file path</param>
		public string AutoRename(string file)
		{
			return AutoRename(file, StartWithUpperCase, RemoveBrackets, RemoveStartingNumber);
		}

		/// <summary>
		/// Get new file name
		/// </summary>
		/// <param name="file">file path</param>
		/// <param name="startWithUpperCase">custom settings</param>
		/// <param name="removeBrackets">custom settings</param>
		/// <param name="removeStartingNumber">remove 1st number</param>
		/// <returns></returns>
		public string AutoRename(string file, bool startWithUpperCase, bool removeBrackets, bool removeStartingNumber)
		{
			if (file == null)
				return null;

            bool isDirectory = Directory.Exists(file);

            string newStr = Path.GetFileNameWithoutExtension(file);

            char separator = FindSeparator(newStr);

	        if (removeStartingNumber && separator != ' ')
	        {
		        newStr = RemoveFirstNumber(newStr);
	        }

            switch (separator)
            {
                case '-':
                    newStr = ProcessHyphen(newStr);
                    break;

                case '.':
                    newStr = ProcessDot(newStr, isDirectory);
		            break;

                case ' ':
                    break;

                case '%':
                    newStr = newStr.Replace("%20", " ");
                    break;

                default:
                    newStr = ProcessDefault(newStr, separator);
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
			
            FileInfo oldFI = new FileInfo(file);
			FileInfo newFI = new FileInfo(oldFI.DirectoryName + "\\" + newStr + oldFI.Extension);

            return newFI.FullName;
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

	    private string RemoveFirstNumber(string newStr)
	    {
		    int i = 0;
		    int strLength = newStr.Length;

		    for (; i < strLength; i++)
		    {
			    if (char.IsDigit(newStr, i))
					continue;

				// char is not digit
			    if (i == 0 || newStr[i] != '.')
				    break;
		    }

		    return newStr.Remove(0, i);
	    }

        private string ProcessHyphen(string file)
        {
	        bool separatorsOnly = true;
            string newStr = string.Empty;

            for (int j = 0; j < file.Length; j++)
            {
                if (file[j] == '-')
                {
	                if (separatorsOnly)
		                continue;

                    if ((j > 0) && (file[j - 1] != newStr[newStr.Length - 1]))
                        newStr += '-';
                    else
                        newStr += ' ';
                }
                else
                {
                    newStr += file[j];
	                separatorsOnly = false;

                }
            }
            return newStr;
        }

		private string ProcessDot(string file, bool isDirectory)
	    {
		    int dotIndex = file.LastIndexOf('.');
		    string newStr = file.Replace('.', ' ');
		    if (!isDirectory && dotIndex >= 0)
		    {
			    newStr = newStr.ReplaceChar(dotIndex, '.');
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

	        Dictionary<char, int> values = new Dictionary<char, int>
	        {
		        ['_'] = 0,
		        ['.'] = 0,
		        ['-'] = 0,
		        ['%'] = 0
	        };

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
