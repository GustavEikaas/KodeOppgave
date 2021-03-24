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
            organization.Vurdering = rating;
            context.Enheter.Update(organization);
            await context.SaveChangesAsync();
        }

        public List<Enheter> GetCompaniesByOrgType(string org)
        {
            var context = _contextFactory.CreateDbContext();
            try
            {
                return context.Enheter.Include(x => x.Organisasjonsform)
                    .Where(x => x.Organisasjonsform.Kode == org)
                    .Take(20).OrderBy(x => x.Organisasjonsnummer)
                    .AsNoTracking().ToList();
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