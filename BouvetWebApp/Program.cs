using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BouvetWebApp.Data;
using BouvetWebApp.Interfaces;
using BouvetWebApp.Models;
using BouvetWebApp.Timer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace BouvetWebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            
            CreateDbIfNotExists(host);
            PopulateDb(host);
            SetRefreshTimer(host);
            host.Run();
        }
        
        private static void CreateDbIfNotExists(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<IDbContextFactory<OrgContext>>().CreateDbContext();
                    context.Database.EnsureCreated();
                   
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred creating the DB.");
                }
            }
        }

        private static void PopulateDb(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var companyRepository = services.GetRequiredService<ICompanyRepository>();
                    var api = new ExternalApi();
                    var updateList = api.FetchDataFromExternalApi().Result;
                    if (updateList != null)
                    {
                        companyRepository.MergeUpdateList(updateList).Wait();
                    }
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred populating the DB.");
                }
            }
        }

        private static void SetRefreshTimer(IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var companyRepository = services.GetRequiredService<ICompanyRepository>();
                var rf = new Refresh(companyRepository);
                rf.SetTimer();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
    }
}