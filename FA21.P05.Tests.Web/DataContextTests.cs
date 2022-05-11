using System;
using System.Linq;
using System.Reflection;
using FA21.P05.Tests.Web.Helpers;
using FA21.P05.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FA21.P05.Tests.Web
{
    [TestClass]
    public class DataContextTests
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
        public void DataContext_IsOneDeclared()
        {
            var type = typeof(Startup).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(DbContext))).ToList();
            Assert.IsTrue(type.Count > 0, "You don't have a DbContext declared yet");
            Assert.IsFalse(type.Count > 1, "You have more than one data context created");
            Assert.IsTrue(type[0].Name == "DataContext", "You need to call your DbContext class 'DataContext' not " + type[0].Name);
        }

        [TestMethod]
        public void DataContext_RegisteredInServices()
        {
            var type = typeof(Startup).Assembly.GetTypes().FirstOrDefault(x => x.IsSubclassOf(typeof(DbContext)));
            if (type == null)
            {
                // not ready for this test
                return;
            }

            using var scope = context.Server.Services.CreateScope();
            var dbContext = GetDataContext(scope);

            Assert.IsNotNull(dbContext, "You need to register your DB context");
        }

        [TestMethod]
        public void DataContext_HasMenuItem()
        {
            using var scope = context.Server.Services.CreateScope();
            var dbContext = GetDataContext(scope);
            if (dbContext == null)
            {
                // not ready for this test
                return;
            }

            EnsureSet("MenuItem", dbContext);
        }

        [TestMethod]
        public void DataContext_HasOrder()
        {
            using var scope = context.Server.Services.CreateScope();
            var dbContext = GetDataContext(scope);
            if (dbContext == null)
            {
                // not ready for this test
                return;
            }

            EnsureSet("Order", dbContext);
        }

        [TestMethod]
        public void DataContext_HasOrderItem()
        {
            using var scope = context.Server.Services.CreateScope();
            var dbContext = GetDataContext(scope);
            if (dbContext == null)
            {
                // not ready for this test
                return;
            }

            EnsureSet("OrderItem", dbContext);
        }
        private static void EnsureSet(string modelName, DbContext dataContext)
        {
            var entityType = GetEntityType(modelName);
            var method = dataContext.GetType().GetMethod("Set", BindingFlags.Public | BindingFlags.Instance, null, CallingConventions.Any, new Type[0], null);
            var generic = method?.MakeGenericMethod(entityType);
            dynamic collection = generic?.Invoke(dataContext, null);
            if (collection == null)
            {
                throw new Exception($"You don't have '{modelName}' registered in your DataContext");
            }

            try
            {
                Enumerable.ToList(collection);
            }
            catch (InvalidOperationException e)
            {
                throw new Exception($"it doesn't look like you registered '{modelName}' on your DbContext", e);
            }
            catch (Exception e)
            {
                throw new Exception($"Attempting to call 'ToList' on your '{modelName}' set didn't work. Are you automatically migrating / seeding on startup? Note: don't put your seed or migration code in Program.cs - that won't work", e);
            }
        }

        private static Type GetEntityType(string entityTypeName)
        {
            try
            {
                var entityType = typeof(Startup).Assembly.GetTypes().Single(x => x.Name == entityTypeName);
                return entityType;
            }
            catch (Exception e)
            {
                throw new Exception($"You don't have the entity '{entityTypeName}' declared at all", e);
            }
        }

        private DbContext GetDataContext(IServiceScope scope)
        {
            try
            {
                var type = typeof(Startup).Assembly.GetTypes().Single(x => x.IsSubclassOf(typeof(DbContext)));
                var dataContext = (DbContext)scope.ServiceProvider.GetService(type);
                return dataContext;
            }
            catch
            {
                return null;
            }
        }
    }
}