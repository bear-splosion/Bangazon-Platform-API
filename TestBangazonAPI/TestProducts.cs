using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TestBangazonAPI
{
    public class TestProducts
    {
        [Fact]
        public async Task Test_Get_All_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/products");


                string responseBody = await response.Content.ReadAsStringAsync();
                var products = JsonConvert.DeserializeObject<List<Product>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(products.Count > 0);
            }
        }

        [Fact]

        public async Task Test_Get_Single_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/products/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<Product>(responseBody);

                /*
                    ASSERT
                */
                //Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                //Assert.Equal(3, product.ProductTypeId);
                //Assert.Equal(2, product.CustomerId);
                //Assert.Equal(5.00, product.Price);
                //Assert.Equal("Book", product.Title);
                //Assert.Equal("Beautiful, beautiful book", product.Description);
                //Assert.Equal(13, product.Quantity);
                //Assert.NotNull(product);
            }

        }

        [Fact]
        public async Task Test_Create_And_Delete_Products()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Product CreateProduct = new Product
                { 
                    CustomerId = 1,
                    ProductTypeId = 1,
                    Price = 10,
                    Title = "Super Duper Awesomeness",
                    Description = "Awesomer than the awesome thing",
                    Quantity = 20
                 };

                var product = JsonConvert.SerializeObject(CreateProduct);

                /*
                    ACT
                */
                var response = await client.PostAsync("/api/products",
                new StringContent(product, Encoding.UTF8, "application/json")
                    );

                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);
                Product tacoProduct = JsonConvert.DeserializeObject<Product>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(CreateProduct.CustomerId, tacoProduct.CustomerId);
                Assert.Equal(CreateProduct.Price, tacoProduct.Price);
                Assert.Equal(CreateProduct.Title, tacoProduct.Title);
                Assert.Equal(CreateProduct.Description, tacoProduct.Description);
                Assert.Equal(CreateProduct.Quantity, tacoProduct.Quantity);

                /*
                    ACT 
                */
                var deleteResponse = await client.DeleteAsync($"/api/products/{tacoProduct.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Modify_Product()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT
                */

                Product ModifiedProduct = new Product
                {
                    ProductTypeId = 1,
                    CustomerId = 1,
                    Price = 20,
                    Title = "This Was Modified by a test",
                    Description = "Modified by test",
                    Quantity = 500
                };
                var jsonBody = JsonConvert.SerializeObject(ModifiedProduct);

                var response = await client.PutAsync(
                    "/api/products/1",
                    new StringContent(jsonBody, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
            }
        }
    }
}