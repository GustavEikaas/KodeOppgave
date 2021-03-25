using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BouvetWebApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BouvetWebApp.Data
{
    public class ExternalApi
    {
        private readonly ILogger<ExternalApi> _logger;
        private readonly IDbContextFactory<OrgContext> _contextFactory;
        private const string Query = @"https://data.brreg.no/enhetsregisteret/api/enheter?size=1000";

        public ExternalApi(IDbContextFactory<OrgContext> contextFactory, ILogger<ExternalApi> logger)
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
        
        private async Task UpdateFromApi()
        {
            var context = _contextFactory.CreateDbContext();
            var updateList = await FetchDataFromExternalApi();
            
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

        private async Task<IEnumerable<Enheter>> FetchDataFromExternalApi()
        {
            using var client = new HttpClient();
                using var res = await client.GetAsync(Query);
                using var content = res.Content;
                
                var data = await content.ReadAsStringAsync();
                var updateList = JsonConvert.DeserializeObject<Root>(data);
                data = null;

                if (updateList != null) return updateList._embedded.Enheter;
                
                _logger.Log(LogLevel.Error, "External API returned no data");
                return null;
        }
    }
}