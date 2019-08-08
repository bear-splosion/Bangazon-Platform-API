using BangazonAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace TestBangazonAPI
{
    public class TestProductType
    {
        [Fact]
        public async Task Test_Get_All_ProductTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                 * Arrange
                 */

                /*
                 * Act
                 */
                var response = await client.GetAsync("/api/producttypes");

                string responseBody = await response.Content.ReadAsStringAsync();
                var productTypes = JsonConvert.DeserializeObject<List<ProductType>>(responseBody);

                /*
                 * ASSERT
                 */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(productTypes.Count > 0);
            }
        }

        [Fact]

        public async Task Test_Get_Single_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    * Arrange
                    */

                /*
                 * Act
                 */
                var response = await client.GetAsync("/api/producttypes/1");

                string responseBody = await response.Content.ReadAsStringAsync();
                var productType = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /* ASSERT
                         */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Groceries", productType.Name);
                Assert.NotNull(productType);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_ProductType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                ProductType product1 = new ProductType
                {
                    Name = "Widget"
                };

                var product1JSON = JsonConvert.SerializeObject(product1);

                /*
                    ACT
                */
                var response = await client.PostAsync("/api/paymentTypes",
                new StringContent(product1AsJSON, Encoding.UTF8, "application/json")
                    );

                string responseBody = await response.Content.ReadAsStringAsync();
                ProductType NewProduct = JsonConvert.DeserializeObject<ProductType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(product1.Name, NewProduct.Name);

                /*
                    ACT 
                */
                var deleteResponse = await client.DeleteAsync($"/api/productTypes/{NewProduct.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }
    }
}