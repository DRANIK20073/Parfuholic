using Parfuholic;
using Parfuholic.Models;
using System.Collections.Generic;
using System.Data.SqlClient;

public static class Favorites
{
    // Строка подключения напрямую
    public static string ConnectionString =
        @"Server=.\SQLEXPRESS;Database=ParfuholicDB;Trusted_Connection=True;";

    public static List<Perfume> GetFavorites(int userId)
    {
        List<Perfume> favorites = new List<Perfume>();

        using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
        {
            conn.Open();
            string sql = @"
            SELECT p.*
            FROM Favorites f
            INNER JOIN Perfumes p ON f.PerfumeId = p.Id
            WHERE f.UserID = @userId";

            using (SqlCommand cmd = new SqlCommand(sql, conn))
            {
                cmd.Parameters.AddWithValue("@userId", userId);

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Perfume p = new Perfume
                        {
                            Id = (int)reader["Id"],
                            Name = reader["Name"].ToString(),
                            Brand = reader["Brand"].ToString(),
                            // остальные поля
                        };
                        favorites.Add(p);
                    }
                }
            }
        }

        return favorites;
    }


    public static bool IsFavorite(int perfumeId)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            var cmd = new SqlCommand(
                "SELECT COUNT(*) FROM Favorites WHERE PerfumeId = @PerfumeId",
                connection);

            cmd.Parameters.AddWithValue("@PerfumeId", perfumeId);

            return (int)cmd.ExecuteScalar() > 0;
        }
    }

    public static void Add(Perfume perfume)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            var cmd = new SqlCommand(@"
                INSERT INTO Favorites
                (PerfumeId, Name, Brand, Volume, AromaGroup, PriceWithDiscount, Image)
                VALUES
                (@PerfumeId, @Name, @Brand, @Volume, @AromaGroup, @Price, @Image)",
                connection);

            cmd.Parameters.AddWithValue("@PerfumeId", perfume.Id);
            cmd.Parameters.AddWithValue("@Name", perfume.Name);
            cmd.Parameters.AddWithValue("@Brand", perfume.Brand);
            cmd.Parameters.AddWithValue("@Volume", perfume.Volume);
            cmd.Parameters.AddWithValue("@AromaGroup", perfume.AromaGroup);
            cmd.Parameters.AddWithValue("@Price", perfume.PriceWithDiscount);
            cmd.Parameters.AddWithValue("@Image", perfume.Image);

            cmd.ExecuteNonQuery();
        }
    }

    public static void Remove(int perfumeId)
    {
        using (SqlConnection connection = new SqlConnection(ConnectionString))
        {
            connection.Open();

            var cmd = new SqlCommand(
                "DELETE FROM Favorites WHERE PerfumeId = @PerfumeId",
                connection);

            cmd.Parameters.AddWithValue("@PerfumeId", perfumeId);
            cmd.ExecuteNonQuery();
        }
    }
}
