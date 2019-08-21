using System;
using System.Collections.Generic;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace YenHoungBakery
{
    public partial class DateSelection : Form
    {
        private OleDbConnection connection = new OleDbConnection();

        YenHuongCalc Yen;

        public string getBeginDate { get; private set; }
        public string getEndDate { get; private set; }

        public DateSelection(YenHuongCalc newYen)
        {
            InitializeComponent();
            connection.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\Brian Lu\Documents\Self-Projects\YenHoungBakeryDB2003.mdb";
            Yen = newYen;
        }

        private void DateSelection_Load(object sender, EventArgs e)
        {
            
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            getBeginDate = StartingDateCal.Value.ToShortDateString();
            getEndDate = EndingDateCal.Value.ToShortDateString();
            ReportTest reportTest = new ReportTest(Yen, this);
            reportTest.ShowDialog();
            this.Close();
        }
    }
}
