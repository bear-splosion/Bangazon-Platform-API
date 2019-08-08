using BangazonAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestBangazonAPI
{
    public class TestOrderProduct
    {
        [Fact]
        public async Task Test_Get_All_OrderProduct()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("api/orderproduct");

                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<List<TestOrderProduct>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);
            }
        }
        [Fact]
        public async Task Test_Get_Single_OrderProduct()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orderproduct/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var orderproduct = JsonConvert.DeserializeObject<OrderProduct>(responseBody);

                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(3, orderproduct.OrderId);
                Assert.Equal(2, orderproduct.ProductId);
                Assert.NotNull(orderproduct);


            }
        }
        [Fact]
        public async Task Test_Get_Nonexistent_OrderProduct()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/orderproduct/99999999");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }
        [Fact]
        public async Task Test_Create_And_Delete_OrderProduct()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*ARRANGE*/
                OrderProduct TestOrderProduct = new OrderProduct
                {
                    OrderId = 7,
                    ProductId = 8
                };
                var TestOrderProductAsJSON = JsonConvert.SerializeObject(TestOrderProduct);
                /*ACT*/
                var response = await client.PostAsync("/api/orderproduct",
                    new StringContent(TestOrderProductAsJSON, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();
                OrderProduct NewOrderProduct = JsonConvert.DeserializeObject<OrderProduct>(responseBody);

                /*ASSERT*/
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(TestOrderProduct.OrderId, NewOrderProduct.OrderId);
                Assert.Equal(TestOrderProduct.ProductId, NewOrderProduct.ProductId);

                /*ACT*/
                var deleteResponse = await client.DeleteAsync($"/api/orderproduct/{NewOrderProduct.Id}");

                /*ASSERT*/
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
    }
}
