using System.IO;
using System.Windows.Media.Imaging;

namespace Parfuholic.Models
{
    public class Perfume
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ForWhom { get; set; }
        public string AromaGroup { get; set; }
        public string BaseNotes { get; set; }
        public string MiddleNotes { get; set; }
        public string TopNotes { get; set; }
        public string Volume { get; set; }
        public string Brand { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public bool IsNew { get; set; }
        public bool IsDiscount { get; set; }

        private int _discountPercent;
        public int DiscountPercent
        {
            get => _discountPercent;
            set
            {
                _discountPercent = value;
                IsDiscount = _discountPercent > 0;
            }
        }

        public decimal PriceWithDiscount
        {
            get
            {
                if (IsDiscount && DiscountPercent > 0)
                {
                    // Вычисляем цену со скидкой и округляем до 2 знаков
                    decimal discountedPrice = Price * (100 - DiscountPercent) / 100;
                    return decimal.Round(discountedPrice, 2);
                }
                else
                {
                    // Без скидки - возвращаем обычную цену
                    return decimal.Round(Price, 2);
                }
            }
        }

        public byte[] ImageData { get; set; }
        public BitmapImage Image
        {
            get
            {
                if (ImageData == null || ImageData.Length == 0) return null;
                using (var ms = new MemoryStream(ImageData))
                {
                    BitmapImage image = new BitmapImage();
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                    return image;
                }
            }
        }
    }
}