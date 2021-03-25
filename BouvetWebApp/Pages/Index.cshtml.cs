using System.Collections.Generic;
using BouvetWebApp.Interfaces;
using BouvetWebApp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BouvetWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IOrganizationRepository _organizationRepository;
        public List<Enheter> Enheter { get; private set; } = new List<Enheter>();
        public int PageNumber { get; private set; }
        public string CurrFilter { get; private set; }

        public IndexModel(
            ICompanyRepository companyRepository,
            IOrganizationRepository organizationRepository)
        {
            _companyRepository = companyRepository;
            _organizationRepository = organizationRepository;
        }
        
        public void OnGet(GetRequest request)
        {
            CurrFilter = null;
            if (request.Id != 0)
            {
                var company = _companyRepository.GetCompanyById(request.Id);
                if (company != null)
                {
                    Enheter.Add(company);
                }
            }
            else if (!string.IsNullOrEmpty(request.Org))
            {
                var result = _companyRepository.GetCompaniesByOrgType(request.Org, request.GoToPage);
                Enheter = result.Companies;
                PageNumber = result.Pages;
                CurrFilter = $"?Org={request.Org}";
            }
            else if (request.GoToPage != 0)
            {
                Enheter = _companyRepository.GetPaginatedResult(request.GoToPage);
                PageNumber = _companyRepository.GetPages();
            }
            else
            {
                Refresh();
            }
        }
        
        public void OnPost()
        {
            if (int.TryParse(Request.Form["Id"], out var id))
            {
                var rating = int.Parse(Request.Form["Rating"]);
                _companyRepository.UpdateRating(id, rating);
            }
            Refresh();
        }

        private void Refresh()
        {
            Enheter = _companyRepository.GetPaginatedResult(1);
            PageNumber = _companyRepository.GetPages();
        }

        public IEnumerable<string> GetOrgTypes()
        {
            return _organizationRepository.GetOrgTypes();
        }
        
    }
}