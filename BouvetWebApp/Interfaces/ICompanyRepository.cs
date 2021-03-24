using System.Collections.Generic;
using System.Threading.Tasks;
using BouvetWebApp.Models;

namespace BouvetWebApp.Interfaces
{
    public interface ICompanyRepository
    {
        public List<Enheter> GetPaginatedResult(int currentPage);
        public Task UpdateRating(int id, int rating);
        public List<Enheter> GetCompaniesByOrgType(string org);

        public Enheter GetCompanyById(int id);

        public int GetPages();
    }
}