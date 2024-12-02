using System;
using System.Data.SqlClient;
using System.Windows;
using Notatki.services;

namespace Notatki
{
    public partial class ModifyWindow : Window
    {

        private string connectionString = $"Server={Environment.MachineName};Integrated Security=True;";

        public ModifyWindow()
        {
            var databaseInitializer = new DatabaseInitializer(connectionString);
            connectionString = databaseInitializer.CreateDatabaseAndTablesIfNotExist();
            InitializeComponent();

        }

        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int noteId = int.Parse(NoteIdTextBox.Text);
                string title = TitleTextBoxModify.Text;
                string category = CategoryTextBoxModify.Text;
                string content = ContentTextBoxModify.Text;

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "UPDATE Note SET Title = @Title, Category = @Category, Content = @Content WHERE Id = @NoteId";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        command.Parameters.AddWithValue("@NoteId", noteId);
                        command.Parameters.AddWithValue("@Title", title);
                        command.Parameters.AddWithValue("@Category", category);
                        command.Parameters.AddWithValue("@Content", content);
                        command.ExecuteNonQuery();
                    }
                }

                MessageBox.Show("Notatka została zmodyfikowana.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas modyfikowania notatki: " + ex.Message);
            }
        }

        private void GetNoteButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                int noteId;
                if (int.TryParse(NoteIdTextBox.Text, out noteId) && noteId > 0)
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();

                        string query = "SELECT Title, Category, Content FROM Note WHERE Id = @NoteId";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@NoteId", noteId);

                            SqlDataReader reader = command.ExecuteReader();
                            if (reader.Read())
                            {
                                TitleTextBoxModify.Text = reader.GetString(0);
                                CategoryTextBoxModify.Text = reader.GetString(1);
                                ContentTextBoxModify.Text = reader.GetString(2);
                            }
                            else
                            {
                                MessageBox.Show("Nie znaleziono notatki o podanym ID.");
                            }
                        }
                    }
                }
                else
                {
                    NoteIdTextBox.Text = "";
                    TitleTextBoxModify.Text = "";
                    CategoryTextBoxModify.Text = "";
                    ContentTextBoxModify.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas pobierania notatki: " + ex.Message);
            }
        }


        private void SaveNote_Click(object sender, RoutedEventArgs e)
        {
            int noteId;
            if (int.TryParse(NoteIdTextBox.Text, out noteId) && noteId > 0)
            {
                try
                {
                    string title = TitleTextBoxModify.Text;
                    string content = ContentTextBoxModify.Text;
                    string category = CategoryTextBoxModify.Text;
                    DateTime modificationDate = DateTime.Now;

                    using SqlConnection connection = new SqlConnection(connectionString);
                    connection.Open();

                    string query = "UPDATE Note SET Title = @Title, Content = @Content, Category = @Category, ModificationDate = @ModificationDate WHERE Id = @NoteId";

                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@NoteId", noteId);
                    command.Parameters.AddWithValue("@Title", title);
                    command.Parameters.AddWithValue("@Content", content);
                    command.Parameters.AddWithValue("@Category", category);
                    command.Parameters.AddWithValue("@ModificationDate", modificationDate);

                    int rowsAffected = command.ExecuteNonQuery();
                    connection.Close();

                    if (rowsAffected > 0)
                    {
                        MessageBox.Show("Notatka została zaktualizowana", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Nie znaleziono notatki o podanym ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Wystąpił błąd podczas aktualizowania notatki: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Nieprawidłowy ID notatki.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

