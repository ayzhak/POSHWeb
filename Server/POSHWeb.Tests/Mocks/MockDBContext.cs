using System;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using POSHWeb.Data;

namespace POSHWebTests.Mocks;

public class MockDBContext
{
    public static Mock<IServiceScopeFactory> CreateMockIServiceScopeFactoryForDB(out DatabaseContext dbContext)
    {

        dbContext = CreateContext();
        var serviceProvider = new Mock<IServiceProvider>();
        serviceProvider.Setup<object>(x => x.GetService(typeof(DatabaseContext))).Returns(dbContext);

        var serviceScope = new Mock<IServiceScope>();
        serviceScope.Setup(x => x.ServiceProvider).Returns(serviceProvider.Object);

        var serviceScopeFactory = new Mock<IServiceScopeFactory>();
        serviceScopeFactory.Setup(x => x.CreateScope()).Returns(serviceScope.Object);

        serviceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory))).Returns(serviceScopeFactory.Object);

        return serviceScopeFactory;
    }
    private static DatabaseContext CreateContext()
    {
        var _connection = new SqliteConnection("Filename=:memory:");
        _connection.Open();
        var _contextOptions = new DbContextOptionsBuilder<DatabaseContext>()
            .UseSqlite(_connection)
            .Options;
        var context = new DatabaseContext(_contextOptions);
        context.Database.EnsureCreated();
        context.Database.Migrate();
        return context;
    }
}