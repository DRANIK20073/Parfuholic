using System.IO;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;

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

        // из БД
        public byte[] ImageData { get; set; }

        // для XAML
        public BitmapImage Image
        {
            get
            {
                if (ImageData == null || ImageData.Length == 0)
                    return null;

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
