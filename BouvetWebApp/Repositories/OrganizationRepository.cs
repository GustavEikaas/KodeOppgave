using System.Collections.Generic;
using System.Linq;
using BouvetWebApp.Data;
using BouvetWebApp.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace BouvetWebApp.Repositories
{
    public class OrganizationRepository : IOrganizationRepository
    {
        private readonly IDbContextFactory<OrgContext> _contextFactory;
        public OrganizationRepository(IDbContextFactory<OrgContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }
        public List<string> GetOrgTypes()
        {
            var context = _contextFactory.CreateDbContext();
            return context.Organisasjonsform.Select(x => x.Kode).Distinct().AsNoTracking().ToList();
        }
    }
}