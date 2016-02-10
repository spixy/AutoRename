using System;
using System.IO;
using System.Windows;

namespace AutoRename
{
    public class FileNameProcessor
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

		public bool ForceOverwrite { get; set; }

		public bool RemoveBrackets { get; set; }

        /// <summary>
        /// Make each word to start with upper case
        /// </summary>
        private string ChangeToUpperCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string[] words = text.Split(' ');

            string result = string.Empty;

            foreach (var word in words)
            {
                if (word.Length > 0)
                {
                    if (UpperCaseExceptions.IsStringInArray(word, true))
                        result += word + ' ';
                    else
                        result += char.ToUpperInvariant(word[0]) + word.Substring(1).ToLowerInvariant() + ' ';
                }
            }

            return result.Substring(0, result.Length - 1);
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
        public string QRename(string file)
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

	        if (RemoveBrackets)
	        {
		        //TODO: implement via automaton
		        foreach (var brackets in BracketsList)
				{
					newStr = RemoveStringInBrackets(newStr, brackets);
		        }
	        }

	        if (StartWithUpperCase)
            {
                newStr = ChangeToUpperCase(newStr);
            }

            newStr = RemoveMultipleSpaces(newStr);

            return new FileInfo(directory + "\\" + newStr + fi.Extension).FullName;
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
            int hyphens = 0;
            int dots = 0;
            int perc20s = 0;

            if (str.Contains(" "))
                return ' ';

            for (int i = 0; i < str.Length; i++)
                switch (str[i])
                {
                    case '_':
                        return '_';

                    case '.':
                        dots++;
                        break;

                    case '-':
                        hyphens++;
                        break;

                    case '0': // "%20"
                        if ((i > 1) && (str[i - 1] == '2') && (str[i - 2] == '%'))
                            perc20s++;
                        break;
                }

            int[] array = { hyphens, dots, perc20s };
            char[] separators = { ' ', '-', '.', '%' };

            int index = Utility.Max(array) + 1;

            return separators[index];
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
