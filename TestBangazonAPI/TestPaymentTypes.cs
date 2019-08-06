using System;
using System.Net;
using Newtonsoft.Json;
using Xunit;
using BangazonAPI.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace TestBangazonAPI
{
    public class TestPaymentTypes
    {
        [Fact]
        public async Task Test_Get_All_PaymentTypes()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymenttypes");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentTypes = JsonConvert.DeserializeObject<List<PaymentType>>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.True(paymentTypes.Count > 0);
            }
        }

        [Fact]
        public async Task Test_Get_Single_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymenttypes/4");


                string responseBody = await response.Content.ReadAsStringAsync();
                var paymentType = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal("mastercard", paymentType.Name);
                Assert.Equal(622854, paymentType.AcctNumber);
                Assert.Equal(3, paymentType.CustomerId);
                Assert.NotNull(paymentType);
            }
        }

        [Fact]
        public async Task Test_Get_Nonexistent_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */


                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymenttypes/99999999");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Create_And_Delete_PaymentType()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */
                PaymentType CreditCard1 = new PaymentType
                {
                    Name = "Visa",
                    AcctNumber = 385858,
                    CustomerId = 4
                };

                var CreditCard1AsJSON = JsonConvert.SerializeObject(CreditCard1);

                /*
                    ACT
                */
                var response = await client.GetAsync("/api/paymenttypes");
                new StringContent(CreditCard1AsJSON, Encoding.UTF8, "application/json")
                    );

                string responseBody = await response.Content.ReadAsStringAsync();
                var NewCreditCard = JsonConvert.DeserializeObject<PaymentType>(responseBody);

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.Created, response.StatusCode);
                Assert.Equal(CreditCard1.Name, NewCreditCard.Name);
                Assert.Equal(CreditCard1.AcctNumber, NewCreditCard.AcctNumber);
                Assert.Equal(CreditCard1.CustomerId, NewCreditCard.CustomerId);

                /*
                    ACT 
                */
                var deleteResponse = await client.DeleteAsync($"/api/animals/{NewCreditCard.Id}");

                /*
                    ASSERT
                */
                Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
            }
        }

        [Fact]
        public async Task Test_Delete_NonExistent_PaymentType_Fails()
        {
            using (var client = new APIClientProvider().Client)
            {
                /*
                    ARRANGE
                */

                /*
                    ACT
                */
                var deleteResponse = await client.DeleteAsync("/api/paymentTypes/99999999");

                /*
                    ASSERT
                */
                Assert.False(deleteResponse.IsSuccessStatusCode);
                Assert.Equal(HttpStatusCode.NotFound, deleteResponse.StatusCode);

            }
        }
    }
}