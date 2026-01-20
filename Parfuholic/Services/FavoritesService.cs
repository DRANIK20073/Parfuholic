using Parfuholic.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Parfuholic.Services
{
    public static class FavoritesService
    {
        public static List<Perfume> GetFavorites(int userId)
        {
            List<Perfume> favorites = new List<Perfume>();

            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();

                string sql = @"
                    SELECT p.Id, p.Name, p.Brand, p.AromaGroup, p.Volume, p.Price, p.DiscountPercent, p.ImageData
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
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Brand = reader.GetString(2),
                                AromaGroup = reader.GetString(3),
                                Volume = reader.GetString(4),
                                Price = reader.GetDecimal(5),
                                DiscountPercent = reader.GetInt32(6),
                                ImageData = reader["ImageData"] as byte[]
                            };
                            favorites.Add(p);
                        }
                    }
                }
            }

            return favorites;
        }

        public static bool IsFavorite(int userId, int perfumeId)
        {
            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();
                string sql = "SELECT COUNT(*) FROM Favorites WHERE UserID = @userId AND PerfumeId = @perfumeId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@perfumeId", perfumeId);

                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        public static void Add(int userId, Perfume p)
        {
            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();
                string sql = "INSERT INTO Favorites (UserID, PerfumeId, Name, Brand) VALUES (@userId, @perfumeId, @name, @brand)";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@perfumeId", p.Id);
                    cmd.Parameters.AddWithValue("@name", p.Name);
                    cmd.Parameters.AddWithValue("@brand", p.Brand);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static void Remove(int userId, int perfumeId)
        {
            using (SqlConnection conn = new SqlConnection(Database.ConnectionString))
            {
                conn.Open();
                string sql = "DELETE FROM Favorites WHERE UserID = @userId AND PerfumeId = @perfumeId";
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    cmd.Parameters.AddWithValue("@userId", userId);
                    cmd.Parameters.AddWithValue("@perfumeId", perfumeId);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
