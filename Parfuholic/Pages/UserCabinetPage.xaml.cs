using System.Windows;
using System.Windows.Controls;
using Parfuholic.Pages;

namespace Parfuholic.Pages
{
    public partial class UserCabinetPage : Page
    {
        public UserCabinetPage()
        {
            InitializeComponent();
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
            CabinetFrame.Navigate(new ProfileDataPage());
        }

        private void Orders_Click(object sender, RoutedEventArgs e)
        {
            ResetButtons();
            OrdersBtn.Tag = "Active";
            CabinetFrame.Navigate(new OrdersHistoryPage());
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
