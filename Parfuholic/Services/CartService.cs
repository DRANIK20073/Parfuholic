using Parfuholic.Models;
using System.Collections.ObjectModel;
using System.Linq;

namespace Parfuholic.Services
{
    public static class CartService
    {
        public static ObservableCollection<CartItem> CartItems { get; } = new ObservableCollection<CartItem>();

        public static void AddToCart(Perfume perfume, int quantity = 1)
        {
            var existingItem = CartItems.FirstOrDefault(item => item.Perfume.Id == perfume.Id);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                CartItems.Add(new CartItem
                {
                    Perfume = perfume,
                    Quantity = quantity
                });
            }
        }

        public static void RemoveFromCart(CartItem item)
        {
            CartItems.Remove(item);
        }

        public static decimal GetTotal()
        {
            return CartItems.Sum(item => item.TotalPrice);
        }

        public static int GetItemCount()
        {
            return CartItems.Sum(item => item.Quantity);
        }

    }
}