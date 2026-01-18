using Parfuholic.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Parfuholic.Pages
{
    public partial class CatalogPage : Page
    {
        public ObservableCollection<Perfume> Perfumes { get; set; }
            = new ObservableCollection<Perfume>();

        private readonly string connectionString =
            @"Data Source=.\SQLEXPRESS;Initial Catalog=ParfuholicDB;Integrated Security=True";

        private bool _isNavigating = false;

        public CatalogPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadPerfumes();
        }

        private void LoadPerfumes()
        {
            Perfumes.Clear();

            try
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    var cmd = new SqlCommand(@"
                        SELECT Id, Name, ForWhom, AromaGroup, BaseNotes,
                               MiddleNotes, TopNotes, Volume, Brand, Price, ImageData
                        FROM Perfumes", conn);

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
                                ImageData = reader["ImageData"] == DBNull.Value
                                    ? null
                                    : (byte[])reader["ImageData"]
                            });
                        }
                    }
                }

                Debug.WriteLine($"Загружено товаров: {Perfumes.Count}");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Ошибка загрузки");
            }
        }

        private void PerfumeCard_Click(object sender, MouseButtonEventArgs e)
        {
            if (_isNavigating) return;
            _isNavigating = true;

            var border = sender as Border;
            var perfume = border?.DataContext as Perfume;
            if (perfume == null) return;

            var nav = NavigationService;
            if (nav != null)
            {
                nav.Navigate(new PerfumePage(perfume.Id, perfume.Volume));
                nav.RemoveBackEntry(); // 🔥 УДАЛЯЕМ ПРЕДЫДУЩУЮ СТРАНИЦУ
            }

            Dispatcher.BeginInvoke(new Action(() =>
            {
                _isNavigating = false;
            }), System.Windows.Threading.DispatcherPriority.Background);
        }
    }
}
