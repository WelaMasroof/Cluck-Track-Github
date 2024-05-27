using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Poultry_Management_System.Login;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Poultry_Management_System
{
    public partial class Show_Expense : Form
    {
        private string ConnectionString = Login.ConnectionString;
        public Show_Expense()
        {
            InitializeComponent();
            LoadDataIntoComboBox();
            expenserecord.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        }
        private void LoadDataIntoComboBox()
        {
            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    // Query to select distinct categories from the Expense table
                    string expenseQuery = "SELECT distinct category FROM Expense";
                    SqlCommand expenseCommand = new SqlCommand(expenseQuery, connection);
                    SqlDataReader expenseReader = expenseCommand.ExecuteReader();

                    // Add categories from Expense table to the combobox
                    while (expenseReader.Read())
                    {
                        expensecombobox.Items.Add(expenseReader["category"].ToString());
                    }
                    expenseReader.Close();

                    // Query to select distinct categories from the addpoultry table
                    string addPoultryQuery = "SELECT distinct category FROM addpoultry";
                    SqlCommand addPoultryCommand = new SqlCommand(addPoultryQuery, connection);
                    SqlDataReader addPoultryReader = addPoultryCommand.ExecuteReader();

                    // Add categories from addpoultry table to the combobox
                    while (addPoultryReader.Read())
                    {
                        expensecombobox.Items.Add(addPoultryReader["category"].ToString());
                    }
                    addPoultryReader.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while loading data into the ComboBox: " + ex.Message);
            }

        }




        private void searchbutton_Click(object sender, EventArgs e)
        {
            string category = expensecombobox.SelectedItem?.ToString();
            DataTable dataTable = new DataTable();

            try
            {
                // Parse the dates from text boxes
                if (!DateTime.TryParse(fromdate.Text, out DateTime startDate) || !DateTime.TryParse(todate.Text, out DateTime endDate))
                {
                    MessageBox.Show("Invalid date format. Please enter valid dates.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Check if start date is after end date
                if (startDate > endDate)
                {
                    MessageBox.Show("Start date must be before end date.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"
SELECT 'AddPoultry' AS Source, Name AS ExpenseName, category, amount, SaleDate AS date, description 
            FROM addpoultry
WHERE (@category IS NULL OR Category = @category) AND SaleDate BETWEEN @StartDate AND @EndDate

UNION ALL

SELECT 'Expense' AS Source, [expense name] AS ExpenseName, category, amount, date, description 
            FROM Expense
WHERE (@category IS NULL OR Category = @category) AND date BETWEEN @StartDate AND @EndDate";

                    SqlCommand command = new SqlCommand(query, connection);

                    // Set parameter values for the date range
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    // Set category parameter only if a category is selected
                    if (!string.IsNullOrEmpty(category))
                    {
                        command.Parameters.AddWithValue("@category", category);
                    }
                    else
                    {
                        command.Parameters.AddWithValue("@category", DBNull.Value);
                    }

                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }

                expenserecord.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }


        }
        private void ClearData()
        {

            expensecombobox.SelectedItem = null;
        }

        private void showdata_Click(object sender, EventArgs e)
        {
            DataTable dataTable = new DataTable();

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();
                    string query = @"
            SELECT 'Expense' AS Source, [expense name] AS ExpenseName, category, amount, date, description 
            FROM Expense 
            
            UNION ALL 
            
            SELECT 'AddPoultry' AS Source, Name AS ExpenseName, category, amount, SaleDate AS date, description 
            FROM addpoultry";

                    SqlCommand command = new SqlCommand(query, connection);
                    SqlDataAdapter adapter = new SqlDataAdapter(command);
                    adapter.Fill(dataTable);
                }

                expenserecord.DataSource = dataTable;
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private void Show_Expense_Load(object sender, EventArgs e)
        {


        }

        private void expenserecord_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            deleteButton.Enabled = expenserecord.SelectedRows.Count > 0;
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            // Prompt user for confirmation
            DialogResult result = MessageBox.Show("Are you sure you want to delete the selected row(s)?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (result == DialogResult.Yes)
            {
                // Delete selected rows from the database and DataGridView
                foreach (DataGridViewRow row in expenserecord.SelectedRows)
                {
                    string Expensename = Convert.ToString(row.Cells["ExpenseName"].Value);
                    string Category = Convert.ToString(row.Cells["category"].Value);
                    string Description = Convert.ToString(row.Cells["description"].Value);
                    int Amount = Convert.ToInt32(row.Cells["amount"].Value);

                    // Delete row from database
                    DeleteExpenseFromDatabase(Expensename, Category, Amount, Description);
                  

                    // Remove row from DataGridView
                    expenserecord.Rows.Remove(row);
                }

                MessageBox.Show("Selected row(s) deleted successfully.", "Delete Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void DeleteExpenseFromDatabase(string Expensename, string Category, int Amount, string Description)
        {
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                try
                {
                    connection.Open();
                    if (Category == "Hen" || Category == "Chick" || Category == "Roaster")
                    {
                        // DELETE statement for addpoultry table using Name column
                        string addPoultryQuery = "DELETE FROM addpoultry WHERE Name = @Expensename AND Category = @Category AND amount = @Amount AND description = @Description";

                        using (SqlCommand addPoultryCommand = new SqlCommand(addPoultryQuery, connection))
                        {
                            addPoultryCommand.Parameters.AddWithValue("@Expensename", Expensename);
                            addPoultryCommand.Parameters.AddWithValue("@Category", Category);
                            addPoultryCommand.Parameters.AddWithValue("@Amount", Amount);
                            addPoultryCommand.Parameters.AddWithValue("@Description", Description);

                            addPoultryCommand.ExecuteNonQuery();
                        }
                    }
                    else
                    {
                        // DELETE statement for Expense table
                        string expenseQuery = "DELETE FROM Expense WHERE [expense name] = @Expensename AND category = @Category AND amount = @Amount AND description = @Description";

                        using (SqlCommand expenseCommand = new SqlCommand(expenseQuery, connection))
                        {
                            expenseCommand.Parameters.AddWithValue("@Expensename", Expensename);
                            expenseCommand.Parameters.AddWithValue("@Category", Category);
                            expenseCommand.Parameters.AddWithValue("@Amount", Amount);
                            expenseCommand.Parameters.AddWithValue("@Description", Description);

                            expenseCommand.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Handle any exceptions
                    MessageBox.Show("An error occurred while deleting the row: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

    
}
}
