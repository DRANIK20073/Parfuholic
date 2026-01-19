using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Parfuholic.Models;

namespace Parfuholic.Pages
{
    public partial class ProfileDataPage : Page
    {
        private User currentUser;

        public ProfileDataPage(int userId)
        {
            InitializeComponent();
            LoadUserData(userId);
        }

        private void LoadUserData(int userId)
        {
            string query = "SELECT * FROM Users WHERE UserID = @UserID";

            using (SqlConnection connection = new SqlConnection(Database.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@UserID", userId);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    if (reader.Read())
                    {
                        currentUser = new User
                        {
                            UserId = (int)reader["UserID"],
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Email = reader["Email"].ToString(),
                            City = reader["City"].ToString(),
                            Address = reader["Address"].ToString()
                        };

                        FirstNameBox.Text = currentUser.FirstName;
                        LastNameBox.Text = currentUser.LastName;
                        PhoneBox.Text = currentUser.Phone;
                        EmailBox.Text = currentUser.Email;
                        CityBox.Text = currentUser.City;
                        AddressBox.Text = currentUser.Address;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при загрузке данных: {ex.Message}");
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (currentUser == null) return;

            string query = @"
                UPDATE Users
                SET FirstName=@FirstName,
                    LastName=@LastName,
                    Phone=@Phone,
                    Email=@Email,
                    City=@City,
                    Address=@Address
                WHERE UserID=@UserID";

            using (SqlConnection connection = new SqlConnection(Database.ConnectionString))
            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FirstName", FirstNameBox.Text);
                command.Parameters.AddWithValue("@LastName", LastNameBox.Text);
                command.Parameters.AddWithValue("@Phone", PhoneBox.Text);
                command.Parameters.AddWithValue("@Email", EmailBox.Text);
                command.Parameters.AddWithValue("@City", CityBox.Text);
                command.Parameters.AddWithValue("@Address", AddressBox.Text);
                command.Parameters.AddWithValue("@UserID", currentUser.UserId);

                try
                {
                    connection.Open();
                    command.ExecuteNonQuery();
                    MessageBox.Show("Данные успешно сохранены!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка при сохранении: {ex.Message}");
                }
            }
        }

        // Телефон: разрешаем только цифры
        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "[0-9]");
        }

        private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            // Сохраняем предыдущую позицию курсора
            int selStart = textBox.SelectionStart;

            // Сохраняем только цифры
            string digits = Regex.Replace(textBox.Text, @"\D", "");

            // Убираем ведущие 375, если есть (чтобы не дублировать)
            if (digits.StartsWith("375"))
                digits = digits.Substring(3);

            if (digits.Length > 9) // максимум 9 цифр после +375
                digits = digits.Substring(0, 9);

            // Форматируем номер
            string formatted = "+375";
            if (digits.Length > 0)
                formatted += "(" + digits.Substring(0, Math.Min(2, digits.Length));
            if (digits.Length > 2)
                formatted += ")" + digits.Substring(2, Math.Min(3, digits.Length - 2));
            if (digits.Length > 5)
                formatted += "-" + digits.Substring(5, Math.Min(2, digits.Length - 5));
            if (digits.Length > 7)
                formatted += "-" + digits.Substring(7, Math.Min(2, digits.Length - 7));

            // Считаем количество цифр перед курсором
            int digitsBeforeCursor = Regex.Replace(textBox.Text.Substring(0, selStart), @"\D", "").Length;

            // Новая позиция курсора с учётом формата
            int newCursorPos = 4; // +375 занимает первые 4 позиции
            int dCount = 0;

            for (int i = 4; i < formatted.Length && dCount < digitsBeforeCursor; i++)
            {
                if (Char.IsDigit(formatted[i]))
                    dCount++;
                newCursorPos = i + 1;
            }

            textBox.Text = formatted;
            textBox.SelectionStart = newCursorPos;
        }


    }
}
