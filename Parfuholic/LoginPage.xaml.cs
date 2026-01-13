using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Data.SqlClient;

namespace Parfuholic
{
    public partial class LoginPage : Page
    {
        public LoginPage()
        {
            InitializeComponent();
            RegisterLink.Click += RegisterLink_Click;
        }

        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            PasswordPlaceholder.Visibility = string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            // Переход на страницу регистрации с заменой текущей страницы
            NavigationService?.Navigate(new RegisterPage());

            // Или используйте этот вариант для полной замены контента
            // var window = Window.GetWindow(this);
            // if (window != null && window.Content is Frame frame)
            // {
            //     frame.Content = new RegisterPage();
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

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginBox.Text) ||
                string.IsNullOrWhiteSpace(PasswordBox.Password))
            {
                MessageBox.Show("Введите логин и пароль");
                return;
            }

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                // проверка пользователя
                var cmd = new SqlCommand(
                    @"SELECT COUNT(*) FROM Users
              WHERE (UserName=@Login OR Email=@Login)
              AND Password=@Password", conn);

                cmd.Parameters.AddWithValue("@Login", LoginBox.Text);
                cmd.Parameters.AddWithValue("@Password", PasswordBox.Password);

                int count = (int)cmd.ExecuteScalar();

                if (count == 1)
                {
                    MessageBox.Show("Вход выполнен");

                    // закрываем overlay
                    var main = (MainWindow)Application.Current.MainWindow;
                    main.CloseOverlay();
                }
                else
                {
                    MessageBox.Show("Неверные данные");
                }
            }
        }

    }
}