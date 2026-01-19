using System;
using System.Collections.ObjectModel;
using System.Windows.Media;

namespace Parfuholic.Models
{
    public class Order
    {
        public int OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalSum { get; set; }

        private string _status;
        public string Status
        {
            get { return _status; }
            set
            {
                _status = value;
                StatusColor = GetStatusColor(value);
            }
        }

        public Brush StatusColor { get; private set; }
        public string DeliveryInfo { get; set; }

        // Список товаров в заказе
        public ObservableCollection<OrderItem> Items { get; set; } = new ObservableCollection<OrderItem>();

        // Метод для определения цвета статуса
        private Brush GetStatusColor(string status)
        {
            if (status == "Создан")
                return Brushes.Black;
            else if (status == "Собирается на складе")
                return Brushes.Orange;
            else if (status == "В пути")
                return Brushes.Blue;
            else if (status == "Доставлен")
                return Brushes.Green;
            else
                return Brushes.Black;
        }
    }

    // Класс для элемента заказа (один товар и его количество)
    public class OrderItem
    {
        public Perfume Perfume { get; set; }
        public int Quantity { get; set; }

        // Итоговая цена этого товара с учётом скидки
        public decimal TotalPrice
        {
            get
            {
                if (Perfume != null)
                    return Perfume.PriceWithDiscount * Quantity;
                return 0;
            }
        }
    }
}
