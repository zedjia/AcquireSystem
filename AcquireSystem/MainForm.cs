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
using CefSharp;
using CefSharp.WinForms;

namespace AcquireSystem
{
    public partial class MainForm : Form
    {
        public ChromiumWebBrowser browser;
        public MainForm()
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

            browser = new ChromiumWebBrowser("http://www.baidu.com");

            //browser.FrameLoadStart += browser_FrameLoadStart;
            //browser.FrameLoadEnd += browser_FrameLoadEnd;
            //browser.LifeSpanHandler = new CustomLifeSpanHandler();
            //browser.RequestHandler = new CustomRequestHandler();

            //CefSharpSettings.LegacyJavascriptBindingEnabled = true;
            //browser.RegisterJsObject("WCShell", new JsEventFunction(this, browser));
            browser.JavascriptObjectRepository.Register("WCShell", new JsEventFunction(this, browser));
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


        private void button3_Click(object sender, EventArgs e)
        {
            browser.ShowDevTools();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            string html = "<!DOCTYPE html><html lang=\"en\" xmlns=\"http://www.w3.org/1999/xhtml\"><head><meta charset=\"utf-8\" /><title></title>" +
                          "<script>"+
                "  CefSharp.BindObjectAsync(\"WCShell\", \"WCShell\"); " +
                          "function test() { WCShell.helloWorld(document.getElementById('txt').value);}</script></head>"+
                "<body><H1> Hello World </H1 ><input type='text' value='hi zed' name='txt' id='txt' /> " +
          
                          "<input type = \"button\" onclick = \"test();\" value = \"测试\" /></body></html> ";


            browser.LoadHtml(html);
            browser.ResetBindings();
        }
    }
}
