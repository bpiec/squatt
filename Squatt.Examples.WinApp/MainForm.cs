using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Dabarto.Data.Squatt.Examples.Orders;
using Dabarto.Data.Squatt.Data;

namespace Dabarto.Data.Squatt.Examples.WinApp
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = true;

            Configuration.ConnectionString = "Data Source=Orders.sqlite;Version=3;";
            Configuration.ProviderName = "SQLiteSquattProvider";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            bindingSource1.DataSource = new Factory<Client>().SelectAll();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            bindingSource1.DataSource = new Factory<Article>().SelectAll();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            bindingSource1.DataSource = new Factory<Order>().SelectAll();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            bindingSource1.DataSource = new Factory<OrderArticle>().SelectAll();
        }
    }
}
