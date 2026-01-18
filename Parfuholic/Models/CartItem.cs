using System.ComponentModel;

namespace Parfuholic.Models
{
    public class CartItem : INotifyPropertyChanged
    {
        private Perfume _perfume;
        public Perfume Perfume
        {
            get => _perfume;
            set
            {
                _perfume = value;
                OnPropertyChanged(nameof(Perfume));
                OnPropertyChanged(nameof(Price));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        private int _quantity;
        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged(nameof(Quantity));
                OnPropertyChanged(nameof(TotalPrice));
            }
        }

        // ТОЛЬКО ДЛЯ ЧТЕНИЯ - вычисляемое свойство
        public decimal Price
        {
            get
            {
                if (Perfume == null) return 0;
                if (Perfume.IsDiscount && Perfume.DiscountPercent > 0)
                {
                    return Perfume.Price * (100 - Perfume.DiscountPercent) / 100;
                }
                return Perfume.Price;
            }
        }

        // ТОЛЬКО ДЛЯ ЧТЕНИЯ - вычисляемое свойство
        public decimal TotalPrice => Price * Quantity;

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}