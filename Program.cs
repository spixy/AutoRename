using System;
using System.Windows.Forms;

namespace QRename
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //if (Environment.GetCommandLineArgs().Length > 1)
                Application.Run(new Form1());
        }
    }
}
