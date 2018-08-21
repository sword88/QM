using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using log4net;

namespace QM.Demo.Meal
{
    static class Program
    {
        private static ILog log = LogManager.GetLogger(typeof(Program));
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            log.Debug("start");
            if (args.Length > 0)
            {
                com.asewh.web.Services s = new com.asewh.web.Services();
                s.HelloWorld();                
                log.Debug("exit");
                Environment.Exit(0);
            }
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
