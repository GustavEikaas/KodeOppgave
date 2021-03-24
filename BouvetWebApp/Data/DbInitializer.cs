using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BouvetWebApp.Models;
using BouvetWebApp.Pages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BouvetWebApp.Data
{
    public class DbInitializer
    {
        private readonly ILogger<DbInitializer> _logger;
        private readonly IDbContextFactory<OrgContext> _contextFactory;
        private const string Query = @"https://data.brreg.no/enhetsregisteret/api/enheter?size=1000";

        public DbInitializer(IDbContextFactory<OrgContext> contextFactory, ILogger<DbInitializer> logger)
        {
            _contextFactory = contextFactory;
            _logger = logger;
        }
        public void Initialize()
        {
            var context = _contextFactory.CreateDbContext();
            context.Database.EnsureCreated();
            UpdateFromApi().Wait();
        }

        
        public async Task UpdateFromApi()
        {
            var context = _contextFactory.CreateDbContext();
            var updateList = await FetchDataFromExternalAPI();
            
            if (updateList != null)
            {
                foreach (var company in updateList)
                {
                    if (!context.Enheter.AsNoTracking().Any(x => x.Organisasjonsnummer == company.Organisasjonsnummer))
                    {
                        await context.Enheter.AddAsync(company);
                    }
                    else
                    {
                        var companyOld = await context.Enheter.AsNoTracking()
                            .FirstAsync(x => x.Organisasjonsnummer == company.Organisasjonsnummer);

                        company.Vurdering = companyOld.Vurdering;
                        context.Entry(company).State = EntityState.Modified;
                    }
                }
                await context.SaveChangesAsync();
            }
                           
        }

        public async Task<List<Enheter>> FetchDataFromExternalAPI()
        {
            using HttpClient client = new HttpClient();
                using HttpResponseMessage res = await client.GetAsync(Query);
                using HttpContent content = res.Content;
                
                var data = await content.ReadAsStringAsync();
                var updateList = JsonConvert.DeserializeObject<Root>(data);
                data = null;

                if (updateList != null) return updateList._embedded.enheter;
                
                _logger.Log(LogLevel.Error, "External API returned no data");
                return null;
        }
    }
}