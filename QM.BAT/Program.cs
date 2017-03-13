using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace QM.BAT
{
    static class Program
    {
        [STAThread]
        static void Main(string[] parms)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1(parms));
        }
    }
}
