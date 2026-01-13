using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Parfuholic
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
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
    }
}