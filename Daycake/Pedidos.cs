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
using MySql.Data.MySqlClient;
using System.Globalization;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;
using System.Runtime.Remoting;
using Daycake;
using MySqlX.XDevAPI;

namespace Daycake
{
    public partial class FormPedido : Form
    {
        MySqlConnection Conexao;
        string data_source = "datasource=localhost;username=root;password=;database=daycake";
        public int? id_pedido_selecionado = null;
        decimal valorTotal = 0;


        List<ClienteItem> ListaClientes = new List<ClienteItem>();
        private bool estaSelecionando = false;
        //private ComboBox cbxNomeCliente = new ComboBox();

        public FormPedido()
        {
           
            InitializeComponent();

            lstListaPedidos.View = View.Details;
            lstListaPedidos.Columns.Clear();
            lstListaPedidos.Items.Clear();

           // lstListaPedidos.Columns.Add("ID Pedido", 100);
            lstListaPedidos.Columns.Add("ID Cliente", 100);
            lstListaPedidos.Columns.Add("Data do Pedido", 100);
            lstListaPedidos.Columns.Add("Data da Entrega", 100);
            lstListaPedidos.Columns.Add("Tipo de Pedido", 100);
            lstListaPedidos.Columns.Add("Valor", 100);
            lstListaPedidos.Columns.Add("Descrição", 100);
            lstListaPedidos.Columns.Add("Forma de Pagamento", 100);
            lstListaPedidos.Columns.Add("Status", 100);

            carregar_pedido();

        }
    
        private void tabConsultarPedidos_Click(object sender, EventArgs e)
        {

        }        
        private void excluir_pedido()
        {
            try
            {

                DialogResult conf = MessageBox.Show("Deseja Excluir o Registro?",
                                                    "Certeza ?",
                                                       MessageBoxButtons.YesNo,
                                                       MessageBoxIcon.Warning);

                if (conf == DialogResult.Yes)
                {


                    Conexao = new MySqlConnection(data_source);
                    Conexao.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = Conexao;

                    cmd.Connection = Conexao;
                    cmd.CommandText = "DELETE FROM pedido WHERE idPedido=@id";
                    cmd.Parameters.AddWithValue("@id", id_pedido_selecionado);

                    cmd.ExecuteNonQuery();


                    MessageBox.Show(
                            "Contato Excluido com Sucesso!",
                            "Sucesso", MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                            );


                    carregar_pedido();


                    zerar_Forms();
                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro " + ex.Number + "Ocorreu: " + ex.Message,
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {

                MessageBox.Show("Ocorreu: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            finally
            {
                Conexao.Close();
            }
        }

        private void AtualizarValorTotal()
        {
            decimal total = 0;
            foreach (ListViewItem item in lstTipoDoce.Items)
            {
                if (item.Tag != null)
                {
                   total += (decimal)item.Tag;
                }
            }
            txtValor.Text = total.ToString("C2");
        }
        private void FormPedido_Load(object sender, EventArgs e)
        {
            cbxNomeCliente.DropDownStyle = ComboBoxStyle.DropDown;

            CarregarCliente();

            cbxNomeCliente.TextChanged += cbxNomeCliente_TextChanged;
            cbxNomeCliente.SelectedIndexChanged += cbxNomeCliente_SelectedIndexChanged;
            cbxNomeCliente.Enter += cbxNomeCliente_Enter;
            cbxNomeCliente.Leave += cbxNomeCliente_Leave;


            cbxStatus.Items.Add("Em andamento");
            cbxStatus.Items.Add("Finalizado");
            cbxStatus.Items.Add("Cancelado");
            cbxStatus.Items.Add("Entregue");

            cbxStatus.SelectedIndex = 0;

            cbxFormaPagamento.Items.Add("PIX");
            cbxFormaPagamento.Items.Add("Cartão de crédito");
            cbxFormaPagamento.Items.Add("Dinheiro");
            cbxFormaPagamento.Items.Add("Débito");

            cbxFormaPagamento.SelectedIndex = 0;

            cbxNomeCliente.DropDownStyle = ComboBoxStyle.DropDown;

            CarregarCliente();

            string connectionString = "datasource=localhost;username=root;password=;database=daycake";
            string query = "SELECT nome FROM Produto";

            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        // Adiciona o nome do produto no ComboBox
                        string nome = reader.GetString("nome");
                        cbxTipoDoce.Items.Add(nome);
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar produtos: " + ex.Message);


                }
            }
            
        }

        private void cbxTipoDoce_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (cbxTipoDoce.SelectedItem == null)
                return;

            string doceSelecionado = cbxTipoDoce.SelectedItem.ToString();
            try
            {
                using (MySqlConnection Conexao = new MySqlConnection(data_source))
                {
                    if (Conexao.State == ConnectionState.Closed)
                        Conexao.Close();

                    Conexao.Open();
                    string query = ("SELECT preco FROM Produto Where nome = @nome");

                    using (MySqlCommand cmd = new MySqlCommand(query, Conexao))
                    {
                        cmd.Parameters.AddWithValue("@nome", doceSelecionado);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                decimal preco = Convert.ToDecimal(reader["preco"]);

                                ListViewItem item = new ListViewItem(doceSelecionado);
                                item.SubItems.Add(preco.ToString("C2"));
                                item.Tag = preco;
                                lstTipoDoce.Items.Add(item);
                            }
                        }
                    }
                }
                    AtualizarValorTotal();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao buscar preço: " + ex.Message);
            }
        }

