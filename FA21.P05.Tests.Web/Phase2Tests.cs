using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using FA21.P05.Tests.Web.Helpers;
using FA21.P05.Web.Features.MenuItems;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FA21.P05.Tests.Web
{
    [TestClass]
    public class Phase2Tests
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
        public async Task GetAllMenuItems_ReturnsData()
        {
            //arrange
            var webClient = context.GetStandardWebClient();

            //act
            var httpResponse = await webClient.GetAsync("/api/menu-items");

            //assert
            await AssertListAllFunctions(httpResponse);
        }

        [TestMethod]
        public async Task GetMenuItemById_NoSuchId_ReturnsNotFound()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var target = await GetItem(webClient);
            if (target == null)
            {
                // you are not ready for this test
                return;
            }
            //act
            var httpResponse = await webClient.GetAsync($"/api/menu-items/{target.Id + 21}");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "we expect an HTTP 404 when calling GET /api/menu-items/{id} with an invalid Id");
        }

        [TestMethod]
        public async Task GetMenuItemById_ReturnsData()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var target = await GetItem(webClient);
            if (target == null)
            {
                // you are not ready for this test
                return;
            }

            //act
            var httpResponse = await webClient.GetAsync($"/api/menu-items/{target.Id}");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we expect an HTTP 200 when calling GET /api/menu-items/{id} (get menu item by Id)");
            var resultDto = await httpResponse.Content.ReadAsJsonAsync<MenuItemDto>();
            resultDto.Should().BeEquivalentTo(target, "we expect get menu item by id to return the same data as the list all menu items endpoint");
        }

        [TestMethod]
        public async Task CreateMenuItem_NoName_ReturnsError()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LogInAsAdmin();
            var request = new MenuItemDto
            {
                Description = "asd",
                Price = 1
            };

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/menu-items", request);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when getting POST /api/menu-items with no name");
        }

        [TestMethod]
        public async Task CreateMenuItem_NameTooLong_ReturnsError()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LogInAsAdmin();
            var request = new MenuItemDto
            {
                Name = "long".PadRight(121,'!'),
                Description = "asd",
                Price = 1
            };

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/menu-items", request);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling POST /api/menu-items with name 121 characters long");
        }

        [TestMethod]
        public async Task CreateMenuItem_NoDescription_ReturnsError()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LogInAsAdmin();
            var request = new MenuItemDto
            {
                Name = "asd",
                Price = 1
            };

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/menu-items", request);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling POST /api/menu-items with no description");
        }

        [TestMethod]
        public async Task CreateMenuItem_NoPrice_ReturnsError()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LogInAsAdmin();
            var request = new MenuItemDto
            {
                Description = "asd",
                Name = "asd"
            };

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/menu-items", request);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling POST /api/menu-items with no price");
        }

        [TestMethod]
        public async Task CreateMenuItem_NegativePrice_ReturnsError()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LogInAsAdmin();
            var request = new MenuItemDto
            {
                Description = "asd",
                Name = "asd"
            };

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/menu-items", request);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling POST /api/menu-items with negative price");
        }

        [TestMethod]
        public async Task CreateMenuItem_Valid_ReturnsResult()
        {
            //arrange
            var request = new MenuItemDto
            {
                Description = "asd",
                Name = "asd",
                Price = 1
            };

            var webClient = context.GetStandardWebClient();
            await webClient.LogInAsAdmin();

            //act
            var httpResponse = await webClient.PostAsJsonAsync("/api/menu-items", request);

            //assert
            await AssertCreateFunctions(httpResponse, request, webClient);
        }

        [TestMethod]
        public async Task DeleteMenuItem_NoSuchItem_ReturnsNotFound()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LogInAsAdmin();
            var request = new MenuItemDto
            {
                Description = "asd",
                Name = "asd",
                Price = 1
            };
            using var itemHandle = await CreateMenuItem(webClient, request);
            if (itemHandle == null)
            {
                // you are not ready for this test
                return;
            }
            //act
            var httpResponse = await webClient.DeleteAsync($"/api/menu-items/{request.Id + 21}");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "we expect an HTTP 404 when calling DELETE /api/menu-items/{id} with an invalid Id");
        }

        [TestMethod]
        public async Task DeleteMenuItem_ValidItem_ReturnsOk()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LogInAsAdmin();
            var request = new MenuItemDto
            {
                Description = "asd",
                Name = "asd",
                Price = 1
            };
            using var itemHandle = await CreateMenuItem(webClient, request);
            if (itemHandle == null)
            {
                // you are not ready for this test
                return;
            }
            //act
            var httpResponse = await webClient.DeleteAsync($"/api/menu-items/{request.Id}");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we expect an HTTP 200 when calling DELETE /api/menu-items/{id} with a valid id");
        }

        [TestMethod]
        public async Task DeleteMenuItem_SameItemTwice_ReturnsNotFound()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LogInAsAdmin();
            var request = new MenuItemDto
            {
                Description = "asd",
                Name = "asd",
                Price = 1
            };
            using var itemHandle = await CreateMenuItem(webClient, request);
            if (itemHandle == null)
            {
                // you are not ready for this test
                return;
            }
            //act
            await webClient.DeleteAsync($"/api/menu-items/{request.Id}");
            var httpResponse = await webClient.DeleteAsync($"/api/menu-items/{request.Id}");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "we expect an HTTP 404 when calling DELETE /api/menu-items/{id} on the same item twice");
        }

        [TestMethod]
        public async Task ListAllSpecials_ReturnsOnlyItemsMarkedSpecial()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            var request = new MenuItemDto
            {
                Description = "asd",
                Name = "asd",
                Price = 1,
                IsSpecial = true
            };
            using var itemHandle = await CreateMenuItem(webClient, request);
            if (itemHandle == null)
            {
                // you are not ready for this test
                return;
            }
            //act
            var httpResponse = await webClient.GetAsync("/api/menu-items/specials");

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we expect an HTTP 200 when calling GET /api/menu-items/specials");

            var resultDto = await httpResponse.Content.ReadAsJsonAsync<List<MenuItemDto>>();
            resultDto.Should().HaveCountGreaterThan(0, "we expect at least 1 item to have a special (as one as added right before this test)");
            resultDto.All(x => !string.IsNullOrWhiteSpace(x.Name)).Should().BeTrue("we expect all menu items to have names");
            resultDto.All(x => !string.IsNullOrWhiteSpace(x.Description)).Should().BeTrue("we expect all menu items to have descriptions");
            resultDto.All(x => x.Price > 0).Should().BeTrue("we expect all menu items to have non zero/non negative prices");
            resultDto.All(x => x.Id > 0).Should().BeTrue("we expect all menu items to have an id");
            resultDto.All(x => x.IsSpecial).Should().BeTrue("we expect all items returned by GET /api/menu-items/specials to be marked 'IsSpecial'");
            var ids = resultDto.Select(x => x.Id).ToArray();
            ids.Should().HaveSameCount(ids.Distinct(), "we expect Id values to be unique for every menu item");
        }

        [TestMethod]
        public async Task UpdateMenuItem_NoName_ReturnsError()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var target = await GetItem(webClient);
            if (target == null)
            {
                // you are not ready for this test
                return;
            }
            target.Name = null;

            //act
            var httpResponse = await webClient.PutAsJsonAsync($"/api/menu-items/{target.Id}", target);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when getting PUT /api/menu-items/{id} with no name");
        }

        [TestMethod]
        public async Task UpdateMenuItem_NameTooLong_ReturnsError()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var target = await GetItem(webClient);
            if (target == null)
            {
                // you are not ready for this test
                return;
            }
            target.Name = "long".PadRight(121, '!');

            //act
            var httpResponse = await webClient.PutAsJsonAsync($"/api/menu-items/{target.Id}", target);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling PUT /api/menu-items/{id} with a name 121 characters long");
        }

        [TestMethod]
        public async Task UpdateMenuItem_NoDescription_ReturnsError()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var target = await GetItem(webClient);
            if (target == null)
            {
                // you are not ready for this test
                return;
            }
            target.Description = null;

            //act
            var httpResponse = await webClient.PutAsJsonAsync($"/api/menu-items/{target.Id}", target);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling PUT /api/menu-items/{id} with no description");
        }

        [TestMethod]
        public async Task UpdateMenuItem_NoPrice_ReturnsError()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var target = await GetItem(webClient);
            if (target == null)
            {
                // you are not ready for this test
                return;
            }
            target.Price = 0;

            //act
            var httpResponse = await webClient.PutAsJsonAsync($"/api/menu-items/{target.Id}", target);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling PUT /api/menu-items/{id} with no price");
        }

        [TestMethod]
        public async Task UpdateMenuItem_NegativePrice_ReturnsError()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var target = await GetItem(webClient);
            if (target == null)
            {
                // you are not ready for this test
                return;
            }
            target.Price = -0.01m;

            //act
            var httpResponse = await webClient.PutAsJsonAsync($"/api/menu-items/{target.Id}", target);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.BadRequest, "we expect an HTTP 400 when calling PUT /api/menu-items/{id} with negative price");
        }

        [TestMethod]
        public async Task UpdateMenuItem_WrongId_ReturnsNotFound()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var target = await GetItem(webClient);
            if (target == null)
            {
                // you are not ready for this test
                return;
            }
            target.Price = 0;

            //act
            var httpResponse = await webClient.PutAsJsonAsync($"/api/menu-items/{target.Id+21}", target);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.NotFound, "we expect an HTTP 404 when calling PUT /api/menu-items/{id} with a bad id");
        }

        [TestMethod]
        public async Task UpdateMenuItem_Valid_ReturnsItem()
        {
            //arrange
            var webClient = context.GetStandardWebClient();
            await webClient.LoginAsStaff();
            var target = await GetItem(webClient);
            if (target == null)
            {
                // you are not ready for this test
                return;
            }

            var random = new Random();
            target.Price = random.Next(1, 1000000) * 0.01m;
            target.Name = Guid.NewGuid().ToString("N");
            target.Description = Guid.NewGuid().ToString("N");
            target.IsSpecial = false;

            //act
            var httpResponse = await webClient.PutAsJsonAsync($"/api/menu-items/{target.Id}", target);

            //assert
            httpResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we expect an HTTP 200 when calling PUT /api/menu-items/{id} with valid data");

            var resultDto = await httpResponse.Content.ReadAsJsonAsync<MenuItemDto>();
            resultDto.Should().BeEquivalentTo(target, "we expect the updated item to be returned");
        }

        private async Task<IDisposable> CreateMenuItem(HttpClient webClient, MenuItemDto request)
        {
            try
            {
                var httpResponse = await webClient.PostAsJsonAsync("/api/menu-items", request);
                await AssertCreateFunctions(httpResponse, request, webClient);
                return new DeleteItem(request, webClient);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<MenuItemDto> GetItem(HttpClient webClient)
        {
            try
            {
                var getAllRequest = await webClient.GetAsync("/api/menu-items");
                var getAllResult = await AssertListAllFunctions(getAllRequest);
                return getAllResult.OrderByDescending(x => x.Id).First();
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static async Task<List<MenuItemDto>> AssertListAllFunctions(HttpResponseMessage httpResponse)
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

        private static async Task AssertCreateFunctions(HttpResponseMessage httpResponse, MenuItemDto request, HttpClient webClient)
        {
            httpResponse.StatusCode.Should().Be(HttpStatusCode.Created, "we expect an HTTP 201 when calling POST /api/menu-items with valid data to create a new menu item");
            var resultDto = await httpResponse.Content.ReadAsJsonAsync<MenuItemDto>();
            request.Id = resultDto.Id;
            resultDto.Id.Should().BeGreaterOrEqualTo(1, "we expect a newly created menu item to return with a positive Id");
            resultDto.Should().BeEquivalentTo(request, "We expect the create menu item endpoint to return the result");
            httpResponse.Headers.Location.Should().NotBeNull("we expect the 'location' header to be set as part of a HTTP 201");
            httpResponse.Headers.Location.Should().Be($"http://localhost/api/menu-items/{resultDto.Id}", "we expect the location header to point to the get menu item by id endpoint");
            var locationResponse = await webClient.GetAsync(httpResponse.Headers.Location);
            locationResponse.StatusCode.Should().Be(HttpStatusCode.OK, "we should be able to get the newly created menu item by id");
            var locationDto = await locationResponse.Content.ReadAsJsonAsync<MenuItemDto>();
            locationDto.Should().BeEquivalentTo(request, "we expect the same result to be returned by a create menu item as what you'd get from get menu item by id");

            var getAllRequest = await webClient.GetAsync("/api/menu-items");
            await AssertListAllFunctions(getAllRequest);
        }

        internal sealed class DeleteItem : IDisposable
        {
            private readonly MenuItemDto request;
            private readonly HttpClient webClient;

            public DeleteItem(MenuItemDto request, HttpClient webClient)
            {
                this.request = request;
                this.webClient = webClient;
            }

            public void Dispose()
            {
                try
                {
                    webClient.DeleteAsync($"/api/menu-items/{request.Id}").Wait();
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }
    }
}
