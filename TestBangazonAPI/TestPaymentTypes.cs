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
                var response = await client.GetAsync("/api/paymentTypes/99999999");

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
                var response = await client.PostAsync("/api/paymentTypes",
                new StringContent(CreditCard1AsJSON, Encoding.UTF8, "application/json")
                    );

                string responseBody = await response.Content.ReadAsStringAsync();
                PaymentType NewCreditCard = JsonConvert.DeserializeObject<PaymentType>(responseBody);

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
                var deleteResponse = await client.DeleteAsync($"/api/paymentTypes/{NewCreditCard.Id}");

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

        [Fact]
        public async Task Test_Modify_PaymentType()
        {
            // New paymentType value to change to and test
            string NewPaymentName = "mastercard vip";

            using (var client = new APIClientProvider().Client)
            {
                /*
                    PUT
                */

                PaymentType ModifiedMastercard = new PaymentType
                {
                    Name = NewPaymentName,
                    AcctNumber = 622854,
                    CustomerId = 3
                };
                var ModifiedMastercardAsJSON = JsonConvert.SerializeObject(ModifiedMastercard);

                var response = await client.PutAsync(
                    "/api/paymentTypes/4",
                    new StringContent(ModifiedMastercardAsJSON, Encoding.UTF8, "application/json"));
                string responseBody = await response.Content.ReadAsStringAsync();

                Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);

                /*
                 GET section
                */
                var GetMastercard = await client.GetAsync("/api/paymentTypes/4");
                GetMastercard.EnsureSuccessStatusCode();

                string GetMastercardBody = await GetMastercard.Content.ReadAsStringAsync();
                PaymentType NewMastercard = JsonConvert.DeserializeObject<PaymentType>(GetMastercardBody);

                Assert.Equal(HttpStatusCode.OK, GetMastercard.StatusCode);
                Assert.Equal(NewPaymentName, NewMastercard.Name);
            }
        }
    }
}