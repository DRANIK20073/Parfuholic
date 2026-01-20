using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Parfuholic.Services;

namespace Parfuholic.Pages
{
    public partial class UserCabinetPage : Page
    {
        private int currentUserId;

        public UserCabinetPage(int userId)
        {
            InitializeComponent();
            currentUserId = userId;

            LoadUserName();

            // по умолчанию открываем Личные данные
            CabinetFrame.Navigate(new ProfileDataPage(currentUserId));
        }

        private void LoadUserName()
        {
            string firstName = null;
            string lastName = null;

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();
                string query = "SELECT FirstName, LastName FROM Users WHERE UserID=@UserID";
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", currentUserId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            firstName = reader["FirstName"]?.ToString();
                            lastName = reader["LastName"]?.ToString();
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(firstName) && !string.IsNullOrWhiteSpace(lastName))
            {
                UserFirstNameText.Text = lastName;
                UserLastNameText.Text = firstName;
            }
            else
            {
                UserFirstNameText.Text = "Любимый";
                UserLastNameText.Text = "пользователь";
            }
        }

        private void ResetButtons()
        {
            ProfileBtn.Tag = null;
            OrdersBtn.Tag = null;
            FavoritesBtn.Tag = null;
        }

        private void Profile_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            ProfileBtn.Tag = "Active";
            CabinetFrame.Navigate(new ProfileDataPage(currentUserId));
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            OrdersBtn.Tag = "Active";
            CabinetFrame.Navigate(new OrdersHistoryPage(currentUserId));
        }

        private void Favorites_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            FavoritesBtn.Tag = "Active";
            CabinetFrame.Navigate(new FavoritesPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show(
                "Вы уверены, что хотите выйти?",
                "Подтверждение выхода",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Получаем текущее окно
                Window window = Window.GetWindow(this);
                if (window is UserMainWindow userWindow)
                {
                    // Вызываем метод Logout в UserMainWindow
                    userWindow.Logout();
                }
            }
        }
    }
}
