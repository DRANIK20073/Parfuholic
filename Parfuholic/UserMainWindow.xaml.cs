using System.Windows;

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
        private void Logo_Click(object sender, RoutedEventArgs e)
        {
            CatalogFrame.Navigate(new CatalogPage());
        }
    }
}
