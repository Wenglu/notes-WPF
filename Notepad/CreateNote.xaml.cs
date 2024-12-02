using Notatki.services;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.Metadata;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Dapper;
using System.Data.SqlClient;

namespace Notatki
{
    /// <summary>
    /// Logika interakcji dla klasy CreateNote.xaml
    /// </summary>
    public partial class CreateNote : Window
    {

        private string connectionString = $"Server={Environment.MachineName};Database=Notatki;Integrated Security=True;";
        private DbConnectionFactory _connectionFactory;
        private UserManager _userManager;
        public CreateNote()
        {
            _connectionFactory = new DbConnectionFactory(connectionString);
            _userManager = new UserManager(_connectionFactory);
            InitializeComponent();
        }

        private void SaveNote_Click(object sender, RoutedEventArgs e)
        {
            string title = TitleTextBox.Text;
            string content = ContentTextBox.Text;
            string category = CategoryTextBox.Text;
            DateTime creationDate = DateTime.Now;

            using SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();

            string query = "INSERT INTO Note (Title, Content, Category, CreationDate, ModificationDate) " +
               "VALUES (@Title, @Content, @Category, @CreationDate, @ModificationDate);";

            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Title", title);
            command.Parameters.AddWithValue("@Content", content);
            command.Parameters.AddWithValue("@Category", category);
            command.Parameters.AddWithValue("@CreationDate", creationDate);
            command.Parameters.AddWithValue("@ModificationDate", creationDate);



            int rowsAffected = command.ExecuteNonQuery();
            connection.Close();
            if (rowsAffected > 0)
            {
                MessageBox.Show("Notatka zostala dodana", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

                CreateNote createNote = new CreateNote();
                createNote.Show();
                this.Close();
            }
            else
            {
                MessageBox.Show("Nie mozna dodać notatki", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }


    }
}
