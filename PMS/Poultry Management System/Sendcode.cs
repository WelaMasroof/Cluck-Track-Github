using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Management;
using System.Drawing;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static Poultry_Management_System.Login;

namespace Poultry_Management_System
{
    
    public partial class Sendcode : Form
    {
        private string ConnectionString = Login.ConnectionString;
        private string resetCode;
        public static string userEmail;
        public Sendcode()
        {
            InitializeComponent();
        }

        


        private bool UserExists(string userEmail)
        {

            try
            {
                using (SqlConnection connection = new SqlConnection(ConnectionString))
                {
                    connection.Open();

                    string query = "SELECT COUNT(*) FROM login WHERE email = @UserEmail";
                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@UserEmail", userEmail);

                        int count = (int)command.ExecuteScalar();

                        return count > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while checking user existence: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
        }
        public string GenerateRandomCode()
        {
            // Generate a random 6-digit code
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }

        private void SendResetCodeEmail(string userEmail, string resetCode)
        {
            try
            {
                using (SmtpClient client = new SmtpClient("smtp.gmail.com", 587))
                {
                    // Configure SMTP client
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("uummeerr0786@gmail.com", "ijkm sdbl ommp luzy");
                    client.EnableSsl = true;
                    // Get the IP address and MAC address of the local machine
                    string ipAddress = GetLocalIPAddress();
                    string macAddress = GetMacAddress();

                    // Compose email
                    using (MailMessage mailMessage = new MailMessage("uummeerr0786@gmail.com", userEmail))
                    {

                        mailMessage.Subject = "Password Reset Code" + "";
                        mailMessage.Body = $"Your password reset code is: {resetCode}\n\n" +
                                   $"This code was sent from:\n" +
                                   $"IP Address: {ipAddress}\n" +
                                   $"MAC Address: {macAddress}";

                        // Send email
                        client.Send(mailMessage);
                        MessageBox.Show("A reset code has been sent to your email. Please check your inbox.");
                    }
                }
            }
            catch (SmtpException ex)
            {
                MessageBox.Show($"Failed to send email: {ex.Message}");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An unexpected error occurred: {ex.Message}");
            }
        }
        
        private void Sendcode_Load(object sender, EventArgs e)
        {

        }


        private void Sendcodebtn_Click_1(object sender, EventArgs e)
        {
            userEmail = EmailTextBox.Text;

            try
            {
                // Check if the email exists in your database
                if (UserExists(userEmail))
                {
                    // Generate a random code for password reset
                    resetCode = GenerateRandomCode();

                    // Save the reset code in your database

                    // Send the reset code to the user's email
                    SendResetCodeEmail(userEmail, resetCode);


                }
                else
                {
                    MessageBox.Show("Email address not found.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}");
            }
        }

        // Method to get the local IP address of the machine
         private string GetLocalIPAddress()
        {
            string localIP = "";
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_NetworkAdapterConfiguration WHERE IPEnabled = 'TRUE'"))
                {
                    ManagementObjectCollection objCol = searcher.Get();
                    foreach (ManagementObject obj in objCol)
                    {
                        string[] addresses = (string[])obj["IPAddress"];
                        if (addresses != null && addresses.Length > 0)
                        {
                            localIP = addresses[0];
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve IP address: {ex.Message}");
            }
            return localIP;
        }



        // Method to get the MAC address of the machine
        private string GetMacAddress()
        {
            string macAddress = "";
            try
            {
                ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
                ManagementObjectCollection moc = mc.GetInstances();
                foreach (ManagementObject mo in moc)
                {
                    if ((bool)mo["IPEnabled"])
                    {
                        macAddress = mo["MacAddress"].ToString();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to retrieve MAC address: {ex.Message}");
            }
            return macAddress;
        }

        private void Verifybtn_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (resetCode == codetextbox.Text)
                {
                    string userEmail = EmailTextBox.Text.Trim();
                    this.Hide();
                    using (ChangePassword changePassword = new ChangePassword(userEmail))
                        changePassword.ShowDialog();
                    this.Close();

                }
                else
                {
                    MessageBox.Show("Wrong Code", "Code", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while verifying the code: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void Closebtn_Click_1(object sender, EventArgs e)
        {

            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
        }
    }
}
