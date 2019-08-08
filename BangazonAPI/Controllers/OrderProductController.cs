using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using BangazonAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BangazonAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    
    public class OrderProductController : Controller
    {
        private readonly IConfiguration _config;

        public OrderProductController(IConfiguration config)
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
        // GET: api/productOrder
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT op.Id, op.ProductId, op.OrderId
                                    FROM OrderProduct op
                                    WHERE 1 = 1";
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    List<OrderProduct> orderProducts = new List<OrderProduct>();
                    while (reader.Read())
                    {
                        OrderProduct orderProduct = new OrderProduct
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId"))
                        };
                        orderProducts.Add(orderProduct);
                    }
                    reader.Close();

                    return Ok(orderProducts);
                }
            }
        }

        // GET api/productOrder/2
        [HttpGet("{id}", Name = "GetOrderProduct")]
        public async Task<IActionResult> Get(int id)

        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT
                            Id, ProductId, OrderId
                        FROM OrderProduct
                        WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));
                    SqlDataReader reader = await cmd.ExecuteReaderAsync();

                    OrderProduct orderProduct = null;
                    if (reader.Read())
                    {
                        orderProduct = new OrderProduct
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                            OrderId = reader.GetInt32(reader.GetOrdinal("OrderId"))
                        };
                    }
                    reader.Close();
                    if (orderProduct == null)
                    {
                        return NotFound();
                    }
                    return Ok(orderProduct);
                }
            }
        }
        // POST api/<controller>
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] OrderProduct orderProduct)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {

                    {
                        cmd.CommandText = @"
                            INSERT INTO OrderProduct (OrderId, ProductId)
                            OUTPUT INSERTED.Id
                            VALUES (@orderId, @productId)";
                        cmd.Parameters.Add(new SqlParameter("@orderId", orderProduct.OrderId));
                        cmd.Parameters.Add(new SqlParameter("@productId", orderProduct.ProductId));

                        orderProduct.Id = (int)await cmd.ExecuteScalarAsync();

                        return CreatedAtRoute("GetOrderProduct", new { id = orderProduct.Id }, orderProduct);

                    }
                }
            }
        }
        

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            try
            {
                using(SqlConnection conn = Connection)
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"DELETE FROM OrderProduct
                                            WHERE Id = @id";
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
                if(!OrderProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        private bool OrderProductExists(int id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id FROM OrderProduct WHERE Id = @id";
                    cmd.Parameters.Add(new SqlParameter("@id", id));

                    SqlDataReader reader = cmd.ExecuteReader();

                    return reader.Read();
                }
            }
        }
    }
}
