using Parfuholic.Models;
using System;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Windows.Controls;

namespace Parfuholic.Pages
{
    public partial class CatalogPage : Page
    {
        public ObservableCollection<Perfume> Perfumes { get; set; } = new ObservableCollection<Perfume>();

        private string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=ParfuholicDB;Integrated Security=True";

        public CatalogPage()
        {
            InitializeComponent();
            DataContext = this;
            LoadPerfumes();
        }

        private void LoadPerfumes()
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string query = @"
                        SELECT Id, Name, ForWhom, AromaGroup, BaseNotes, MiddleNotes, TopNotes,
                               Volume, Brand, Price, ImageData
                        FROM dbo.Perfumes";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
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
                                ImageData = reader["ImageData"] == DBNull.Value ? null : (byte[])reader["ImageData"]
                            });
                        }
                    }
                }

                Debug.WriteLine($"Loaded {Perfumes.Count} perfumes");
            }
            catch (SqlException ex)
            {
                Debug.WriteLine($"SQL Error: {ex.Message}");
            }
        }
    }
}
