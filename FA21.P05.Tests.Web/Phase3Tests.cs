using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FA21.P05.Tests.Web.Helpers;
using FA21.P05.Web.Features.MenuItems;
using FA21.P05.Web.Features.Orders;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FA21.P05.Tests.Web
{
    [TestClass]
    public class Phase3Tests
    {
        private WebTestContext context;

        [TestInitialize]
        public void Init()
        {
            context = new WebTestContext();
        }

        [TestCleanup]
        public void Cleanup()
        {
            context.Dispose();
        }

        [TestMethod]
        public async Task CreateOrder_ValidRequest_ReturnsCreated()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var allItems = await GetMenuItems(webClient);
            if (allItems == null)
            {
                //not ready for this test
                return;
            }
            var newOrder = new CreateOrderDto
            {
                OrderItems = allItems.Select(x => new CreateOrderItemDto
                {
                    LineItemQuantity = 1,
                    MenuItemId = x.Id
                }).ToList()
            };

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/orders", newOrder);

            //assert
            await AssertCreateOrderFunctions(httpResponse, newOrder, webClient);
        }

        [TestMethod]
        public async Task CreateOrder_LargeQuantities_ReturnsCreated()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var allItems = await GetMenuItems(webClient);
            if (allItems == null)
            {
                //not ready for this test
                return;
            }
            var newOrder = new CreateOrderDto
            {
                OrderItems = allItems.Select(x => new CreateOrderItemDto
                {
                    LineItemQuantity = 9000,
                    MenuItemId = x.Id
                }).ToList()
            };

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/orders", newOrder);

            //assert
            await AssertCreateOrderFunctions(httpResponse, newOrder, webClient);
        }

        [TestMethod]
        public async Task CreateOrder_OrderQuantityZero_ReturnsBadRequest()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var allItems = await GetMenuItems(webClient);
            if (allItems == null)
            {
                //not ready for this test
                return;
            }
            var newOrder = new CreateOrderDto
            {
                OrderItems = allItems.Select(x => new CreateOrderItemDto
                {
                    LineItemQuantity = 0,
                    MenuItemId = x.Id
                }).ToList()
            };

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/orders", newOrder);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when getting calling POST /api/orders with a zero line item quantity");
        }

        [TestMethod]
        public async Task CreateOrder_OrderQuantityNegative_ReturnsBadRequest()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var allItems = await GetMenuItems(webClient);
            if (allItems == null)
            {
                //not ready for this test
                return;
            }
            var newOrder = new CreateOrderDto
            {
                OrderItems = allItems.Select(x => new CreateOrderItemDto
                {
                    LineItemQuantity = -1,
                    MenuItemId = x.Id
                }).ToList()
            };

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/orders", newOrder);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when getting calling POST /api/orders with a negative line item quantity");
        }

        [TestMethod]
        public async Task CreateOrder_MenuItemDoesNotExist_ReturnsBadRequest()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var allItems = await GetMenuItems(webClient);
            if (allItems == null)
            {
                //not ready for this test
                return;
            }
            var newOrder = new CreateOrderDto
            {
                OrderItems = allItems.Select(x => new CreateOrderItemDto
                {
                    LineItemQuantity = 1,
                    MenuItemId = x.Id + 9000
                }).ToList()
            };

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/orders", newOrder);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when getting calling POST /api/orders with an order that doesn't exist");
        }

        [TestMethod]
        public async Task DeleteMenuItem_HasOrdersOnIt_ReturnsBadRequest()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LogInAsAdmin();
            var targetOrder = await CreateOrder(webClient);
            if (targetOrder == null)
            {
                //not ready for this test
                return;
            }

            //act
            var httpResponse = await webClient.DeleteAsync($"/api/menu-items/{targetOrder.OrderItems.ElementAt(0).Id}");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when getting calling DELETE /api/menu-items/{id} for a menu item that has orders related to it");
        }

        [TestMethod]
        public async Task CancelOrder_ValidOrder_ReturnsOk()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var targetOrder = await CreateOrder(webClient);
            if (targetOrder == null)
            {
                //not ready for this test
                return;
            }

            //act
            var httpResponse = await webClient.PutAsync($"/api/orders/{targetOrder.Id}/cancel");

            //assert
            await AssertCancelOrderFunctions(httpResponse, targetOrder, webClient);
        }

        [TestMethod]
        public async Task CancelOrder_NoSuchId_ReturnsNotFound()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var targetOrder = await CreateOrder(webClient);
            if (targetOrder == null)
            {
                //not ready for this test
                return;
            }

            //act
            var httpResponse = await webClient.PutAsync($"/api/orders/{targetOrder.Id + 3434}/cancel");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "we expect an HTTP 404 when canceling a not found id");
        }

        [TestMethod]
        public async Task CancelOrder_OrderAlreadyCanceled_ReturnsBadRequest()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var targetOrder = await CreateCanceledOrder(webClient);
            if (targetOrder == null)
            {
                //not ready for this test
                return;
            }

            //act
            var httpResponse = await webClient.PutAsync($"/api/orders/{targetOrder.Id}/cancel");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when canceling the same order twice");
        }

        [TestMethod]
        public async Task CancelOrder_OrderAlreadyStarted_ReturnsBadRequest()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var targetOrder = await CreateStartedOrder(webClient);
            if (targetOrder == null)
            {
                //not ready for this test
                return;
            }

            //act
            var httpResponse = await webClient.PutAsync($"/api/orders/{targetOrder.Id}/cancel");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when canceling an order was already started");
        }

        [TestMethod]
        public async Task StartOrder_ValidOrder_ReturnsOk()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var targetOrder = await CreateOrder(webClient);
            if (targetOrder == null)
            {
                //not ready for this test
                return;
            }

            //act
            var httpResponse = await webClient.PutAsync($"/api/orders/{targetOrder.Id}/start");

            //assert
            await AssertStartOrderFunctions(httpResponse, targetOrder, webClient);
        }

        [TestMethod]
        public async Task StartOrder_NoSuchId_ReturnsNotFound()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var targetOrder = await CreateOrder(webClient);
            if (targetOrder == null)
            {
                //not ready for this test
                return;
            }

            //act
            var httpResponse = await webClient.PutAsync($"/api/orders/{targetOrder.Id + 42324}/start");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "we expect an HTTP 404 when start a not found id");
        }

        [TestMethod]
        public async Task StartOrder_OrderAlreadyStarted_ReturnsBadRequest()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var targetOrder = await CreateStartedOrder(webClient);
            if (targetOrder == null)
            {
                //not ready for this test
                return;
            }

            //act
            var httpResponse = await webClient.PutAsync($"/api/orders/{targetOrder.Id}/start");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when starting the same order twice");
        }

        [TestMethod]
        public async Task StartOrder_OrderAlreadyCanceled_ReturnsBadRequest()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var targetOrder = await CreateCanceledOrder(webClient);
            if (targetOrder == null)
            {
                //not ready for this test
                return;
            }

            //act
            var httpResponse = await webClient.PutAsync($"/api/orders/{targetOrder.Id}/start");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when starting an order was already canceled");
        }

        private async Task<OrderDto> CreateOrder(HttpClient webClient)
        {
            try
            {
                var allItems = await GetMenuItems(webClient);
                if (allItems == null)
                {
                    return null;
                }
                var newOrder = new CreateOrderDto
                {
                    OrderItems = allItems.Select(x => new CreateOrderItemDto
                    {
                        LineItemQuantity = 1,
                        MenuItemId = x.Id
                    }).ToList()
                };

                var httpResponse = await webClient.PostAsJsonAsync("/api/orders", newOrder);
                return await AssertCreateOrderFunctions(httpResponse, newOrder, webClient);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<OrderDto> CreateCanceledOrder(HttpClient webClient)
        {
            try
            {
                var created = await CreateOrder(webClient);
                if (created == null)
                {
                    return null;
                }

                var httpResponse = await webClient.PutAsync($"/api/orders/{created.Id}/cancel");

                return await AssertCancelOrderFunctions(httpResponse, created, webClient);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<OrderDto> CreateStartedOrder(HttpClient webClient)
        {
            try
            {
                var created = await CreateOrder(webClient);
                if (created == null)
                {
                    return null;
                }

                var httpResponse = await webClient.PutAsync($"/api/orders/{created.Id}/start");

                return await AssertStartOrderFunctions(httpResponse, created, webClient);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<List<MenuItemDto>> GetMenuItems(HttpClient webClient)
        {
            try
            {
                var getAllRequest = await webClient.GetAsync("/api/menu-items");
                var getAllResult = await AssertMenuListAllFunctions(getAllRequest);
                return getAllResult.OrderBy(x => x.Id).Take(3).ToList();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private async Task<OrderDto> AssertCancelOrderFunctions(HttpResponseMessage httpResponse, OrderDto targetOrder, HttpClient webClient)
        {
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, $"we expect an HTTP 200 when calling PUT /api/orders/{targetOrder.Id}/cancel with a freshly made order");

            var getByIdResponse = await webClient.GetAsync($"/api/orders/{targetOrder.Id}");
            getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we should be able to get the canceled order by id");

            var freshlyFetched = await getByIdResponse.Content.ReadAsJsonAsync<OrderDto>();
            freshlyFetched.Should().NotBeNull("we should be able to get the canceled order by id");
            var canceledDuration = (freshlyFetched.Canceled - DateTimeOffset.UtcNow)?.Duration();
            canceledDuration.Should().BeLessThan(TimeSpan.FromSeconds(10), "we expect the cancellation date to be pretty close to the current time");

            return freshlyFetched;
        }

        private async Task<OrderDto> AssertStartOrderFunctions(HttpResponseMessage httpResponse, OrderDto targetOrder, HttpClient webClient)
        {
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, $"we expect an HTTP 200 when calling PUT /api/orders/{targetOrder.Id}/start with a freshly made order");

            var getByIdResponse = await webClient.GetAsync($"/api/orders/{targetOrder.Id}");
            getByIdResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we should be able to get the started order by id");

            var freshlyFetched = await getByIdResponse.Content.ReadAsJsonAsync<OrderDto>();
            freshlyFetched.Should().NotBeNull("we should be able to get the started order by id");
            var canceledDuration = (freshlyFetched.Started - DateTimeOffset.UtcNow)?.Duration();
            canceledDuration.Should().BeLessThan(TimeSpan.FromSeconds(10), "we expect the started date to be pretty close to the current time");

            return freshlyFetched;
        }

        private static async Task<OrderDto> AssertCreateOrderFunctions(HttpResponseMessage httpResponse, CreateOrderDto request, HttpClient webClient)
        {
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Created, "we expect an HTTP 201 when calling POST /api/orders with valid data to create a new order");
            var resultDto = await httpResponse.Content.ReadAsJsonAsync<OrderDto>();
            resultDto.Id.Should().BeGreaterOrEqualTo(1, "we expect a newly created order to return with a positive Id");

            resultDto.Should().BeEquivalentTo(request, "we expect that the returned order DTO from create order has the same information as what's provided in the request");

            httpResponse.Headers.Location.Should().NotBeNull("we expect the 'location' header to be set as part of a HTTP 201");
            httpResponse.Headers.Location.Should().Be($"http://localhost/api/orders/{resultDto.Id}", "we expect the location header to point to the get order by id endpoint");
            var locationResponse = await webClient.GetAsync(httpResponse.Headers.Location);
            locationResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we should be able to get the newly created order by id");
            var locationDto = await locationResponse.Content.ReadAsJsonAsync<OrderDto>();
            locationDto.Should().BeEquivalentTo(resultDto, "we expect the same result to be returned by a create order as what you'd get from get order by id");

            var allMenuItemsRequest = await webClient.GetAsync("/api/menu-items");
            var allMenuItemsResult = await allMenuItemsRequest.Content.ReadAsJsonAsync<List<MenuItemDto>>();

            foreach (var orderItemDto in resultDto.OrderItems)
            {
                var matchingMenuItem = allMenuItemsResult.FirstOrDefault(x => x.Id == orderItemDto.MenuItemId);
                if (matchingMenuItem == null)
                {
                    Assert.Fail("We are missing menu item " + orderItemDto.MenuItemId + " when we tried to get it from get all menu items");
                }

                orderItemDto.LineItemPrice.Should().Be(matchingMenuItem.Price, "we expect the line item's price to match the menu item price at time of sale");
                var expectedLineItemTotal = matchingMenuItem.Price * orderItemDto.LineItemQuantity;
                orderItemDto.LineItemTotal.Should().Be(expectedLineItemTotal, "we expect the line item's total to be the price * quantity");
            }

            var expectedTotal = resultDto.OrderItems.Sum(x => x.LineItemTotal);
            resultDto.OrderTotal.Should().Be(expectedTotal, "we expect the order's total to represent the sum of the line item totals");

            return resultDto;
        }

        private static async Task<List<MenuItemDto>> AssertMenuListAllFunctions(HttpResponseMessage httpResponse)
        {
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we expect an HTTP 200 when getting calling GET /api/menu-items");
            var resultDto = await httpResponse.Content.ReadAsJsonAsync<List<MenuItemDto>>();
            resultDto.Should().HaveCountGreaterThan(2, "we expect at least 3 records");
            resultDto.All(x => !string.IsNullOrWhiteSpace(x.Name)).Should().BeTrue("we expect all menu items to have names");
            resultDto.All(x => !string.IsNullOrWhiteSpace(x.Description)).Should().BeTrue("we expect all menu items to have descriptions");
            resultDto.All(x => x.Price > 0).Should().BeTrue("we expect all menu items to have non zero/non negative prices");
            resultDto.All(x => x.Id > 0).Should().BeTrue("we expect all menu items to have an id");
            var ids = resultDto.Select(x => x.Id).ToArray();
            ids.Should().HaveSameCount(ids.Distinct(), "we expect Id values to be unique for every menu item");
            return resultDto;
        }
    }
}