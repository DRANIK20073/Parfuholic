using Parfuholic.Models;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Parfuholic.Pages
{
    public partial class CatalogPage : Page, INotifyPropertyChanged
    {
        public ObservableCollection<Perfume> Perfumes { get; set; } = new ObservableCollection<Perfume>();

        private readonly string connectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=ParfuholicDB;Integrated Security=True";

        private string _category;
        private bool _isNavigating = false;

        private bool _hasPerfumes;
        public bool HasPerfumes
        {
            get => _hasPerfumes;
            set
            {
                if (_hasPerfumes != value)
                {
                    _hasPerfumes = value;
                    OnPropertyChanged(nameof(HasPerfumes));
                    OnPropertyChanged(nameof(HasNoPerfumes));
                }
            }
        }

        public bool HasNoPerfumes => !HasPerfumes;

        public CatalogPage(string category = "All")
        {
            InitializeComponent();
            DataContext = this;
            _category = category;
            LoadPerfumes(category);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private void LoadPerfumes(string category)
        {
            Perfumes.Clear();

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                string sql = @"SELECT Id, Name, ForWhom, AromaGroup, BaseNotes,
                                      MiddleNotes, TopNotes, Volume, Brand, Price, ImageData, Quantity, IsNew, DiscountPercent
                               FROM Perfumes";

                if (category == "New")
                    sql += " WHERE IsNew = 1";
                else if (category == "Discount")
                    sql += " WHERE DiscountPercent > 0";
                else if (category == "для женщин")
                    sql += " WHERE ForWhom = @Category OR ForWhom = 'унисекс'";
                else if (category == "для мужчин")
                    sql += " WHERE ForWhom = @Category OR ForWhom = 'унисекс'";
                else if (category == "унисекс")
                    sql += " WHERE ForWhom = 'унисекс'";

                using (var cmd = new SqlCommand(sql, conn))
                {
                    if (category == "для женщин") cmd.Parameters.AddWithValue("@Category", "для женщин");
                    if (category == "для мужчин") cmd.Parameters.AddWithValue("@Category", "для мужчин");

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Perfumes.Add(new Perfume
                            {
                                Id = reader.GetInt32(0),
                                Name = reader["Name"] as string,
                                ForWhom = reader["ForWhom"] as string,
                                AromaGroup = reader["AromaGroup"] as string,
                                BaseNotes = reader["BaseNotes"] as string,
                                MiddleNotes = reader["MiddleNotes"] as string,
                                TopNotes = reader["TopNotes"] as string,
                                Volume = reader["Volume"] as string,
                                Brand = reader["Brand"] as string,
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                ImageData = reader["ImageData"] == DBNull.Value ? null : (byte[])reader["ImageData"],
                                IsNew = Convert.ToBoolean(reader["IsNew"]),
                                DiscountPercent = reader["DiscountPercent"] != DBNull.Value ? Convert.ToInt32(reader["DiscountPercent"]) : 0
                            });
                        }
                    }
                }
            }

            HasPerfumes = Perfumes.Count > 0;
        }

        private void PerfumeCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            if (sender is Border border && border.DataContext is Perfume perfume)
            {
                NavigationService?.Navigate(new PerfumePage(perfume.Id, perfume.Volume));
                NavigationService?.RemoveBackEntry();
            }

            Dispatcher.BeginInvoke(new Action(() => _isNavigating = false),
                                   DispatcherPriority.Background);
        }
    }
}
