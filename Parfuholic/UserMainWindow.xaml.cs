using Parfuholic.Pages;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Parfuholic
{
    public partial class UserMainWindow : Window
    {
        private bool _logoLocked;
        private readonly TimeSpan LogoClickDelay = TimeSpan.FromMilliseconds(400);

        public UserMainWindow()
        {
            InitializeComponent();

            // ✅ Загружаем каталог ОДИН РАЗ
            CatalogFrame.Navigate(new CatalogPage());
        }

        // 🏠 ЛОГО → В КАТАЛОГ
        private void LogoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_logoLocked)
                return;

            _logoLocked = true;

            OpenCatalog();

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
            // ❗ если каталог уже открыт — просто выходим
            if (CatalogFrame.Content is CatalogPage)
                return;

            CatalogFrame.Navigate(new CatalogPage());

            // чистим историю, чтобы Back не ломал логику
            while (CatalogFrame.CanGoBack)
                CatalogFrame.RemoveBackEntry();
        }

        // 👤 КАБИНЕТ
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            // если кабинет уже открыт — ничего не делаем
            if (CatalogFrame.Content is UserCabinetPage)
                return;

            CatalogFrame.Navigate(new UserCabinetPage());

            while (CatalogFrame.CanGoBack)
                CatalogFrame.RemoveBackEntry();
        }

        // 🚪 ВЫХОД
        public void Logout()
        {
            NavigationWindow nav = new NavigationWindow();
            nav.Show();
            Close();
        }
    }
}
