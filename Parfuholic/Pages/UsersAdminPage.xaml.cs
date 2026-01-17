using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using Parfuholic.Models;

namespace Parfuholic.Pages
{
    public partial class UsersAdminPage : Page
    {
        public UsersAdminPage()
        {
            InitializeComponent();
            LoadUsers();
        }

        // Загрузка пользователей
        private void LoadUsers()
        {
            List<User> users = new List<User>();

            try
            {
                using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"
                        SELECT UserId, Login, Email, IsBlocked
                        FROM Users", conn);

                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        users.Add(new User
                        {
                            UserId = (int)reader["UserId"],
                            Login = reader["Login"].ToString(),
                            Email = reader["Email"].ToString(),
                            IsBlocked = (bool)reader["IsBlocked"]
                        });
                    }
                }

                UsersGrid.ItemsSource = users;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при загрузке пользователей: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Блокировка / разблокировка пользователя
        private void BlockUser_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                try
                {
                    int userId = Convert.ToInt32(btn.Tag);

                    using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
                    {
                        conn.Open();

                        SqlCommand cmd = new SqlCommand(@"
                            UPDATE Users
                            SET IsBlocked = CASE 
                                WHEN IsBlocked = 1 THEN 0 
                                ELSE 1 
                            END
                            WHERE UserId = @id", conn);

                        cmd.Parameters.AddWithValue("@id", userId);
                        cmd.ExecuteNonQuery();
                    }

                    LoadUsers(); // обновляем таблицу
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при блокировке пользователя: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}
