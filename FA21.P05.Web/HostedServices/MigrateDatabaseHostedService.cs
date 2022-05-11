using System;
using System.Threading;
using System.Threading.Tasks;
using FA21.P05.Web.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FA21.P05.Web.HostedServices
{
    public class MigrateDatabaseHostedService : IHostedService
    {
        private readonly IServiceProvider serviceProvider;

        public MigrateDatabaseHostedService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = serviceProvider.CreateScope();

            await using var dataContext = scope.ServiceProvider.GetService<DataContext>() ?? throw new Exception("Missing DataContext");
            await dataContext.Database.MigrateAsync(cancellationToken);
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}