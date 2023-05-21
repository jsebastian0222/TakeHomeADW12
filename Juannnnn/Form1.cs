using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Juannnnn
{
    public partial class Form1 : Form
    {
        MySqlConnection sqlConnection;
        MySqlCommand sqlCommand;
        MySqlDataAdapter sqlDataAdapter;
        MySqlDataReader sqlDataReader;
        string connection = "server=localhost;uid=root;pwd=123;database=premier_league";
        string query = "";

        DataTable DtNation = new DataTable();
        DataTable DtPlayer = new DataTable();
        DataTable DtTeamName = new DataTable();
        DataTable DtEditManager = new DataTable();
        DataTable DtRemovePlayer = new DataTable();
        DataTable DtManager2 = new DataTable();
        DataTable DtManager = new DataTable();
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            query = "SELECT nation, nationality_id FROM nationality;";
            sqlConnection = new MySqlConnection(connection);
            sqlCommand = new MySqlCommand(query, sqlConnection);
            sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(DtNation);
            comboBoxNationally.DataSource = DtNation;
            comboBoxNationally.DisplayMember = "nation";
            comboBoxNationally.ValueMember = "nationality_id";

            query = "SELECT p.player_name, p.team_number, n.nation, p.playing_pos, p.height, p.weight, p.birthdate\r\nFROM player p, nationality n\r\nWHERE p.nationality_id=n.nationality_id && p.status=1;";
            sqlConnection = new MySqlConnection(connection);
            sqlCommand = new MySqlCommand(query, sqlConnection);
            sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(DtPlayer);
            dataGridView2.DataSource = DtPlayer;
            dataGridView2.ClearSelection();

            query = $"SELECT m.manager_id, m.manager_name, n.nation, m.birthdate FROM manager m LEFT JOIN nationality n ON n.nationality_id = m.nationality_id WHERE m.working = 0;";
            sqlConnection = new MySqlConnection(connection);
            sqlCommand = new MySqlCommand(query, sqlConnection);
            sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(DtManager);
            dataGridView1.DataSource = DtManager;
            dataGridView1.ClearSelection();


            query = "SELECT team_name, team_id FROM team;";
            sqlConnection = new MySqlConnection(connection);
            sqlCommand = new MySqlCommand(query, sqlConnection);
            sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(DtTeamName);
            sqlDataAdapter.Fill(DtEditManager);
            sqlDataAdapter.Fill(DtRemovePlayer);

            comboBoxTeamName.DataSource = DtTeamName;
            comboBoxEditManager.DataSource = DtEditManager;
            comboBoxRemovePlayer.DataSource = DtRemovePlayer;

            comboBoxTeamName.DisplayMember = "team_name";
            comboBoxEditManager.DisplayMember = "team_name";
            comboBoxRemovePlayer.DisplayMember = "team_name";

            comboBoxTeamName.ValueMember = "team_id";
            comboBoxEditManager.ValueMember = "team_id";
            comboBoxRemovePlayer.ValueMember = "team_id";

            comboBoxRemovePlayer.SelectedIndexChanged += comboBoxRemovePlayer_SelectedIndexChanged;
            comboBoxEditManager.SelectedIndexChanged += comboBoxEditManager_SelectedIndexChanged;
        }

        private void buttonAdd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(textBoxID.Text) ||
            string.IsNullOrWhiteSpace(textBoxPlayerName.Text) ||
            string.IsNullOrWhiteSpace(textBoxTeamNumber.Text) ||
            string.IsNullOrWhiteSpace(comboBoxNationally.Text) ||
            string.IsNullOrWhiteSpace(textBoxPos.Text) ||
            string.IsNullOrWhiteSpace(textBoxHeight.Text) ||
            string.IsNullOrWhiteSpace(textBoxWeight.Text) ||
            string.IsNullOrEmpty(dateTimePicker1.Text) ||
            string.IsNullOrWhiteSpace(comboBoxTeamName.Text))
            {
                MessageBox.Show("nulis yg bener", "error");
                return;
            }

            string id = textBoxID.Text;
            string name = textBoxPlayerName.Text;
            string num = textBoxTeamNumber.Text;
            string nation = comboBoxNationally.SelectedValue.ToString();
            string pos = textBoxPos.Text;
            string height = textBoxHeight.Text;
            string weight = textBoxWeight.Text;
            string date = dateTimePicker1.Value.ToString("yyyy-MM-dd");
            string team = comboBoxTeamName.SelectedValue.ToString();

            query = $"INSERT INTO player values ('{id}', '{num}', '{name}', '{nation}', '{pos}', '{height}', '{weight}', '{date}', '{team}', '1', '0');";
            executeSQL(query);

            textBoxPlayerName.Text = "";
            textBoxID.Text = "";
            textBoxWeight.Text = "";
            textBoxHeight.Text = "";
            textBoxPos.Text = "";
            textBoxTeamNumber.Text = "";
            dataGridView2.ClearSelection();

        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentRow != null && comboBoxEditManager.SelectedItem != null && dataGridView3.CurrentRow != null)
            {
                string teamId = dataGridView1.CurrentRow.Cells[0].Value.ToString();
                string managerId = dataGridView3.CurrentRow.Cells[0].Value.ToString();

                query = $"UPDATE team SET manager_id = '{teamId}' WHERE team_id = '{comboBoxEditManager.SelectedValue}';";
                executeSQL3(query);

                query = $"UPDATE manager SET working = 0 WHERE manager_id = '{managerId}';";
                executeSQL3(query);

                query = $"UPDATE manager SET working = 1 WHERE manager_id = '{teamId}';";
                executeSQL3(query);

                updatedatagridview3();
                dataGridView1.ClearSelection();
            }
            else
            {
                MessageBox.Show("pilih manager dan tim", "error");
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            int rows = DtPlayer.Rows.Count;
            if (comboBoxRemovePlayer.SelectedItem != null && rows >= 11)
            {
                if (dataGridView2.SelectedRows.Count != 0)
                {
                    string name = dataGridView2.SelectedRows[0].Cells["player_name"].Value.ToString();
                    query = $"UPDATE player SET status = 0 WHERE player_name = '{name}';";
                    executeSQL2(query);
                    dataGridView2.ClearSelection();
                    comboBoxRemovePlayer.SelectedIndexChanged -= comboBoxRemovePlayer_SelectedIndexChanged;
                    comboBoxRemovePlayer.SelectedItem = null;
                    comboBoxRemovePlayer.SelectedIndexChanged += comboBoxRemovePlayer_SelectedIndexChanged;
                    MessageBox.Show("Player udah di hapus");
                }
                else
                {
                    MessageBox.Show("pilih pemain", "error");
                }
            }
            else if (comboBoxRemovePlayer.SelectedItem == null)
            {
                MessageBox.Show("pilih tim", "error");
            }
            else
            {
                MessageBox.Show("pemain kurang dari 11!", "error");
            }

        }

        private void executeSQL(string query)
        {
            try
            {
                sqlConnection.Open();
                sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlDataReader = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
                updatedatagridview2();
            }
        }

        private void executeSQL2(string query)
        {
            try
            {
                sqlConnection.Open();
                sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlDataReader = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
                updatedatagridview1();
            }
        }

        private void executeSQL3(string query)
        {
            try
            {
                sqlConnection.Open();
                sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlDataReader = sqlCommand.ExecuteReader();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                sqlConnection.Close();
            }
        }

        private void updatedatagridview1()
        {
            DtPlayer.Clear();
            try
            {
                query = $"SELECT p.player_name, p.team_number, n.nation, p.playing_pos, p.height, p.weight, p.birthdate, t.team_name\r\nFROM player p join nationality n on p.nationality_id=n.nationality_id\r\nJOIN team t on t.team_id=p.team_id\r\nWHERE p.status=1;";
                sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(DtPlayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updatedatagridview2()
        {
            DtPlayer.Clear();
            try
            {
                query = "SELECT p.player_name, p.team_number, n.nation, p.playing_pos, p.height, p.weight, p.birthdate\r\nFROM player p, nationality n\r\nWHERE p.nationality_id=n.nationality_id && p.status=1;";
                sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(DtPlayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void updatedatagridview3()
        {
            try
            {
                DtManager2.Clear();
                string team = comboBoxEditManager.SelectedValue.ToString();
                query = $"SELECT m.manager_id, m.manager_name, t.team_name, m.birthdate, n.nation\r\nFROM manager m, team t, nationality n\r\nWHERE t.manager_id=m.manager_id && n.nationality_id=m.nationality_id && t.team_id = '{team}' && t.team_name is not null;";
                sqlConnection = new MySqlConnection(connection);
                sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(DtManager2);
                dataGridView1.DataSource = DtManager2;
                dataGridView1.ClearSelection();

                DtManager.Clear();
                query = $"SELECT m.manager_id, m.manager_name, n.nation, m.birthdate FROM manager m LEFT JOIN nationality n ON n.nationality_id = m.nationality_id WHERE m.working = 0;";
                sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(DtManager);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void comboBoxEditManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            string team = comboBoxEditManager.SelectedValue.ToString();
            DtManager2.Clear();
            query = $"SELECT m.manager_id, m.manager_name, t.team_name, m.birthdate, n.nation\r\nFROM manager m, team t, nationality n\r\nWHERE t.manager_id=m.manager_id && n.nationality_id=m.nationality_id && t.team_id = '{team}' && t.team_name is not null;";
            sqlConnection = new MySqlConnection(connection);
            sqlCommand = new MySqlCommand(query, sqlConnection);
            sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
            sqlDataAdapter.Fill(DtManager2);
            dataGridView3.DataSource = DtManager2;
            dataGridView3.ClearSelection();
        }

        private void comboBoxRemovePlayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            DtPlayer.Clear();
            if (comboBoxRemovePlayer.SelectedItem != null)
            {
                string team = comboBoxRemovePlayer.SelectedValue.ToString();
                query = $"SELECT p.player_name, p.team_number, n.nation, p.playing_pos, p.height, p.weight, p.birthdate, t.team_name\r\nFROM player p join nationality n on p.nationality_id=n.nationality_id\r\nJOIN team t on t.team_id=p.team_id\r\nWHERE p.status=1 && t.team_id = '{team}';";
                sqlConnection = new MySqlConnection(connection);
                sqlCommand = new MySqlCommand(query, sqlConnection);
                sqlDataAdapter = new MySqlDataAdapter(sqlCommand);
                sqlDataAdapter.Fill(DtPlayer);
                dataGridView2.DataSource = DtPlayer;
                dataGridView2.ClearSelection();
            }
        }

       
    }
}
