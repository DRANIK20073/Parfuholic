using LiveCharts;
using LiveCharts.Wpf;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace Parfuholic.Pages
{
    public partial class ReportsPage : Page
    {
        private readonly string connectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=ParfuholicDB;Integrated Security=True";

        public ReportsPage()
        {
            InitializeComponent();
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today;
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            if (startDate == null || endDate == null)
            {
                MessageBox.Show("Выберите даты.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (startDate > endDate)
            {
                MessageBox.Show("Дата начала не может быть позже даты конца.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var data = GetSalesData(startDate.Value, endDate.Value);
                DrawChart(data);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при построении отчета:\n" + ex.Message);
            }
        }

        private List<SaleSummary> GetSalesData(DateTime start, DateTime end)
        {
            var list = new List<SaleSummary>();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT p.Name AS PerfumeName, SUM(od.Quantity) AS TotalQuantity, SUM(od.Quantity * p.Price) AS TotalSum
                    FROM Orders o
                    INNER JOIN OrderDetails od ON o.OrderID = od.OrderID
                    INNER JOIN Perfumes p ON od.PerfumeID = p.Id
                    WHERE o.OrderDate >= @StartDate AND o.OrderDate <= @EndDate
                    GROUP BY p.Name
                    ORDER BY p.Name";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@StartDate", start.Date);
                    cmd.Parameters.AddWithValue("@EndDate", end.Date.AddDays(1).AddSeconds(-1));

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new SaleSummary
                            {
                                PerfumeName = reader["PerfumeName"].ToString(),
                                TotalQuantity = Convert.ToInt32(reader["TotalQuantity"]),
                                TotalSum = Convert.ToDecimal(reader["TotalSum"])
                            });
                        }
                    }
                }
            }

            return list;
        }

        private void DrawChart(List<SaleSummary> data)
        {
            if (data.Count == 0)
            {
                MessageBox.Show("Продаж за выбранный период нет.");
                SalesChart.Series = null;
                SalesChart.AxisX.Clear();
                SalesChart.AxisY.Clear();
                return;
            }

            // Создаем серии
            var quantitySeries = new ColumnSeries
            {
                Title = "Количество",
                Values = new ChartValues<int>(data.Select(x => x.TotalQuantity))
            };

            var sumSeries = new ColumnSeries
            {
                Title = "Сумма",
                Values = new ChartValues<decimal>(data.Select(x => x.TotalSum))
            };

            SalesChart.Series = new SeriesCollection { quantitySeries, sumSeries };

            // Настройка оси X
            SalesChart.AxisX.Clear();
            SalesChart.AxisX.Add(new Axis
            {
                Labels = data.Select(x => x.PerfumeName).ToArray(),
                LabelsRotation = 15
            });

            // Настройка оси Y
            SalesChart.AxisY.Clear();
            SalesChart.AxisY.Add(new Axis
            {
                Title = "Значение"
            });
        }
    }

    public class SaleSummary
    {
        public string PerfumeName { get; set; }
        public int TotalQuantity { get; set; }
        public decimal TotalSum { get; set; }
    }
}
