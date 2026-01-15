using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Data.SqlClient;
using System;

namespace Parfuholic
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            RegisterLink.Click += RegisterLink_Click;
        }

        // ===== Placeholder для пароля =====
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility =
                string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        // ===== Переход на регистрацию =====
        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegisterPage());
        }

        // ===== Закрытие overlay при клике на фон =====
        private void BackgroundGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseOverlay();
        }

        // ===== Блокируем клик внутри окна =====
        private void ContainerGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }

        private void CloseOverlay()
        {
            var window = Window.GetWindow(this);
            if (window?.Content is Frame frame)
            {
                frame.Content = null;
            }
        }

        // ===== ВХОД =====
        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM Users
                WHERE Login = @login
                  AND Password = @password", conn);

                    cmd.Parameters.AddWithValue("@login", LoginBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@password", PasswordBox.Password);

                    int count = (int)cmd.ExecuteScalar();

                    if (count == 1)
                    {
                        MessageBox.Show("Вход выполнен");

                        // переключаемся на UserMainWindow
                        UserMainWindow userWindow = new UserMainWindow();
                        userWindow.Show();

                        // закрываем окно навигации
                        Window.GetWindow(this)?.Close();
                    }
                    else
                    {
                        MessageBox.Show("Неверный логин или пароль");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка входа:\n" + ex.Message);
            }
        }



    }
}
