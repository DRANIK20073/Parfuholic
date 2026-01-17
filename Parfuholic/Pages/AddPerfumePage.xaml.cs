using Microsoft.Win32;
using System;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Globalization;

namespace Parfuholic.Pages
{
    public partial class AddPerfumePage : Page
    {
        private byte[] imageData;

        public AddPerfumePage()
        {
            InitializeComponent();
        }

        private void SelectImage_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Filter = "Images (*.png;*.jpg)|*.png;*.jpg";

            if (dialog.ShowDialog() == true)
            {
                imageData = File.ReadAllBytes(dialog.FileName);

                // Показать превью
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = new MemoryStream(imageData);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                PerfumeImagePreview.Source = bitmap;
                PerfumeImagePreview.Visibility = Visibility.Visible;
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // 1. Проверка обязательных полей
            if (string.IsNullOrWhiteSpace(NameBox.Text) ||
                string.IsNullOrWhiteSpace(BrandBox.Text) ||
                string.IsNullOrWhiteSpace(ForWhomBox.Text))
            {
                MessageBox.Show("Заполните название, бренд и для кого предназначен парфюм.",
                                "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // 2. Проверка числовых полей
            string priceInput = PriceBox.Text.Trim().Replace(',', '.');
            if (!decimal.TryParse(priceInput, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out decimal price))
            {
                MessageBox.Show("Введите корректную цену. Пример: 221.25", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!int.TryParse(QuantityBox.Text.Trim(), out int quantity))
            {
                MessageBox.Show("Введите корректное количество.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(@"
                        INSERT INTO Perfumes
                        (Name, Brand, ForWhom, AromaGroup, TopNotes, MiddleNotes, BaseNotes, Volume, Price, Quantity, ImageData)
                        VALUES
                        (@Name, @Brand, @ForWhom, @AromaGroup, @TopNotes, @MiddleNotes, @BaseNotes, @Volume, @Price, @Quantity, @ImageData)", conn);

                    cmd.Parameters.AddWithValue("@Name", NameBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@Brand", BrandBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@ForWhom", ForWhomBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@AromaGroup", AromaGroupBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@TopNotes", TopNotesBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@MiddleNotes", MiddleNotesBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@BaseNotes", BaseNotesBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@Volume", VolumeBox.Text.Trim());
                    cmd.Parameters.AddWithValue("@Price", price);
                    cmd.Parameters.AddWithValue("@Quantity", quantity);

                    if (imageData != null)
                        cmd.Parameters.AddWithValue("@ImageData", imageData);
                    else
                        cmd.Parameters.AddWithValue("@ImageData", DBNull.Value);

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Парфюм успешно добавлен!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

                // Очистка полей и скрытие превью
                NameBox.Clear();
                BrandBox.Clear();
                ForWhomBox.Clear();
                AromaGroupBox.Clear();
                TopNotesBox.Clear();
                MiddleNotesBox.Clear();
                BaseNotesBox.Clear();
                VolumeBox.Clear();
                PriceBox.Clear();
                QuantityBox.Clear();
                imageData = null;
                PerfumeImagePreview.Source = null;
                PerfumeImagePreview.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при добавлении парфюма: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
