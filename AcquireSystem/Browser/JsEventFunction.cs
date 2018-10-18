using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcquireSystem.Forms;
using CefSharp;
using CefSharp.WinForms;

namespace AcquireSystem.Browser
{
    public class JsEventFunction
    {
        //private BaseForm MainForm;
        private ChromiumWebBrowser _browser;
        public JsEventFunction( ChromiumWebBrowser browser)
        {
            //MainForm = form;
            _browser = browser;
        }

        public void CloseForm()
        {
            Application.ExitThread();
            Application.Exit();
            Process.GetCurrentProcess().Kill();
            System.Environment.Exit(0);
        }

        ///// <summary>
        ///// 是否支持jQuery
        ///// </summary>
        ///// <param name="isSupportjQuery"></param>
        //public void callbackjQueryChecker(bool isSupportjQuery)
        //{
        //    MainForm.OnCheckjQueryCallBack(isSupportjQuery);
        //}
        
        public void HelloWorld(string msg)
        {
            MessageBox.Show(msg);
        }


    }
}
