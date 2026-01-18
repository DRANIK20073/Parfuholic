using Parfuholic.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Parfuholic
{
    public partial class UserMainWindow : Window
    {
        private bool _logoLocked;
        private readonly TimeSpan LogoClickDelay = TimeSpan.FromMilliseconds(400);

        private string _currentCategory = "All";

        public UserMainWindow()
        {
            InitializeComponent();

            // ✅ По умолчанию открываем "Все"
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

        // 📦 ОТКРЫТИЕ КАТАЛОГА
        private void OpenCatalog(string category)
        {
            // ❗ если уже открыта эта же категория — ничего не делаем
            if (CatalogFrame.Content is CatalogPage && _currentCategory == category)
                return;

            _currentCategory = category;

            CatalogFrame.Navigate(new CatalogPage(category));

            // чистим историю
            while (CatalogFrame.CanGoBack)
                CatalogFrame.RemoveBackEntry();
        }

        // 👤 ЛИЧНЫЙ КАБИНЕТ
        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (CatalogFrame.Content is UserCabinetPage)
                return;

            CatalogFrame.Navigate(new UserCabinetPage());

            while (CatalogFrame.CanGoBack)
                CatalogFrame.RemoveBackEntry();
        }

        // 🗂 ВЫБОР КАТЕГОРИИ
        private void Category_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag != null)
            {
                string category = btn.Tag.ToString();
                OpenCatalog(category);
            }
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
