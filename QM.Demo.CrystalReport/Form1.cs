using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using CrystalDecisions.CrystalReports.Engine;

namespace QM.Demo.CrystalReport
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string rpt = @"c:\qm\rhm.rpt";

            ReportDocument doc = new ReportDocument();
            doc.Load(rpt);
            doc.ExportToDisk(CrystalDecisions.Shared.ExportFormatType.EditableRTF, @"c:\qm\123.rtf");
        }
    }
}
