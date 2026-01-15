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
            if (string.IsNullOrWhiteSpace(UserNameBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Заполните все поля");
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(
                        "INSERT INTO Users (Login, Email, Password) VALUES (@u, @e, @p)", conn);

                    cmd.Parameters.AddWithValue("@u", UserNameBox.Text);
                    cmd.Parameters.AddWithValue("@e", EmailBox.Text);
                    cmd.Parameters.AddWithValue("@p", PasswordBox.Password);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Регистрация успешна");

                // закрываем overlay
                var main = Application.Current.MainWindow as NavigationWindow;
                main?.CloseOverlay();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}