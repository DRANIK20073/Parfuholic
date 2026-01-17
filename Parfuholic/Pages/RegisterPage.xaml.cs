using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Data.SqlClient;
using System;

namespace Parfuholic
{
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
            LoginLink.Click += LoginLink_Click;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void LoginLink_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу входа с заменой текущей страницы
            NavigationService?.Navigate(new LoginPage());

            // Или используйте этот вариант для полной замены контента
            // var window = Window.GetWindow(this);
            // if (window != null && window.Content is Frame frame)
            // {
            //     frame.Content = new LoginPage();
            // }
        }

        private void BackgroundGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Закрываем текущую страницу
            CloseCurrentPage();
        }

        private void ContainerGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CloseCurrentPage()
        {
            var window = Window.GetWindow(this);
            if (window != null)
            {
                if (window.Content is Frame frame)
                {
                    // Если используется Frame, очищаем его
                    frame.Content = null;
                }
                else if (window.Content is Page)
                {
                    // Если окно содержит напрямую Page, заменяем на null
                    window.Content = null;
                }
            }
        }

        //РЕГИСТРАЦИЯ ПОЛЬЗОВАТЕЛЯ В БД
        private void Register_Click(object sender, RoutedEventArgs e)
        {
            string login = LoginBox.Text.Trim();
            string email = EmailBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(login) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                // 1️⃣ Проверка на админа
                SqlCommand checkAdmin = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM Admins 
                    WHERE Login = @login", conn);

                checkAdmin.Parameters.AddWithValue("@login", login);

                int adminExists = (int)checkAdmin.ExecuteScalar();

                if (adminExists > 0)
                {
                    MessageBox.Show("Этот логин зарезервирован");
                    return;
                }

                // 2️⃣ Проверка на пользователя
                SqlCommand checkUser = new SqlCommand(@"
                    SELECT COUNT(*) 
                    FROM Users 
                    WHERE Login = @login OR Email = @email", conn);

                checkUser.Parameters.AddWithValue("@login", login);
                checkUser.Parameters.AddWithValue("@email", email);

                int userExists = (int)checkUser.ExecuteScalar();

                if (userExists > 0)
                {
                    MessageBox.Show("Пользователь с таким логином или почтой уже существует");
                    return;
                }

                // 3️⃣ Регистрация
                SqlCommand insertUser = new SqlCommand(@"
                    INSERT INTO Users (Login, Email, Password)
                    VALUES (@login, @email, @password)", conn);

                insertUser.Parameters.AddWithValue("@login", login);
                insertUser.Parameters.AddWithValue("@email", email);
                insertUser.Parameters.AddWithValue("@password", password);

                insertUser.ExecuteNonQuery();
            }

            MessageBox.Show("Регистрация успешна");
        }


    }
}