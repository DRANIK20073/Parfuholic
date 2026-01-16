using Parfuholic.Pages;
using System.Windows;
using System.Windows.Input;

namespace Parfuholic
{
    public partial class UserMainWindow : Window
    {
        public UserMainWindow()
        {
            InitializeComponent();

            // 👇 ПО УМОЛЧАНИЮ КАТАЛОГ
            CatalogFrame.Navigate(new CatalogPage());
        }

        // 👤 КАБИНЕТ
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            UserCabinetFrame.Navigate(new UserCabinetPage());
        }

        // 🏠 КАТАЛОГ
        private void Logo_Click(object sender, MouseButtonEventArgs e)
        {
            UserCabinetFrame.Navigate(new CatalogPage());
        }


        // ВЫХОД ИЗ АККАУНТА
        public void Logout()
        {
            NavigationWindow nav = new NavigationWindow();
            nav.Show();
            this.Close();
        }

    }

}
