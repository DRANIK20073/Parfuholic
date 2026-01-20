using Parfuholic.Models;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System;

namespace Parfuholic.Pages
{
    public partial class AdminOrdersPage : Page
    {
        public ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order>();

        private readonly List<string> Statuses = new List<string>
        {
            "Создан",
            "Собирается на складе",
            "В пути",
            "Доставлен"
        };

        public AdminOrdersPage()
        {
            InitializeComponent();
            OrdersDataGrid.ItemsSource = Orders;
            LoadOrders();
        }

        private void LoadOrders()
        {
            Orders.Clear();

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                // Загружаем заказы с новыми полями
                string sqlOrders = @"
                    SELECT OrderID, OrderDate, TotalSum, Status, FirstName, LastName, City, Address
                    FROM Orders";

                using (SqlCommand cmdOrders = new SqlCommand(sqlOrders, conn))
                using (SqlDataReader readerOrders = cmdOrders.ExecuteReader())
                {
                    while (readerOrders.Read())
                    {
                        Orders.Add(new Order
                        {
                            OrderID = readerOrders.GetInt32(0),
                            OrderDate = readerOrders.GetDateTime(1),
                            TotalSum = readerOrders.GetDecimal(2),
                            Status = readerOrders["Status"] != DBNull.Value ? readerOrders.GetString(3) : "Создан",
                            FirstName = readerOrders["FirstName"] != DBNull.Value ? readerOrders.GetString(4) : "",
                            LastName = readerOrders["LastName"] != DBNull.Value ? readerOrders.GetString(5) : "",
                            City = readerOrders["City"] != DBNull.Value ? readerOrders.GetString(6) : "",
                            Address = readerOrders["Address"] != DBNull.Value ? readerOrders.GetString(7) : ""
                        });
                    }
                }

                // Загружаем товары для каждого заказа
                foreach (var order in Orders)
                {
                    string sqlItems = @"
                        SELECT p.Id, p.Name, p.Brand, p.Price, p.DiscountPercent, p.ImageData, od.Quantity
                        FROM OrderDetails od
                        INNER JOIN Perfumes p ON od.PerfumeID = p.Id
                        WHERE od.OrderID = @orderId";

                    using (SqlCommand cmdItems = new SqlCommand(sqlItems, conn))
                    {
                        cmdItems.Parameters.AddWithValue("@orderId", order.OrderID);

                        using (SqlDataReader readerItems = cmdItems.ExecuteReader())
                        {
                            while (readerItems.Read())
                            {
                                var perfume = new Perfume
                                {
                                    Id = readerItems.GetInt32(0),
                                    Name = readerItems.GetString(1),
                                    Brand = readerItems.GetString(2),
                                    Price = readerItems.GetDecimal(3),
                                    DiscountPercent = readerItems.GetInt32(4),
                                    ImageData = readerItems["ImageData"] != DBNull.Value ? (byte[])readerItems["ImageData"] : null
                                };

                                var orderItem = new OrderItem
                                {
                                    Perfume = perfume,
                                    Quantity = readerItems.GetInt32(6)
                                };

                                order.Items.Add(orderItem);
                            }
                        }
                    }
                }
            }
        }

        // Заполнение ComboBox статусами
        private void StatusComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                comboBox.ItemsSource = Statuses;
            }
        }

        // Сохранение изменений
        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
                {
                    conn.Open();

                    foreach (var order in Orders)
                    {
                        string sql = @"
                            UPDATE Orders
                            SET Status = @status,
                                FirstName = @firstName,
                                LastName = @lastName,
                                City = @city,
                                Address = @address
                            WHERE OrderID = @orderId";

                        using (SqlCommand cmd = new SqlCommand(sql, conn))
                        {
                            cmd.Parameters.AddWithValue("@status", order.Status);
                            cmd.Parameters.AddWithValue("@firstName", order.FirstName ?? "");
                            cmd.Parameters.AddWithValue("@lastName", order.LastName ?? "");
                            cmd.Parameters.AddWithValue("@city", order.City ?? "");
                            cmd.Parameters.AddWithValue("@address", order.Address ?? "");
                            cmd.Parameters.AddWithValue("@orderId", order.OrderID);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }

                MessageBox.Show("Изменения успешно сохранены!", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при сохранении: " + ex.Message, "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
