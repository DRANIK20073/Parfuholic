using Parfuholic.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Windows;
using System.Windows.Controls;

namespace Parfuholic.Pages
{
    public partial class ManagePerfumesPage : Page
    {
        private List<Perfume> perfumes = new List<Perfume>();

        public ManagePerfumesPage()
        {
            InitializeComponent();
            LoadPerfumes();
        }

        private void LoadPerfumes()
        {
            perfumes.Clear();

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(@"
            SELECT Id, Name, Brand, ForWhom, AromaGroup,
                   TopNotes, MiddleNotes, BaseNotes,
                   Volume, Price, Quantity, IsNew, IsDiscount, DiscountPercent
            FROM Perfumes", conn);

                SqlDataReader reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    perfumes.Add(new Perfume
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Brand = reader["Brand"].ToString(),
                        ForWhom = reader["ForWhom"].ToString(),
                        AromaGroup = reader["AromaGroup"].ToString(),
                        TopNotes = reader["TopNotes"].ToString(),
                        MiddleNotes = reader["MiddleNotes"].ToString(),
                        BaseNotes = reader["BaseNotes"].ToString(),
                        Volume = reader["Volume"].ToString(),
                        Price = reader["Price"] != DBNull.Value ? Convert.ToDecimal(reader["Price"]) : 0,
                        Quantity = reader["Quantity"] != DBNull.Value ? Convert.ToDecimal(reader["Quantity"]) : 0, // <- исправлено
                        IsNew = reader["IsNew"] != DBNull.Value && (bool)reader["IsNew"],
                        IsDiscount = reader["IsDiscount"] != DBNull.Value && (bool)reader["IsDiscount"],
                        DiscountPercent = reader["DiscountPercent"] != DBNull.Value ? Convert.ToInt32(reader["DiscountPercent"]) : 0
                    });
                }
            }

            PerfumesGrid.ItemsSource = null;
            PerfumesGrid.ItemsSource = perfumes;
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            // фиксируем редактирование
            PerfumesGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            PerfumesGrid.CommitEdit(DataGridEditingUnit.Row, true);

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                foreach (var p in perfumes)
                {
                    // автоматически ставим IsDiscount
                    p.IsDiscount = p.DiscountPercent > 0;

                    SqlCommand cmd = new SqlCommand(@"
                        UPDATE Perfumes SET
                            Name = @Name,
                            Brand = @Brand,
                            ForWhom = @ForWhom,
                            AromaGroup = @AromaGroup,
                            TopNotes = @TopNotes,
                            MiddleNotes = @MiddleNotes,
                            BaseNotes = @BaseNotes,
                            Volume = @Volume,
                            Price = @Price,
                            Quantity = @Quantity,
                            IsNew = @IsNew,
                            IsDiscount = @IsDiscount,
                            DiscountPercent = @DiscountPercent
                        WHERE Id = @Id", conn);

                    cmd.Parameters.AddWithValue("@Id", p.Id);
                    cmd.Parameters.AddWithValue("@Name", p.Name);
                    cmd.Parameters.AddWithValue("@Brand", p.Brand);
                    cmd.Parameters.AddWithValue("@ForWhom", p.ForWhom);
                    cmd.Parameters.AddWithValue("@AromaGroup", p.AromaGroup);
                    cmd.Parameters.AddWithValue("@TopNotes", p.TopNotes);
                    cmd.Parameters.AddWithValue("@MiddleNotes", p.MiddleNotes);
                    cmd.Parameters.AddWithValue("@BaseNotes", p.BaseNotes);
                    cmd.Parameters.AddWithValue("@Volume", p.Volume);
                    cmd.Parameters.AddWithValue("@Price", p.Price);
                    cmd.Parameters.AddWithValue("@Quantity", p.Quantity);
                    cmd.Parameters.AddWithValue("@IsNew", p.IsNew);
                    cmd.Parameters.AddWithValue("@IsDiscount", p.IsDiscount);
                    cmd.Parameters.AddWithValue("@DiscountPercent", p.DiscountPercent);

                    cmd.ExecuteNonQuery();
                }
            }

            MessageBox.Show("Все изменения сохранены", "Готово",
                MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
