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

namespace YenHoungBakery
{
    public partial class NewStoreForm : Form
    {
        private OleDbConnection connection = new OleDbConnection();
        YenHuongCalc yen;


        public string getStoreNameAndArea { get; private set; }

        public NewStoreForm(YenHuongCalc newYen)
        {
            InitializeComponent();
            this.yen = newYen;
            connection.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\Brian Lu\Documents\Self-Projects\YenHoungBakeryDB2003.mdb";
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            try
            {
                connection.Open();

                //OleDbCommand command = new OleDbCommand();
                //command.Connection = connection;
                //string query = " [Customer] Text, [Date] datetime, [Invoice] Text, [Banh Bia] Text, [Sau Rieng] Text, [Trung Thu] Text, " +
                //    "[Thap Cam] Text, [Banh Deo] Text, [Banh Sen] Text, [Banh Dau] Text, [Deo Thap Cam] Text, [Banh In] Text, [Red Bean] Text, " +
                //    "[Mochi] Text, [Dua Trung] Text, [Mut Dau Xanh] Text, [Green Tea/Mango] Text, [Banh To Lon] Text, [Banh To Nho] Text, " +
                //    "[New Year Cake] Text, [Amount] Text, [Phone] Text, [Address] Text ";
                //command.CommandText = "CREATE TABLE Customers(" + query + ")";
                //command.ExecuteNonQuery();

                if (NewStoreNameTxt.Text == "" || AreaCodeTxt.Text == "")
                {
                    MessageBox.Show("One or more fields cannot be empty");
                }
                else
                {
                    string storeNameAndArea = NewStoreNameTxt.Text + " - " + AreaCodeTxt.Text;
                    getStoreNameAndArea = storeNameAndArea;

                    MessageBox.Show("'" + storeNameAndArea + "' added to list");
                    this.Close();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }
    }
}
