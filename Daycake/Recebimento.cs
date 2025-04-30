using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Daycake
{
    public partial class Recebimento : Form
    {
        MySqlConnection Conexao;
        private string data_source = "datasource=localhost;username=root;password=;database=daycake";
        //public int? id_filtro_selecionado = null;

        public Recebimento()
        {
            InitializeComponent();

            lstRecebimentos.View = View.Details;
            lstRecebimentos.Columns.Clear();
            lstRecebimentos.Items.Clear();

            lstRecebimentos.Columns.Add("ID Pedido", 100);
            lstRecebimentos.Columns.Add("ID Cliente", 100);
            lstRecebimentos.Columns.Add("Data do Pedido", 100);
            lstRecebimentos.Columns.Add("Data da Entrega", 100);
        }

        private void btnFiltrar_Click(object sender, EventArgs e)
        {
            
        }
    }   
}
