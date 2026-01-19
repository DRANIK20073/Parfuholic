using Parfuholic.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Parfuholic
{
    public partial class UserMainWindow : Window
    {
        private int loggedUserId;
        private bool _logoLocked = false;
        private readonly TimeSpan LogoClickDelay = TimeSpan.FromMilliseconds(400);
        private string _currentCategory = "All";

        public UserMainWindow(int userId)
        {
            InitializeComponent();
            loggedUserId = userId;

            // По умолчанию открываем каталог "Все"
            OpenCatalog("All");
        }

        private void LogoButton_Click(object sender, RoutedEventArgs e)
        {
            if (_logoLocked) return;

            _logoLocked = true;
            OpenCatalog("All");

            DispatcherTimer timer = new DispatcherTimer { Interval = LogoClickDelay };
            timer.Tick += (s, args) => { _logoLocked = false; timer.Stop(); };
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

        private void OpenCatalog(string category)
        {
            if (CatalogFrame.Content is CatalogPage && _currentCategory == category) return;

            _currentCategory = category;
            CatalogFrame.Navigate(new CatalogPage(category));

            while (CatalogFrame.CanGoBack)
                CatalogFrame.RemoveBackEntry();
        }

        private void AccountButton_Click(object sender, RoutedEventArgs e)
        {
            if (CatalogFrame.Content is UserCabinetPage) return;

            CatalogFrame.Navigate(new UserCabinetPage(loggedUserId));

            while (CatalogFrame.CanGoBack)
                CatalogFrame.RemoveBackEntry();
        }

        private void CartButton_Click(object sender, RoutedEventArgs e)
        {
            CatalogFrame.Navigate(new CartPage(loggedUserId));
        }

        public void Logout()
        {
            NavigationWindow nav = new NavigationWindow();
            nav.Show();
            Close();
        }
    }
}
