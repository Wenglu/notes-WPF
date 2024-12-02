using System.Text;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data.SqlClient;
using System.Data;
using Notatki.services;
using Notatki.Model;

namespace Notatki
{
    public partial class MainWindow : Window
    {

        private Authorize authorize;
        private string connectionString = $"Server={Environment.MachineName};Integrated Security=True;";
        private DbConnectionFactory _connectionFactory;
        private UserManager _userManager;
        public MainWindow()
        {
            var databaseInitializer = new DatabaseInitializer(connectionString);
            connectionString = databaseInitializer.CreateDatabaseAndTablesIfNotExist();
            _connectionFactory = new DbConnectionFactory(connectionString);
            _userManager = new UserManager(_connectionFactory);
            InitializeComponent();
        }


        private void OpenNewWindow_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            bool authenticated = _userManager.AuthenticateUser(username, password);
            if(authenticated)
            {
            NewWindow newWindow = new NewWindow();
            newWindow.Show();
            this.Close();
            }
            else
            {
                MessageBox.Show("Niepoprawne dane", "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            }
        }

        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string username = UsernameTextBox.Text;
            string password = PasswordTextBox.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please provide both username and password.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Register the user
            bool registrationSuccess = _userManager.RegisterUser(username, password);
            if (registrationSuccess)
            {
                MessageBox.Show("User registered successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Error registering user. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void Username_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void Password_TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}