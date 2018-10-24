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
using AcquireSystem.Forms;
using AcquireSystem.Sqlite;
using CefSharp;
using CefSharp.WinForms;
using Common.Forms;
using Z.Lib.Helper;

namespace AcquireSystem
{
    public partial class MainForm : NormalForm
    {
        public MainForm()
        {
            InitializeComponent();
            
        }

        /// <summary>
        /// 录入户主
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            FormHelper.ShowForm<AcquireMaster_Form>(this);
        }

        /// <summary>
        /// 录入人口信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            FormHelper.ShowForm<AcquirePopulations_Form>(this);

        }

        /// <summary>
        /// GIS采集
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            FormHelper.ShowForm<GisMeasure_Form>(this);

        }

        private void button3_Click(object sender, EventArgs e)
        {
            FormHelper.ShowForm<Config_Form>(this);
        }
    }
}