        private void CarregarCliente()
        {
            ListaClientes.Clear();
            cbxNomeCliente.Items.Clear();

            cbxNomeCliente.DropDownStyle = ComboBoxStyle.DropDown;
            cbxNomeCliente.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
            cbxNomeCliente.AutoCompleteSource = AutoCompleteSource.CustomSource;

            AutoCompleteStringCollection nomes = new AutoCompleteStringCollection();

            using (MySqlConnection conexao = new MySqlConnection("datasource = localhost; username = root; password =; database = daycake"))
            {
                try
                {

                    conexao.Open();
                    string sql = "SELECT idCliente, nome FROM Cliente";

                    using (MySqlCommand cmd = new MySqlCommand(sql, conexao))
                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            ClienteItem cliente = new ClienteItem()
                            {
                                IDCliente = Convert.ToInt32(reader["idCliente"]),
                                nomeCliente = reader["nome"].ToString()
                            };

                            ListaClientes.Add(cliente);
                            cbxNomeCliente.Items.Add(cliente);
                            nomes.Add(cliente.nomeCliente);
                        }
                    }
                    cbxNomeCliente.AutoCompleteCustomSource = nomes;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar clientes: " + ex.Message);
                }
            }
        }
        private void btnFazerPedido_Click(object sender, EventArgs e)
        {
            ClienteItem clienteSelecionado = cbxNomeCliente.SelectedItem as ClienteItem;

            if (clienteSelecionado == null)
            {
                MessageBox.Show("Cliente inválido.");
                return;
            }

            int clienteId = clienteSelecionado.IDCliente;

            try
            {

                using (MySqlConnection conexao = new MySqlConnection("datasource=localhost;username=root;password=;database=daycake"))
                {
                    conexao.Open();
                    MySqlCommand cmd = new MySqlCommand();
                    cmd.Connection = conexao;

                    if (id_pedido_selecionado == null)
                    {
                        cmd.CommandText =
                             "INSERT INTO pedido (clienteid, data_pedido, data_entrega, valor, tipo_de_doce, descricao, forma_pagamento,status)" +
                             " VALUES (@clienteid, @data_pedido, @data_entrega, @valor, @tipoDoce, @descricao, @forma_pagamento, @status)";

                        cmd.Parameters.AddWithValue("@clienteid", clienteId);
                        cmd.Parameters.AddWithValue("@data_pedido", mtbDataPedido.Text);
                        cmd.Parameters.AddWithValue("@data_entrega", mtbDataEntrega.Text);
                        cmd.Parameters.AddWithValue("@valor", decimal.Parse(txtValor.Text,
                        NumberStyles.Currency, CultureInfo.CurrentCulture));
                        cmd.Parameters.AddWithValue("@tipoDoce", lstTipoDoce.Text);
                        cmd.Parameters.AddWithValue("@descricao", txtDescricao.Text);
                        cmd.Parameters.AddWithValue("@forma_pagamento", cbxFormaPagamento.Text);
                        cmd.Parameters.AddWithValue("@status", cbxStatus.Text);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Pedido Inserido com Sucesso", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        cmd.CommandText =
                        "UPDATE pedido SET clienteid = @clienteid, data_pedido = @data_pedido, data_entrega = @data_entrega, valor = *valor " +
                        "tipo_de_doce = @tipoDoce, descricao = @descricao, forma_pagamento = @forma_pagamento, status = @status " +
                        "WHERE idPedido = @idPedido";

                        cmd.Parameters.AddWithValue("@clienteid", clienteId);
                        cmd.Parameters.AddWithValue("@data_pedido", mtbDataPedido.Text);
                        cmd.Parameters.AddWithValue("@data_entrega", mtbDataEntrega.Text);
                        cmd.Parameters.AddWithValue("@valor", decimal.Parse(txtValor.Text, NumberStyles.Currency, CultureInfo.CurrentCulture));
                        cmd.Parameters.AddWithValue("@tipoDoce", lstTipoDoce.Text);
                        cmd.Parameters.AddWithValue("@descricao", txtDescricao.Text);
                        cmd.Parameters.AddWithValue("@forma_pagamento", cbxFormaPagamento.Text);
                        cmd.Parameters.AddWithValue("@status", cbxStatus.Text);
                        cmd.Parameters.AddWithValue("@idPedido", id_pedido_selecionado);

                        cmd.ExecuteNonQuery();
                        MessageBox.Show("Pedido Atualizado com Sucesso", "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }

                }
            }
            catch (MySqlException ex)
            {
                MessageBox.Show("Erro ao cadastrar pedido: " + ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro inesperado: " + ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                if (Conexao != null && Conexao.State == ConnectionState.Open)
                {
                    Conexao.Close();
                }
            }
        }


        private void btnBuscarPedidos_Click(object sender, EventArgs e)
        {
            try
            {
                Conexao.Open();

                string sql = "SELECT * FROM Pedido";

                MySqlCommand cmd = new MySqlCommand(sql, Conexao);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string[] row =
                    {
                       // reader.GetInt32(0).ToString(),  //id pedido
                        reader.GetInt32(1).ToString(),  // id cliente
                        reader.GetString(2),  // data pedido
                        reader.GetString(3),  // data entrega
                        reader.GetString(4),  // valor
                        reader.GetString(5),  // tipo doce
                        reader.GetString(6),  // observação
                        reader.GetString(7),  // forma pagamento
                       };
                    var linha_list_view = new ListViewItem(row);
                    lstListaPedidos.Items.Add(linha_list_view);
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Conexao.Close();
            }
        }
        private void carregar_pedido()
        {
            try
            {
                Conexao = new MySqlConnection(data_source);

                string sql = "SELECT * FROM pedido ORDER BY idPedido ASC";

                Conexao.Open();

                MySqlCommand buscar = new MySqlCommand(sql, Conexao);

                MySqlDataReader reader = buscar.ExecuteReader();

                lstListaPedidos.Items.Clear();

                while (reader.Read())
                {
                    string[] row =
                    {
                        //reader.GetInt32(0).ToString(), //id pedido
                        reader.GetInt32(1).ToString(), // id cliente
                        reader.GetString(2), // data pedido
                        reader.GetString(3), // data entrega
                        reader.GetString(4), // valor
                        reader.GetString(5), // tipo doce
                        reader.GetString(6), // observação
                        reader.GetString(7), // forma pagamento
                    };

                    var linha_list_view = new ListViewItem(row);
                    lstListaPedidos.Items.Add(linha_list_view);
                }


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Conexao.Close();
            }
        }

        private void btnExcluirPedidos_Click(object sender, EventArgs e)
        {
            excluir_pedido();
        }


        private void zerar_Forms()
        {
            id_pedido_selecionado = null;
            mtbDataPedido.Text = null;
            mtbDataEntrega.Text = null;
            lstTipoDoce.Text = null;
            txtValor.Text = null;
            txtDescricao.Text = null;
            cbxTipoDoce = null;
            cbxFormaPagamento.Text = null;
            cbxStatus.Text = null;
        }

        private void lstTipoDoce_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete && lstTipoDoce.SelectedItems.Count > 0)

            {
                lstTipoDoce.Items.Remove(lstTipoDoce.SelectedItems[0]);
            }
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void cbxNomeCliente_TextChanged(object sender, EventArgs e)
        {
            if (estaSelecionando)
                return;

            string textoDigitado = cbxNomeCliente.Text.ToLower();

            if (!cbxNomeCliente.Focused)

                return;

            cbxNomeCliente.BeginUpdate();
            cbxNomeCliente.Items.Clear();

            foreach (ClienteItem cliente in ListaClientes)
            {
                if (cliente.nomeCliente.ToLower().Contains(textoDigitado))
                {
                    cbxNomeCliente.Items.Add(cliente);
                }
            }

            cbxNomeCliente.EndUpdate();
            if (cbxNomeCliente.Items.Count > 0)
            {
                cbxNomeCliente.DroppedDown = false;
            }
            cbxNomeCliente.SelectionStart = textoDigitado.Length;
            cbxNomeCliente.SelectionLength = 0;

            this.Cursor = Cursors.Default;
        }

        private void cbxNomeCliente_Enter(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void cbxNomeCliente_Leave(object sender, EventArgs e)
        {
            this.Cursor = Cursors.Default;
        }

        private void cbxNomeCliente_SelectedIndexChanged(object sender, EventArgs e)
        {
            estaSelecionando = true;
            ClienteItem clienteSelecionado = cbxNomeCliente.SelectedItem as ClienteItem;
            estaSelecionando = false;
        }
    }
}


