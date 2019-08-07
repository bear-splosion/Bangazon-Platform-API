using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IConfiguration _config;

        public ProductsController(IConfiguration config)
        {
            _config = config;
        }

        private SqlConnection Connection
        {
            get
            {
                return new SqlConnection(_config.GetConnectionString("DefaultConnection"));
            }
        }

        // GET api/products
        [HttpGet]
        public async Task<IActionResult> Get(string _include = null)
        {
            return await GetProducts(include: _include);
        }

        // GET api/products/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, string _include = null)
        {
            return await GetProducts(id, _include);
        }
        private async Task<IActionResult> GetProducts(int? id = null, string include = null)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include == "products")
                    {
                        cmd.CommandText = @"SELECT p.Price
                                                   p.Title
                                                   p.Description
                                                   p.Quantity
                                                   p.CustomerId
                                                   p.ProductTypeId
                                                   FROM Product p
                                                   LEFT JOIN ProductType pt
                                                   ON p.ProductTypeId = pt.Id
                                                   LEFT JOIN Customer c
                                                   ON p.CustomerId = c.Id;";
                    }

                    else
                    {
                        cmd.CommandText = @"SELECT p.Price
													p.Title
													p.Description
													p.Quantity
													FROM Product p";
                    }

                    SqlDataReader reader = await cmd.ExecuteReaderAsync();
                    List<Product> products = new List<Product>();
                    while (reader.Read())
                    {
                        Product product = null;
                        var productId = reader.GetInt32(reader.GetOrdinal("Id"));
                        product = products.FirstOrDefault(x => x.Id == productId);
                        if (product == null)
                        {
                            product = new Product
                            {
                                Id = productId,
                                ProductTypeId = reader.GetInt32(reader.GetOrdinal("ProductTypeId")),
                                CustomerId = reader.GetInt32(reader.GetOrdinal("CustomerId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                            };
                        };
                    }
                    //QUESTION -- SHOULD CUSTOMERID BE ABLE TO BE NULL?​
                    reader.Close();
                    
                    return Ok(products);
                }
            }
        }


        // POST api/values
        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] Customer customer)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        //            // More string interpolation
        //            cmd.CommandText = @"
        //                INSERT INTO Customer ()
        //                OUTPUT INSERTED.Id
        //                VALUES ()
        //            ";
        //            cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));

        //            customer.Id = (int)await cmd.ExecuteScalarAsync();

        //            return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);
        //        }
        //    }
        //}

        // PUT api/values/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"
        //                    UPDATE Customer
        //                    SET FirstName = @firstName
        //                    -- Set the remaining columns here
        //                    WHERE Id = @id
        //                ";
        //                cmd.Parameters.Add(new SqlParameter("@id", customer.Id));
        //                cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));

        //                int rowsAffected = await cmd.ExecuteNonQueryAsync();

        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }

        //                throw new Exception("No rows affected");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!CustomerExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        // DELETE api/values/5
        //[HttpDelete("{id}")]
        //public async Task<IActionResult> Delete([FromRoute] int id)

        //{
        //    try
        //    {
        //        using (SqlConnection conn = Connection)
        //        {
        //            conn.Open();
        //            using (SqlCommand cmd = conn.CreateCommand())
        //            {
        //                cmd.CommandText = @"DELETE FROM Customers WHERE Id = @id";
        //                cmd.Parameters.Add(new SqlParameter("@id", id));

        //                int rowsAffected = cmd.ExecuteNonQuery();
        //                if (rowsAffected > 0)
        //                {
        //                    return new StatusCodeResult(StatusCodes.Status204NoContent);
        //                }
        //                throw new Exception("No rows affected");
        //            }
        //        }
        //    }
        //    catch (Exception)
        //    {
        //        if (!CustomerExists(id))
        //        {
        //            return NotFound();
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }
        //}

        //private bool CustomerExists(int id)
        //{
        //    using (SqlConnection conn = Connection)
        //    {
        //        conn.Open();
        //        using (SqlCommand cmd = conn.CreateCommand())
        //        {
        // More string interpolation
        //            cmd.CommandText = "SELECT Id FROM Customer WHERE Id = @id";
        //            cmd.Parameters.Add(new SqlParameter("@id", id));

        //            SqlDataReader reader = cmd.ExecuteReader();

        //            return reader.Read();
        //        }
        //    }
        //}
    }
}