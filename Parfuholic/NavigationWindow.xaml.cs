using Parfuholic.Pages;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Parfuholic
{
    public partial class NavigationWindow : Window
    {
        public NavigationWindow()
        {
            InitializeComponent();
            CatalogFrame.Navigate(new CatalogPage());
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Показываем Frame и загружаем страницу логина
            LoginPage.Visibility = Visibility.Visible;
            LoginPage.Navigate(new LoginPage());
        }

        private void OverlayFrame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Закрываем оверлей при клике на фон
            CloseOverlay();
        }

        public void CloseOverlay()
        {
            // Скрываем Frame и очищаем навигацию
            LoginPage.Visibility = Visibility.Collapsed;
            LoginPage.Content = null;

            // Очищаем историю навигации
            while (LoginPage.CanGoBack)
            {
                LoginPage.RemoveBackEntry();
            }
            while (LoginPage.CanGoForward)
            {
                LoginPage.RemoveBackEntry();
            }
        }

        public void OpenUserMode()
        {
            UserMainWindow userWindow = new UserMainWindow();
            userWindow.Show();
            this.Close();
        }

    }
}