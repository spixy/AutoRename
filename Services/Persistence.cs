using System.IO;
using System.Windows;

namespace AutoRename
{
    public class Persistence
    {
        /// <summary>
        /// Save file path
        /// </summary>
        public string FilePath { get; }

        public Persistence(string filePath)
        {
            FilePath = filePath;
        }

        /// <summary>
        /// Load app settings
        /// </summary>
        public void LoadSettings(FileNameProcessor fileNameProcessor, MainViewModel model, Window window)
        {
            string[] lines = File.ReadAllLines(FilePath);
            bool boolValue;
            Point vec2Value;

            foreach (string line in lines)
            {
                string lineInLower = line.ToLowerInvariant();

                if (Utility.TryGetBoolValue(lineInLower, "overwrite", out boolValue))
                {
                    fileNameProcessor.ForceOverwrite = boolValue;
                }
                else if (Utility.TryGetBoolValue(lineInLower, "uppercase", out boolValue))
                {
                    model.StartWithUpperCase = boolValue;
                }
                else if (Utility.TryGetBoolValue(lineInLower, "remove brackets", out boolValue))
                {
                    model.RemoveBrackets = boolValue;
                }
                else if (Utility.TryGetBoolValue(lineInLower, "remove starting number", out boolValue))
                {
                    model.RemoveStartingNumber = boolValue;
                }
                else if (Utility.TryGetBoolValue(lineInLower, "extension", out boolValue))
                {
                    model.ShowExtension = boolValue;
                }
                else if (Utility.TryGetBoolValue(lineInLower, "full path", out boolValue))
                {
                    model.ShowFullPath = boolValue;
                }
                else if (Utility.TryGetBoolValue(lineInLower, "grid lines", out boolValue))
                {
                    model.ShowGridLines = boolValue;
                }
                else if (Utility.TryGetBoolValue(lineInLower, "Exit after rename", out boolValue))
                {
                    model.ExitAfterRename = boolValue;
                }
                else if (Utility.TryGetVec2Value(lineInLower, "position", out vec2Value))
                {
                    Rect screen = SystemParameters.WorkArea;
                    window.Left = Utility.Clamp(vec2Value.X, screen.Left, screen.Right);
                    window.Top = Utility.Clamp(vec2Value.Y, screen.Top, screen.Bottom);
                }
                else if (Utility.TryGetVec2Value(lineInLower, "window", out vec2Value))
                {
                    window.Width = vec2Value.X;
                    window.Height = vec2Value.Y;
                }
            }
        }

        /// <summary>
        /// Save app settings
        /// </summary>
        public void SaveSettings(FileNameProcessor fileNameProcessor, MainViewModel model, Window window)
        {
            using (StreamWriter sw = new StreamWriter(FilePath, false))
            {
                sw.WriteLine("Overwrite " + fileNameProcessor.ForceOverwrite);
                sw.WriteLine("Uppercase " + fileNameProcessor.StartWithUpperCase);
                sw.WriteLine("Remove brackets " + fileNameProcessor.RemoveBrackets);
                sw.WriteLine("Remove starting number " + fileNameProcessor.RemoveStartingNumber);
                sw.WriteLine("Extension " + model.ShowExtension);
                sw.WriteLine("Full path " + model.ShowFullPath);
                sw.WriteLine("Grid lines " + model.ShowGridLines);
                sw.WriteLine("Exit after rename " + model.ExitAfterRename);
                sw.WriteLine("Position " + window.Left + "x" + window.Top);
                sw.WriteLine("Window " + window.Width + "x" + window.Height);
            }
        }

    }
}
