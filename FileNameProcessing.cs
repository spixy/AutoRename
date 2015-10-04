using System.IO;

namespace QRename
{
    static class FileNameProcessor
    {
        /// <summary>
        /// Make each word to start with upper case
        /// </summary>
        public static string ChangeToUpperCase(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            string[] words = text.Split(' ');

            string result = string.Empty;

            foreach (var word in words)
            {
                result += char.ToUpperInvariant(word[0]) + word.Substring(1) + ' ';
            }

            result = result.Substring(0, result.Length - 1);

            return result;
        }

        /// <summary>
        /// Get new file name
        /// </summary>
        public static string QRename(string file, bool uppercase)
        {
            bool isDirectory = Directory.Exists(file);
            string newstr = string.Empty;

            int slashpos = file.LastIndexOf('\\') + 1;
            if (slashpos == 0) slashpos = file.LastIndexOf('/') + 1;

            string directory = file.Substring(0, slashpos);
            file = file.Substring(slashpos);
            char sep = FindSeparator(file, true);

            switch (sep)
            {
                case '-':
                    for (int j = 0; j < file.Length; j++)
                    {
                        if (file[j] == sep)
                        {
                            if ((j > 0) && (file[j - 1] != newstr[newstr.Length - 1]))
                                newstr += '-';
                            else newstr += ' ';
                        }
                        else
                        {
                            newstr += file[j];
                        }
                    }
                    break;

                case '.':
                    int bodkaIndex = file.LastIndexOf('.');
                    newstr = file.Replace(sep, ' ');
                    if (!isDirectory)
                        newstr = Utility.ReplaceChar(newstr, bodkaIndex, '.');
                    break;

                case 'X':
                    sep = FindSeparator(file, false);

                    newstr = file.Replace(" " + sep + " ", "/X/");
                    newstr = newstr.Replace(" " + sep, "/X");
                    newstr = newstr.Replace(sep + " ", "X/");

                    newstr = newstr.Replace(sep, ' ');

                    newstr = newstr.Replace("/X/", " " + sep + " ");
                    newstr = newstr.Replace("/X", " " + sep);
                    newstr = newstr.Replace("X/", sep + " ");
                    break;

                case ' ':
                    newstr = file;
                    break;

                case '%':
                    newstr = file.Replace("%20", " ");
                    break;

                default:
                    newstr = file.Replace(sep, ' ');
                    break;
            }

            if (uppercase)
            {
                newstr = ChangeToUpperCase(newstr);
            }

            newstr = directory + newstr;

            return PostProcess(newstr);
        }

        private static char FindSeparator(string str, bool primary)
        {
            int podtrznik = 0;
            int pomlcka = 0;
            int bodka = -1;
            int perc20 = 0;

            for (int i = 0; i < str.Length; i++)
                switch (str[i])
                {
                    case '_':
                        podtrznik++;
                        break;
                    case '.':
                        bodka++;
                        break;
                    case '-':
                        pomlcka++;
                        break;
                    case ' ':
                        if (primary) return ' ';
                        break;
                    case '0':
                        if ((i > 1) && (str[i - 1] == '2') && (str[i - 2] == '%'))
                            perc20++;
                        break;
                }

            int[] pocet = { podtrznik, pomlcka, bodka, perc20 };
            char[] separatory = { ' ', '_', '-', '.', '%' };

            int index = Utility.Max(pocet) + 1;

            return separatory[index];
        }

        private static string PostProcess(string file)
        {
            string newFile = string.Empty;

            bool lastSpace = false;

            foreach (char s in file)
            {
                if (s == ' ')
                {
                    if (lastSpace) // dalsie
                        continue;

                    lastSpace = true; // prva
                }
                else
                {
                    lastSpace = false; // ziadna
                }

                newFile += s;
            }

            return newFile;
        }
    }
}
