using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace FA21.P05.Tests.Web.Helpers
{
    public static class HttpClientExtensions
    {
        public static async Task LogInAsAdmin(this HttpClient httpClient)
        {
            var result = await httpClient.PostAsJsonAsync("/api/authentication/login", new
            {
                UserName = "galkadi",
                Password = "Password123!"
            });
            if (result.IsSuccessStatusCode)
            {
                return;
            }
            Assert.Fail("Failed to login as admin");
        }

        public static async Task LoginAsStaff(this HttpClient httpClient)
        {
            var result = await httpClient.PostAsJsonAsync("/api/authentication/login", new
            {
                UserName = "bob",
                Password = "Password123!"
            });
            if (result.IsSuccessStatusCode)
            {
                return;
            }
            Assert.Fail("Failed to login as staff");
        }

        public static Task<HttpResponseMessage> PutAsync(this HttpClient httpClient, string url)
        {
            var requestMessage = new HttpRequestMessage(HttpMethod.Put, url);
            return httpClient.SendAsync(requestMessage);
        }

        public static Task<HttpResponseMessage> PostAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PostAsync(url, content);
        }

        public static Task<HttpResponseMessage> PutAsJsonAsync<T>(this HttpClient httpClient, string url, T data)
        {
            var dataAsString = JsonConvert.SerializeObject(data);
            var content = new StringContent(dataAsString);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/json");

            return httpClient.PutAsync(url, content);
        }

        public static async Task<T> ReadAsJsonAsync<T>(this HttpContent content)
        {
            var dataAsString = await content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(dataAsString);
        }
    }
}