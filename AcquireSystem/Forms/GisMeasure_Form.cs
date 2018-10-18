using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AcquireSystem.Browser;
using AcquireSystem.Sqlite;
using CefSharp;
using CefSharp.WinForms;
using Common.Forms;

namespace AcquireSystem.Forms
{
    public partial class GisMeasure_Form : NormalForm
    {
        public ChromiumWebBrowser browser;
        public GisMeasure_Form()
        {
            InitializeComponent();
            InitBrowser();
        }


        #region browser

        void InitBrowser()
        {
            var setting = new CefSettings();
            setting.Locale = "zh-CN";
            setting.CefCommandLineArgs.Add("renderer-process-limit", "5");
            Cef.Initialize(setting);

            browser = new ChromiumWebBrowser(Application.StartupPath + @"\Browser\test.html");

            //browser.FrameLoadStart += browser_FrameLoadStart;
            //browser.FrameLoadEnd += browser_FrameLoadEnd;
            //browser.LifeSpanHandler = new CustomLifeSpanHandler();
            //browser.RequestHandler = new CustomRequestHandler();

            //CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            //browser.RegisterJsObject("WCShell", new JsEventFunction(this, browser));
            browser.JavascriptObjectRepository.Register("WCShell", new JsEventFunction(browser));
            //browser.MenuHandler = new MenuHandler();

            this.panel3.Controls.Add(browser);
            browser.Dock = DockStyle.Fill;

        }
        //public  void CallJsFunction(string jsFunction)
        //{
        //    browser.ExecuteScriptAsync(jsFunction);
        //    //browser.ExecuteScriptAsync(" console.log('" + jsFunction + "') ");

        //}

        #endregion





        private void button1_Click(object sender, EventArgs e)
        {
            string url = textBox1.Text.Trim();
            browser.Load(url);
            browser.ResetBindings();
        }

        /// <summary>
        /// 调用开发调试工具
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            browser.ShowDevTools();
        }

        /// <summary>
        /// 调用js方法
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            browser.ExecuteScriptAsync("alert(document.getElementById('txt').value);");
        }

        //private void button4_Click(object sender, EventArgs e)
        //{
        //    string html = "<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta charset=\"utf-8\" /><title></title>" +
        //                  "<script>"+
        //        "  CefSharp.BindObjectAsync(\"WCShell\", \"WCShell\"); " +
        //                  "function test() { WCShell.helloWorld(document.getElementById('txt').value);}</script></head>"+
        //        "<body><H1> Hello World </H1 ><input type='text' value='hi zed' name='txt' id='txt' /> " +

        //                  "<input type = \"button\" onclick = \"test();\" value = \"测试\" /></body></html> ";


        //    browser.LoadHtml(html);
        //    browser.ResetBindings();
        //}


        /// <summary>
        /// 窗体关闭释放资源
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                browser.CloseDevTools();
                browser.GetBrowser().CloseBrowser(true);
            }
            catch { }

            try
            {
                if (browser != null)
                {
                    browser.Dispose();
                    Cef.Shutdown();
                }
            }
            catch { }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SqliteSerivces sqlite = new SqliteSerivces();
            if (sqlite.SqlTest())
            {
                MessageBox.Show("数据库连接成功!");
            }
        }

    }
}
