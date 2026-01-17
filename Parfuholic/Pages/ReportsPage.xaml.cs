using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace Parfuholic.Pages
{
    public partial class ReportsPage : Page
    {
        public ReportsPage()
        {
            InitializeComponent();

            // Устанавливаем даты по умолчанию на сегодня
            StartDatePicker.SelectedDate = DateTime.Today;
            EndDatePicker.SelectedDate = DateTime.Today;
        }

        private void GenerateReport_Click(object sender, RoutedEventArgs e)
        {
            DateTime? startDate = StartDatePicker.SelectedDate;
            DateTime? endDate = EndDatePicker.SelectedDate;

            if (startDate == null || endDate == null)
            {
                MessageBox.Show("Пожалуйста, выберите даты начала и конца периода.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (startDate > endDate)
            {
                MessageBox.Show("Дата начала не может быть позже даты конца.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var salesReport = GetDummySalesReport(startDate.Value, endDate.Value);
            ReportsDataGrid.ItemsSource = salesReport;
        }

        private List<SaleRecord> GetDummySalesReport(DateTime start, DateTime end)
        {
            var list = new List<SaleRecord>
            {
                new SaleRecord { Date = start.ToShortDateString(), Product = "Парфюм A", Quantity = 3, Total = 4500 },
                new SaleRecord { Date = start.AddDays(1).ToShortDateString(), Product = "Парфюм B", Quantity = 1, Total = 1500 },
                new SaleRecord { Date = end.ToShortDateString(), Product = "Парфюм C", Quantity = 2, Total = 3000 }
            };

            return list;
        }
    }

    public class SaleRecord
    {
        public string Date { get; set; }
        public string Product { get; set; }
        public int Quantity { get; set; }
        public decimal Total { get; set; }
    }
}
