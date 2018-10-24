using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace AcquireSystem
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ApplicationExit += Application_ApplicationExit;

            try
            {
                var setting = new CefSettings();
                setting.Locale = "zh-CN";
                setting.CefCommandLineArgs.Add("renderer-process-limit", "5");
                Cef.Initialize(setting);
            }
            catch (Exception e)
            {
                MessageBox.Show($"初始化cef时出现错误!{e.Message}");
            }


            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        private static void Application_ApplicationExit(object sender, EventArgs e)
        {
            try
            {
                Cef.Shutdown();
            }
            catch (Exception exception)
            {
                MessageBox.Show($"关闭cef时出现错误!{exception.Message}");
            }
        }
    }
}
