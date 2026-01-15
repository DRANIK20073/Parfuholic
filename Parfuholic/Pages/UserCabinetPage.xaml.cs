using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Parfuholic
{
    public partial class UserCabinetPage : Page
    {
        public UserCabinetPage()
        {
            InitializeComponent();
        }

        private void Logout_Click(object sender, MouseButtonEventArgs e)
        {
            // Возврат к окну до входа
            NavigationWindow nav = new NavigationWindow();
            nav.Show();

            Application.Current.Windows
                .OfType<UserMainWindow>()
                .FirstOrDefault()
                ?.Close();
        }
    }
}
