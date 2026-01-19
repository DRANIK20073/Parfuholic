using Parfuholic.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Parfuholic.Pages
{
    public partial class OrdersHistoryPage : Page
    {
        private int currentUserId;

        public ObservableCollection<Order> Orders { get; set; } = new ObservableCollection<Order>();

        public OrdersHistoryPage(int userId)
        {
            InitializeComponent();
            currentUserId = userId;
            OrdersList.ItemsSource = Orders;
            LoadOrders();
        }

        private void LoadOrders()
        {
            Orders.Clear();
            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();
                // Сначала берем все заказы пользователя
                string orderQuery = "SELECT * FROM Orders WHERE UserID=@UserID ORDER BY OrderDate DESC";
                using (SqlCommand cmd = new SqlCommand(orderQuery, conn))
                {
                    cmd.Parameters.AddWithValue("@UserID", currentUserId);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var order = new Order
                            {
                                OrderID = Convert.ToInt32(reader["OrderID"]),
                                OrderDate = Convert.ToDateTime(reader["OrderDate"]),
                                TotalSum = Convert.ToDecimal(reader["TotalSum"]),
                                Status = reader["Status"].ToString(),
                                DeliveryInfo = reader["City"].ToString() + ", " + reader["Address"].ToString()
                            };
                            Orders.Add(order);
                        }
                    }
                }

                // Потом загружаем товары для каждого заказа
                string itemQuery = "SELECT od.OrderID, od.Quantity, p.Id, p.Name, p.Brand, p.Volume, p.ImageData " +
                                   "FROM OrderDetails od " +
                                   "JOIN Perfumes p ON od.PerfumeID=p.Id " +
                                   "WHERE od.OrderID=@OrderID";

                foreach (var order in Orders)
                {
                    using (SqlCommand cmd = new SqlCommand(itemQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@OrderID", order.OrderID);
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = new OrderItem
                                {
                                    Quantity = Convert.ToInt32(reader["Quantity"]),
                                    Perfume = new Perfume
                                    {
                                        Id = Convert.ToInt32(reader["Id"]),
                                        Name = reader["Name"].ToString(),
                                        Brand = reader["Brand"].ToString(),
                                        Volume = reader["Volume"].ToString(),
                                        ImageData = (byte[])reader["ImageData"]
                                    }
                                };
                                order.Items.Add(item);
                            }
                        }
                    }
                }
            }
        }

        private void CancelOrder_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.Tag is int orderId)
            {
                var result = MessageBox.Show("Вы уверены, что хотите отменить заказ?", "Подтверждение", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (result == MessageBoxResult.Yes)
                {
                    using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
                    {
                        conn.Open();
                        string deleteDetails = "DELETE FROM OrderDetails WHERE OrderID=@OrderID";
                        string deleteOrder = "DELETE FROM Orders WHERE OrderID=@OrderID";
                        using (SqlCommand cmd = new SqlCommand(deleteDetails, conn))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            cmd.ExecuteNonQuery();
                        }
                        using (SqlCommand cmd = new SqlCommand(deleteOrder, conn))
                        {
                            cmd.Parameters.AddWithValue("@OrderID", orderId);
                            cmd.ExecuteNonQuery();
                        }
                    }
                    // Убираем из списка сразу
                    var order = Orders.FirstOrDefault(o => o.OrderID == orderId);
                    if (order != null)
                        Orders.Remove(order);
                }
            }
        }
    }
}
