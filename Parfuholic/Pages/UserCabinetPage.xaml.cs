using System.Windows;
using System.Windows.Controls;
using Parfuholic.Pages;

namespace Parfuholic.Pages
{
    public partial class UserCabinetPage : Page
    {
        private int currentUserId; // поле для хранения ID пользователя

        // Конструктор теперь принимает userId
        public UserCabinetPage(int userId)
        {
            InitializeComponent();
            currentUserId = userId;

            // по умолчанию открываем избранное
            CabinetFrame.Navigate(new FavoritesPage());
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

            // передаем текущий ID пользователя в ProfileDataPage
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
            // получаем текущее окно
            Window window = Window.GetWindow(this);

            if (window is UserMainWindow userWindow)
            {
                userWindow.Logout();
            }
        }
    }
}
