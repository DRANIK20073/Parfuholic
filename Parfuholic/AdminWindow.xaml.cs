using System.Windows;
using Parfuholic.Pages;

namespace Parfuholic
{
    public partial class AdminWindow : Window
    {
        public AdminWindow()
        {
            InitializeComponent();
            AdminFrame.Navigate(new UsersAdminPage());
        }

        private void Users_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new UsersAdminPage());
        }

        private void Products_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new ManagePerfumesPage());
        }

        private void AddPerfume_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new AddPerfumePage());
        }

        private void Reports_Click(object sender, RoutedEventArgs e)
        {
            AdminFrame.Navigate(new ReportsPage());
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            new NavigationWindow().Show();
            Close();
        }
    }
}
