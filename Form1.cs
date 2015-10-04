using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Media;
using System.Windows.Forms;
using QRename.Properties;

namespace QRename
{
    public partial class Form1 : Form
    {
        private Updater _updater = null;
        private Updater updater
        {
            get
            {
                if (_updater == null)
                {
                    _updater = new Updater(Resources.UpdateFile);
                    _updater.UpdateAvailableAction = UpdateFound;
                }

                return _updater;
            }
        }

        private bool Overwrite
        {
            get { return overwriteExistingFilesToolStripMenuItem.Checked; }
            set { overwriteExistingFilesToolStripMenuItem.Checked = value; }
        }

        private bool StartWithUpperCase
        {
            get { return wordsWithUpperCaseToolStripMenuItem.Checked; }
            set { wordsWithUpperCaseToolStripMenuItem.Checked = value; }
        }

        private bool ShowExtension
        {
            get { return fileExtensionToolStripMenuItem.Checked; }
            set { fileExtensionToolStripMenuItem.Checked = value; }
        }

        private bool ShowFullPath
        {
            get { return fullPathToolStripMenuItem.Checked; }
            set { fullPathToolStripMenuItem.Checked = value; }
        }

        public Form1()
        {
            InitializeComponent();
            dataGridView1.Columns[0].Width = 220;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            LoadSettings();
            LoadArgv();
        }

        private void LoadArgv()
        {
            string[] argv = Environment.GetCommandLineArgs();

            for (int i = 1; i < argv.Length; i++)
            {
                switch (argv[i].ToLower())
                {
                    case "-s":
                        StartWithUpperCase = true;
                        continue;
                    case "-f":
                        Overwrite = true;
                        continue;
                }

                if (Utility.ItemExists(argv[i]))
                {
                    string newLine = FileNameProcessor.QRename(argv[i], StartWithUpperCase);
                    dataGridView1.AddLine(ShowExtension, ShowFullPath, argv[i], newLine);
                }
            }
        }

        private bool Rename(DataGridViewRow row)
        {
            if (!Overwrite && File.Exists((string)row.Cells[1].Value))
                return false;

            string from = (string)row.Cells[0].Tag;

            string to_name = (string)row.Cells[1].Value;
            string to_parent = (string)row.Cells[1].Tag;
            string to;

            if (ShowFullPath && ShowExtension)
            {
                to = to_name;
            }
            else if (ShowFullPath)
            {
                to = to_name + new FileInfo(from).Extension;
            }
            else if (ShowExtension)
            {
                to = to_parent + "\\" + to_name;
            }
            else
            {
                to = to_parent + "\\" + to_name + new FileInfo(from).Extension;
            }

            if (Utility.ItemExists(from))
            {
                try
                {
                    Directory.Move(from, to);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error");
                    return false;
                }
            }
            else
            {
                MessageBox.Show("File " + from + " does not exist", "Error");
                return false;
            }

            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (dataGridView1.RowCount == 0)
                return;

            bool success = true;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                bool result = Rename(dataGridView1.Rows[i]);

                if (!result)
                    dataGridView1.Rows[i].DefaultCellStyle.BackColor = Color.Red;

                success &= result;
            }

            if (success)
            {
                Application.Exit();
            }
            else
            {
                SystemSounds.Beep.Play();
            }
        }

