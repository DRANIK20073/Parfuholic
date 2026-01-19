using System.Windows;
using System.Windows.Controls;
using System.Data.SqlClient;
using Parfuholic.Pages;
using System;
using Parfuholic.Services;

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
            PasswordPlaceholder.Visibility =
                string.IsNullOrEmpty(PasswordBox.Password)
                ? Visibility.Visible
                : Visibility.Collapsed;
        }

        private void RegisterLink_Click(object sender, RoutedEventArgs e)
        {
            NavigationService?.Navigate(new RegisterPage());
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(LoginBox.Text) || string.IsNullOrWhiteSpace(PasswordBox.Password))
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

                    // Проверка админа
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

                    // Проверка пользователя
                    SqlCommand userCmd = new SqlCommand(@"
                            SELECT UserID, IsBlocked
                            FROM Users
                            WHERE Login = @login AND Password = @password", conn);

                    userCmd.Parameters.AddWithValue("@login", login);
                    userCmd.Parameters.AddWithValue("@password", password);

                    SqlDataReader reader = userCmd.ExecuteReader();
                    if (!reader.Read())
                    {
                        MessageBox.Show("Неверный логин или пароль");
                        return;
                    }

                    int userId = reader.GetInt32(0);
                    bool isBlocked = reader.GetBoolean(1);
                    reader.Close();

                    if (isBlocked)
                    {
                        MessageBox.Show("Вход заблокирован. Обратитесь к администратору.");
                        return;
                    }

                    AuthService.Login(userId, login);

                    UserMainWindow userWindow = new UserMainWindow(userId);
                    userWindow.Show();
                    Window.GetWindow(this)?.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка входа:\n" + ex.Message);
            }
        }


        private void BackgroundGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            CloseOverlay();
        }

        private void ContainerGrid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            e.Handled = true; // предотвращаем закрытие при клике внутри контейнера
        }

        private void CloseOverlay()
        {
            var window = Window.GetWindow(this);
            if (window?.Content is Frame frame)
            {
                frame.Content = null;
            }
        }

    }
}
