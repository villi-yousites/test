using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Presentation.Models
{
    public class ProductModel
    {
        private readonly string connStr;
        public string ProductName { get; set; }
        public string ProductDesc { get; set; }
        public List<string> ProductImages { get; set; }

        public ProductModel()
        {
            connStr =
                "Data Source=GAL-PC\\SQLEXPRESS;Initial Catalog=villi_test;Persist Security Info=True;User ID=villi;Password=vk123;User Instance=False";
        }

        public List<ProductModel> GetProductsByName(string prdName)
        {
            List<ProductModel> Products = new List<ProductModel>();
            
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sSQL = "Select * from Products WHERE Name LIKE '%" + prdName + "%'";
                SqlCommand cmd = new SqlCommand(sSQL, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            ProductModel Product = new ProductModel();
                            Product.ProductName = reader["Name"].ToString();

                            Products.Add(Product);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            return Products;
        }

        public ProductModel GetProductByName(string prdName)
        {
            ProductModel Product = new ProductModel();

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sSQL = "Select * from Products WHERE Name='" + prdName + "'";
                SqlCommand cmd = new SqlCommand(sSQL, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {

                            Product.ProductName = reader["Name"].ToString();
                            Product.ProductDesc = reader["Description"].ToString();
                        }
                        else
                        {
                            return null;
                        }
                    }
                }

                Product.ProductImages = new List<string>();
                sSQL = "Select * from ProductImages2 WHERE productName='" + Product.ProductName + "'";
                cmd = new SqlCommand(sSQL, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            Product.ProductImages.Add(reader["imgUrl"].ToString());
                        }
                    }

                    return Product;
                }

            }
        }

        public bool CheckIfProductNameExists(string prdName)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sSQL = "SELECT * FROM Products WHERE Name='" + prdName + "'";
                SqlCommand cmd = new SqlCommand(sSQL, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while(reader.Read())
                    {
                        if(reader.HasRows)
                        {
                            return true;
                        }
                    }
                }

            }

            return false;
        }

        public bool AddNewProduct(string prdName, string prdDesc, string prdImage)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sSQL = "INSERT INTO Products VALUES('" + prdName + "','" + prdDesc + "')";
                SqlCommand cmd = new SqlCommand(sSQL, conn);
                
                if (cmd.ExecuteNonQuery() < 0) return false; //check the rows affected if less than 0 return false

                string sSQL1 = "INSERT INTO ProductImages2 VALUES('" + prdName + "','" + prdImage + "')";
                SqlCommand cmd1 = new SqlCommand(sSQL1, conn);

                if (cmd1.ExecuteNonQuery() < 0) return false; //check the rows affected if less than 0 return false
            }

            return true;
        }

        public List<string> GetAllProducts()
        {
            List<string> Products = new List<string>();
            string name = string.Empty;

            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sSQL = "Select * from Products";
                SqlCommand cmd = new SqlCommand(sSQL, conn);
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            name = reader["Name"].ToString();

                            Products.Add(name);
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

            return Products;
        }

        public void DeleteProductByName(string prdName)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sSQL = "DELETE FROM Products WHERE Name='" + prdName + "'";
                SqlCommand cmd = new SqlCommand(sSQL, conn);

                cmd.ExecuteNonQuery();

                sSQL = "DELETE FROM ProductImages2 WHERE productName='" + prdName + "'";
                cmd = new SqlCommand(sSQL, conn);
                cmd.ExecuteNonQuery();
            }
        }

        public void UpdateProduct(string prdOriginalName, string prdName, string prdDesc, string prdImage)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sSQL = "UPDATE Products SET Name='" + prdName + "', Description='" + prdDesc +
                              "' WHERE Name='" + prdOriginalName + "'";
                SqlCommand cmd = new SqlCommand(sSQL, conn);

                cmd.ExecuteNonQuery();

                sSQL = "UPDATE ProductImages2 SET productName='" + prdName + "' WHERE productName='" + prdOriginalName + "'";
                cmd = new SqlCommand(sSQL, conn);
                cmd.ExecuteNonQuery();

                if(prdImage != string.Empty)
                {
                    sSQL = "INSERT INTO ProductImages2 VALUES('" + prdName + "','" + prdImage + "')";
                    cmd = new SqlCommand(sSQL, conn);
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteImage(string imageName)
        {
            using (SqlConnection conn = new SqlConnection(connStr))
            {
                conn.Open();
                string sSQL = "DELETE FROM ProductImages2 WHERE imgUrl='" + imageName + "'";
                SqlCommand cmd = new SqlCommand(sSQL, conn);

                cmd.ExecuteNonQuery();
            }
        }

    }
}