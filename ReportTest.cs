using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;
using System.Drawing.Printing;

namespace YenHoungBakery
{
    public partial class ReportTest : Form
    {
        private OleDbConnection connection = new OleDbConnection();
        YenHuongCalc Yen;
        string storeName, beginDate, endDate;
        DataTable dataTable;
        public ReportTest(YenHuongCalc newYen, DateSelection date)
        {
            InitializeComponent();
            this.Yen = newYen;
            storeName = this.Yen.storeName;
            beginDate = date.getBeginDate;
            endDate = date.getEndDate;
            connection.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\Brian Lu\Documents\Self-Projects\YenHoungBakeryDB2003.mdb";
        }

        private void ReportTest_Load(object sender, EventArgs e)
        {
            try
            {
                dataGridView1.Visible = false;
                connection.Open();
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                string query = "select * from Customers where Customer = '" + storeName + 
                    "' and Date between '" + beginDate + "' and '" + endDate + "'";
                command.CommandText = query;

                OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                dataTable = new DataTable("Customers");
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;

                dataGridView1.Columns["Amount"].DefaultCellStyle.Format = "c";     

                connection.Close();
                this.print();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
                this.Close();
            }
        }

        private void copyAlltoClipBoard()
        {
            dataGridView1.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            dataGridView1.MultiSelect = true;
            dataGridView1.SelectAll();
            DataObject dataObject = dataGridView1.GetClipboardContent();
            if (dataObject != null)
                Clipboard.SetDataObject(dataObject);
        }
        public void print()
        {
            Microsoft.Office.Interop.Excel.Application xlExcel;
            Microsoft.Office.Interop.Excel.Workbook xlWorkbook;
            Microsoft.Office.Interop.Excel.Worksheet xlWorksheet;
            object misValue = System.Reflection.Missing.Value;
            xlExcel = new Microsoft.Office.Interop.Excel.Application();
            xlExcel.Visible = true;
            xlWorkbook = xlExcel.Workbooks.Add(misValue);
            xlWorksheet = (Microsoft.Office.Interop.Excel.Worksheet)xlWorkbook.Worksheets.get_Item(1);

            //paste the data from the datagridview on to excel sheet
            copyAlltoClipBoard();

            Microsoft.Office.Interop.Excel.Range range = (Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[8, 1];
            range.Select();
            xlWorksheet.PasteSpecial(range, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, true);
            xlWorksheet.Range["B7", "B7"].EntireColumn.Delete(null); //Deletes Customer Name Column
            xlWorksheet.Range["A1", "A1"].EntireColumn.Delete(null);
            xlWorksheet.Columns.AutoFit(); //Autofits each column

            //place address of bakery in this cell
            string payment = "Please make check paybale to" + Environment.NewLine +
                            "Luong Muoi or Yen Huong" + Environment.NewLine +
                            "15055 Woodforest" + Environment.NewLine + "Channelview, TX 77530" + Environment.NewLine +
                            "Tel: 713-223-3344" + Environment.NewLine + "Fax: 713-223-9716";
            Clipboard.SetText(payment);
            //Autofit
            Microsoft.Office.Interop.Excel.Range PaymentRange = (Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[1, 1];
            PaymentRange.Select();
            xlWorksheet.Paste(PaymentRange, false);

            //place store name in this cell
            Clipboard.SetText(storeName);
            Microsoft.Office.Interop.Excel.Range StoreRange = (Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[1, 11];
            StoreRange.Select();
            xlWorksheet.Paste(StoreRange, false);
            xlWorksheet.Range["K1", "K1"].Font.Size = 20;

            //get the total from the 'Amount' Column
            int rows = xlWorksheet.UsedRange.Rows.Count;
            xlWorksheet.Cells[rows + 2, 20] = "=SUM(T2:T" + rows + ")";
            double total = xlWorksheet.Cells[rows + 2, 20].Value;
            string finalTotal = "$" + total.ToString();
            xlWorksheet.Cells[rows + 2, 20] = finalTotal;
            xlWorksheet.Cells[rows + 2, 19] = "Total:";

            //Alignment
            xlWorksheet.Range["A8", "T8"].Cells.HorizontalAlignment = Microsoft.Office.Interop.Excel.XlHAlign.xlHAlignCenter;
            xlWorksheet.Range["A8", "T8"].Cells.VerticalAlignment = Microsoft.Office.Interop.Excel.XlVAlign.xlVAlignCenter;
            Microsoft.Office.Interop.Excel.Range alignment = xlWorksheet.Range["C8", "S8"];
            alignment.Select();
            alignment.Font.Size = 9;
            alignment.ColumnWidth = 5.645;
            alignment.WrapText = true;

            //footer
            xlWorksheet.Cells[rows + 2, 11] = "YEN HUONG BAKERY Thank You For Your Business!";

            Microsoft.Office.Interop.Excel.Range end = (Microsoft.Office.Interop.Excel.Range)xlWorksheet.Cells[rows + 2, xlWorksheet.UsedRange.Columns.Count - 1];
            end.Select();

            //closes the window after clicking the 'print' button
            this.Close();
        }
    }
}
