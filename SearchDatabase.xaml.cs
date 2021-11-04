using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;

namespace UdlaansSystem
{
    /// <summary>
    /// Interaction logic for SearchDatabase.xaml
    /// </summary>
    public partial class SearchDatabase : Page
    {
        static SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["RUS_UdlaanSystem"].ConnectionString);

        public SearchDatabase()
        {
            InitializeComponent();
        }

        #region CLICK BUTTON SEARCHES

        private void BtnShowLoaners_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string title;
                LoanerColumns();

                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM Loaner";
                cmd.ExecuteNonQuery();

                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(dataTable);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (dataRow["isStudent"].ToString() == "True")
                    {
                        title = "Elev";
                    }
                    else
                    {
                        title = "Lærer";
                    }

                    DataGridView.Items.Add(new { Column1 = dataRow["login"].ToString(), Column2 = dataRow["name"].ToString(), Column3 = dataRow["phone"].ToString(), Column4 = title });
                }

                conn.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Applikationen kunne ikke forbinde til serveren.");
            }
        }

        private void BtnShowPCs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PCColumns();

                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM PC";
                cmd.ExecuteNonQuery();

                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(dataTable);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    DataGridView.Items.Add(new { Column1 = dataRow["qrId"].ToString(), Column2 = dataRow["model"].ToString(), Column3 = dataRow["serial"].ToString() });
                }

                conn.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Applikationen kunne ikke forbinde til serveren.");
            }
        }

        private void BtnShowLoans_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoanColumns();

                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM ((Loan INNER JOIN Loaner ON Loan.uniLogin = Loaner.login) INNER JOIN PC ON Loan.qrId = PC.qrId)";
                cmd.ExecuteNonQuery();

                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(dataTable);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    DataGridView.Items.Add(new { Column1 = dataRow["loanId"].ToString(), Column2 = dataRow["model"].ToString(), Column3 = dataRow["qrId"].ToString(), Column4 = dataRow["startDate"].ToString().Remove(dataRow["startDate"].ToString().Length - 8), Column5 = dataRow["endDate"].ToString().Remove(dataRow["endDate"].ToString().Length - 8), Column6 = dataRow["uniLogin"].ToString(), Column7 = dataRow["name"].ToString(), Column8 = dataRow["phone"].ToString() });
                }

                conn.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Applikationen kunne ikke forbinde til serveren.");
            }
        }

        private void BtnShowExpired_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                LoanColumns();

                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM ((Loan INNER JOIN Loaner ON Loan.uniLogin = Loaner.login) INNER JOIN PC ON Loan.qrId = PC.qrId)";
                cmd.ExecuteNonQuery();

                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(dataTable);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    DateTime date = Convert.ToDateTime(dataRow["endDate"]);
                    if (date <= DateTime.Now)
                    {
                        DataGridView.Items.Add(new { Column1 = dataRow["loanId"].ToString(), Column2 = dataRow["model"].ToString(), Column3 = dataRow["qrId"].ToString(), Column4 = dataRow["startDate"].ToString().Remove(dataRow["startDate"].ToString().Length - 8), Column5 = dataRow["endDate"].ToString().Remove(dataRow["endDate"].ToString().Length - 8), Column6 = dataRow["uniLogin"].ToString(), Column7 = dataRow["name"].ToString(), Column8 = dataRow["phone"].ToString() });
                    }
                }
                conn.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Applikationen kunne ikke forbinde til serveren.");
            }
        }

        private void BtnShowAvailablePCs_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PCColumns();

                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM PC WHERE NOT EXISTS (SELECT * FROM Loan WHERE qrId = PC.qrId)";
                cmd.ExecuteNonQuery();

                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(dataTable);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    DataGridView.Items.Add(new { Column1 = dataRow["qrId"].ToString(), Column2 = dataRow["model"].ToString(), Column3 = dataRow["serial"].ToString() });
                }

                conn.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Applikationen kunne ikke forbinde til serveren.");
            }
        }

        #endregion

        #region USER INPUT SEARCHES

        private void BtnSearchButton_Click(object sender, RoutedEventArgs e)
        {
            UserInputSearch();
        }

        private void BtnSearchInput_KeyUp(object sender, KeyEventArgs e)
        {

            UserInputSearch();
        }

        public void UserInputSearch()
        {
            try
            {
                string input = BtnSearchInput.Text.ToLower();
                LoanColumns();

                conn.Open();
                SqlCommand cmd = conn.CreateCommand();

                cmd.CommandType = CommandType.Text;
                cmd.CommandText = @"SELECT * FROM ((Loan INNER JOIN Loaner ON Loan.uniLogin = Loaner.login) INNER JOIN PC ON Loan.qrId = PC.qrId)";
                cmd.ExecuteNonQuery();

                DataTable dataTable = new DataTable();
                SqlDataAdapter dataAdapter = new SqlDataAdapter(cmd);

                dataAdapter.Fill(dataTable);
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (dataRow["loanId"].ToString().ToLower().Contains(input) || dataRow["model"].ToString().ToLower().Contains(input) || dataRow["qrId"].ToString().ToLower().Contains(input) || dataRow["uniLogin"].ToString().ToLower().Contains(input) || dataRow["name"].ToString().ToLower().Contains(input) || dataRow["phone"].ToString().ToLower().Contains(input))
                    {
                        DataGridView.Items.Add(new { Column1 = dataRow["loanId"].ToString(), Column2 = dataRow["model"].ToString(), Column3 = dataRow["qrId"].ToString(), Column4 = dataRow["startDate"].ToString().Remove(dataRow["startDate"].ToString().Length - 8), Column5 = dataRow["endDate"].ToString().Remove(dataRow["endDate"].ToString().Length - 8), Column6 = dataRow["uniLogin"].ToString(), Column7 = dataRow["name"].ToString(), Column8 = dataRow["phone"].ToString() });
                    }
                }

                conn.Close();
            }
            catch (Exception)
            {
                MessageBox.Show("Applikationen kunne ikke forbinde til serveren.");
            }
        }

        #endregion

        #region COLUMNS

        private void LoanerColumns()
        {
            DataGridView.Items.Clear();

            ((GridView)DataGridView.View).Columns[0].Header = "UNI Login :";
            ((GridView)DataGridView.View).Columns[1].Header = "Navn :";
            ((GridView)DataGridView.View).Columns[2].Header = "Telefon :";
            ((GridView)DataGridView.View).Columns[3].Header = "Titel :";
            ((GridView)DataGridView.View).Columns[4].Header = "";
            ((GridView)DataGridView.View).Columns[5].Header = "";
            ((GridView)DataGridView.View).Columns[6].Header = "";
            ((GridView)DataGridView.View).Columns[7].Header = "";
        }

        private void PCColumns()
        {
            DataGridView.Items.Clear();

            ((GridView)DataGridView.View).Columns[0].Header = "QR ID :";
            ((GridView)DataGridView.View).Columns[1].Header = "Model :";
            ((GridView)DataGridView.View).Columns[2].Header = "Løbenummer :";
            ((GridView)DataGridView.View).Columns[3].Header = "";
            ((GridView)DataGridView.View).Columns[4].Header = "";
            ((GridView)DataGridView.View).Columns[5].Header = "";
            ((GridView)DataGridView.View).Columns[6].Header = "";
            ((GridView)DataGridView.View).Columns[7].Header = "";
        }

        private void LoanColumns()
        {
            DataGridView.Items.Clear();

            ((GridView)DataGridView.View).Columns[0].Header = "Lån ID :";
            ((GridView)DataGridView.View).Columns[1].Header = "PC Model :";
            ((GridView)DataGridView.View).Columns[2].Header = "QR ID :";
            ((GridView)DataGridView.View).Columns[3].Header = "Start Dato :";
            ((GridView)DataGridView.View).Columns[4].Header = "Slut Dato :";
            ((GridView)DataGridView.View).Columns[5].Header = "UNI Login :";
            ((GridView)DataGridView.View).Columns[6].Header = "Låner Navn :";
            ((GridView)DataGridView.View).Columns[7].Header = "Telefon :";
        }

        #endregion
    }
}
