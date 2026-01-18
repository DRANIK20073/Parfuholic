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
        private string _currentCategory = "All";

        public NavigationWindow()
        {
            InitializeComponent();

            // ✅ по умолчанию — ВСЕ
            OpenCatalog("All");
        }

        // 🏠 ЛОГО → ВСЕ ПАРФЮМЫ
        private void LogoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_logoLocked)
                return;

            _logoLocked = true;

            OpenCatalog("All");

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

        private void Category_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                string category = btn.Tag.ToString();
                OpenCatalog(category);
            }
        }

        // 📦 ОТКРЫТИЕ КАТАЛОГА С КАТЕГОРИЕЙ
        private void OpenCatalog(string category)
        {
            if (CatalogFrame.Content is CatalogPage && _currentCategory == category)
                return;

            _currentCategory = category;

            CatalogFrame.Navigate(new CatalogPage(category));

            while (CatalogFrame.CanGoBack)
                CatalogFrame.RemoveBackEntry();
        }

        // 🔐 ВХОД
        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginPage.Visibility = Visibility.Visible;
            LoginPage.Navigate(new LoginPage());

            while (LoginPage.CanGoBack)
                LoginPage.RemoveBackEntry();
        }

        // 🖱 КЛИК ПО ФОНУ — ЗАКРЫТЬ ОВЕРЛЕЙ
        private void OverlayFrame_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            CloseOverlay();
        }

        // ❌ ЗАКРЫТИЕ ОВЕРЛЕЯ
        public void CloseOverlay()
        {
            LoginPage.Visibility = Visibility.Collapsed;
            LoginPage.Content = null;

            while (LoginPage.CanGoBack)
                LoginPage.RemoveBackEntry();
        }

        // 👤 ПЕРЕХОД В РЕЖИМ ПОЛЬЗОВАТЕЛЯ
        public void OpenUserMode()
        {
            UserMainWindow userWindow = new UserMainWindow();
            userWindow.Show();
            Close();
        }

        //Корзина
        private void Cart_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Для доступа к корзине необходимо войти в аккаунт.",
                          "Требуется авторизация",
                          MessageBoxButton.OK,
                          MessageBoxImage.Information);
        }

    }
}
