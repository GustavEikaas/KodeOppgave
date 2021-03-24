using System;
using System.Collections.Generic;

namespace BouvetWebApp.Interfaces
{
    public interface IOrganizationRepository
    {
        public List<String> GetOrgTypes();
    }
}