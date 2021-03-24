using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BouvetWebApp.Data;
using BouvetWebApp.Interfaces;
using BouvetWebApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace BouvetWebApp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ICompanyRepository _companyRepository;
        private readonly IOrganizationRepository _organizationRepository;
        public List<Enheter> Enheter { get; set; } = new List<Enheter>();

        public IndexModel(
            ICompanyRepository companyRepository,
            IOrganizationRepository organizationRepository)
        {
            _companyRepository = companyRepository;
            _organizationRepository = organizationRepository;
        }
        
        public void OnGet(GetRequest request)
        {
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
                Enheter = _companyRepository.GetCompaniesByOrgType(request.Org);
                
            }
            else if (request.GoToPage != 0)
            {
                Enheter = _companyRepository.GetPaginatedResult(request.GoToPage);
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

        public void Refresh()
        {
            Enheter = _companyRepository.GetPaginatedResult(1);
        }

        public int GetPages()
        {
            return _companyRepository.GetPages();
        }
        
        public IEnumerable<string> GetOrgTypes()
        {
            return _organizationRepository.GetOrgTypes();
        }
        
    }
}