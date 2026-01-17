using Parfuholic.Models;
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

            SqlConnection conn = new SqlConnection(Database.ConnectionString);
            conn.Open();

            SqlCommand cmd = new SqlCommand(@"
                SELECT Id, Name, Brand, ForWhom, AromaGroup,
                       TopNotes, MiddleNotes, BaseNotes,
                       Volume, Price, Quantity
                FROM Perfumes", conn);

            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Perfume p = new Perfume();
                p.Id = (int)reader["Id"];
                p.Name = reader["Name"].ToString();
                p.Brand = reader["Brand"].ToString();
                p.ForWhom = reader["ForWhom"].ToString();
                p.AromaGroup = reader["AromaGroup"].ToString();
                p.TopNotes = reader["TopNotes"].ToString();
                p.MiddleNotes = reader["MiddleNotes"].ToString();
                p.BaseNotes = reader["BaseNotes"].ToString();
                p.Volume = reader["Volume"].ToString();
                p.Price = (decimal)reader["Price"];
                p.Quantity = (int)reader["Quantity"];

                perfumes.Add(p);
            }

            reader.Close();
            conn.Close();

            PerfumesGrid.ItemsSource = perfumes;
        }

        private void SaveRow_Click(object sender, RoutedEventArgs e)
        {
            PerfumesGrid.CommitEdit(DataGridEditingUnit.Cell, true);
            PerfumesGrid.CommitEdit(DataGridEditingUnit.Row, true);

            Button btn = sender as Button;
            if (btn == null) return;

            Perfume p = btn.DataContext as Perfume;
            if (p == null) return;

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

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
                Quantity = @Quantity
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

                cmd.ExecuteNonQuery();
            }

            MessageBox.Show("Изменения сохранены");
        }

    }
}
