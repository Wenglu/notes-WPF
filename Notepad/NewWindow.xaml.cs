using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
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
using Notatki.services;
using Notatki.Model;
using Dapper;
using System.Collections.ObjectModel;

namespace Notatki
{
    /// <summary>
    /// Logika interakcji dla klasy NewWindow.xaml
    /// </summary>
    public partial class NewWindow : Window
    {
        private string connectionString = $"Server={Environment.MachineName};Integrated Security=True;";
        private readonly DbConnectionFactory _connectionFactory;


        public NewWindow()
        {
            var databaseInitializer = new DatabaseInitializer(connectionString);
            connectionString = databaseInitializer.CreateDatabaseAndTablesIfNotExist();
            InitializeComponent();
            LoadNotes();
        }
        private void CreateNote_Click(object sender, RoutedEventArgs e)
        {
            CreateNote createNote = new CreateNote();
            createNote.Show();
        }
        private void ModifyButton_Click(object sender, RoutedEventArgs e)
        {
            ModifyWindow modifyWindow = new ModifyWindow();
            modifyWindow.Show();
        }
        private void LoadNotes()
        {
            try
            {
                using SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                {
                    string query = "SELECT Id, Title, Content, Category, CreationDate FROM Note";

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int id = reader.GetInt32(0);
                                string title = reader.GetString(1);
                                string content = reader.GetString(2);
                                string category = reader.GetString(3);
                                DateTime creationDate = reader.GetDateTime(4);

                                string noteInfo = $"ID: {id}, Title: {title}, Category: {category}, Creation Date: {creationDate}\nContent: {content}\n";

                                NotesListBox.Items.Add(noteInfo);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Wystąpił błąd podczas pobierania notatek: " + ex.Message);
            }


        }
        private void ViewAllNotes_Click(object sender, RoutedEventArgs e)
        {
            NotesListBox.Items.Clear();
            LoadNotes();
        }

        private void RemoveNoteButton_Click(object sender, RoutedEventArgs e)
        {
            if (NotesListBox.SelectedItem != null)
            {
                string selectedNote = NotesListBox.SelectedItem.ToString();

                // Pobranie ID notatki
                int noteId;
                if (int.TryParse(selectedNote.Split(',')[0].Split(':')[1].Trim(), out noteId))
                {
                    using (SqlConnection connection = new SqlConnection(connectionString))
                    {
                        connection.Open();
                        string query = "DELETE FROM Note WHERE Id = @NoteId";

                        using (SqlCommand command = new SqlCommand(query, connection))
                        {
                            command.Parameters.AddWithValue("@NoteId", noteId);
                            command.ExecuteNonQuery();
                        }
                    }

                    MessageBox.Show("Notatka została usunięta.");
                }
                else
                {
                    MessageBox.Show("Nie można pobrać ID notatki.");
                }
            }
        }



    }
}
