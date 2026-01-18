using Parfuholic.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Parfuholic
{
    public partial class NavigationWindow : Window
    {

        private bool _logoLocked = false;
        private readonly TimeSpan LogoClickDelay = TimeSpan.FromMilliseconds(600);

        public NavigationWindow()
        {
            InitializeComponent();
            CatalogFrame.Navigate(new CatalogPage());
        }

        private void LogoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_logoLocked)
                return;

            // 🔒 блокируем повторный клик
            _logoLocked = true;

            OpenCatalog();

            // ⏳ разблокировка через 600 мс
            DispatcherTimer timer = new DispatcherTimer
            {
                Interval = LogoClickDelay
            };
            timer.Tick += (s, args) =>
            {
                _logoLocked = false;
                timer.Stop();
            };
            timer.Start();
        }

        private void OpenCatalog()
        {
            // ❗ если каталог уже открыт — ничего не делаем
            if (CatalogFrame.Content is CatalogPage)
                return;

            // 🔥 полностью очищаем старую страницу
            CatalogFrame.Content = null;

            GC.Collect();
            GC.WaitForPendingFinalizers();

            // 🔁 загружаем заново
            CatalogFrame.Navigate(new CatalogPage());

            // ❌ убираем историю
            while (CatalogFrame.CanGoBack)
                CatalogFrame.RemoveBackEntry();
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