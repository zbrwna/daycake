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

namespace Daycake
{
    public partial class FormPedido : Form
    {
        MySqlConnection Conexao;
        private string data_source = "datasource=localhost;username=root;password=;database=daycake";
        public int? id_pedido_selecionado = null;

        public FormPedido()
        {
            InitializeComponent();

            lstListaPedidos.View = View.Details;
            lstListaPedidos.Columns.Clear();
            lstListaPedidos.Items.Clear();

            lstListaPedidos.Columns.Add("ID Pedido", 100);
            lstListaPedidos.Columns.Add("ID Cliente", 100);
            lstListaPedidos.Columns.Add("Data do Pedido", 100);
            lstListaPedidos.Columns.Add("Data da Entrega", 100);
            lstListaPedidos.Columns.Add("Tipo de Pedido", 100);
            lstListaPedidos.Columns.Add("Valor", 100);
            lstListaPedidos.Columns.Add("Descrição", 100);
            lstListaPedidos.Columns.Add("Forma de Pagamento", 100);
            lstListaPedidos.Columns.Add("Status", 100);
        }

        private void tabConsultarPedidos_Click(object sender, EventArgs e)
        {

        }

        private void btnFazerPedido_Click(object sender, EventArgs e)
        {
            try
            {
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;


                if (id_pedido_selecionado == null)
                {
                    cmd.Parameters.Clear();
                    cmd.CommandText =
                        "INSERT INTO pedido " +
                        "(clienteid, data_pedido, data_entrega, , valor, tipo_de_doce, descricao, forma_pagamento,status) " +
                        "VALUES " +
                        "(@clienteid, @data_pedido, @data_entrega, @valor, @tipoDoce, @descricao, @forma_pagamento, @status)";

                    cmd.Parameters.AddWithValue("@clienteid", mtbIdCliente.Text);
                    cmd.Parameters.AddWithValue("@data_pedido", mtbDataPedido.Text);
                    cmd.Parameters.AddWithValue("@data_entrega", mtbDataEntrega.Text);
                    cmd.Parameters.AddWithValue("@valor", txtValor.Text);
                    cmd.Parameters.AddWithValue("@tipoDoce", lvwTipoDoce.Text);
                    cmd.Parameters.AddWithValue("@descricao", txtDescricao.Text);
                    cmd.Parameters.AddWithValue("@forma_pagamento", cbxFormaPagamento.Text);
                    cmd.Parameters.AddWithValue("@status", cbxStatus.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Pedido Inserido com Sucesso", "Sucesso",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    // update
                    cmd.Parameters.Clear(); // limpa os parâmetros antigos
                    cmd.CommandText =
                        "UPDATE pedido " +
                        "SET clienteid = @clienteid, data_pedido = @data_pedido, data_entrega = @data_entrega, valor = @valor, tipo_de_doce = @tipoDoce, descricao = @descricao," +
                        " forma_pagamento = @forma_pagamento, status = @status " +
                        "WHERE idPedido = @idPedido";

                    cmd.Parameters.AddWithValue("@clienteid", mtbIdCliente.Text);
                    cmd.Parameters.AddWithValue("@data_pedido", mtbDataPedido.Text);
                    cmd.Parameters.AddWithValue("@data_entrega", mtbDataEntrega.Text);
                    cmd.Parameters.AddWithValue("@valor", txtValor.Text);
                    cmd.Parameters.AddWithValue("@tipoDoce", lvwTipoDoce.Text);
                    cmd.Parameters.AddWithValue("@descricao", txtDescricao.Text);
                    cmd.Parameters.AddWithValue("@forma_pagamento", cbxFormaPagamento.Text);
                    cmd.Parameters.AddWithValue("@status", cbxStatus.Text);
                    cmd.Parameters.AddWithValue("@id", id_pedido_selecionado);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Pedido Atualizado com Sucesso", "Sucesso",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
            }
            catch (MySqlException ex)

            {
                MessageBox.Show("Error " + "has occured: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
            catch (Exception ex)
            {
                MessageBox.Show("Has occured: " + ex.Message,
                    "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Conexao.Close();
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
                        reader.GetInt32(0).ToString(),  //id pedido
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

        private void btnExcluirPedidos_Click(object sender, EventArgs e)
        {
            excluir_pedido();
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
                        reader.GetInt32(0).ToString(), //id pedido
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

        private void zerar_Forms()
        {
            id_pedido_selecionado = null;
            mtbDataPedido.Text = null;
            mtbDataEntrega.Text = null;
            lvwTipoDoce.Text = null;
            txtValor.Text = null;
            txtDescricao.Text = null;
            cbxTipoDoce = null;
            cbxFormaPagamento.Text = null;
            cbxStatus.Text = null;
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

        private void btnAdicionar_Click(object sender, EventArgs e)
        {

        }
    }
}
