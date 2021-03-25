using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BouvetWebApp.Data;
using BouvetWebApp.Interfaces;
using BouvetWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace BouvetWebApp.Repositories
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly IDbContextFactory<OrgContext> _contextFactory;
        private const int Pagesize = 20;

        public CompanyRepository(IDbContextFactory<OrgContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public List<Enheter> GetPaginatedResult(int currentPage = 1)
        {
            var context = _contextFactory.CreateDbContext();

            var companies = context.Enheter.Include(x => x.Organisasjonsform)
                .OrderBy(x => x.Organisasjonsnummer);
            
            return companies.Skip((currentPage - 1) * Pagesize).Take(Pagesize)
                .AsNoTracking().ToList();
        }

        public async Task UpdateRating(int id, int rating)
        {
            var context = _contextFactory.CreateDbContext();
            
            var organization = context.Enheter.AsNoTracking().First(z => z.Organisasjonsnummer == id);
            if (organization != null)
            {
                organization.Vurdering = rating;
                context.Enheter.Update(organization);
                await context.SaveChangesAsync();
            }
        }

        public QueryResult GetCompaniesByOrgType(string org, int? page)
        {
            var context = _contextFactory.CreateDbContext();
            var queryResult = new QueryResult();
            try
            {
                var query = context.Enheter.Include(x => x.Organisasjonsform)
                    .Where(x => x.Organisasjonsform.Kode == org);

                queryResult.Companies = query.Skip((page ?? 1 - 1) * 20).Take(Pagesize)
                    .AsNoTracking().ToList();

                queryResult.Pages = context.Enheter.Where(x => x.Organisasjonsform.Kode == org).Count() / Pagesize;
                return queryResult;
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public Enheter GetCompanyById(int id)
        {
            var context = _contextFactory.CreateDbContext();
            try
            {
                return context.Enheter.Include(s => s.Organisasjonsform).AsNoTracking()
                    .First(x => x.Organisasjonsnummer == id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public int GetPages()
        {
            var context = _contextFactory.CreateDbContext();
            var count = context.Enheter.Count();
            return count / Pagesize;
        }
    }
}