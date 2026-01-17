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

            string login = LoginBox.Text.Trim();
            string password = PasswordBox.Password;

            try
            {
                using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
                {
                    conn.Open();

                    // ===== Проверяем админа =====
                    SqlCommand adminCmd = new SqlCommand(@"
                SELECT COUNT(*) 
                FROM Admins 
                WHERE Login = @login AND Password = @password", conn);

                    adminCmd.Parameters.AddWithValue("@login", login);
                    adminCmd.Parameters.AddWithValue("@password", password);

                    int adminCount = (int)adminCmd.ExecuteScalar();
                    if (adminCount > 0)
                    {
                        MessageBox.Show("Вход выполнен как администратор");
                        AdminWindow adminWindow = new AdminWindow();
                        adminWindow.Show();
                        Window.GetWindow(this)?.Close();
                        return;
                    }

                    // ===== Проверяем пользователя =====
                    SqlCommand userCmd = new SqlCommand(@"
                SELECT IsBlocked
                FROM Users
                WHERE Login = @login AND Password = @password", conn);

                    userCmd.Parameters.AddWithValue("@login", login);
                    userCmd.Parameters.AddWithValue("@password", password);

                    object result = userCmd.ExecuteScalar();

                    if (result == null)
                    {
                        MessageBox.Show("Неверный логин или пароль");
                        return;
                    }

                    bool isBlocked = (bool)result;
                    if (isBlocked)
                    {
                        MessageBox.Show("Вход заблокирован. Обратитесь к администратору.");
                        return;
                    }

                    // ===== Вход успешен =====
                    MessageBox.Show("Вход выполнен");
                    UserMainWindow userWindow = new UserMainWindow();
                    userWindow.Show();
                    Window.GetWindow(this)?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка входа:\n" + ex.Message);
            }
        }

    }
}