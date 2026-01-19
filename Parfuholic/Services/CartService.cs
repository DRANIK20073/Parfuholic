using Parfuholic.Models;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Parfuholic.Services
{
    public static class CartService
    {
        public static ObservableCollection<CartItem> CartItems { get; }
            = new ObservableCollection<CartItem>();

        public static void AddToCart(Perfume perfume, int quantity = 1)
        {
            if (!AuthService.IsLoggedIn)
            {
                MessageBox.Show(
                    "Войдите в аккаунт, чтобы добавить товар в корзину",
                    "Требуется вход",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return;
            }

            var existing = CartItems.FirstOrDefault(i => i.Perfume.Id == perfume.Id);

            if (existing != null)
                existing.Quantity += quantity;
            else
                CartItems.Add(new CartItem
                {
                    Perfume = perfume,
                    Quantity = quantity
                });
        }

        public static void RemoveFromCart(CartItem item)
        {
            CartItems.Remove(item);
        }

        public static decimal GetTotal()
        {
            return CartItems.Sum(i => i.Perfume.PriceWithDiscount * i.Quantity);
        }
    }
}
