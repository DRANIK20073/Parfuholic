using Parfuholic.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;

namespace Parfuholic.Pages
{
    public partial class PerfumePage : Page
    {
        private Perfume selectedVariant;
        private List<Perfume> variants; // все варианты одного парфюма (разные объёмы)
        private string initialVolume;    // объем, на который пользователь перешёл

        public PerfumePage(int perfumeId, string selectedVolume)
        {
            InitializeComponent();

            initialVolume = selectedVolume;

            LoadPerfumeVariants(perfumeId); // загружаем все объёмы одного парфюма

            if (variants.Count > 0)
            {
                // выбираем вариант по переданному объему, иначе первый
                selectedVariant = variants.Find(v => v.Volume == initialVolume) ?? variants[0];

                LoadData(selectedVariant);
                CreateVolumeButtons();
            }
        }

        // ===== Загрузка данных выбранного варианта =====
        private void LoadData(Perfume p)
        {
            NameText.Text = p.Name;
            BrandText.Text = p.Brand;
            PriceText.Text = $"{p.Price} BYN";

            ForWhomText.Text = p.ForWhom;
            AromaGroupText.Text = p.AromaGroup;
            BaseNotesText.Text = p.BaseNotes;
            MiddleNotesText.Text = p.MiddleNotes;
            TopNotesText.Text = p.TopNotes;
            VolumeText.Text = p.Volume;

            if (p.ImageData != null)
            {
                BitmapImage image = new BitmapImage();
                using (MemoryStream ms = new MemoryStream(p.ImageData))
                {
                    image.BeginInit();
                    image.CacheOption = BitmapCacheOption.OnLoad;
                    image.StreamSource = ms;
                    image.EndInit();
                    image.Freeze();
                }
                PerfumeImage.Source = image;
            }
        }

        // ===== Загрузка всех вариантов одного парфюма =====
        private void LoadPerfumeVariants(int perfumeId)
        {
            variants = new List<Perfume>();

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
            SELECT * FROM Perfumes
            WHERE Id = @id OR Name = (SELECT Name FROM Perfumes WHERE Id = @id)
        ", conn);

                cmd.Parameters.AddWithValue("@id", perfumeId);

                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    Perfume p = new Perfume
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Brand = reader["Brand"].ToString(),
                        ForWhom = reader["ForWhom"].ToString(),
                        AromaGroup = reader["AromaGroup"].ToString(),
                        BaseNotes = reader["BaseNotes"].ToString(),
                        MiddleNotes = reader["MiddleNotes"].ToString(),
                        TopNotes = reader["TopNotes"].ToString(),
                        Volume = reader["Volume"].ToString(),
                        Price = Convert.ToDecimal(reader["Price"]),
                        Quantity = Convert.ToDecimal(reader["Quantity"]),
                        ImageData = reader["ImageData"] as byte[]
                    };

                    variants.Add(p);
                }
            }

            // ===== Сортировка по числу объема в мл =====
            variants.Sort((a, b) =>
            {
                int volA = ExtractVolume(a.Volume);
                int volB = ExtractVolume(b.Volume);
                return volA.CompareTo(volB);
            });
        }

        // ===== Вспомогательный метод: извлекает число из строки объема =====
        private int ExtractVolume(string volume)
        {
            if (string.IsNullOrEmpty(volume))
                return 0;

            // ищем цифры в начале строки
            string digits = "";
            foreach (char c in volume)
            {
                if (char.IsDigit(c))
                    digits += c;
                else
                    break;
            }

            return int.TryParse(digits, out int val) ? val : 0;
        }

        // ===== Создание кнопок объёмов =====
        private void CreateVolumeButtons()
        {
            VolumeButtonsPanel.Children.Clear();

            foreach (var variant in variants)
            {
                Border border = new Border
                {
                    Width = 60,
                    Height = 60,
                    Margin = new Thickness(5, 0, 0, 0),
                    BorderThickness = new Thickness(2),
                    BorderBrush = variant == selectedVariant ? Brushes.Black : (Brush)new BrushConverter().ConvertFromString("#B3B3B3"),
                    Background = Brushes.White,
                    Cursor = System.Windows.Input.Cursors.Hand,
                    CornerRadius = new CornerRadius(0) // квадратные кнопки
                };

                TextBlock text = new TextBlock
                {
                    Text = variant.Volume,
                    FontSize = 14,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = Brushes.Black,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };

                border.Child = text;

                // отключаем стандартное выделение при наведении
                border.MouseEnter += (s, e) => { border.Background = Brushes.White; };
                border.MouseLeave += (s, e) => { border.Background = Brushes.White; };

                // клик по Border
                border.MouseLeftButtonUp += (s, e) =>
                {
                    selectedVariant = variant;
                    LoadData(selectedVariant);
                    UpdateVolumeButtons();
                };

                VolumeButtonsPanel.Children.Add(border);
            }
        }

        // ===== Обновление подсветки выбранного варианта =====
        private void UpdateVolumeButtons()
        {
            for (int i = 0; i < VolumeButtonsPanel.Children.Count; i++)
            {
                Border border = VolumeButtonsPanel.Children[i] as Border;
                Perfume variant = variants[i];

                border.BorderBrush = variant == selectedVariant
                    ? Brushes.Black
                    : (Brush)new BrushConverter().ConvertFromString("#B3B3B3");
            }
        }
    }
}
