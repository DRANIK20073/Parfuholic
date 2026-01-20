using Parfuholic.Models;
using Parfuholic.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Parfuholic.Pages
{
    public partial class FavoritesPage : Page
    {
        public ObservableCollection<Perfume> Favorites { get; set; } = new ObservableCollection<Perfume>();

        public FavoritesPage()
        {
            InitializeComponent();

            // Получаем текущего пользователя (обязательно int)
            int userId = AuthService.CurrentUserId ?? 0; // если CurrentUserId int?, приводим к int

            // Загружаем избранное
            var favoritesList = FavoritesService.GetFavorites(userId);
            Favorites = new ObservableCollection<Perfume>(favoritesList);
            FavoritesItemsControl.ItemsSource = Favorites;

            // Проверка на пустоту
            NoFavoritesMessage.Visibility = Favorites.Any() ? Visibility.Collapsed : Visibility.Visible;
        }

        private void PerfumeCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (sender is Border border && border.DataContext is Perfume perfume)
            {
                NavigationService?.Navigate(new PerfumePage(perfume.Id, perfume.Volume));
            }
        }
    }
}
