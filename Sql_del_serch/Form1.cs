using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;

namespace Sql_del_serch
{
    public partial class Form1 : Form
    {
        string connection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=PhoneDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        int PhoneBookId = 0;
        public Form1()
        {
            InitializeComponent();
        }

        private void textBox5_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (txtname.Text.Trim() != "" && txtsrname.Text.Trim() != "" && txtcontact.Text.Trim() != "")
            {
                Regex reg = new Regex(@"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,17}))$");
                Match math = reg.Match(txtemail.Text.Trim());
                if (math.Success)
                {

                    using (SqlConnection sqlconn = new SqlConnection(connection))
                    {
                        sqlconn.Open();
                        SqlCommand sqlCmd = new SqlCommand("ContactAddOrEdit", sqlconn);
                        sqlCmd.CommandType = CommandType.StoredProcedure;
                        sqlCmd.Parameters.AddWithValue("@PhoneBookID", PhoneBookId);
                        sqlCmd.Parameters.AddWithValue("@FirstName", txtname.Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@LastName", txtsrname.Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@Contact", txtcontact.Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@Email", txtemail.Text.Trim());
                        sqlCmd.Parameters.AddWithValue("@Address", txtaddress.Text.Trim());
                        sqlCmd.ExecuteNonQuery();
                        MessageBox.Show("submitted successfully");
                        Clear();
                        GridFill();

                    }
                }
                else
                    MessageBox.Show("Email is not valid");
            }
            else
                MessageBox.Show("fill all ");

        }
         void Clear()
        {
            txtname.Text = txtsrname.Text = txtcontact.Text = txtemail.Text = txtaddress.Text =txtsearch.Text= "";
            PhoneBookId = 0;
            btnSave.Text = "Save";
            btnDell.Enabled = false;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Clear();
        }

        void GridFill()
        {
            using (SqlConnection sqlConn = new SqlConnection(connection))
            {
                sqlConn.Open();
                SqlDataAdapter adapter = new SqlDataAdapter("ContactViewAll",sqlConn);
                adapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                DataTable dataTable = new DataTable();
                adapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;


            }
          
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            GridFill();
            btnDell.Enabled = false;
        }

        private void dataGridView1_DoubleClick(object sender, EventArgs e)
        {
            if(dataGridView1.CurrentRow.Index!=-1)
            {
                txtname.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
                txtsrname.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
                txtcontact.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
                txtemail.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
                txtaddress.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
                PhoneBookId = Convert.ToInt32(dataGridView1.CurrentRow.Cells[0].Value.ToString());
                btnSave.Text = "Update";
                btnDell.Enabled = true;
            }
        }

        private void btnDell_Click(object sender, EventArgs e)
        {
            using (SqlConnection conn = new SqlConnection(connection))
            {
                conn.Open();
                SqlCommand sqlCommand = new SqlCommand("DelleteById",conn);
                sqlCommand.CommandType = CommandType.StoredProcedure;
                sqlCommand.Parameters.AddWithValue("@PhoneBookID", PhoneBookId);
                sqlCommand.ExecuteNonQuery();
                MessageBox.Show("Dellete succsessfully");
                Clear();
                GridFill();
            }

        }

        private void txtsearch_TextChanged(object sender, EventArgs e)
        {
            using (SqlConnection sqlConnection = new SqlConnection(connection))
            {

                sqlConnection.Open();
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter("ContactSearchVale",sqlConnection);
                sqlDataAdapter.SelectCommand.CommandType = CommandType.StoredProcedure;
                sqlDataAdapter.SelectCommand.Parameters.AddWithValue("@SearchValue",txtsearch.Text.Trim());
                DataTable dataTable = new DataTable();
                sqlDataAdapter.Fill(dataTable);
                dataGridView1.DataSource = dataTable;

            }

        }
    }
}
