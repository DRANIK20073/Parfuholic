using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Data.SqlClient;
using Parfuholic.Services;
using Parfuholic.Models;
using System.Collections.Generic;
using System.IO;
using System.Windows.Controls;
using System.Windows;
using System;

namespace Parfuholic.Pages
{
    public partial class PerfumePage : Page
    {
        private Perfume selectedVariant;
        private List<Perfume> variants;
        private string initialVolume;

        private bool _isFavorite; // состояние избранного

        public PerfumePage(int perfumeId, string selectedVolume)
        {
            InitializeComponent();
            initialVolume = selectedVolume;

            LoadPerfumeVariants(perfumeId);

            if (variants.Count > 0)
            {
                selectedVariant = variants.Find(v => v.Volume == initialVolume) ?? variants[0];
                LoadData(selectedVariant);
                CreateVolumeButtons();

                // Проверяем, есть ли этот вариант в избранном
                if (AuthService.IsLoggedIn)
                {
                    if (AuthService.CurrentUserId.HasValue)
                    {
                        _isFavorite = FavoritesService.IsFavorite(AuthService.CurrentUserId.Value, selectedVariant.Id);
                    }
                    else
                    {
                        _isFavorite = false;
                    }
                    UpdateFavoriteButton();
                }
            }
        }

        private void LoadData(Perfume p)
        {
            NameText.Text = p.Name;
            BrandText.Text = p.Brand;
            ForWhomText.Text = p.ForWhom;
            AromaGroupText.Text = p.AromaGroup;
            BaseNotesText.Text = p.BaseNotes;
            MiddleNotesText.Text = p.MiddleNotes;
            TopNotesText.Text = p.TopNotes;
            VolumeText.Text = p.Volume;

            // ===== Цена и скидка =====
            if (p.IsDiscount && p.DiscountPercent > 0)
            {
                decimal discountedPrice = Math.Round(p.Price * (100 - p.DiscountPercent) / 100, 2);
                PriceText.Text = $"{discountedPrice:F2} BYN";
                PriceText.Foreground = Brushes.Black;
                PriceText.FontSize = 28;
                PriceText.FontWeight = FontWeights.Bold;

                DiscountLabel.Text = $"со скидкой {p.DiscountPercent}%";
                DiscountLabel.Visibility = Visibility.Visible;
                DiscountLabel.Foreground = Brushes.Black;
                DiscountLabel.FontSize = 14;

                OldPriceText.Text = $"{p.Price:F2} BYN";
                OldPriceText.Visibility = Visibility.Visible;
                OldPriceText.Foreground = Brushes.Gray;
                OldPriceText.FontSize = 28;
                OldPriceText.FontWeight = FontWeights.Bold;
                OldPriceText.TextDecorations = TextDecorations.Strikethrough;

                OldPriceLabel.Visibility = Visibility.Visible;
                OldPriceLabel.Foreground = Brushes.Gray;
                OldPriceLabel.FontSize = 14;
            }
            else
            {
                PriceText.Text = $"{p.Price:F2} BYN";
                PriceText.Foreground = Brushes.Black;
                PriceText.FontSize = 28;
                PriceText.FontWeight = FontWeights.Bold;

                DiscountLabel.Visibility = Visibility.Collapsed;
                OldPriceText.Visibility = Visibility.Collapsed;
                OldPriceLabel.Visibility = Visibility.Collapsed;
            }

            // ===== Изображение =====
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

        private void LoadPerfumeVariants(int perfumeId)
        {
            variants = new List<Perfume>();

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
                    SELECT * FROM Perfumes
                    WHERE Name = (SELECT Name FROM Perfumes WHERE Id = @id)
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
                        Quantity = Convert.ToInt32(reader["Quantity"]),
                        ImageData = reader["ImageData"] as byte[],
                        IsDiscount = Convert.ToBoolean(reader["IsDiscount"])
                    };

                    object discountPercentValue = reader["DiscountPercent"];
                    if (discountPercentValue != DBNull.Value && discountPercentValue != null)
                    {
                        if (discountPercentValue is decimal)
                            p.DiscountPercent = Convert.ToInt32((decimal)discountPercentValue);
                        else if (discountPercentValue is int)
                            p.DiscountPercent = (int)discountPercentValue;
                        else
                            p.DiscountPercent = Convert.ToInt32(discountPercentValue);
                    }
                    else
                    {
                        p.DiscountPercent = 0;
                    }

                    variants.Add(p);
                }
            }

            variants.Sort((a, b) => ExtractVolume(a.Volume).CompareTo(ExtractVolume(b.Volume)));
        }

        private int ExtractVolume(string volume)
        {
            if (string.IsNullOrEmpty(volume)) return 0;
            string digits = "";
            foreach (char c in volume)
            {
                if (char.IsDigit(c)) digits += c;
                else break;
            }
            return int.TryParse(digits, out int val) ? val : 0;
        }

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
                    CornerRadius = new CornerRadius(0)
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

                border.MouseLeftButtonUp += (s, e) =>
                {
                    selectedVariant = variant;
                    LoadData(selectedVariant);
                    UpdateVolumeButtons();

                    // обновляем состояние кнопки избранного
                    if (AuthService.IsLoggedIn)
                    {
                        if (AuthService.CurrentUserId.HasValue)
                        {
                            _isFavorite = FavoritesService.IsFavorite(AuthService.CurrentUserId.Value, selectedVariant.Id);
                        }
                        else
                        {
                            _isFavorite = false;
                        }
                        UpdateFavoriteButton();
                    }
                };

                VolumeButtonsPanel.Children.Add(border);
            }
        }

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

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (NavigationService.CanGoBack)
                NavigationService.GoBack();
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            if (!AuthService.IsLoggedIn)
            {
                MessageBox.Show(
                    "Чтобы добавить товар в корзину, войдите в аккаунт.",
                    "Требуется вход",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning
                );
                return;
            }

            if (selectedVariant != null)
            {
                CartService.AddToCart(selectedVariant);

                MessageBox.Show(
                    $"{selectedVariant.Name} добавлен в корзину",
                    "Добавлено",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information
                );
            }
        }

        private void UpdateFavoriteButton()
        {
            if (_isFavorite)
            {
                AddToFavoritesButton.Content = "-";
                AddToFavoritesButton.Background = Brushes.White;
                AddToFavoritesButton.Foreground = Brushes.Black;
            }
            else
            {
                AddToFavoritesButton.Content = "+";
                AddToFavoritesButton.Background = Brushes.Black;
                AddToFavoritesButton.Foreground = Brushes.White;
            }
        }

        private void AddToFavoritesButton_Click(object sender, RoutedEventArgs e)
        {
            if (selectedVariant == null || !AuthService.IsLoggedIn) return;

            if (!AuthService.CurrentUserId.HasValue) return;
            int userId = AuthService.CurrentUserId.Value;

            if (_isFavorite)
            {
                FavoritesService.Remove(userId, selectedVariant.Id);
                _isFavorite = false;
                MessageBox.Show($"{selectedVariant.Name} удалён из избранного", "Избранное", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                FavoritesService.Add(userId, selectedVariant);
                _isFavorite = true;
                MessageBox.Show($"{selectedVariant.Name} добавлен в избранное", "Избранное", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            UpdateFavoriteButton();
        }
    }
}
