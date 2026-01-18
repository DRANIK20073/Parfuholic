using Parfuholic.Models;
using Parfuholic.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Parfuholic.Pages
{
    public partial class CartPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<CartItem> CartItems => CartService.CartItems;

        // Итоговая цена со скидкой
        private decimal _itemsTotal;
        public decimal ItemsTotal
        {
            get => _itemsTotal;
            set
            {
                _itemsTotal = value;
                OnPropertyChanged(nameof(ItemsTotal));
                OnPropertyChanged(nameof(TotalSum));
                OnPropertyChanged(nameof(DiscountSum));
                OnPropertyChanged(nameof(OriginalTotal));
            }
        }

        // Итоговая цена без скидок
        public decimal OriginalTotal => CartItems.Sum(i => i.Perfume.Price * i.Quantity);

        // Сумма скидки
        public decimal DiscountSum => OriginalTotal - ItemsTotal;

        // Итоговая цена с учетом скидки
        public decimal TotalSum => ItemsTotal;

        // Булево: есть ли скидка
        public bool HasDiscount => DiscountSum > 0;

        public bool HasItems => CartItems.Count > 0;

        public CartPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadCartItems();

            CartItems.CollectionChanged += (s, e) =>
            {
                UpdateCartSummary();
                UpdateEmptyMessage();
            };
        }

        private void LoadCartItems()
        {
            UpdateCartSummary();
            UpdateEmptyMessage();
        }

        private void UpdateCartSummary()
        {
            ItemsTotal = CartService.GetTotal();
            OnPropertyChanged(nameof(HasItems));
            OnPropertyChanged(nameof(HasDiscount));
        }

        private void UpdateEmptyMessage()
        {
            if (EmptyCartMessage != null)
            {
                EmptyCartMessage.Visibility = HasItems ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CartItem item)
            {
                item.Quantity++;
                UpdateCartSummary();
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CartItem item)
            {
                if (item.Quantity > 1)
                {
                    item.Quantity--;
                    UpdateCartSummary();
                }
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is CartItem item)
            {
                CartService.RemoveFromCart(item);
                UpdateCartSummary();
                UpdateEmptyMessage();
            }
        }

        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!HasItems)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(PhoneBox.Text) ||
                string.IsNullOrWhiteSpace(EmailBox.Text) ||
                string.IsNullOrWhiteSpace(FirstNameBox.Text) ||
                string.IsNullOrWhiteSpace(LastNameBox.Text) ||
                string.IsNullOrWhiteSpace(CityBox.Text) ||
                string.IsNullOrWhiteSpace(AddressBox.Text))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                SaveOrderToDatabase();
                MessageBox.Show("Заказ успешно оформлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                CartService.CartItems.Clear();
                UpdateCartSummary();
                UpdateEmptyMessage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при оформлении заказа: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveOrderToDatabase()
        {
            // TODO: Реализовать сохранение заказа в БД
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
