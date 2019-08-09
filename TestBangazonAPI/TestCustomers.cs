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
    public class TestCustomers
    {
        [Fact]
        public async Task Test_Get_All_Customers()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customers");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customers = JsonConvert.DeserializeObject<List<Customer>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(customers.Count > 0);
            }
        }

        [Fact]

        public async Task Test_Get_Single_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/customers/1");


                string responseBody = await response.Content.ReadAsStringAsync();
                var customer = JsonConvert.DeserializeObject<Customer>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("Inès", customer.FirstName);
                Assert.Equal("Northeast", customer.LastName);
                Assert.NotNull(customer);
            }

        }

        [Fact]
        public async Task Test_Modify_Customer()
        {

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT section
                 */
                string NewFirstName = "Tom";
                string NewLastName = "Ford";
                Customer modifiedTom = new Customer
                {
                    FirstName = NewFirstName,
                    LastName = NewLastName,
                    Id = 3
                   
                };
                var ModifiedTomAsJSON = JsonConvert.SerializeObject(modifiedTom);

                var response = await client.PutAsync(
                    "/api/customers/3",
                    new StringContent(ModifiedTomAsJSON, Encoding.UTF8, "application/json")
                );
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                    GET section
                 */
                var GetTom= await client.GetAsync("/api/customers/3");
                GetTom.EnsureSuccessStatusCode();

                string GetTomBody = await GetTom.Content.ReadAsStringAsync();
                Customer NewTom = JsonConvert.DeserializeObject<Customer>(GetTomBody);

                Assert.Equal(HttpStatusCode.OK, GetTom.StatusCode);
                Assert.Equal(NewFirstName, NewTom.FirstName);
                Assert.Equal(NewLastName, NewTom.LastName);
            }
        }

        [Fact]
        public async Task Test_Create_Customer()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                Customer Berry = new Customer
                {
                    FirstName = "Berry",
                    LastName = "White"                                    
                };
                var BerryAsJSON = JsonConvert.SerializeObject(Berry);

                /*
                    ACT
                */
                var response = await client.PostAsync(
                    "/api/customers",
                    new StringContent(BerryAsJSON, Encoding.UTF8, "application/json")
                );


                string responseBody = await response.Content.ReadAsStringAsync();
                var NewBerry = JsonConvert.DeserializeObject<Customer>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(Berry.FirstName, NewBerry.FirstName);
                Assert.Equal(Berry.LastName, NewBerry.LastName);

             
            }
        }

    }
}
