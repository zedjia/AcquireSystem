using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Common.Forms;
using Z.Lib.Helper;

namespace AcquireSystem.Forms
{
    public partial class AcquireMaster_Form : NormalForm
    {
        public AcquireMaster_Form()
        {
            InitializeComponent();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            FormHelper.ShowForm<GisMeasure_Form>(this);
        }
    }
}
