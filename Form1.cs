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
    public partial class YenHuongCalc : Form
    {
        //database connection
        private OleDbConnection connection = new OleDbConnection();

        //invoice generator
        int invoiceNumber = 0;

        //Arraylist to get cake prices
        ArrayList cakePrices = new ArrayList();

        //checking variables
        ArrayList checkStoreList = new ArrayList();
        int checkPriceChange = 0;
        int checkHighestInvoiceNumber = 0;

        //global Total Price
        double TotalPrice = 0;

        //setters and getters
        public string storeName { get; private set; }
        public ComboBox getStoreList { get; private set; }
        public ArrayList getCheckStoreList { get; private set; }

        public YenHuongCalc()
        {
            InitializeComponent();
            connection.ConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\Brian Lu\Documents\Self-Projects\YenHoungBakeryDB2003.mdb";
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            try
            {
                //Update Label set to "" if not in retrieve mode
                UpdateLabel.Text = "";

                //sets Date TextBox to current date
                DateText.Text = DateTime.Now.ToShortDateString();

                //Disables update button unless retrieving data
                UpdateButton.Enabled = false;
                ExitUpdateButton.Visible = false;

                //ComboBox Handling
                StoreNameList.DropDownStyle = ComboBoxStyle.DropDownList;

                //gets the cake prices from the cake prices table
                DataTable cakeTable = new DataTable();
                string CakeQuery = "select * from CakePrices";
                using (OleDbCommand command = new OleDbCommand(CakeQuery, connection))
                {
                    connection.Open();
                    OleDbDataAdapter adapter = new OleDbDataAdapter(command);
                    adapter.Fill(cakeTable);
                }

                DataRow latestCakePrices = cakeTable.Rows[cakeTable.Rows.Count - 1];
                BanhBiaPriceTxt.Text = latestCakePrices[0].ToString();
                SauRiengPriceTxt.Text = latestCakePrices[1].ToString();
                TrungThuPriceTxt.Text = latestCakePrices[2].ToString();
                ThapCamPriceTxt.Text = latestCakePrices[3].ToString();
                BanhDeoPriceTxt.Text = latestCakePrices[4].ToString();
                BanhSenPriceTxt.Text = latestCakePrices[5].ToString();
                BanhDauPriceTxt.Text = latestCakePrices[6].ToString();
                DeoThapCamPriceTxt.Text = latestCakePrices[7].ToString();
                BanhInPriceTxt.Text = latestCakePrices[8].ToString();
                RedBeanPriceTxt.Text = latestCakePrices[9].ToString();
                MochiPriceTxt.Text = latestCakePrices[10].ToString();
                DuaTrungPriceTxt.Text = latestCakePrices[11].ToString();
                MutDauXanhPriceTxt.Text = latestCakePrices[12].ToString();
                GreenTeaMangoPriceTxt.Text = latestCakePrices[13].ToString();
                BanhToLonPriceTxt.Text = latestCakePrices[14].ToString();
                BanhToNhoPriceTxt.Text = latestCakePrices[15].ToString();
                NewYearCakePriceTxt.Text = latestCakePrices[16].ToString();

                //gets only the store names for the combo box
                string query = "select distinct Customer from Customers";
                using (OleDbCommand command = new OleDbCommand(query, connection))
                {
                    using(OleDbDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            checkStoreList.Add(reader["Customer"].ToString());
                            StoreNameList.Items.Add(reader["Customer"].ToString());
                        }
                    }
                }

                //gets highes invoice number for auto invoice generation
                string numberQuery = "select MAX(Invoice) from Customers";
                using (OleDbCommand command = new OleDbCommand(numberQuery, connection))
                {
                    using(OleDbDataReader numberReader = command.ExecuteReader())
                    {
                        while (numberReader.Read())
                        {
                            invoiceNumber = int.Parse(numberReader[0].ToString());
                        }
                    }
                }

                invoiceNumber++;
                InvoiceTxt.Text = invoiceNumber.ToString();
                checkHighestInvoiceNumber = invoiceNumber;
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            try
             {
                //if any of the required fields are left empty
                if(StoreNameList.Text == "")
                {
                    MessageBox.Show("Please select a store");
                    return;
                }
                if(DateText.Text == "")
                {
                    MessageBox.Show("Please enter the date");
                    return;
                }
                //checks if the invoice number has been changed before saving
                if(InvoiceTxt.Text != checkHighestInvoiceNumber.ToString())
                {
                    MessageBox.Show("Did you mean to retrieve?");
                    return;
                }

                //checks if the prices are numbers in case user modifies
                if(BanhBiaPriceTxt.Text == "" || SauRiengPriceTxt.Text == "" || TrungThuPriceTxt.Text == "")
                {
                    MessageBox.Show("Please enter numbers only for cake prices");
                    return;
                }
                if (ThapCamPriceTxt.Text == "" || BanhDeoPriceTxt.Text == "" || BanhSenPriceTxt.Text == "")
                {
                    MessageBox.Show("Please enter numbers only for cake prices");
                    return;
                }
                if (BanhDauPriceTxt.Text == "" || DeoThapCamPriceTxt.Text == "" || BanhInPriceTxt.Text == "")
                {
                    MessageBox.Show("Please enter numbers only for cake prices");
                    return;
                }
                if (RedBeanPriceTxt.Text == "" || MochiPriceTxt.Text == "" || DuaTrungPriceTxt.Text == "")
                {
                    MessageBox.Show("Please enter numbers only for cake prices");
                    return;
                }
                if (MutDauXanhPriceTxt.Text == "" || GreenTeaMangoPriceTxt.Text == "" || BanhToLonPriceTxt.Text == "")
                {
                    MessageBox.Show("Please enter numbers only for cake prices");
                    return;
                }
                if (BanhToNhoPriceTxt.Text == "" || NewYearCakePriceTxt.Text == "")
                {
                    MessageBox.Show("Please enter numbers only for cake prices");
                    return;
                }

                //Calculates in case user does no hit calculate
                this.AutoCalculate();

                //Fills in empty cake entries
                if (BanhBiaTxt.Text == "") { BanhBiaTxt.Text = "0"; }
                if (SauRiengTxt.Text == "") { SauRiengTxt.Text = "0"; }
                if (TrungThuTxt.Text == "") { TrungThuTxt.Text = "0"; }
                if (ThapCamTxt.Text == "") { ThapCamTxt.Text = "0"; }
                if (BanhDeoText.Text == "") { BanhDeoText.Text = "0"; }
                if (BanhSenTxt.Text == "") { BanhSenTxt.Text = "0"; }
                if (BanhDauTxt.Text == "") { BanhDauTxt.Text = "0"; }
                if (DeoThapCamTxt.Text == "") { DeoThapCamTxt.Text = "0"; }
                if (BanhInTxt.Text == "") { BanhInTxt.Text = "0"; }
                if (RedBeanTxt.Text == "") { RedBeanTxt.Text = "0"; }
                if (MochiTxt.Text == "") { MochiTxt.Text = "0"; }
                if (DuaTrungTxt.Text == "") { DuaTrungTxt.Text = "0"; }
                if (MutDauXanhTxt.Text == "") { MutDauXanhTxt.Text = "0"; }
                if (GreenTeaMangoTxt.Text == "") { GreenTeaMangoTxt.Text = "0"; }
                if (BanhToLonTxt.Text == "") { BanhToLonTxt.Text = "0"; }
                if (BanhToNhoTxt.Text == "") { BanhToNhoTxt.Text = "0"; }
                if (NewYearCakeTxt.Text == "") { NewYearCakeTxt.Text = "0"; }

                //inserts the cake data into Customer Table
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                command.CommandText = "insert into Customers ([Customer], [Date], [Invoice], [Banh Bia], [Sau Rieng], [Trung Thu], [Thap Cam], [Banh Deo], [Banh Sen]," +
                    "[Banh Dau], [Deo Thap Cam], [Banh In], [Red Bean], [Mochi], [Dua Trung], [Mut Dau Xanh], [Green Tea/Mango], [Banh To Lon],[Banh To Nho], [New Year Cake], [Amount] )" +
                    " values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";

                command.Parameters.AddWithValue("@Customer", StoreNameList.Text);
                command.Parameters.AddWithValue("@Date", DateText.Text);
                command.Parameters.AddWithValue("@Invoice", InvoiceTxt.Text);
                command.Parameters.AddWithValue("@Banh_Bia", BanhBiaTxt.Text);
                command.Parameters.AddWithValue("@Sau_Rieng", SauRiengTxt.Text);
                command.Parameters.AddWithValue("@Trung_Thu", TrungThuTxt.Text);
                command.Parameters.AddWithValue("@Thap_Cam", ThapCamTxt.Text);
                command.Parameters.AddWithValue("@Banh_Deo", BanhDeoText.Text);
                command.Parameters.AddWithValue("@Banh_Sen", BanhSenTxt.Text);
                command.Parameters.AddWithValue("@Banh_Dau", BanhDauTxt.Text);
                command.Parameters.AddWithValue("@Deo_Thap_Cam", DeoThapCamTxt.Text);
                command.Parameters.AddWithValue("@Banh_In", BanhInTxt.Text);
                command.Parameters.AddWithValue("@Red_Bean", RedBeanTxt.Text);
                command.Parameters.AddWithValue("@Mochi", MochiTxt.Text);
                command.Parameters.AddWithValue("@Dua_Trung", DuaTrungTxt.Text);
                command.Parameters.AddWithValue("@Mut_Dau_Xanh", MutDauXanhTxt.Text);
                command.Parameters.AddWithValue("@Green_Tea/Mango", GreenTeaMangoTxt.Text);
                command.Parameters.AddWithValue("@Banh_To_Lon", BanhToLonTxt.Text);
                command.Parameters.AddWithValue("@Banh_To_Nho", BanhToNhoTxt.Text);
                command.Parameters.AddWithValue("@New_Year_Cake", NewYearCakeTxt.Text);
                command.Parameters.AddWithValue("@Amount", TotalTxt.Text);

                command.ExecuteNonQuery();

                //checks if there is a price change to update the CakePrice Table
                if (checkPriceChange == 1)
                {
                    OleDbCommand cakeCommand = new OleDbCommand();
                    cakeCommand.Connection = connection;
                    string query = "insert into CakePrices ([BanhBiaPrice], [SauRiengPrice], [TrungThuPrice], [ThapCamPrice], [BanhDeoPrice], [BanhSenPrice], [BanhDauPrice], " +
                        "[DeoThapCamPrice], [BanhInPrice], [RedBeanPrice], [MochiPrice], [DuaTrungPrice], [MutDauXangPrice], [GreenTeaMangoPrice], [BanhToLonPrice], [BanhToNhoPrice], [NewYearCakePrice] )" +
                        " values (?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?,?)";
                    cakeCommand.CommandText = query;

                    cakeCommand.Parameters.AddWithValue("@BanhBiaPrice", BanhBiaPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@SauRiengPrice", SauRiengPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@TrungThuPrice", TrungThuPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@ThapCamPrice", ThapCamPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@BanhDeoPrice", BanhDeoPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@BanhSenPrice", BanhSenPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@BanhDauPrice", BanhDauPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@DeoThapCamPrice", DeoThapCamLabel.Text);
                    cakeCommand.Parameters.AddWithValue("@BanhInPrice", BanhInPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@RedBeanPrice", RedBeanPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@MochiPrice", MochiPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@DuaTrungPrice", DuaTrungPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@MutDauXangPrice", MutDauXanhPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@GreenTeaMangoPrice", GreenTeaMangoPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@BanhToLonPrice", BanhToLonPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@BanhToNhoPrice", BanhToNhoPriceTxt.Text);
                    cakeCommand.Parameters.AddWithValue("@NewYearCakePrice", NewYearCakePriceTxt.Text);

                    cakeCommand.ExecuteNonQuery();
                    checkPriceChange = 0;
                }
                MessageBox.Show("Saved");

                //Increments the invoice number after saving;
                invoiceNumber++;
                InvoiceTxt.Text = invoiceNumber.ToString();

                //Clears cake entries
                BanhBiaTxt.Text = "";
                SauRiengTxt.Text = ""; TrungThuTxt.Text = "";
                ThapCamTxt.Text = ""; BanhDeoText.Text = "";
                BanhSenTxt.Text = ""; BanhDauTxt.Text = "";
                DeoThapCamTxt.Text = ""; BanhInTxt.Text = "";
                RedBeanTxt.Text = ""; MochiTxt.Text = "";
                DuaTrungTxt.Text = ""; MutDauXanhTxt.Text = "";
                GreenTeaMangoTxt.Text = ""; BanhToLonTxt.Text = "";
                BanhToNhoTxt.Text = ""; NewYearCakeTxt.Text = "";

                //refreshes the store list in case of new customer
                this.Update();
                this.Refresh();
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        private void Label1_Click(object sender, EventArgs e)
        {
            //intentionally left blank
        }

        //closes on exit or with "exit" button
        private void YenHuongCalc_FormClosing(object sender, FormClosedEventArgs e)
        {
            if (string.Equals((sender as Button).Name, @"ExitButton"))
                YenHuongCalc.ActiveForm.Close();
            else
                YenHuongCalc.ActiveForm.Close();
        }

        private void CalculateButton_Click(object sender, EventArgs e)
        {
            //if any of the entries are left blank
            if (BanhBiaTxt.Text == "") { BanhBiaTxt.Text = "0"; }
            if (SauRiengTxt.Text == "") { SauRiengTxt.Text = "0"; }
            if (TrungThuTxt.Text == "") { TrungThuTxt.Text = "0"; }
            if (ThapCamTxt.Text == "") { ThapCamTxt.Text = "0"; }
            if (BanhDeoText.Text == "") { BanhDeoText.Text = "0"; }
            if (BanhSenTxt.Text == "") { BanhSenTxt.Text = "0"; }
            if (BanhDauTxt.Text == "") { BanhDauTxt.Text = "0"; }
            if (DeoThapCamTxt.Text == "") { DeoThapCamTxt.Text = "0"; }
            if (BanhInTxt.Text == "") { BanhInTxt.Text = "0"; }
            if (RedBeanTxt.Text == "") { RedBeanTxt.Text = "0"; }
            if (MochiTxt.Text == "") { MochiTxt.Text = "0"; }
            if (DuaTrungTxt.Text == "") { DuaTrungTxt.Text = "0"; }
            if (MutDauXanhTxt.Text == "") { MutDauXanhTxt.Text = "0"; }
            if (GreenTeaMangoTxt.Text == "") { GreenTeaMangoTxt.Text = "0"; }
            if (BanhToLonTxt.Text == "") { BanhToLonTxt.Text = "0"; }
            if (BanhToNhoTxt.Text == "") { BanhToNhoTxt.Text = "0"; }
            if (NewYearCakeTxt.Text == "") { NewYearCakeTxt.Text = "0"; }

            //finds total of each cake based on user entry and cake price
            double price;
            if (!double.TryParse(BanhBiaTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Bian Bia quantity");
                return;
            }
            double TotalPriceBanhBia = price * double.Parse(BanhBiaPriceTxt.Text);

            if (!double.TryParse(SauRiengTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Sau Rieng quantity");
                return;
            }
            double TotalPriceSauRieng = price * double.Parse(SauRiengPriceTxt.Text);

            if (!double.TryParse(TrungThuTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Trung Thu quantity");
                return;
            }
            double TotalPriceTrungThu = price * double.Parse(SauRiengPriceTxt.Text);

            if (!double.TryParse(ThapCamTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Thap Cam quantity");
                return;
            }
            double TotalPriceThapCam = price * double.Parse(ThapCamPriceTxt.Text);

            if (!double.TryParse(BanhDeoText.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Banh Deo quantity");
                return;
            }
            double TotalPriceBanhDeo = price * double.Parse(BanhDeoPriceTxt.Text);

            if (!double.TryParse(BanhSenTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Banh Sen quantity");
                return;
            }
            double TotalPriceBanhSen = price * double.Parse(BanhSenPriceTxt.Text);

            if (!double.TryParse(BanhDauTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Banh Dau quantity");
                return;
            }
            double TotalPriceBanhDau = price * double.Parse(BanhDauPriceTxt.Text);

            if (!double.TryParse(DeoThapCamTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Deo Thap Cam quantity");
                return;
            }
            double TotalPriceDeoThapCam = price * double.Parse(DeoThapCamPriceTxt.Text);

            if (!double.TryParse(BanhInTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Banh In quantity");
                return;
            }
            double TotalPriceBanhIn = price * double.Parse(BanhInPriceTxt.Text);

            if (!double.TryParse(RedBeanTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Red Bean quantity");
                return;
            }
            double TotalPriceRedBean = price * double.Parse(RedBeanPriceTxt.Text);

            if (!double.TryParse(MochiTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Mochi quantity");
                return;
            }
            double TotalPriceMochi = price * double.Parse(MochiPriceTxt.Text);

            if (!double.TryParse(DuaTrungTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Dua Trung quantity");
                return;
            }
            double TotalPriceDuaTrung = price * double.Parse(DuaTrungPriceTxt.Text);

            if (!double.TryParse(MutDauXanhTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Mut Dau Xanh quantity");
                return;
            }
            double TotalPriceMutDauXanh = price * double.Parse(MutDauXanhPriceTxt.Text);

            if (!double.TryParse(GreenTeaMangoTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Green Tea/Mango quantity");
                return;
            }
            double TotalPriceGreenTeaMango = price * double.Parse(GreenTeaMangoPriceTxt.Text);

            if (!double.TryParse(BanhToLonTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Banh To Lon quantity");
                return;
            }
            double TotalPriceBanhToLon = price * double.Parse(BanhToLonPriceTxt.Text);

            if (!double.TryParse(BanhToNhoTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for Banh To Nho quantity");
                return;
            }
            double TotalPriceBanhToNho = price * double.Parse(BanhToNhoPriceTxt.Text);

            if (!double.TryParse(NewYearCakeTxt.Text, out price))
            {
                MessageBox.Show("Please enter numbers only for New Year Cake quantity");
                return;
            }
            double TotalPriceNewYearCake = price * double.Parse(NewYearCakePriceTxt.Text);

            //calculates the amount of each cake
            BanhBiaPriceAmountTxt.Text = TotalPriceBanhBia.ToString();
            SauRiengPriceAmountTxt.Text = TotalPriceSauRieng.ToString();
            TrungThuPriceAmountTxt.Text = TotalPriceTrungThu.ToString();
            ThapCamPriceAmountTxt.Text = TotalPriceThapCam.ToString();
            BanhDeoPriceAmountTxt.Text = TotalPriceBanhDeo.ToString();
            BanhSenPriceAmountTxt.Text = TotalPriceBanhSen.ToString();
            BanhDauPriceAmountTxt.Text = TotalPriceBanhDau.ToString();
            DeoThapCamPriceAmountTxt.Text = TotalPriceDeoThapCam.ToString();
            BanhInPriceAmountTxt.Text = TotalPriceBanhIn.ToString();
            RedBeanPriceAmountTxt.Text = TotalPriceRedBean.ToString();
            MochiPriceAmountTxt.Text = TotalPriceMochi.ToString();
            DuaTrungPriceAmountTxt.Text = TotalPriceDuaTrung.ToString();
            MutDauXanhPriceAmountTxt.Text = TotalPriceMutDauXanh.ToString();
            GreenTeaMangoPriceAmountTxt.Text = TotalPriceGreenTeaMango.ToString();
            BanhToLonPriceAmountTxt.Text = TotalPriceBanhToLon.ToString();
            BanhToNhoPriceAmountTxt.Text = TotalPriceBanhToNho.ToString();
            NewYearCakePriceAmountTxt.Text = TotalPriceNewYearCake.ToString();

            //calculates total price
            double TotalFinalPrice = TotalPriceBanhBia + TotalPriceSauRieng + TotalPriceTrungThu + TotalPriceThapCam
                + TotalPriceBanhDeo + TotalPriceBanhSen + TotalPriceBanhDau + TotalPriceDeoThapCam + TotalPriceBanhIn
                + TotalPriceRedBean + TotalPriceMochi + TotalPriceDuaTrung + TotalPriceMutDauXanh + TotalPriceGreenTeaMango
                + TotalPriceBanhToLon + TotalPriceBanhToNho + TotalPriceNewYearCake;

            //displays the final amount
            TotalTxt.Text = "$" + TotalFinalPrice.ToString("0.00");
            TotalPrice = TotalFinalPrice;

            //sets the the program-entered 0s back into blanks for database
            if (BanhBiaTxt.Text == "0") { BanhBiaTxt.Text = ""; }
            if (SauRiengTxt.Text == "0") { SauRiengTxt.Text = ""; }
            if (TrungThuTxt.Text == "0") { TrungThuTxt.Text = ""; }
            if (ThapCamTxt.Text == "0") { ThapCamTxt.Text = ""; }
            if (BanhDeoText.Text == "0") { BanhDeoText.Text = ""; }
            if (BanhSenTxt.Text == "0") { BanhSenTxt.Text = ""; }
            if (BanhDauTxt.Text == "0") { BanhDauTxt.Text = ""; }
            if (DeoThapCamTxt.Text == "0") { DeoThapCamTxt.Text = ""; }
            if (BanhInTxt.Text == "0") { BanhInTxt.Text = ""; }
            if (RedBeanTxt.Text == "0") { RedBeanTxt.Text = ""; }
            if (MochiTxt.Text == "0") { MochiTxt.Text = ""; }
            if (DuaTrungTxt.Text == "0") { DuaTrungTxt.Text = ""; }
            if (MutDauXanhTxt.Text == "0") { MutDauXanhTxt.Text = ""; }
            if (GreenTeaMangoTxt.Text == "0") { GreenTeaMangoTxt.Text = ""; }
            if (BanhToLonTxt.Text == "0") { BanhToLonTxt.Text = ""; }
            if (BanhToNhoTxt.Text == "0") { BanhToNhoTxt.Text = ""; }
            if (NewYearCakeTxt.Text == "0") { NewYearCakeTxt.Text = ""; }
        }

        //enables user to change price of each cake
        private void ChangePriceButton_Click(object sender, EventArgs e)
        {
            BanhBiaPriceTxt.Enabled = true; SauRiengPriceTxt.Enabled = true;
            TrungThuPriceTxt.Enabled = true; ThapCamPriceTxt.Enabled = true;
            BanhDeoPriceTxt.Enabled = true; BanhSenPriceTxt.Enabled = true;
            BanhDauPriceTxt.Enabled = true; DeoThapCamPriceTxt.Enabled = true;
            BanhInPriceTxt.Enabled = true; RedBeanPriceTxt.Enabled = true;
            MochiPriceTxt.Enabled = true; DuaTrungPriceTxt.Enabled = true;
            MutDauXanhPriceTxt.Enabled = true; GreenTeaMangoPriceTxt.Enabled = true;
            BanhToLonPriceTxt.Enabled = true; BanhToNhoPriceTxt.Enabled = true;
            NewYearCakePriceTxt.Enabled = true;

            checkPriceChange = 1;
        }
        private void ExitButton_Click(object sender, EventArgs e)
        {
            connection.Close();
            YenHuongCalc.ActiveForm.Close();
        }

        private void NewStoreButton_Click(object sender, EventArgs e)
        {
            NewStoreForm newStore = new NewStoreForm(this);
            newStore.ShowDialog(this);

            if(newStore.getStoreNameAndArea != null)
            {
                checkStoreList.Add(newStore.getStoreNameAndArea);
                StoreNameList.Items.Add(newStore.getStoreNameAndArea);
            }
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            try
            {
                int InvoiceReader = 0;
                Int32.TryParse(InvoiceTxt.Text, out InvoiceReader);
                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                string query = "select * from Customers where Invoice=" + InvoiceReader +"";
                command.CommandText = query;

                OleDbDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    StoreNameList.Text = reader["Customer"].ToString();
                    DateText.Text = reader["Date"].ToString(); InvoiceTxt.Text = reader["Invoice"].ToString();
                    BanhBiaTxt.Text = reader["Banh Bia"].ToString(); SauRiengTxt.Text = reader["Sau Rieng"].ToString();
                    TrungThuTxt.Text = reader["Trung Thu"].ToString(); ThapCamTxt.Text = reader["Thap Cam"].ToString();
                    BanhDeoText.Text = reader["Banh Deo"].ToString(); BanhSenTxt.Text = reader["Banh Sen"].ToString();
                    BanhDauTxt.Text = reader["Banh Dau"].ToString(); DeoThapCamTxt.Text = reader["Deo Thap Cam"].ToString();
                    BanhInTxt.Text = reader["Banh In"].ToString(); RedBeanTxt.Text = reader["Red Bean"].ToString();
                    MochiTxt.Text = reader["Mochi"].ToString(); DuaTrungTxt.Text = reader["Dua Trung"].ToString();
                    MutDauXanhTxt.Text = reader["Mut Dau Xanh"].ToString(); GreenTeaMangoTxt.Text = reader["Green Tea/Mango"].ToString();
                    BanhToLonTxt.Text = reader["Banh To Lon"].ToString(); BanhToNhoTxt.Text = reader["Banh To Nho"].ToString();
                    NewYearCakeTxt.Text = reader["New Year Cake"].ToString(); TotalTxt.Text = reader["Amount"].ToString();
                }
                saveButton.Enabled = false;
                UpdateButton.Enabled = true;
                InvoiceTxt.Enabled = false;
                DateText.Enabled = false;
                UpdateLabel.Text = "Updating...";
                ExitUpdateButton.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        private void ReportButton_Click(object sender, EventArgs e)
        {
            if(StoreNameList.Text != "")
            {
                storeName = StoreNameList.Text;
                DateSelection dateSelectionWindow = new DateSelection(this);
                dateSelectionWindow.ShowDialog();
            }
            else
            {
                MessageBox.Show("Please select a store to report");
            }
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                this.AutoCalculate();

                int BanhBiaNum = 0; int SauRiengNum = 0; int TrungThuNum = 0; int BanhDeoNum = 0;
                int BanhSenNum = 0; int BanhDauNum = 0; int DeoThapCamNum = 0; int BanhInNum = 0;
                int RedBeanNum = 0; int MochiNum = 0; int DuaTrungNum = 0; int MutDauXangNum = 0;
                int GreenTeaMangoNum = 0; int BanhToLonNum = 0; int BangToNhoNum = 0; int NewYearCakeNum = 0;
                int ThapCamNum = 0; int CurrentInvoiceNum = 0;

                Int32.TryParse(BanhBiaTxt.Text, out BanhBiaNum);
                Int32.TryParse(SauRiengTxt.Text, out SauRiengNum);
                Int32.TryParse(TrungThuTxt.Text, out TrungThuNum);
                Int32.TryParse(ThapCamTxt.Text, out ThapCamNum);
                Int32.TryParse(BanhDeoText.Text, out BanhDeoNum);
                Int32.TryParse(BanhSenTxt.Text, out BanhSenNum);
                Int32.TryParse(BanhDauTxt.Text, out BanhDauNum);
                Int32.TryParse(DeoThapCamTxt.Text, out DeoThapCamNum);
                Int32.TryParse(BanhInTxt.Text, out BanhInNum);
                Int32.TryParse(RedBeanTxt.Text, out RedBeanNum);
                Int32.TryParse(MochiTxt.Text, out MochiNum);
                Int32.TryParse(DuaTrungTxt.Text, out DuaTrungNum);
                Int32.TryParse(MutDauXanhTxt.Text, out MutDauXangNum);
                Int32.TryParse(GreenTeaMangoTxt.Text, out GreenTeaMangoNum);
                Int32.TryParse(BanhToLonTxt.Text, out BanhToLonNum);
                Int32.TryParse(BanhToNhoTxt.Text, out BangToNhoNum);
                Int32.TryParse(NewYearCakeTxt.Text, out NewYearCakeNum);

                Int32.TryParse(InvoiceTxt.Text, out CurrentInvoiceNum);

                OleDbCommand command = new OleDbCommand();
                command.Connection = connection;
                string query = "update Customers set [Banh Bia]=" + BanhBiaNum + ", [Sau Rieng]=" + SauRiengNum + ", [Trung Thu]=" + TrungThuNum + ", [Thap Cam]=" + ThapCamNum +
                    ", [Banh Deo]=" + BanhDeoNum + ", [Banh Sen]=" + BanhSenNum + ", [Banh Dau]=" + BanhDauNum + ", [Deo Thap Cam]=" + DeoThapCamNum + ", [Banh In]=" + BanhInNum +
                    ", [Red Bean]=" + RedBeanNum + ", [Mochi]=" + MochiNum + ", [Dua Trung]=" + DuaTrungNum + ", [Mut Dau Xanh]=" + MutDauXangNum + ", [Green Tea/Mango]=" + GreenTeaMangoNum +
                    ", [Banh To Lon]=" + BanhToLonNum + ", [Banh To Nho]=" + BangToNhoNum + ", [New Year Cake]=" + NewYearCakeNum + ", [Amount]=" + TotalPrice + " where Invoice=" + CurrentInvoiceNum + "";
                command.CommandText = query;

                command.ExecuteNonQuery();
                MessageBox.Show("Invoice Updated");
                UpdateButton.Enabled = false;
                saveButton.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        //Auto Calculates in case user hits update without calculating
        private void AutoCalculate()
        {
            try
            {
                //if any of the entries are left blank
                if (BanhBiaTxt.Text == "") { BanhBiaTxt.Text = "0"; }
                if (SauRiengTxt.Text == "") { SauRiengTxt.Text = "0"; }
                if (TrungThuTxt.Text == "") { TrungThuTxt.Text = "0"; }
                if (ThapCamTxt.Text == "") { ThapCamTxt.Text = "0"; }
                if (BanhDeoText.Text == "") { BanhDeoText.Text = "0"; }
                if (BanhSenTxt.Text == "") { BanhSenTxt.Text = "0"; }
                if (BanhDauTxt.Text == "") { BanhDauTxt.Text = "0"; }
                if (DeoThapCamTxt.Text == "") { DeoThapCamTxt.Text = "0"; }
                if (BanhInTxt.Text == "") { BanhInTxt.Text = "0"; }
                if (RedBeanTxt.Text == "") { RedBeanTxt.Text = "0"; }
                if (MochiTxt.Text == "") { MochiTxt.Text = "0"; }
                if (DuaTrungTxt.Text == "") { DuaTrungTxt.Text = "0"; }
                if (MutDauXanhTxt.Text == "") { MutDauXanhTxt.Text = "0"; }
                if (GreenTeaMangoTxt.Text == "") { GreenTeaMangoTxt.Text = "0"; }
                if (BanhToLonTxt.Text == "") { BanhToLonTxt.Text = "0"; }
                if (BanhToNhoTxt.Text == "") { BanhToNhoTxt.Text = "0"; }
                if (NewYearCakeTxt.Text == "") { NewYearCakeTxt.Text = "0"; }

                //finds total of each cake based on user entry and cake price
                double price;
                if (!double.TryParse(BanhBiaTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Bian Bia quantity");
                    return;
                }
                double TotalPriceBanhBia = price * double.Parse(BanhBiaPriceTxt.Text);

                if (!double.TryParse(SauRiengTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Sau Rieng quantity");
                    return;
                }
                double TotalPriceSauRieng = price * double.Parse(SauRiengPriceTxt.Text);

                if (!double.TryParse(TrungThuTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Trung Thu quantity");
                    return;
                }
                double TotalPriceTrungThu = price * double.Parse(SauRiengPriceTxt.Text);

                if (!double.TryParse(ThapCamTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Thap Cam quantity");
                    return;
                }
                double TotalPriceThapCam = price * double.Parse(ThapCamPriceTxt.Text);

                if (!double.TryParse(BanhDeoText.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Banh Deo quantity");
                    return;
                }
                double TotalPriceBanhDeo = price * double.Parse(BanhDeoPriceTxt.Text);

                if (!double.TryParse(BanhSenTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Banh Sen quantity");
                    return;
                }
                double TotalPriceBanhSen = price * double.Parse(BanhSenPriceTxt.Text);

                if (!double.TryParse(BanhDauTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Banh Dau quantity");
                    return;
                }
                double TotalPriceBanhDau = price * double.Parse(BanhDauPriceTxt.Text);

                if (!double.TryParse(DeoThapCamTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Deo Thap Cam quantity");
                    return;
                }
                double TotalPriceDeoThapCam = price * double.Parse(DeoThapCamPriceTxt.Text);

                if (!double.TryParse(BanhInTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Banh In quantity");
                    return;
                }
                double TotalPriceBanhIn = price * double.Parse(BanhInPriceTxt.Text);

                if (!double.TryParse(RedBeanTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Red Bean quantity");
                    return;
                }
                double TotalPriceRedBean = price * double.Parse(RedBeanPriceTxt.Text);

                if (!double.TryParse(MochiTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Mochi quantity");
                    return;
                }
                double TotalPriceMochi = price * double.Parse(MochiPriceTxt.Text);

                if (!double.TryParse(DuaTrungTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Dua Trung quantity");
                    return;
                }
                double TotalPriceDuaTrung = price * double.Parse(DuaTrungPriceTxt.Text);

                if (!double.TryParse(MutDauXanhTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Mut Dau Xanh quantity");
                    return;
                }
                double TotalPriceMutDauXanh = price * double.Parse(MutDauXanhPriceTxt.Text);

                if (!double.TryParse(GreenTeaMangoTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Green Tea/Mango quantity");
                    return;
                }
                double TotalPriceGreenTeaMango = price * double.Parse(GreenTeaMangoPriceTxt.Text);

                if (!double.TryParse(BanhToLonTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Banh To Lon quantity");
                    return;
                }
                double TotalPriceBanhToLon = price * double.Parse(BanhToLonPriceTxt.Text);

                if (!double.TryParse(BanhToNhoTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for Banh To Nho quantity");
                    return;
                }
                double TotalPriceBanhToNho = price * double.Parse(BanhToNhoPriceTxt.Text);

                if (!double.TryParse(NewYearCakeTxt.Text, out price))
                {
                    MessageBox.Show("Please enter numbers only for New Year Cake quantity");
                    return;
                }
                double TotalPriceNewYearCake = price * double.Parse(NewYearCakePriceTxt.Text);

                //calculates the amount of each cake
                BanhBiaPriceAmountTxt.Text = TotalPriceBanhBia.ToString();
                SauRiengPriceAmountTxt.Text = TotalPriceSauRieng.ToString();
                TrungThuPriceAmountTxt.Text = TotalPriceTrungThu.ToString();
                ThapCamPriceAmountTxt.Text = TotalPriceThapCam.ToString();
                BanhDeoPriceAmountTxt.Text = TotalPriceBanhDeo.ToString();
                BanhSenPriceAmountTxt.Text = TotalPriceBanhSen.ToString();
                BanhDauPriceAmountTxt.Text = TotalPriceBanhDau.ToString();
                DeoThapCamPriceAmountTxt.Text = TotalPriceDeoThapCam.ToString();
                BanhInPriceAmountTxt.Text = TotalPriceBanhIn.ToString();
                RedBeanPriceAmountTxt.Text = TotalPriceRedBean.ToString();
                MochiPriceAmountTxt.Text = TotalPriceMochi.ToString();
                DuaTrungPriceAmountTxt.Text = TotalPriceDuaTrung.ToString();
                MutDauXanhPriceAmountTxt.Text = TotalPriceMutDauXanh.ToString();
                GreenTeaMangoPriceAmountTxt.Text = TotalPriceGreenTeaMango.ToString();
                BanhToLonPriceAmountTxt.Text = TotalPriceBanhToLon.ToString();
                BanhToNhoPriceAmountTxt.Text = TotalPriceBanhToNho.ToString();
                NewYearCakePriceAmountTxt.Text = TotalPriceNewYearCake.ToString();

                //calculates total price
                double TotalFinalPrice = TotalPriceBanhBia + TotalPriceSauRieng + TotalPriceTrungThu + TotalPriceThapCam
                    + TotalPriceBanhDeo + TotalPriceBanhSen + TotalPriceBanhDau + TotalPriceDeoThapCam + TotalPriceBanhIn
                    + TotalPriceRedBean + TotalPriceMochi + TotalPriceDuaTrung + TotalPriceMutDauXanh + TotalPriceGreenTeaMango
                    + TotalPriceBanhToLon + TotalPriceBanhToNho + TotalPriceNewYearCake;

                //displays the final amount
                TotalTxt.Text = "$" + TotalFinalPrice.ToString("0.00");
                TotalPrice = TotalFinalPrice;

                //sets the the program-entered 0s back into blanks for database
                if (BanhBiaTxt.Text == "0") { BanhBiaTxt.Text = ""; }
                if (SauRiengTxt.Text == "0") { SauRiengTxt.Text = ""; }
                if (TrungThuTxt.Text == "0") { TrungThuTxt.Text = ""; }
                if (ThapCamTxt.Text == "0") { ThapCamTxt.Text = ""; }
                if (BanhDeoText.Text == "0") { BanhDeoText.Text = ""; }
                if (BanhSenTxt.Text == "0") { BanhSenTxt.Text = ""; }
                if (BanhDauTxt.Text == "0") { BanhDauTxt.Text = ""; }
                if (DeoThapCamTxt.Text == "0") { DeoThapCamTxt.Text = ""; }
                if (BanhInTxt.Text == "0") { BanhInTxt.Text = ""; }
                if (RedBeanTxt.Text == "0") { RedBeanTxt.Text = ""; }
                if (MochiTxt.Text == "0") { MochiTxt.Text = ""; }
                if (DuaTrungTxt.Text == "0") { DuaTrungTxt.Text = ""; }
                if (MutDauXanhTxt.Text == "0") { MutDauXanhTxt.Text = ""; }
                if (GreenTeaMangoTxt.Text == "0") { GreenTeaMangoTxt.Text = ""; }
                if (BanhToLonTxt.Text == "0") { BanhToLonTxt.Text = ""; }
                if (BanhToNhoTxt.Text == "0") { BanhToNhoTxt.Text = ""; }
                if (NewYearCakeTxt.Text == "0") { NewYearCakeTxt.Text = ""; }
            }
            catch(Exception ex)
            {
                MessageBox.Show("Error: " + ex);
            }
        }

        //Resets to default invoice interface
        private void ExitUpdateButton_Click(object sender, EventArgs e)
        {
            StoreNameList.Text = "";
            InvoiceTxt.Text = checkHighestInvoiceNumber.ToString();
            BanhBiaTxt.Clear(); TotalTxt.Clear();
            SauRiengTxt.Clear();TrungThuTxt.Clear();
            ThapCamTxt.Clear(); BanhDeoText.Clear();
            BanhSenTxt.Clear(); BanhDauTxt.Clear();
            DeoThapCamTxt.Clear(); BanhInTxt.Clear();
            RedBeanTxt.Clear(); MochiTxt.Clear();
            DuaTrungTxt.Clear(); MutDauXanhTxt.Clear();
            GreenTeaMangoTxt.Clear(); BanhToLonTxt.Clear();
            BanhToNhoTxt.Clear(); NewYearCakeTxt.Clear();
            DateText.Text = DateTime.Now.ToShortDateString();
            DateText.Enabled = true;
            InvoiceTxt.Enabled = true; saveButton.Enabled = true;
            UpdateButton.Enabled = true; ExitUpdateButton.Visible = false;
            UpdateLabel.Text = "";
        }
    }
}


