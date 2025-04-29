using MySql.Data.MySqlClient;
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

namespace Daycake
{
    public partial class CadastroConsultaClientes : Form
    {
        MySqlConnection Conexao;
        private string data_source = "datasource=localhost;username=root;password=1007;database=daycake";
        public int? id_cliente_selecionado = null;

        public CadastroConsultaClientes()
        {
            InitializeComponent();

            lstListaClientes.View = View.Details;
            lstListaClientes.Columns.Clear();
            lstListaClientes.Items.Clear();

            lstListaClientes.Columns.Add("ID", 50);
            lstListaClientes.Columns.Add("Nome", 50);
            lstListaClientes.Columns.Add("Telefone", 100);
            lstListaClientes.Columns.Add("Email", 100);
            lstListaClientes.Columns.Add("Endereço", 100);
            lstListaClientes.Columns.Add("Bairro", 30);
            lstListaClientes.Columns.Add("Número", 80);
            lstListaClientes.Columns.Add("Data Cadastro", 50);

        }

        private void btnBuscar_Click(object sender, EventArgs e)
        {
            Conexao = new MySqlConnection(data_source);
            try
            {
                Conexao.Open();

                string sql = "SELECT * FROM Cliente";

                MySqlCommand cmd = new MySqlCommand(sql, Conexao);
                MySqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    string[] row =
                    {
                        reader.GetInt32(0).ToString(), //id
                        reader.GetString(1), // data de cadastro
                        reader.GetString(2), // nome
                        reader.GetString(3), // email
                        reader.GetString(4), // endereço
                        reader.GetString(5), // numero
                        reader.GetString(6), // bairro
                        reader.GetString(7), // telefone
                       };
                    var linha_list_view = new ListViewItem(row);
                    lstListaClientes.Items.Add(linha_list_view);
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

        private void btnCadastrarCliente_Click(object sender, EventArgs e)
        {
            try
            {
                // Criar a conexão com o MySQL
                Conexao = new MySqlConnection(data_source);
                Conexao.Open();
                MySqlCommand cmd = new MySqlCommand();
                cmd.Connection = Conexao;

                // Habilitando o Update para o meu botão salvar

                if (id_cliente_selecionado == null)
                {
                    // insert
                    cmd.Parameters.Clear(); // limpa os parâmetros antigos
                    cmd.CommandText =
                        "INSERT INTO cliente " +
                        "(nome, telefone, email, endereco, bairro, numero, data_cadastro) " +
                        "VALUES " +
                        "(@nome, @telefone, @email, @endereco, @bairro, @numero, @data_cadastro)";

                    cmd.Parameters.AddWithValue("@nome", txtNomeCompleto.Text);
                    cmd.Parameters.AddWithValue("@telefone", mtxTelefone.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@endereco", txtEndereco.Text);
                    cmd.Parameters.AddWithValue("@bairro", txtBairro.Text);
                    cmd.Parameters.AddWithValue("@numero", txtNumero.Text);
                    cmd.Parameters.AddWithValue("@data_cadastro", mtbDataCadastro.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cliente Inserido com Sucesso", "Sucesso",
                                    MessageBoxButtons.OK,
                                    MessageBoxIcon.Information);
                }
                else
                {
                    // update
                    cmd.Parameters.Clear(); // limpa os parâmetros antigos
                    cmd.CommandText =
                        "UPDATE cliente " +
                        "SET nome = @nome, telefone = @telefone, email = @email, endereco = @endereco, bairro = @bairro, numero = @numero, data_cadastro = @data_cadastro " +
                        "WHERE idCliente = @idCliente";

                    cmd.Parameters.AddWithValue("@nome", txtNomeCompleto.Text);
                    cmd.Parameters.AddWithValue("@telefone", mtxTelefone.Text);
                    cmd.Parameters.AddWithValue("@email", txtEmail.Text);
                    cmd.Parameters.AddWithValue("@endereco", txtEndereco.Text);
                    cmd.Parameters.AddWithValue("@bairro", txtBairro.Text);
                    cmd.Parameters.AddWithValue("@numero", txtNumero.Text);
                    cmd.Parameters.AddWithValue("@data_cadastro", mtbDataCadastro.Text);

                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Cliente Atualizado com Sucesso", "Sucesso",
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

        private void btnExcluir_Click(object sender, EventArgs e)
        {
            excluir_cliente();
        }

        private void zerar_forms()
        {
            id_cliente_selecionado = null;
            txtNomeCompleto.Text = String.Empty;
            txtNumero.Text = String.Empty;
            txtEmail.Text = String.Empty;
            txtEndereco.Text = String.Empty;
            mtxTelefone = null;
            txtBairro.Text = String.Empty;
            mtbDataCadastro = null;
        }

        private void carregar_clientes()
        {
            try
            {
                Conexao = new MySqlConnection(data_source);

                string sql = "SELECT * FROM contato ORDER BY id ASC";

                Conexao.Open();

                MySqlCommand buscar = new MySqlCommand(sql, Conexao);

                MySqlDataReader reader = buscar.ExecuteReader();

                lstListaClientes.Items.Clear();

                while (reader.Read())
                {
                    string[] row =
                    {
                        reader.GetInt32(0).ToString(), //id
                        reader.GetString(1), // data de cadastro
                        reader.GetString(2), // nome
                        reader.GetString(3), // email
                        reader.GetString(4), // endereço
                        reader.GetString(5), // numero
                        reader.GetString(6), // bairro
                        reader.GetString(7), // telefone
                    };

                    var linha_list_view = new ListViewItem(row);
                    lstListaClientes.Items.Add(linha_list_view);
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

        private void excluir_cliente()
        {
            try
            {

                DialogResult conf = MessageBox.Show("Deseja Excluir o Registro com ?",
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
                    cmd.CommandText = "DELETE FROM contato WHERE id=@id";
                    cmd.Parameters.AddWithValue("@id", id_cliente_selecionado);

                    cmd.ExecuteNonQuery();


                    MessageBox.Show(
                            "Contato Excluido com Sucesso!",
                            "Sucesso", MessageBoxButtons.OK,
                            MessageBoxIcon.Information
                            );


                    carregar_clientes();


                    zerar_forms();
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
    }
}