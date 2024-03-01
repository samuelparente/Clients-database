using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ListView;

namespace clientsbd
{
    public partial class mainForm : Form
    {
        public mainForm()
        {
            InitializeComponent();
        }

        private SQLiteConnection con;

        //Loading form
        private void mainForm_Load(object sender, EventArgs e)
        {
            //Connect to db
            con = ConnectDb(); 

            //Check connection
            if(con != null)
            {
                CountRecords(con);
                LoadDataIntoDataGridView(dataViewBlock,con);
                // Attach the CellClick event handler
                dataViewBlock.CellClick += DataGridView1_SelectionChanged;
            }
            else
            {
                MessageBox.Show("Cannoct connect to database.");
            }
        }

        //connect to sqlite
        private SQLiteConnection ConnectDb()
        {
            try
            {
                //Create object
                SQLiteConnection sqliteConnection;

                // Create a db if it doesn't exist
                sqliteConnection = new SQLiteConnection("Data Source=./clients.sqlite;Version=3;");
                if (!System.IO.File.Exists("clients.sqlite"))
                {
                    SQLiteConnection.CreateFile("clients.sqlite");
                }
                sqliteConnection.Open();

                // Create a table if it doesn't exist
                string createTableQuery = "CREATE TABLE IF NOT EXISTS clients_table (ID INTEGER PRIMARY KEY AUTOINCREMENT, [First Name] TEXT, [Last Name] TEXT, Email TEXT, [Phone number] TEXT, Country TEXT)";
                SQLiteCommand createCommand = new SQLiteCommand(createTableQuery, sqliteConnection);
                createCommand.ExecuteNonQuery();

                //Update statusbar
                conStatus.Text = "Connected to database.";
                conStatus.ForeColor = Color.Green;

                //Return connection
                return sqliteConnection;
            }
            catch (Exception ex)
            {
                // Handle the exception
                conStatus.Text = "Error connecting to the database: " + ex.Message;
                conStatus.ForeColor = Color.Red;
                return null;
            }
        }