        private void LoadSettings()
        {
            if (!File.Exists(Resources.ConfigFile))
                return;

            try
            {
                string[] lines = File.ReadAllLines(Resources.ConfigFile);

                foreach (string line in lines)
                {
                    if (line.Contains("overwrite "))
                    {
                        bool val;
                        if (bool.TryParse(line.Replace("overwrite ", ""), out val))
                            Overwrite = val;
                    }

                    if (line.Contains("uppercase "))
                    {
                        bool val;
                        if (bool.TryParse(line.Replace("uppercase ", ""), out val))
                            StartWithUpperCase = val;
                    }

                    if (line.Contains("extension "))
                    {
                        bool val;
                        if (bool.TryParse(line.Replace("extension ", ""), out val))
                            ShowExtension = val;
                    }

                    if (line.Contains("full path  "))
                    {
                        bool val;
                        if (bool.TryParse(line.Replace("full path  ", ""), out val))
                            ShowFullPath = val;
                    }

                    if (line.Contains("window ") && line.Length > 9)
                    {
                        string res = line.Substring(7);
                        Width = Convert.ToInt32(res.Substring(0, res.IndexOf('x')));
                        Height = Convert.ToInt32(res.Substring(res.IndexOf('x') + 1));
                    }

                    if (line.Contains("delim ") && line.Length > 6)
                    {
                        int val;
                        if (int.TryParse(line.Substring(6), out val))
                            dataGridView1.Columns[0].Width = val;
                    }
                }
            }
            catch
            {
                // ignored
            }

            /*
            if (File.Exists(config2))
            {
                str = File.ReadAllLines(config2);
                if (Utilities.IsStringInArray(str, "*", true))
                {
                    str = new string[1];
                    str[0] = ".*";
                }
            }
            else str = new string[0];

            settings = new Info[str.Length];

            for (int i = 0; i < str.Length; i++)
            {
                settings[i].Ext = str[i];

                try
                {
                    string path = settings[i].Ext;
                    RegistryKey rk = Registry.ClassesRoot.OpenSubKey(path, false);

                    if (rk == null)
                    {
                        settings[i].Enabled = false;
                        continue;
                    }

                    if (rk.GetValue("") != null) path = rk.GetValue("").ToString();
                    rk = Registry.ClassesRoot.OpenSubKey(path + "\\shell\\Quick rename\\command", false);

                    if (rk == null)
                    {
                        settings[i].Enabled = false;
                    }
                    else
                    {
                        settings[i].Enabled = (rk.ValueCount > 0);
                    }
                }
                catch
                {
                    // ignored
                }
            }*/
        }

        private void SaveSettings()
        {
            try
            {
                using (StreamWriter sw = new StreamWriter(Resources.ConfigFile, false))
                {
                    sw.WriteLine("overwrite " + Overwrite);
                    sw.WriteLine("uppercase " + StartWithUpperCase);
                    sw.WriteLine("extension " + ShowExtension);
                    sw.WriteLine("full path " + ShowFullPath);            
                    sw.WriteLine("window " + Width + "x" + Height);
                    sw.WriteLine("delim " + dataGridView1.Columns[0].Width);
                }
            }
            catch
            {
                // ignored
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveSettings();
        }

        private void Form1_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[]) e.Data.GetData(DataFormats.FileDrop);

            foreach (string file in files)
            {
                string newLine = FileNameProcessor.QRename(file, StartWithUpperCase);
                dataGridView1.AddLine(ShowExtension, ShowFullPath, file, newLine);
            }
        }

        private void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false))
                e.Effect = DragDropEffects.All;
        }

        private void clearSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = dataGridView1.SelectedRows.Count - 1; i >= 0; i--)
                dataGridView1.Rows.Remove(dataGridView1.SelectedRows[i]);
        }

        private void clearAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Rows.Clear();
        }

        private void dataGridView1_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            label1.Hide();
        }

        private void dataGridView1_RowsRemoved(object sender, DataGridViewRowsRemovedEventArgs e)
        {
            if (dataGridView1.Rows.Count == 0)
                label1.Show();
        }

        private void wordsWithUpperCaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.Enabled = false;

            for (int i = 0; i < dataGridView1.RowCount; i++)
            {
                string output = (string)dataGridView1.Rows[i].Cells[1].Value;

                if (StartWithUpperCase)
                    dataGridView1.Rows[i].Cells[1].Value = FileNameProcessor.ChangeToUpperCase(output);
            }

            dataGridView1.Enabled = true;
        }

        private void fileExtensionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.EditExtension(ShowExtension);
        }

        private void fullPathToolStripMenuItem_Click(object sender, EventArgs e)
        {
            dataGridView1.EditFullPath(ShowFullPath);
        }

        private void checkForUpdateToolStripMenuItem_Click(object sender, EventArgs e)
        {
            updater.IsUpdateAvailableAsync();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Process.Start(Resources.HomePage);
        }

        private void UpdateFound()
        {
            if (MessageBox.Show("Update Available. Download new version?", "Update Available", MessageBoxButtons.YesNo) == DialogResult.Yes)
                Process.Start(Resources.HomePage);
        }
  
    }
}
