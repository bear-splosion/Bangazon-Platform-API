﻿using BangazonAPI.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
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
                var orderProduct = JsonConvert.DeserializeObject<OrderProduct>(responseBody);

                /*
                    ASSERT
                */

                Assert.Equal(HttpStatusCode.OK, response.StatusCode);
                Assert.Equal(3, orderProduct.OrderId);
                Assert.Equal(2, orderProduct.ProductId);
                Assert.NotNull(orderProduct);


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
    }
}
