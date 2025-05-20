using PlastMB.Common;
using NLog;
using System;
using System.Windows.Forms;

namespace PlastMB
{
    internal static class Program
    {
        // create a static logger field
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            logger.Info("========================================================================================================");
            logger.Info($"Version: {Constant.VERSION_CODE} - Release Date: {Constant.VERSION_DATE}");

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MenuForm());
        }
    }
}