        public void CountRecords(SQLiteConnection sqliteConnection)
        {
            try
            {
                string countQuery = "SELECT COUNT(*) FROM clients_table";
                SQLiteCommand command = new SQLiteCommand(countQuery, sqliteConnection);

                int count = Convert.ToInt32(command.ExecuteScalar());

                // Update the label
                recordsCount.Text = "Records: " + count.ToString();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        //Add client to database
        private void createClient(SQLiteConnection sqliteConnection, DataGridView dataViewBlock)
        {
            string firstName = this.firstName.Text;
            string lastName = this.lastName.Text;
            string phoneNumber = this.phoneNumber.Text;
            string email = this.email.Text;
            string country = this.country.Text;

            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(country) && !string.IsNullOrWhiteSpace(phoneNumber))
            {
               
                string insertQuery = "INSERT INTO clients_table ([First Name], [Last Name], Email, [Phone number], Country) VALUES (@firstName, @lastName, @email, @phoneNumber, @country)";
                SQLiteCommand insertCommand = new SQLiteCommand(insertQuery, sqliteConnection);
               
                insertCommand.Parameters.AddWithValue("@firstName", firstName);
                insertCommand.Parameters.AddWithValue("@lastName", lastName);
                insertCommand.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                insertCommand.Parameters.AddWithValue("@email", email);
                insertCommand.Parameters.AddWithValue("@country", country);

                try
                {
                    insertCommand.ExecuteNonQuery();
                    MessageBox.Show("Client added successfully.");
                    CountRecords(sqliteConnection);
                    LoadDataIntoDataGridView(dataViewBlock, sqliteConnection);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding client: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please enter all fields.");
            }

        }

        //Update selected client
        private void updateClient(SQLiteConnection sqliteConnection, DataGridView dataViewBlock)
        {
            string firstName = this.firstName.Text;
            string lastName = this.lastName.Text;
            string phoneNumber = this.phoneNumber.Text;
            string email = this.email.Text;
            string country = this.country.Text;
            DataGridViewRow selectedRow = dataViewBlock.SelectedRows[0];
            string clientId = selectedRow.Cells["ID"].Value.ToString();


            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(country) && !string.IsNullOrWhiteSpace(phoneNumber))
            {

                string updateQuery = "UPDATE clients_table SET [First Name] = @firstName, [Last Name] = @lastName, " +
                                 "[Phone number] = @phoneNumber, [Email] = @email, [Country] = @country " +
                                 "WHERE ID = @clientId";
                SQLiteCommand updateCommand = new SQLiteCommand(updateQuery, sqliteConnection);

                updateCommand.Parameters.AddWithValue("@firstName", firstName);
                updateCommand.Parameters.AddWithValue("@lastName", lastName);
                updateCommand.Parameters.AddWithValue("@phoneNumber", phoneNumber);
                updateCommand.Parameters.AddWithValue("@email", email);
                updateCommand.Parameters.AddWithValue("@country", country);
                updateCommand.Parameters.AddWithValue("@clientId", clientId);

                try
                {
                    updateCommand.ExecuteNonQuery();
                    MessageBox.Show("Client updated successfully.");
                    CountRecords(sqliteConnection);
                    LoadDataIntoDataGridView(dataViewBlock, sqliteConnection);
                    ClearTextBoxes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating client: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Please select a client from the list.");
            }

        }

        //Delete selected client
        private void deleteClient(SQLiteConnection sqliteConnection, DataGridView dataViewBlock)
        {
            string firstName = this.firstName.Text;
            string lastName = this.lastName.Text;
            string phoneNumber = this.phoneNumber.Text;
            string email = this.email.Text;
            string country = this.country.Text;
            DataGridViewRow selectedRow = dataViewBlock.SelectedRows[0];
            string clientId = selectedRow.Cells["ID"].Value.ToString();


            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName) && !string.IsNullOrWhiteSpace(email) && !string.IsNullOrWhiteSpace(country) && !string.IsNullOrWhiteSpace(phoneNumber))
            {

                string deleteQuery = "DELETE FROM clients_table WHERE ID = @clientId";
                SQLiteCommand deleteCommand = new SQLiteCommand(deleteQuery, sqliteConnection);

                deleteCommand.Parameters.AddWithValue("@clientId", clientId);

                try
                {
                    deleteCommand.ExecuteNonQuery();
                    MessageBox.Show("Client deleted successfully.");
                    CountRecords(sqliteConnection);
                    LoadDataIntoDataGridView(dataViewBlock, sqliteConnection);
                    ClearTextBoxes();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error deleting client: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("To delete a record, no fields can be empty.");
            }

        }

        //Clear all fields
        private void ClearTextBoxes()
        {
            firstName.Clear();
            lastName.Clear();
            phoneNumber.Clear();
            email.Clear();
            country.Clear();
        }

        //Show the records
        private void LoadDataIntoDataGridView(DataGridView dataGridView, SQLiteConnection sqliteConnection)
        {
            try
            {
                string selectQuery = "SELECT * FROM clients_table";
                SQLiteDataAdapter dataAdapter = new SQLiteDataAdapter(selectQuery, sqliteConnection);
                DataSet dataSet = new DataSet();
                dataAdapter.Fill(dataSet, "clients_table");
                dataGridView.DataSource = dataSet.Tables["clients_table"];
                // Set the SelectionMode to FullRowSelect
                dataGridView.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            }
            catch (Exception ex)
            {
                generalMessage.Text = ("Error loading records: " + ex.Message);
            }
        }

        //Populate the info abou the client when row clicked
        private void DataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            // Check if there is a selected row
            if (dataViewBlock.SelectedRows.Count > 0)
            {
                // Get the values of the selected row
                string firstNameText = dataViewBlock.SelectedRows[0].Cells["First Name"].Value.ToString();
                string lastNameText = dataViewBlock.SelectedRows[0].Cells["Last Name"].Value.ToString();
                string phoneNumberText = dataViewBlock.SelectedRows[0].Cells["Phone number"].Value.ToString();
                string emailText = dataViewBlock.SelectedRows[0].Cells["Email"].Value.ToString();
                string countryText = dataViewBlock.SelectedRows[0].Cells["Country"].Value.ToString();

                // Populate the TextBoxes with the selected values
                firstName.Text = firstNameText;
                lastName.Text = lastNameText;
                phoneNumber.Text = phoneNumberText;
                email.Text = emailText;
                country.Text = countryText;
            }
        }

        #region Button actions
        //Button click for new client
        private void insertBtn_Click(object sender, EventArgs e)
        {
            createClient(con, dataViewBlock);
        }
        //Button click for update client
        private void updateBtn_Click(object sender, EventArgs e)
        {
            updateClient(con, dataViewBlock);
        }
        //Button click for dele client
        private void deleteBtn_Click(object sender, EventArgs e)
        {
            // Check if a row is selected in the DataGridView
            if (dataViewBlock.SelectedRows.Count > 0)
            {
                // Confirm with the user before deleting
                DialogResult result = MessageBox.Show("Are you sure you want to delete this client?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    // Call method to delete the client from the database
                    deleteClient(con, dataViewBlock);
                }
            }
            else
            {
                MessageBox.Show("Please select a row to delete.");
            }

        }
        #endregion

    }
}
