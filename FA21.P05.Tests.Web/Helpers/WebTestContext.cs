using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using FA21.P05.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Mvc.Testing.Handlers;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Net.Http.Headers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FA21.P05.Tests.Web.Helpers
{
    [TestClass]
    public sealed class WebTestContext : IDisposable
    {
        private readonly SqlServerTestDatabaseProvider databaseProvider;

        public WebTestContext()
        {
            databaseProvider = new SqlServerTestDatabaseProvider();
            var connectionString = databaseProvider.GetConnectionString();
            Server = new WebHostFactory<Startup>(connectionString);
        }

        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            SqlServerTestDatabaseProvider.AssemblyInit();
        }

        [AssemblyCleanup]
        public static void ApplicationCleanup()
        {
            SqlServerTestDatabaseProvider.ApplicationCleanup();
        }

        public WebHostFactory<Startup> Server { get; }

        public HttpClient GetStandardWebClient()
        {
            var cookieContainer = new CookieContainer(100);
            return Server.CreateDefaultClient(new RedirectHandler(10), new NonSecureCookieHandler(cookieContainer));
        }

        public void Dispose()
        {
            Server.Dispose();
            databaseProvider.Dispose();
        }

        public class WebHostFactory<TStartup> : WebApplicationFactory<TStartup> where TStartup : class
        {
            private readonly string connectionString;

            public WebHostFactory(string connectionString)
            {
                this.connectionString = connectionString;
            }

            protected override void ConfigureWebHost(IWebHostBuilder x)
            {
                x.ConfigureAppConfiguration(y =>
                {
                    y.Add(new MemoryConfigurationSource
                    {
                        InitialData = new List<KeyValuePair<string, string>>
                        {
                            new("ConnectionStrings:DataContext", connectionString),
                            new("Logging:LogLevel:Microsoft", "Error"),
                            new("Logging:LogLevel:Microsoft.Hosting.Lifetime", "Error"),
                            new("Logging:LogLevel:Default", "Error")
                        }
                    });
                });
                base.ConfigureWebHost(x);
            }
        }

        public class NonSecureCookieHandler : DelegatingHandler
        {
            private readonly CookieContainer cookieContainer;

            public NonSecureCookieHandler(CookieContainer cookieContainer)
            {
                this.cookieContainer = cookieContainer;
            }

            protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                var cookieHeader = cookieContainer.GetCookieHeader(request.RequestUri ?? throw new Exception("No request uri"));

                request.Headers.Add(HeaderNames.Cookie, cookieHeader);

                var response = await base.SendAsync(request, cancellationToken);
                if (response.Headers.TryGetValues(HeaderNames.SetCookie, out var values))
                {
                    foreach (var header in values)
                    {
                        // HACK: we cannot test on https so we have to force it to be insecure
                        var keys = header.Split("; ").Where(x => !string.Equals(x, "secure"));
                        var result = string.Join("; ", keys);
                        cookieContainer.SetCookies(response.RequestMessage?.RequestUri ?? throw new Exception("No request uri"), result);
                    }
                }

                return response;
            }
        }
    }
}