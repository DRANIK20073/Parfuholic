using System.Windows;
using System.Windows.Controls;
using WpfApp;

namespace Parfuholic
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Создаём объект DatabaseHelper
            DatabaseHelper dbHelper = new DatabaseHelper();

            // Проверяем подключение
            dbHelper.TestConnection();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            // Навигация на страницу входа
            LoginPage.Navigate(new LoginPage());
        }
    }
}
