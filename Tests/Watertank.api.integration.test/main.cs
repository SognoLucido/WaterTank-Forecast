using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataFlow_ReadAPI;
using Dbcheck;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Testcontainers.PostgreSql;
using Watertank.api.integration.test.Services;

namespace Watertank.api.integration.test;

public class ProgramTestApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
{

    private readonly PostgreSqlContainer _Dbcontainer;
    
    public ProgramTestApplicationFactory()
    {
        // _Dbcontainer = new PostgreSqlBuilder().Build();
        _Dbcontainer = new PostgreSqlBuilder()
            .WithImage("timescale/timescaledb:latest-pg16") 
            .WithDatabase("WaterTank")
            .WithUsername("postgres")
            .WithPassword("postgres")
          //  .WithPortBinding(5544, true)
            .WithCleanUp(true)
            .WithCommand("-c", "fsync=off")
            .WithCommand("-c", "full_page_writes=off")
            .WithCommand("-c", "synchronous_commit=off")
            .Build();


    }

  //  private readonly PostgreSqlContainer _mongoDbContainer = 
    //private readonly Loggermock _loggermock;


    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {

            var dbContext = services.SingleOrDefault(d => d.ServiceType == typeof(Dbinit));

            if (dbContext is not null)
                services.Remove(dbContext);


            services.AddSingleton<Dbinit>(opt =>
            {
                return new Dbinit(null, null, _Dbcontainer.GetConnectionString());
            });


            services.AddSingleton<IDatabaseInserter,DatabaseInserter>();

        });

        builder.ConfigureLogging(opt =>
        {
            opt.ClearProviders();
        });

       


    }



    public async Task InitializeAsync()
    {
        await _Dbcontainer.StartAsync();
    }

    public new async Task DisposeAsync()
    {
        await _Dbcontainer.DisposeAsync();
    }




}
