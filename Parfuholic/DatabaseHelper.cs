using System;
using System.Data.SqlClient;
using System.Windows;

namespace WpfApp
{
    public class DatabaseHelper
    {
        private string connectionString = @"Server=.\SQLEXPRESS;Database=ParfuholicDB;Trusted_Connection=True;TrustServerCertificate=True;";

        public void TestConnection()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open(); // Пытаемся открыть соединение
                    MessageBox.Show("✅ Подключение к базе успешно!"); // Всплывающее окно
                }
                catch (Exception ex)
                {
                    MessageBox.Show("❌ Ошибка подключения: " + ex.Message); // Всплывающее окно
                }
            }
        }
    }
}
