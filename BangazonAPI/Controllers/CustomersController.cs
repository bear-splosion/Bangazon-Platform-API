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
    public class CustomersController : ControllerBase
    {
        private readonly IConfiguration _config;

        public CustomersController(IConfiguration config)
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

        // GET api/values
        [HttpGet]
        public async Task<IActionResult> Get(string _include = null)
        {
            return await GetCustomers(include: _include);
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id, string _include = null)
        {
            return await GetCustomers(id, _include);
        }

        private async Task<IActionResult> GetCustomers(int? id = null, string include = null)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    if (include == "products")
                    {
                        cmd.CommandText = @"SELECT c.Id,
                                                   c.FirstName,
                                                   c.LastName,
                                                   p.Id AS ProductId,
                                                   p.Price,
                                                   p.Title,
                                                   p.[Description],
                                                   p.Quantity
                                            FROM Customer c
                                            LEFT OUTER JOIN Product p
                                                ON p.CustomerId = c.Id";
                    }
                    else if (include == "payments")
                    {
                        cmd.CommandText = @"SELECT c.Id,
                                                   c.FirstName,
                                                   c.LastName,
                                                   p.Id AS PaymentTypeId,
                                                   p.AcctNumber,
                                                   p.Name
                                            FROM Customer c
                                            LEFT OUTER JOIN PaymentType p
                                                ON p.CustomerId = c.Id";
                    }
                    else
                    {

                        cmd.CommandText = @"SELECT c.Id,
                                                   c.FirstName,
                                                   c.LastName
                                            FROM Customer c";
                    }
                    if (id.HasValue)
                    {
                        cmd.CommandText += @" WHERE c.Id = @id";
                        cmd.Parameters.AddWithValue("@id", id.Value);
                    }
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<Customer> customers = new List<Customer>();
                    while (reader.Read())
                    {
                        Customer customer = null;
                        var customerId = reader.GetInt32(reader.GetOrdinal("Id"));
                        customer = customers.FirstOrDefault(x => x.Id == customerId);
                        if (customer == null)
                        {
                            customer = new Customer
                            {
                                Id = customerId,
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName"))
                            };
                        }


                        if (include == "products")
                        {
                            Product product = new Product
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                Price = reader.GetDouble(reader.GetOrdinal("Price")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity"))
                            };

                            customer.Products.Add(product);
                        }

                        if (include == "payments")
                        {
                            PaymentType paymentType = new PaymentType
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("PaymentTypeId")),
                                AcctNumber = reader.GetInt32(reader.GetOrdinal("AcctNumber")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };

                            customer.PaymentTypes.Add(paymentType);
                        }

                        customers.Add(customer);
                    }

                    reader.Close();

                    return Ok(customers);
                }
            }
        }


        // POST api/values
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Customer customer)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = @"
                        INSERT INTO Customer ()
                        OUTPUT INSERTED.Id
                        VALUES ()
                    ";
                    cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));

                    customer.Id = (int)await cmd.ExecuteScalarAsync();

                    return CreatedAtRoute("GetCustomer", new { id = customer.Id }, customer);
                }
            }
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] Customer customer)
        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                            UPDATE Customer
                            SET FirstName = @firstName
                            -- Set the remaining columns here
                            WHERE Id = @id
                        ";
                        cmd.Parameters.Add(new SqlParameter("@id", customer.Id));
                        cmd.Parameters.Add(new SqlParameter("@firstName", customer.FirstName));

                        int rowsAffected = await cmd.ExecuteNonQueryAsync();

                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }

                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)

        {
            try
            {
                using (SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM Customers WHERE Id = @id";
                        cmd.Parameters.Add(new SqlParameter("@id", id));

                        int rowsAffected = cmd.ExecuteNonQuery();
                        if (rowsAffected > 0)
                        {
                            return new StatusCodeResult(StatusCodes.Status204NoContent);
                        }
                        throw new Exception("No rows affected");
                    }
                }
            }
            catch (Exception)
            {
                if (!CustomerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        private bool CustomerExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    // More string interpolation
                    cmd.CommandText = "SELECT Id FROM Customer WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
