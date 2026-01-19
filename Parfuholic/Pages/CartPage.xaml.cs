using Parfuholic.Models;
using Parfuholic.Services;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Parfuholic.Pages
{
    public partial class CartPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<CartItem> CartItems => CartService.CartItems;

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

        public decimal OriginalTotal => CartItems.Sum(i => i.Perfume.Price * i.Quantity);
        public decimal DiscountSum => OriginalTotal - ItemsTotal;
        public decimal TotalSum => ItemsTotal;
        public bool HasDiscount => DiscountSum > 0;
        public bool HasItems => CartItems.Count > 0;

        private User currentUser;

        // ===== Привязка полей корзины =====
        private string _firstName;
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(nameof(FirstName)); } }

        private string _lastName;
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(nameof(LastName)); } }

        private string _phone;
        public string Phone { get => _phone; set { _phone = value; OnPropertyChanged(nameof(Phone)); } }

        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(nameof(Email)); } }

        private string _city;
        public string City { get => _city; set { _city = value; OnPropertyChanged(nameof(City)); } }

        private string _address;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(nameof(Address)); } }

        private int currentUserId;

        public CartPage(int userId)
        {
            InitializeComponent();
            DataContext = this;

            currentUserId = userId;

            LoadCartItems();
            LoadUserData(userId);

            CartItems.CollectionChanged += (s, e) =>
            {
                UpdateCartSummary();
                UpdateEmptyMessage();
            };

            PhoneBox.PreviewTextInput += PhoneBox_PreviewTextInput;
            PhoneBox.TextChanged += PhoneBox_TextChanged;
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
                EmptyCartMessage.Visibility = HasItems ? Visibility.Collapsed : Visibility.Visible;
        }

        private void LoadUserData(int userId)
        {
            string query = "SELECT * FROM Users WHERE UserID=@UserID";
            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@UserID", userId);
                try
                {
                    conn.Open();
                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        currentUser = new User
                        {
                            UserId = userId,
                            FirstName = reader["FirstName"].ToString(),
                            LastName = reader["LastName"].ToString(),
                            Phone = reader["Phone"].ToString(),
                            Email = reader["Email"].ToString(),
                            City = reader["City"].ToString(),
                            Address = reader["Address"].ToString()
                        };

                        // ✅ Заполняем привязанные свойства
                        FirstName = currentUser.FirstName;
                        LastName = currentUser.LastName;
                        Phone = currentUser.Phone;
                        Email = currentUser.Email;
                        City = currentUser.City;
                        Address = currentUser.Address;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ошибка загрузки данных пользователя: {ex.Message}");
                }
            }
        }

        // ===== Кнопки управления корзиной =====
        private void IncreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CartItem item)
            {
                item.Quantity++;
                UpdateCartSummary();
            }
        }

        private void DecreaseQuantity_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CartItem item && item.Quantity > 1)
            {
                item.Quantity--;
                UpdateCartSummary();
            }
        }

        private void RemoveItem_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is CartItem item)
            {
                CartService.RemoveFromCart(item);
                UpdateCartSummary();
                UpdateEmptyMessage();
            }
        }

        // ===== Телефон =====
        private void PhoneBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Regex.IsMatch(e.Text, "[0-9]");
        }

        private void PhoneBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var textBox = sender as TextBox;
            if (textBox == null) return;

            int selStart = textBox.SelectionStart;
            string digits = Regex.Replace(textBox.Text, @"\D", "");

            if (digits.StartsWith("375"))
                digits = digits.Substring(3);

            if (digits.Length > 9)
                digits = digits.Substring(0, 9);

            string formatted = "+375";
            if (digits.Length > 0)
                formatted += "(" + digits.Substring(0, Math.Min(2, digits.Length));
            if (digits.Length > 2)
                formatted += ")" + digits.Substring(2, Math.Min(3, digits.Length - 2));
            if (digits.Length > 5)
                formatted += "-" + digits.Substring(5, Math.Min(2, digits.Length - 5));
            if (digits.Length > 7)
                formatted += "-" + digits.Substring(7, Math.Min(2, digits.Length - 7));

            int digitsBeforeCursor = Regex.Replace(textBox.Text.Substring(0, selStart), @"\D", "").Length;
            int newCursorPos = 4;
            int dCount = 0;
            for (int i = 4; i < formatted.Length && dCount < digitsBeforeCursor; i++)
            {
                if (Char.IsDigit(formatted[i]))
                    dCount++;
                newCursorPos = i + 1;
            }

            textBox.Text = formatted;
            textBox.SelectionStart = newCursorPos;
        }

        // ===== Checkout =====
        private void CheckoutButton_Click(object sender, RoutedEventArgs e)
        {
            if (!HasItems)
            {
                MessageBox.Show("Корзина пуста!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(Phone) ||
                string.IsNullOrWhiteSpace(Email) ||
                string.IsNullOrWhiteSpace(FirstName) ||
                string.IsNullOrWhiteSpace(LastName) ||
                string.IsNullOrWhiteSpace(City) ||
                string.IsNullOrWhiteSpace(Address))
            {
                MessageBox.Show("Заполните все поля!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var phoneDigits = new string(Phone.Where(char.IsDigit).ToArray());
            if (phoneDigits.Length != 12)
            {
                MessageBox.Show("Введите корректный номер телефона!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
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
            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                // ===== 1. Сохраняем запись в Orders =====
                string orderQuery = @"
                    INSERT INTO Orders (UserID, OrderDate, FirstName, LastName, Phone, Email, City, Address, ItemsTotal, DiscountSum, TotalSum, Status)
                    OUTPUT INSERTED.OrderID
                    VALUES (@UserID, @OrderDate, @FirstName, @LastName, @Phone, @Email, @City, @Address, @ItemsTotal, @DiscountSum, @TotalSum, @Status)";

                int orderId;
                using (SqlCommand cmd = new SqlCommand(orderQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", currentUserId);
                    cmd.Parameters.AddWithValue("@OrderDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@FirstName", FirstName);
                    cmd.Parameters.AddWithValue("@LastName", LastName);
                    cmd.Parameters.AddWithValue("@Phone", Phone);
                    cmd.Parameters.AddWithValue("@Email", Email);
                    cmd.Parameters.AddWithValue("@City", City);
                    cmd.Parameters.AddWithValue("@Address", Address);
                    cmd.Parameters.AddWithValue("@ItemsTotal", OriginalTotal);
                    cmd.Parameters.AddWithValue("@DiscountSum", DiscountSum);
                    cmd.Parameters.AddWithValue("@TotalSum", TotalSum);
                    cmd.Parameters.AddWithValue("@Status", "В обработке");

                    orderId = (int)cmd.ExecuteScalar();
                }

                // ===== 2. Сохраняем детали заказа в OrderDetails =====
                string detailQuery = @"
                    INSERT INTO OrderDetails (OrderID, PerfumeID, Quantity)
                    VALUES (@OrderID, @PerfumeID, @Quantity)";

                foreach (var item in CartItems)
                {
                    using (SqlCommand cmdDetail = new SqlCommand(detailQuery, conn))
                    {
                        cmdDetail.Parameters.AddWithValue("@OrderID", orderId);
                        cmdDetail.Parameters.AddWithValue("@PerfumeID", item.Perfume.Id); // поле Id в таблице Perfumes
                        cmdDetail.Parameters.AddWithValue("@Quantity", item.Quantity);
                        cmdDetail.ExecuteNonQuery();
                    }
                }
            }
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
